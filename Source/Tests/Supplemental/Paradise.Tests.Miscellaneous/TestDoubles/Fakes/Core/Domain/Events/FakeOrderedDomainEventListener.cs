using Paradise.Domain.Base.Events;
using Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.Domain.Events;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.Domain.Events;

/// <summary>
/// Fake <see cref="IOrderedDomainEventListener{TEvent}"/> implementation.
/// </summary>
public abstract class FakeOrderedDomainEventListener : FakeDomainEventListener, IOrderedDomainEventListener<SpyDomainEvent>
{
    #region Properties
    /// <inheritdoc/>
    public int ProcessingOrder { get; private set; }
    #endregion

    #region Public methods
    /// <summary>
    /// Sets the <see cref="ProcessingOrder"/> return value.
    /// </summary>
    /// <param name="value">
    /// The <see cref="ProcessingOrder"/> return value.
    /// </param>
    public void SetProcessingOrder(int value)
        => ProcessingOrder = value;
    #endregion
}

/// <summary>
/// Fake <see cref="IOrderedDomainEventListener{TEvent}"/> implementation.
/// </summary>
public sealed class FakeOrderedPrimaryDomainEventListener : FakeOrderedDomainEventListener;

/// <summary>
/// Fake <see cref="IOrderedDomainEventListener{TEvent}"/> implementation.
/// </summary>
public sealed class FakeOrderedSecondaryDomainEventListener : FakeOrderedDomainEventListener;