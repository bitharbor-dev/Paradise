using Paradise.Primitives.Extensions;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using static Paradise.Localization.ExceptionHandling.ExceptionMessagesProvider;

namespace Paradise.DataAccess.Repositories.Base.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IQueryable{T}"/> interface.
/// </summary>
internal static class IQueryableExtensions
{
    #region Constants
    private const char PropertyPathSeparator = '.';
    #endregion

    #region Fields
    private static readonly MethodInfo _containsDefinition =
        typeof(string)
        .GetMethod(nameof(string.Contains), [typeof(string)])!;

    private static readonly MethodInfo _toUpperDefinition =
        typeof(string)
        .GetMethod(nameof(string.ToUpper), Type.EmptyTypes)!;

    private static readonly ConcurrentDictionary<(Type, string), PropertyInfo[]> _propertyChainCache = new();
    #endregion

    #region Public methods
    /// <summary>
    /// Filters a sequence of values based on the given
    /// <paramref name="propertyNames"/> and <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="TSource">
    /// Element type.
    /// </typeparam>
    /// <param name="query">
    /// Input query.
    /// </param>
    /// <param name="propertyNames">
    /// Filtering property names.
    /// </param>
    /// <param name="value">
    /// Filter value.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> that contains elements from the input sequence
    /// whose properties with the given <paramref name="propertyNames"/> contains the given <paramref name="value"/>.
    /// <para>
    /// Only <see cref="string"/> properties are supported. Filtering is case insensitive.
    /// </para>
    /// </returns>
    public static IQueryable<TSource> FilterBy<TSource>(this IQueryable<TSource> query, IEnumerable<string> propertyNames, string? value)
    {
        if (!propertyNames.Any())
            return query;

        if (value.IsNullOrWhiteSpace())
            return query;

        var expression = ConstructFilterExpression<TSource>(propertyNames, value);

        return query.Where(expression);
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending or descending order
    /// according to the given <paramref name="name"/>.
    /// </summary>
    /// <typeparam name="TSource">
    /// Element type.
    /// </typeparam>
    /// <param name="query">
    /// Input query.
    /// </param>
    /// <param name="name">
    /// Ordering property name.
    /// </param>
    /// <param name="orderAscending">
    /// Indicates whether the items should be ordered
    /// ascending or descending.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> whose elements are sorted
    /// by the given <paramref name="name"/>.
    /// </returns>
    public static IQueryable<TSource> OrderByPropertyName<TSource>(this IQueryable<TSource> query, string? name, bool orderAscending)
    {
        if (name.IsNullOrWhiteSpace())
            return query;

        var expression = ConstructOrderingExpression<TSource>(name);

        return orderAscending
            ? query.OrderBy(expression)
            : query.OrderByDescending(expression);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Constructs the filtering expression to be passed into
    /// the <see cref="Queryable.Where{TSource}(IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
    /// invocation call.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type to parametrize expression.
    /// </typeparam>
    /// <param name="propertyPaths">
    /// The list of properties to be included into filtering expression.
    /// </param>
    /// <param name="value">
    /// Filtering <see cref="string"/> value to be checked for containment in the
    /// entity properties with the given <paramref name="propertyPaths"/>.
    /// </param>
    /// <returns>
    /// An <see cref="Expression"/> instance to be passed into
    /// the <see cref="Queryable.Where{TSource}(IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
    /// invocation call.
    /// </returns>
    private static Expression<Func<T, bool>> ConstructFilterExpression<T>(IEnumerable<string> propertyPaths, string value)
    {
        value = value.ToUpperInvariant();

        var entityType = typeof(T);

        var searchValueExpression = Expression.Constant(value, typeof(string));
        var argument = Expression.Parameter(entityType, entityType.Name.ToUpperInvariant());

        var orExpression = propertyPaths
            .Select(path =>
            {
                var property = ConstructPropertyChainExpression(entityType, argument, path);

                if (property.Type != typeof(string))
                {
                    var message = GetMessagePropertyHasInvalidType();

                    throw new InvalidOperationException(message);
                }

                return property;
            })
            .Select(call => Expression.Call(call, _toUpperDefinition))
            .Select(call => Expression.Call(call, _containsDefinition, searchValueExpression) as Expression)
            .Aggregate(Expression.OrElse);

        return Expression.Lambda<Func<T, bool>>(orExpression, argument);
    }

    /// <summary>
    /// Constructs the ordering expression to be passed into the
    /// <see cref="Queryable.OrderBy{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/> or
    /// <see cref="Queryable.OrderByDescending{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/> call.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type to parametrize expression.
    /// </typeparam>
    /// <param name="propertyName">
    /// Ordering property name.
    /// </param>
    /// <returns>
    /// An <see cref="Expression"/> instance to be passed into the
    /// <see cref="Queryable.OrderBy{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/> or
    /// <see cref="Queryable.OrderByDescending{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/> call.
    /// </returns>
    private static Expression<Func<T, object>> ConstructOrderingExpression<T>(string propertyName)
    {
        var entityType = typeof(T);

        var argument = Expression.Parameter(entityType, entityType.Name.ToUpperInvariant());
        var property = ConstructPropertyChainExpression(entityType, argument, propertyName);
        var boxedProperty = Expression.Convert(property, typeof(object));

        return Expression.Lambda<Func<T, object>>(boxedProperty, argument);
    }

    /// <summary>
    /// Constructs the property access expression for a potentially nested property path.
    /// </summary>
    /// <param name="entityType">
    ///
    /// </param>
    /// <param name="parameter">
    /// The source expression.
    /// </param>
    /// <param name="propertyPath">
    /// The dot-separated property path.
    /// </param>
    /// <returns>
    /// The <see cref="Expression"/> representing the property access.
    /// </returns>
    private static Expression ConstructPropertyChainExpression(Type entityType, ParameterExpression parameter, string propertyPath)
    {
        var propertyChain = _propertyChainCache.GetOrAdd((entityType, propertyPath), key =>
        {
            var (type, path) = key;

            var segments = path.Split(PropertyPathSeparator);
            var result = new PropertyInfo[segments.Length];

            var currentType = type;

            for (var index = 0; index < segments.Length; index++)
            {
                var property = GetPropertyInfo(currentType, segments[index]);
                result[index] = property;
                currentType = property.PropertyType;
            }

            return result;
        });

        return propertyChain.Aggregate((Expression)parameter, Expression.Property);
    }

    /// <summary>
    /// Gets the <see cref="PropertyInfo"/> of the <paramref name="entityType"/>
    /// with the given <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="entityType">
    /// Entity type in which the <see cref="PropertyInfo"/> is to be searched.
    /// </param>
    /// <param name="propertyName">
    /// Property name.
    /// </param>
    /// <returns>
    /// A <see cref="PropertyInfo"/> instance containing information
    /// about the property with the given <paramref name="propertyName"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if property is not declared in <paramref name="entityType"/>.
    /// </exception>
    private static PropertyInfo GetPropertyInfo(Type entityType, string propertyName)
    {
        var propertyInfo = entityType.GetProperty(propertyName);
        if (propertyInfo is null)
        {
            var message = GetMessagePropertyNotDeclared(propertyName, entityType);

            throw new InvalidOperationException(message);
        }

        return propertyInfo;
    }
    #endregion
}