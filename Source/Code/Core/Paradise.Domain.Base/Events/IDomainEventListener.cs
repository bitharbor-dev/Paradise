namespace Paradise.Domain.Base.Events;

/// <summary>
/// Defines a listener that reacts to a specific type of domain event.
/// </summary>
/// <typeparam name="TEvent">
/// The type of domain event this listener handles.
/// </typeparam>
public interface IDomainEventListener<TEvent> where TEvent : IDomainEvent
{
    #region Methods
    /// <summary>
    /// Processes the given <paramref name="domainEvent"/>.
    /// </summary>
    /// <param name="domainEvent">
    /// The domain event instance to process.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task ProcessAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
    #endregion
}