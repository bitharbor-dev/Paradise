using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.System.Collections.Generic;

/// <summary>
/// Fake <see cref="IAsyncEnumerable{T}"/> implementation.
/// </summary>
/// <typeparam name="T">
/// The type of values to enumerate.
/// </typeparam>
public sealed class FakeAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeAsyncEnumerable{T}"/> class.
    /// </summary>
    /// <param name="enumerable">
    /// A collection to associate with the new instance.
    /// </param>
    public FakeAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeAsyncEnumerable{T}"/> class.
    /// </summary>
    /// <param name="expression">
    /// An expression tree to associate with the new instance.
    /// </param>
    public FakeAsyncEnumerable(Expression expression) : base(expression) { }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumerator = this.AsEnumerable().GetEnumerator();

        return new FakeAsyncEnumerator<T>(enumerator, cancellationToken);
    }

    IQueryProvider IQueryable.Provider
        => new FakeAsyncQueryProvider(this);
    #endregion
}