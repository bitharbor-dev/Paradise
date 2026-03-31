namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.System.Collections.Generic;

/// <summary>
/// Fake <see cref="IAsyncEnumerator{T}"/> implementation.
/// </summary>
/// <typeparam name="T">
/// The type of objects to enumerate.
/// </typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeAsyncEnumerator{T}"/> class.
/// </remarks>
/// <param name="enumerator">
/// The underlying enumerator.
/// </param>
/// <param name="cancellationToken">
/// A <see cref="CancellationToken"/> to observe
/// while waiting for the task to complete.
/// </param>
public sealed class FakeAsyncEnumerator<T>(IEnumerator<T> enumerator, CancellationToken cancellationToken) : IAsyncEnumerator<T>
{
    #region Properties
    /// <inheritdoc/>
    public T Current
        => enumerator.Current;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        enumerator.Dispose();

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask<bool> MoveNextAsync()
    {
        cancellationToken.ThrowIfCancellationRequested();

        return ValueTask.FromResult(enumerator.MoveNext());
    }
    #endregion
}