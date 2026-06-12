using System.Linq.Expressions;

namespace Paradise.Tests.Doubles.Fakes.System.Collections.Generic;

/// <summary>
/// Fake <see cref="IAsyncEnumerable{T}"/> implementation.
/// </summary>
/// <typeparam name="T">
/// The type of values to enumerate.
/// </typeparam>
public sealed class FakeAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    #region Fields
    private readonly IQueryProvider _provider;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeAsyncEnumerable{T}"/> class.
    /// </summary>
    /// <param name="enumerable">
    /// A collection to associate with the new instance.
    /// </param>
    /// <param name="provider">
    /// The <see cref="IQueryProvider"/> that is associated with this data source.
    /// </param>
    public FakeAsyncEnumerable(IEnumerable<T> enumerable, IQueryProvider provider) : base(enumerable)
        => _provider = provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeAsyncEnumerable{T}"/> class.
    /// </summary>
    /// <param name="expression">
    /// An expression tree to associate with the new instance.
    /// </param>
    /// <param name="provider">
    /// The <see cref="IQueryProvider"/> that is associated with this data source.
    /// </param>
    public FakeAsyncEnumerable(Expression expression, IQueryProvider provider) : base(expression)
        => _provider = provider;
    #endregion

    #region Properties
    IQueryProvider IQueryable.Provider
        => _provider;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumerator = this.AsEnumerable().GetEnumerator();

        return new FakeAsyncEnumerator<T>(enumerator, cancellationToken);
    }
    #endregion
}