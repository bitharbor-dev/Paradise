using Paradise.Domain.Base.Events;

namespace Paradise.Tests.Doubles.Spies.Core.Domain.Base.Events;

/// <summary>
/// Provides event data for the <see cref="SpyDomainEventDispatcher.DomainEventPulled"/> event.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DomainEventPulledEventArgs"/> class.
/// </remarks>
/// <param name="domainEvent">
/// The pulled domain event.
/// </param>
public sealed class DomainEventPulledEventArgs(IDomainEvent domainEvent) : EventArgs
{
    #region Properties
    /// <summary>
    /// The pulled domain event.
    /// </summary>
    public IDomainEvent DomainEvent { get; } = domainEvent;
    #endregion
}