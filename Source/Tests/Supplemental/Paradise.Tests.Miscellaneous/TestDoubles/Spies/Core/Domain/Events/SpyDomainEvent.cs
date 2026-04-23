using Paradise.Domain.Base.Events;
using Paradise.Domain.Base.Events.Base;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.Domain.Events;

/// <summary>
/// Spy <see cref="IDomainEvent"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SpyDomainEvent"/> class.
/// </remarks>
/// <param name="occurredOn">
/// The timestamp indicating when the domain event occurred.
/// </param>
public sealed class SpyDomainEvent(DateTimeOffset? occurredOn = null) : DomainEventBase(occurredOn ?? DateTimeOffset.UnixEpoch)
{
    #region Properties
    /// <summary>
    /// The number of <see cref="IDomainEventListener{TEvent}.ProcessAsync"/>
    /// method invocations on the current instance.
    /// </summary>
    public ushort Invocations { get; set; }
    #endregion
}