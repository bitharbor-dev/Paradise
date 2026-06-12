using Microsoft.EntityFrameworkCore.Query;
using Paradise.Tests.Doubles.Fakes.System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Paradise.Tests.Doubles.Fakes.Microsoft.EntityFrameworkCore.Query;

/// <summary>
/// Fake <see cref="IAsyncQueryProvider"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeAsyncQueryProvider"/> class.
/// </remarks>
public sealed class FakeAsyncQueryProvider : IAsyncQueryProvider
{
    #region Fields
    private static readonly CompositeFormat _executeAsyncExceptionMessageFormat = CompositeFormat
        .Parse("Async execution requires Task<T> or ValueTask<T>. Actual type: {0}.");

    private static readonly CompositeFormat _nullQueryProviderMessageFormat = CompositeFormat
        .Parse("The `InnerQueryProvider` was not set. Expression: {0}");

    private static readonly MethodInfo _taskFromResultMethodInfo = typeof(Task)
        .GetMethod(nameof(Task.FromResult))!;

    private static readonly MethodInfo _valueTaskFromResultMethodInfo = typeof(ValueTask)
        .GetMethod(nameof(ValueTask.FromResult))!;
    #endregion

    #region Properties
    /// <summary>
    /// The inner <see cref="IQueryProvider"/>.
    /// </summary>
    public IQueryProvider? InnerQueryProvider { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public IQueryable CreateQuery(Expression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        ThrowIfNullProvider(expression);

        var elementType = expression.Type.GetGenericArguments().Single();
        var queryableType = typeof(FakeAsyncEnumerable<>).MakeGenericType(elementType);

        return (IQueryable)Activator.CreateInstance(queryableType, InnerQueryProvider.CreateQuery(expression))!;
    }

    /// <inheritdoc/>
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        ThrowIfNullProvider(expression);

        return new FakeAsyncEnumerable<TElement>(InnerQueryProvider.CreateQuery<TElement>(expression), this);
    }

    /// <inheritdoc/>
    public object? Execute(Expression expression)
    {
        ThrowIfNullProvider(expression);

        return InnerQueryProvider.Execute(expression);
    }

    /// <inheritdoc/>
    public TResult Execute<TResult>(Expression expression)
    {
        ThrowIfNullProvider(expression);

        return InnerQueryProvider.Execute<TResult>(expression);
    }

    /// <inheritdoc/>
    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var resultType = typeof(TResult);

            if (IsTask(resultType))
                return CreateTaskResult<TResult>(expression);

            if (IsValueTask(resultType))
                return CreateValueTaskResult<TResult>(expression);

            var message = string.Format(CultureInfo.InvariantCulture,
                                        _executeAsyncExceptionMessageFormat,
                                        resultType);

            throw new NotSupportedException(message);
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            throw exception.InnerException;
        }
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Determines whether the specified type represents a generic <see cref="Task{TResult}"/>.
    /// </summary>
    /// <param name="type">
    /// The type to examine.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the type is <see cref="Task{TResult}"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    private static bool IsTask(Type type)
        => type.IsGenericType
        && type.GetGenericTypeDefinition() == typeof(Task<>);

    /// <summary>
    /// Determines whether the specified type represents a generic <see cref="ValueTask{TResult}"/>.
    /// </summary>
    /// <param name="type">
    /// The type to examine.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the type is <see cref="ValueTask{TResult}"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    private static bool IsValueTask(Type type)
        => type.IsGenericType
        && type.GetGenericTypeDefinition() == typeof(ValueTask<>);

    /// <summary>
    /// Executes the specified expression synchronously and wraps the result
    /// in a completed <see cref="Task{TResult}"/>.
    /// </summary>
    /// <typeparam name="TTask">
    /// The expected task type.
    /// </typeparam>
    /// <param name="expression">
    /// The expression to execute.
    /// </param>
    /// <returns>
    /// A completed <see cref="Task{TResult}"/> containing the execution result.
    /// </returns>
    private TTask CreateTaskResult<TTask>(Expression expression)
    {
        ThrowIfNullProvider(expression);

        var result = InnerQueryProvider.Execute(expression);
        var resultType = typeof(TTask).GetGenericArguments()[0];

        return (TTask)_taskFromResultMethodInfo
            .MakeGenericMethod(resultType)
            .Invoke(null, [result])!;
    }

    /// <summary>
    /// Executes the specified expression synchronously and wraps the result
    /// in a completed <see cref="ValueTask{TResult}"/>.
    /// </summary>
    /// <typeparam name="TValueTask">
    /// The expected value task type.
    /// </typeparam>
    /// <param name="expression">
    /// The expression to execute.
    /// </param>
    /// <returns>
    /// A completed <see cref="ValueTask{TResult}"/> containing the execution result.
    /// </returns>
    private TValueTask CreateValueTaskResult<TValueTask>(Expression expression)
    {
        ThrowIfNullProvider(expression);

        var result = InnerQueryProvider.Execute(expression);
        var resultType = typeof(TValueTask).GetGenericArguments()[0];

        return (TValueTask)_valueTaskFromResultMethodInfo
            .MakeGenericMethod(resultType)
            .Invoke(null, [result])!;
    }

    [MemberNotNull(nameof(InnerQueryProvider))]
    private void ThrowIfNullProvider(Expression expression)
    {
        if (InnerQueryProvider is null)
        {
            var message = string.Format(CultureInfo.InvariantCulture,
                                        _nullQueryProviderMessageFormat,
                                        expression);

            throw new InvalidOperationException(message);
        }
    }
    #endregion
}