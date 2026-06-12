using Paradise.Domain.Base.Events;

namespace Paradise.Tests.Doubles.Dummies.Core.Domain.Base.Events;

/// <summary>
/// Dummy <see cref="IDomainEvent"/> implementation.
/// </summary>
public sealed class DummyDomainEvent : IDomainEvent
{
    #region Properties
    /// <inheritdoc/>
    public DateTimeOffset OccurredOn { get; }
    #endregion
}