using Microsoft.EntityFrameworkCore.Query;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using static Paradise.Localization.ExceptionsHandling.ExceptionMessagesProvider;

namespace Paradise.Tests.Miscellaneous.Fakes.Microsoft.EntityFrameworkCore.Query;

/// <summary>
/// Fake <see cref="IOrderedQueryable{T}"/>, <see cref="IAsyncEnumerable{T}"/>
/// and <see cref="IAsyncQueryProvider"/> implementation.
/// </summary>
/// <typeparam name="TEntity">
/// Entity type.
/// </typeparam>
public sealed class FakeAsyncQueryProvider<TEntity> : IOrderedQueryable<TEntity>, IAsyncEnumerable<TEntity>, IAsyncQueryProvider
{
    #region Fields
    private IEnumerable<TEntity>? _entities;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeAsyncQueryProvider{TEntity}"/> class.
    /// </summary>
    /// <param name="expression">
    /// The expression tree associated with the current instance.
    /// </param>
    public FakeAsyncQueryProvider(Expression expression)
        => Expression = expression;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeAsyncQueryProvider{TEntity}"/> class.
    /// </summary>
    /// <param name="entities">
    /// The input sequence.
    /// </param>
    public FakeAsyncQueryProvider(IEnumerable<TEntity> entities)
    {
        _entities = entities;
        Expression = entities.AsQueryable().Expression;
    }
    #endregion

    #region Properties
    /// <inheritdoc/>
    public Type ElementType
        => typeof(TEntity);

    /// <inheritdoc/>
    public Expression Expression { get; }

    /// <inheritdoc/>
    public IQueryProvider Provider
        => this;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public IQueryable CreateQuery(Expression expression)
    {
        if (expression is MethodCallExpression methodCallExpression)
        {
            var elementType = methodCallExpression
                .Method
                .ReturnType
                .GetGenericArguments()[0];

            return CreateQueryable(elementType, expression);
        }

        return CreateQueryable<TEntity>(expression);
    }

    /// <inheritdoc/>
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => CreateQueryable<TElement>(expression);

    /// <inheritdoc/>
    public object? Execute(Expression expression)
        => Compile<object>(expression);

    /// <inheritdoc/>
    public TResult Execute<TResult>(Expression expression)
        => Compile<TResult>(expression);

    /// <inheritdoc/>
    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        // The TResult have to be Task<T>,
        // so T would be the type of parameterizer required to execute the query
        var invocationParameterizerType = typeof(TResult).GetGenericArguments()[0];

        try
        {
            // Invokes the IQueryProvider.Execute<T>(Expression) method on the current instance
            // parametrized with "invocationParameterizerType"
            var invocationResult = typeof(IQueryProvider)
                .GetMethod(nameof(IQueryProvider.Execute), 1, [typeof(Expression)])!
                .MakeGenericMethod(invocationParameterizerType)
                .Invoke(this, [expression]);

            var fromResultMethod = typeof(Task).GetMethod(nameof(Task.FromResult))!;

            var genericMethod = fromResultMethod.MakeGenericMethod(invocationParameterizerType);

            return (TResult)genericMethod.Invoke(null, [invocationResult])!;
        }
        catch (TargetInvocationException e)
        {
            // Need to catch the TargetInvocationException
            // and re-throw its inner exception to
            // satisfy asynchronous test assertions,
            // e.g.: "Assert.ThrowsAsync<InvalidOperationException>(MethodWhichThrows);"
            if (e.InnerException is not null)
                throw e.InnerException;

            throw;
        }
    }

    /// <inheritdoc/>
    public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new FakeAsyncEnumerator(GetEnumerator());

    /// <inheritdoc/>
    public IEnumerator<TEntity> GetEnumerator()
    {
        _entities ??= Compile<IEnumerable<TEntity>>(Expression);
        return _entities.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
    #endregion

    #region Private methods
    /// <summary>
    /// Creates a new instance of the <see cref="IQueryable{T}"/> parametrized with
    /// <paramref name="elementType"/>.
    /// </summary>
    /// <param name="elementType">
    /// The type of the element(s) that are returned when the expression tree associated
    /// with the returned instance of <see cref="IQueryable{T}"/> is executed.
    /// </param>
    /// <param name="expression">
    /// The expression tree associated
    /// with the returned instance of <see cref="IQueryable{T}"/>.
    /// </param>
    /// <returns>
    /// A new instance of the <see cref="IQueryable{T}"/> parametrized with
    /// <paramref name="elementType"/>.
    /// </returns>
    private IQueryable CreateQueryable(Type elementType, Expression expression)
    {
        var boxedQueryable = CreateBoxedQueryable(elementType, expression);

        return boxedQueryable is IQueryable queryable ? queryable : throw new InvalidOperationException();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="IQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Queryable parameterizer type.
    /// </typeparam>
    /// <param name="expression">
    /// The expression tree associated
    /// with the returned instance of <see cref="IQueryable{T}"/>.
    /// </param>
    /// <returns>
    /// A new instance of the <see cref="IQueryable{T}"/>.
    /// </returns>
    private IQueryable<T> CreateQueryable<T>(Expression expression)
    {
        var boxedQueryable = CreateBoxedQueryable(typeof(T), expression);

        if (boxedQueryable is not IQueryable<T> queryable)
        {
            var message = GetFailedToCastMessage(boxedQueryable?.GetType(), typeof(IQueryable<T>));

            throw new InvalidCastException(message);
        }

        return queryable;
    }

    private object? CreateBoxedQueryable(Type elementType, Expression expression)
    {
        var queryableType = GetType()
            .GetGenericTypeDefinition()
            .MakeGenericType(elementType);

        return Activator.CreateInstance(queryableType, expression);
    }

    /// <summary>
    /// Executes the given <paramref name="expression"/>
    /// to retrieve the result of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Result type.
    /// </typeparam>
    /// <param name="expression">
    /// The <see cref="System.Linq.Expressions.Expression"/> to be executed.
    /// </param>
    /// <returns>
    /// The <typeparamref name="T"/> value which was resulted
    /// from the <paramref name="expression"/> execution.
    /// </returns>
    private static T Compile<T>(Expression expression)
    {
        var body = FakeExpressionVisitor.VisitExpression(expression);

        var function = Expression.Lambda<Func<T>>(body);

        return function.Compile().Invoke();
    }
    #endregion

    #region Nested types
    /// <summary>
    /// Fake <see cref="ExpressionVisitor"/> implementation.
    /// </summary>
    private sealed class FakeExpressionVisitor : ExpressionVisitor
    {
        #region Public methods
        /// <inheritdoc cref="ExpressionVisitor.Visit(Expression?)"/>
        public static Expression VisitExpression(Expression expression)
            => new FakeExpressionVisitor().Visit(expression);
        #endregion
    }

    /// <summary>
    /// Fake <see cref="IAsyncEnumerator{T}"/> implementation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="FakeAsyncEnumerator"/> class.
    /// </remarks>
    /// <param name="enumerator">
    /// The input enumerator.
    /// </param>
    private sealed class FakeAsyncEnumerator(IEnumerator<TEntity> enumerator) : IAsyncEnumerator<TEntity>
    {
        #region Properties
        /// <inheritdoc/>
        public TEntity Current
            => enumerator.Current;
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public ValueTask<bool> MoveNextAsync()
            => new(enumerator.MoveNext());

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            enumerator.Dispose();
            return new();
        }
        #endregion
    }
    #endregion
}