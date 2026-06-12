using Paradise.Domain.Base.Events;
using Paradise.Tests.Doubles.Spies.Core.Domain.Base.Events;

namespace Paradise.Tests.Doubles.Fakes.Core.Domain.Base.Events;

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