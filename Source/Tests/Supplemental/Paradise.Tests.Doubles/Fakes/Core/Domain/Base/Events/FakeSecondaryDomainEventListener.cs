using Paradise.Domain.Base.Events;

namespace Paradise.Tests.Doubles.Fakes.Core.Domain.Base.Events;

/// <summary>
/// Fake <see cref="IDomainEventListener{TEvent}"/> implementation.
/// </summary>
public sealed class FakeSecondaryDomainEventListener : FakeDomainEventListener;