using Paradise.Domain.Base.Events;

namespace Paradise.WebApi.Services.Background;

/// <summary>
/// A background service which starts the <see cref="IDomainEventDispatcher"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DomainEventsDispatchingService"/> class.
/// </remarks>
/// <param name="dispatcher">
/// The <see cref="IDomainEventDispatcher"/> instance used to dispatch the events.
/// </param>
internal sealed class DomainEventsDispatchingService(IDomainEventDispatcher dispatcher) : BackgroundService
{
    #region Protected methods
    /// <inheritdoc/>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => dispatcher.StartDispatchingAsync(stoppingToken);
    #endregion
}