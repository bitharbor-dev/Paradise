using Paradise.Domain.Base.Events;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.Domain.Events;

/// <summary>
/// Provides event data for the <see cref="SpyDomainEventSink.Pushed"/> event.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DomainEventPushedEventArgs"/> class.
/// </remarks>
/// <param name="domainEvent">
/// The pushed domain event.
/// </param>
public sealed class DomainEventPushedEventArgs(IDomainEvent domainEvent) : EventArgs
{
    #region Properties
    /// <summary>
    /// The pushed domain event.
    /// </summary>
    public IDomainEvent DomainEvent { get; } = domainEvent;
    #endregion
}