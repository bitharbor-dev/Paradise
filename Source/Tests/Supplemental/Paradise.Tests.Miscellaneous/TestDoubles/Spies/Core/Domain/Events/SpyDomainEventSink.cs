using Paradise.Domain.Base.Events;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.Domain.Events;

/// <summary>
/// Spy <see cref="IDomainEventSink"/> implementation.
/// </summary>
public sealed class SpyDomainEventSink : IDomainEventSink
{
    #region Public methods
    /// <inheritdoc/>
    public ValueTask PushAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Pushed?.Invoke(this, new(domainEvent));

        return ValueTask.CompletedTask;
    }
    #endregion

    #region Events
    /// <summary>
    /// Occurs when a domain event is pushed to the sink.
    /// </summary>
    public event EventHandler<DomainEventPushedEventArgs>? Pushed;
    #endregion
}