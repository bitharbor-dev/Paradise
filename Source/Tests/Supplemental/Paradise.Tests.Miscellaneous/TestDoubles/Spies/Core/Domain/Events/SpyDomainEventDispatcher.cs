using Paradise.Domain.Base.Events;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.Domain.Events;

/// <summary>
/// Spy <see cref="IDomainEventDispatcher"/> implementation.
/// </summary>
public sealed class SpyDomainEventDispatcher : IDomainEventDispatcher
{
    #region Properties
    /// <summary>
    /// Indicates whether the <see cref="StartDispatchingAsync"/> method was invoked.
    /// </summary>
    public bool StartDispatchingAsyncInvoked { get; private set; }

    /// <summary>
    /// The token passed to <see cref="StartDispatchingAsync"/>.
    /// </summary>
    public CancellationToken? ReceivedToken { get; private set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task StartDispatchingAsync(CancellationToken cancellationToken = default)
    {
        StartDispatchingAsyncInvoked = true;
        ReceivedToken = cancellationToken;
        return Task.CompletedTask;
    }
    #endregion
}