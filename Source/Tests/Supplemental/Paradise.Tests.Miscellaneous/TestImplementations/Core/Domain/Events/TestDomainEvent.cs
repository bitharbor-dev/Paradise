using Paradise.Domain.Base.Events.Base;

namespace Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Events;

/// <summary>
/// Test <see cref="DomainEventBase"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TestDomainEvent"/> class.
/// </remarks>
/// <param name="occurredOn">
/// The timestamp indicating when the domain event occurred.
/// </param>
public sealed class TestDomainEvent(DateTimeOffset? occurredOn = null) : DomainEventBase(occurredOn ?? DateTimeOffset.UnixEpoch);