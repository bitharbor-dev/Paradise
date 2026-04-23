using Paradise.Domain.Base.Events;
using Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.Domain.Events;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.Domain.Events;

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

/// <summary>
/// Fake <see cref="IDomainEventListener{TEvent}"/> implementation.
/// </summary>
public sealed class FakePrimaryDomainEventListener : FakeDomainEventListener;

/// <summary>
/// Fake <see cref="IDomainEventListener{TEvent}"/> implementation.
/// </summary>
public sealed class FakeSecondaryDomainEventListener : FakeDomainEventListener;