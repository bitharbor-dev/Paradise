using Paradise.Domain.Base.Events;
using Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.Domain.Events;
using Paradise.WebApi.Services.Background;

namespace Paradise.WebApi.Tests.Unit.Services.Background;

/// <summary>
/// <see cref="DomainEventsDispatchingService"/> test class.
/// </summary>
public sealed class DomainEventsDispatchingServiceTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="DomainEventsDispatchingService.ExecuteAsync"/> method should
    /// delegate execution to <see cref="IDomainEventDispatcher.StartDispatchingAsync"/>
    /// and pass the same <see cref="CancellationToken"/>.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync()
    {
        // Arrange
        var dispatcher = new SpyDomainEventDispatcher();
        using var service = new DomainEventsDispatchingService(dispatcher);

        using var tokenSource = new CancellationTokenSource();

        // Act
        await service.StartAsync(tokenSource.Token);

        await service.ExecuteTask!
            .ConfigureAwait(true);

        await tokenSource.CancelAsync();

        // Assert
        Assert.True(dispatcher.StartDispatchingAsyncInvoked);
        Assert.True(dispatcher.ReceivedToken.HasValue);
        Assert.True(dispatcher.ReceivedToken.Value.IsCancellationRequested);
    }
    #endregion
}