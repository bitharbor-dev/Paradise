using Paradise.Domain.Base.Events;

namespace Paradise.Tests.Doubles.Dummies.Core.Domain.Base.Events;

/// <summary>
/// Dummy <see cref="IDomainEventListener{TEvent}"/> implementation.
/// </summary>
public sealed class DummyDomainEventListener : IDomainEventListener<DummyDomainEvent>
{
    #region Public methods
    /// <inheritdoc/>
    public Task ProcessAsync(DummyDomainEvent domainEvent, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
    #endregion
}