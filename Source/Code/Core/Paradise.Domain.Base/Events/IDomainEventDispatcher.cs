namespace Paradise.Domain.Base.Events;

/// <summary>
/// Provides domain events dispatching functionalities.
/// </summary>
public interface IDomainEventDispatcher
{
    #region Methods
    /// <summary>
    /// Starts the asynchronous dispatching of domain events.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task StartDispatchingAsync(CancellationToken cancellationToken = default);
    #endregion
}