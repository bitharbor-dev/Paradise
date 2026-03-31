using Paradise.Domain.Base.Events;
using System.Runtime.CompilerServices;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Core.Domain.Events;

/// <summary>
/// Stub <see cref="IDomainEventSource"/> implementation.
/// </summary>
/// <remarks>
/// This implementation does not implement real streaming behavior (indefinite event awaiting),
/// and is intended to be used in tests where the source of domain events is not relevant,
/// or when the test needs to enqueue specific domain events to be pulled by the dispatcher.
/// <para>
/// Real streaming behavior must be tested in integration tests
/// with a real implementation of <see cref="IDomainEventSource"/>.
/// </para>
/// </remarks>
public sealed class StubDomainEventSource : IDomainEventSource
{
    #region Fields
    private readonly IList<IDomainEvent> _currentEvents = [];
    #endregion

    #region Public methods
    /// <summary>
    /// Enqueues a domain event to the source.
    /// </summary>
    /// <param name="domainEvent">
    /// The <see cref="IDomainEvent"/> to enqueue.
    /// </param>
    public void Enqueue(IDomainEvent domainEvent)
        => _currentEvents.Add(domainEvent);

    /// <inheritdoc/>
    public async IAsyncEnumerable<IDomainEvent> PullAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var @event in _currentEvents)
        {
            yield return @event;
            await Task.Yield();
        }
    }
    #endregion
}