namespace Paradise.Domain.Base.Events;

/// <summary>
/// Represents a source of domain events that can be pulled asynchronously.
/// </summary>
public interface IDomainEventSource
{
    #region Methods
    /// <summary>
    /// Asynchronously pulls all domain events from the queue as they become available.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IAsyncEnumerable{T}"/> of <see cref="IDomainEvent"/>.
    /// </returns>
    IAsyncEnumerable<IDomainEvent> PullAsync(CancellationToken cancellationToken = default);
    #endregion
}