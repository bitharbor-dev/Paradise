using Paradise.Domain.Base.Events;
using Paradise.Tests.Doubles.Spies.Core.Domain.Base.Events;

namespace Paradise.Tests.Doubles.Fakes.Core.Domain.Base.Events;

/// <summary>
/// Fake <see cref="IDomainEventListener{TEvent}"/> implementation.
/// </summary>
public abstract class FakeDomainEventListener : IDomainEventListener<SpyDomainEvent>
{
    #region Properties
    /// <summary>
    /// <see cref="ProcessAsync"/> result.
    /// </summary>
    public Func<SpyDomainEvent, CancellationToken, Task>? ProcessAsyncResult { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task ProcessAsync(SpyDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (ProcessAsyncResult is not null)
            return ProcessAsyncResult(domainEvent, cancellationToken);

        ArgumentNullException.ThrowIfNull(domainEvent);

        domainEvent.Invocations++;

        return Task.CompletedTask;
    }
    #endregion
}