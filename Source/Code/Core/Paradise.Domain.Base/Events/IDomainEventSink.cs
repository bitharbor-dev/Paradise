namespace Paradise.Domain.Base.Events;

/// <summary>
/// Represents a target for domain events that can be pushed for later processing.
/// </summary>
public interface IDomainEventSink
{
    #region Methods
    /// <summary>
    /// Pushes a <see cref="IDomainEvent"/> for later processing.
    /// </summary>
    /// <param name="domainEvent">
    /// The <see cref="IDomainEvent"/> to push.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> that represents the asynchronous push operation.
    /// </returns>
    ValueTask PushAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    #endregion
}