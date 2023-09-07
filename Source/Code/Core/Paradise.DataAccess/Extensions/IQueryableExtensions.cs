using Paradise.Common.Extensions;
using Paradise.Localization.ExceptionsHandling;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Paradise.DataAccess.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IQueryable{T}"/> interface.
/// </summary>
internal static class IQueryableExtensions
{
    #region Constants
    /// <summary>
    /// The number of arguments passed into <see cref="string.ToLower()"/> method.
    /// </summary>
    private const int ToLowerMethodArgumentsNumber = 0;
    /// <summary>
    /// The number of arguments passed into
    /// <see cref="Queryable.Where{TSource}(IQueryable{TSource}, Expression{Func{TSource, bool}})"/> method.
    /// </summary>
    private const int WhereMethodArgumentsNumber = 2;
    /// <summary>
    /// The number of arguments passed into
    /// <see cref="Queryable.OrderBy{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    /// or
    /// <see cref="Queryable.OrderByDescending{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    /// methods.
    /// </summary>
    private const int OrderByMethodsArgumentsNumber = 2;
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

        var expression = ConstructFilterExpression(entityType, propertyNames, value.ToLower());

        var whereMethod = GetWhereMethodInfo(entityType);

        return InvokeStaticMethodWithCast<IQueryable<TSource>>(whereMethod, query, expression);
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

        var orderByMethod = GetOrderByMethodInfo(orderAscending, entityType, orderingProperty.PropertyType);

        var expression = ConstructOrderingExpression(entityType, name);

        return InvokeStaticMethodWithCast<IOrderedQueryable<TSource>>(orderByMethod, query, expression);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Gets the <see cref="Queryable.Where{TSource}(IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
    /// method info.
    /// </summary>
    /// <param name="entityType">
    /// Entity type to parametrize method info.
    /// </param>
    /// <returns>
    /// A <see cref="MethodInfo"/> instance
    /// containing information about
    /// <see cref="Queryable.Where{TSource}(IQueryable{TSource}, Expression{Func{TSource, bool}})"/> method.
    /// </returns>
    private static MethodInfo GetWhereMethodInfo(Type entityType)
    {
        return typeof(Queryable)
            .GetMethods()
            .Where(method => method.Name is nameof(Queryable.Where) && method.IsGenericMethodDefinition)
            .First(method => method.GetParameters().Length is OrderByMethodsArgumentsNumber)
            .MakeGenericMethod(entityType);
    }

    /// <summary>
    /// Gets the <see cref="Queryable.OrderBy{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    /// or
    /// <see cref="Queryable.OrderByDescending{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    /// method info.
    /// </summary>
    /// <param name="orderAscending">
    /// Indicates whether the
    /// <see cref="Queryable.OrderBy{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    /// or
    /// <see cref="Queryable.OrderByDescending{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    /// method info should be retrieved.
    /// </param>
    /// <param name="entityType">
    /// Entity type to parametrize method info.
    /// </param>
    /// <param name="orderingPropertyType">
    /// Property type to parametrize method info.
    /// </param>
    /// <returns>
    /// A <see cref="MethodInfo"/> instance
    /// containing information about
    /// <see cref="Queryable.OrderBy{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    /// or
    /// <see cref="Queryable.OrderByDescending{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
    /// method.
    /// </returns>
    private static MethodInfo GetOrderByMethodInfo(bool orderAscending, Type entityType, Type orderingPropertyType)
    {
        var methodName = orderAscending
            ? nameof(Queryable.OrderBy)
            : nameof(Queryable.OrderByDescending);

        return typeof(Queryable)
            .GetMethods()
            .Where(method => method.Name == methodName && method.IsGenericMethodDefinition)
            .First(method => method.GetParameters().Length is OrderByMethodsArgumentsNumber)
            .MakeGenericMethod(entityType, orderingPropertyType);
    }

    /// <summary>
    /// Gets the <see cref="string.Contains(string)"/> method info.
    /// </summary>
    /// <returns>
    /// A <see cref="MethodInfo"/> instance
    /// containing information about
    /// <see cref="string.Contains(string)"/> method.
    /// </returns>
    private static MethodInfo GetContainsMethodInfo()
    {
        var stringType = typeof(string);

        return stringType.GetMethod(nameof(string.Contains), new[] { stringType })!;
    }

    /// <summary>
    /// Gets the <see cref="string.ToLower()"/> method info.
    /// </summary>
    /// <returns>
    /// A <see cref="MethodInfo"/> instance
    /// containing information about
    /// <see cref="string.ToLower()"/> method.
    /// </returns>
    private static MethodInfo GetToLowerMethodInfo()
    {
        return typeof(string)
            .GetMethods()
            .Where(m => m.Name is nameof(string.ToLower))
            .First(m => m.GetParameters().Length is ToLowerMethodArgumentsNumber);
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
            var messageFormat = ExceptionMessages.PropertyNotDeclared;
            var entityName = entityType.Name;

            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, propertyName, entityName);

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
    private static Expression ConstructFilterExpression(Type entityType, IEnumerable<string> propertyNames, string? value)
    {
        var toLowerMethod = GetToLowerMethodInfo();

        var containsMethod = GetContainsMethodInfo();

        var searchValueExpression = Expression.Constant(value, typeof(string));
        var argument = Expression.Parameter(entityType, entityType.Name.ToLowerInvariant());

        var loweredPropertiesContainsCalls = propertyNames
            .Select(name => Expression.Property(argument, name))
            .Select(propertyCall => Expression.Call(propertyCall, toLowerMethod))
            .Select(loweredStringProperty => Expression.Call(loweredStringProperty, containsMethod, searchValueExpression));

        LambdaExpression? selector = null;

        if (loweredPropertiesContainsCalls.Count() is 1)
        {
            selector = Expression.Lambda(loweredPropertiesContainsCalls.First(), argument);
        }
        else
        {
            var firstTwoStatements = loweredPropertiesContainsCalls.Take(2);
            var otherStatements = loweredPropertiesContainsCalls.Skip(2);

            var orStatement = Expression.OrElse(firstTwoStatements.First(), firstTwoStatements.Last());

            foreach (var statement in otherStatements)
                orStatement = Expression.OrElse(orStatement, statement);

            selector = Expression.Lambda(orStatement, argument);
        }

        return selector;
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
    private static Expression ConstructOrderingExpression(Type entityType, string propertyName)
    {
        var argument = Expression.Parameter(entityType, entityType.Name.ToLowerInvariant());
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
                throw new InvalidOperationException(ExceptionMessages.InvalidPropertyType);
        }
    }

    /// <summary>
    /// Invokes the given static <paramref name="method"/>
    /// with the given <paramref name="args"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Expected invocation result type.
    /// </typeparam>
    /// <param name="method">
    /// The method to be invoked.
    /// </param>
    /// <param name="args">
    /// Method arguments.
    /// </param>
    /// <returns>
    /// A new instance of <typeparamref name="T"/> as a result
    /// of the <paramref name="method"/> invocation.
    /// </returns>
    /// <exception cref="InvalidCastException">
    /// Thrown if the invocation result does not have a type of <typeparamref name="T"/>.
    /// </exception>
    private static T InvokeStaticMethodWithCast<T>(MethodInfo method, params object?[] args)
    {
        var boxedResult = method.Invoke(null, args);

        if (boxedResult is not T result)
        {
            var messageFormat = ExceptionMessages.FailedToCast;
            var actualType = boxedResult?.GetType();
            var expectedType = typeof(T);

            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, actualType, expectedType);

            throw new InvalidCastException(message);
        }

        return result;
    }
    #endregion
}