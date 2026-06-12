using Paradise.Domain.Base.Events;

namespace Paradise.Tests.Doubles.Fakes.Core.Domain.Base.Events;

/// <summary>
/// Fake <see cref="IOrderedDomainEventListener{TEvent}"/> implementation.
/// </summary>
public sealed class FakeOrderedSecondaryDomainEventListener : FakeOrderedDomainEventListener;