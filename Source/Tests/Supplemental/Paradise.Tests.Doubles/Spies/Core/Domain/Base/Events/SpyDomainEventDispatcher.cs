using Paradise.Domain.Base.Events;

namespace Paradise.Tests.Doubles.Spies.Core.Domain.Base.Events;

/// <summary>
/// Spy <see cref="IDomainEventDispatcher"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SpyDomainEventDispatcher"/> class.
/// </remarks>
/// <param name="eventSource">
/// The <see cref="IDomainEvent"/> source.
/// </param>
public sealed class SpyDomainEventDispatcher(IDomainEventSource eventSource) : IDomainEventDispatcher
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="SpyDomainEventDispatcher"/> class.
    /// </summary>
    public SpyDomainEventDispatcher() : this(null!) { }
    #endregion

    #region Properties
    /// <summary>
    /// Indicates whether the <see cref="StartDispatchingAsync"/> method was invoked.
    /// </summary>
    public bool StartDispatchingAsyncInvoked { get; private set; }

    /// <summary>
    /// The token passed to <see cref="StartDispatchingAsync"/>.
    /// </summary>
    public CancellationToken? ReceivedToken { get; private set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async Task StartDispatchingAsync(CancellationToken cancellationToken = default)
    {
        StartDispatchingAsyncInvoked = true;
        ReceivedToken = cancellationToken;

        if (eventSource is not null)
        {
            await foreach (var domainEvent in eventSource.PullAsync(cancellationToken).ConfigureAwait(false))
                DomainEventPulled?.Invoke(this, new(domainEvent));
        }
    }
    #endregion

    #region Events
    /// <summary>
    /// Occurs when a domain event is pulled from an internal event source.
    /// </summary>
    public event EventHandler<DomainEventPulledEventArgs>? DomainEventPulled;
    #endregion
}