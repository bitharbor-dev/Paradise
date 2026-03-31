using Paradise.Common.Extensions;
using System.Linq.Expressions;
using System.Reflection;
using static Paradise.Localization.ExceptionHandling.ExceptionMessages;

namespace Paradise.DataAccess.Repositories.Base.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IQueryable{T}"/> interface.
/// </summary>
internal static class IQueryableExtensions
{
    #region Fields
    private static readonly MethodInfo _whereDefinition =
        ((Func<IQueryable<object>, Expression<Func<object, bool>>, IQueryable<object>>)Queryable.Where)
        .Method
        .GetGenericMethodDefinition();

    private static readonly MethodInfo _orderByDefinition =
        ((Func<IQueryable<object>, Expression<Func<object, object>>, IQueryable<object>>)Queryable.OrderBy)
        .Method
        .GetGenericMethodDefinition();

    private static readonly MethodInfo _orderByDescendingDefinition =
        ((Func<IQueryable<object>, Expression<Func<object, object>>, IQueryable<object>>)Queryable.OrderByDescending)
        .Method
        .GetGenericMethodDefinition();

    private static readonly MethodInfo _containsDefenition =
        typeof(string)
        .GetMethod(nameof(string.Contains), [typeof(string)])!;

    private static readonly MethodInfo _toUpperDefinition =
        typeof(string)
        .GetMethod(nameof(string.ToUpper), Type.EmptyTypes)!;
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

        var entityType = typeof(TSource);

        ValidateStringProperties(entityType, propertyNames);

        var expression = ConstructFilterExpression(entityType, propertyNames, value);

        var whereMethod = _whereDefinition.MakeGenericMethod(entityType);

        return (IQueryable<TSource>)whereMethod.Invoke(null, [query, expression])!;
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

        var entityType = typeof(TSource);

        var orderingProperty = GetPropertyInfo(entityType, name);

        var definition = orderAscending
            ? _orderByDefinition
            : _orderByDescendingDefinition;

        var orderByMethod = definition.MakeGenericMethod(entityType, orderingProperty.PropertyType);

        var expression = ConstructOrderingExpression(entityType, name);

        return (IOrderedQueryable<TSource>)orderByMethod.Invoke(null, [query, expression])!;
    }
    #endregion

    #region Private methods
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

    /// <summary>
    /// Constructs the filtering expression to be passed into
    /// the <see cref="Queryable.Where{TSource}(IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
    /// invocation call.
    /// </summary>
    /// <param name="entityType">
    /// Entity type to parametrize expression.
    /// </param>
    /// <param name="propertyNames">
    /// The list of properties to be included into filtering expression.
    /// </param>
    /// <param name="value">
    /// Filtering <see cref="string"/> value to be checked for containment in the
    /// entity properties with the given <paramref name="propertyNames"/>.
    /// </param>
    /// <returns>
    /// An <see cref="Expression"/> instance to be passed into
    /// the <see cref="Queryable.Where{TSource}(IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
    /// invocation call.
    /// </returns>
    private static LambdaExpression ConstructFilterExpression(Type entityType, IEnumerable<string> propertyNames, string value)
    {
        value = value.ToUpperInvariant();

        var searchValueExpression = Expression.Constant(value, typeof(string));
        var argument = Expression.Parameter(entityType, entityType.Name.ToUpperInvariant());

        var orExpression = propertyNames
            .Select(name => Expression.Property(argument, name))
            .Select(propertyCall => Expression.Call(propertyCall, _toUpperDefinition))
            .Select(uppercaseStringProperty => Expression.Call(uppercaseStringProperty, _containsDefenition, searchValueExpression) as Expression)
            .Aggregate(Expression.OrElse);

        return Expression.Lambda(orExpression, argument);
    }

    /// <summary>
    /// Constructs the ordering expression to be passed into
    /// the
    /// <see cref="Queryable.OrderBy{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    /// or
    /// <see cref="Queryable.OrderByDescending{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    /// invocation call.
    /// </summary>
    /// <param name="entityType">
    /// Entity type to parametrize expression.
    /// </param>
    /// <param name="propertyName">
    /// Ordering property name.
    /// </param>
    /// <returns>
    /// An <see cref="Expression"/> instance to be passed into
    /// the
    /// <see cref="Queryable.OrderBy{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    /// or
    /// <see cref="Queryable.OrderByDescending{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    /// invocation call.
    /// </returns>
    private static LambdaExpression ConstructOrderingExpression(Type entityType, string propertyName)
    {
        var argument = Expression.Parameter(entityType, entityType.Name.ToUpperInvariant());
        var property = Expression.Property(argument, propertyName);
        var selector = Expression.Lambda(property, argument);

        return selector;
    }

    /// <summary>
    /// Validates existence and <see cref="string"/> type
    /// of the properties with the given <paramref name="propertyNames"/>
    /// on the entity with the given <paramref name="entityType"/>.
    /// </summary>
    /// <param name="entityType">
    /// Entity type in which the properties are to be searched.
    /// </param>
    /// <param name="propertyNames">
    /// The list of property names to be searched in the <paramref name="entityType"/>.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if any property does not have a type of <see cref="string"/>.
    /// </exception>
    private static void ValidateStringProperties(Type entityType, IEnumerable<string> propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            var propertyInfo = GetPropertyInfo(entityType, propertyName);

            if (propertyInfo.PropertyType != typeof(string))
            {
                var message = GetMessagePropertyHasInvalidType();

                throw new InvalidOperationException(message);
            }
        }
    }
    #endregion
}