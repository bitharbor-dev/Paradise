using Paradise.Domain.Base.Events;
using Paradise.Domain.Base.Events.Implementation;
using Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Events;

namespace Paradise.Domain.Base.Tests.Unit.Events.Implementation;

/// <summary>
/// <see cref="InMemoryDomainEventQueue"/> test class.
/// </summary>
public sealed class InMemoryDomainEventQueueTests
{
    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    internal InMemoryDomainEventQueue Queue { get; } = new();

    /// <summary>
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </summary>
    public CancellationToken Token { get; } = TestContext.Current.CancellationToken;
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="InMemoryDomainEventQueue.PullAsync"/> method should
    /// return each <see cref="IDomainEvent"/> pushed in the same order.
    /// </summary>
    [Fact]
    public async Task PullAsync_ReturnsEventsInPushOrder()
    {
        // Arrange
        var first = new TestDomainEvent();
        var second = new TestDomainEvent();

        var events = new List<IDomainEvent>();

        // Act
        await Queue.PushAsync(first, Token);
        await Queue.PushAsync(second, Token);

        await foreach (var domainEvent in Queue.PullAsync(Token))
        {
            events.Add(domainEvent);

            if (events.Count is 2)
                break;
        }

        // Assert
        Assert.Collection(events, domainEvent => Assert.Same(first, domainEvent),
                                  domainEvent => Assert.Same(second, domainEvent));
    }

    /// <summary>
    /// The <see cref="InMemoryDomainEventQueue.PullAsync"/> method should
    /// return the pushed <see cref="IDomainEvent"/> instance.
    /// </summary>
    [Fact]
    public async Task PullAsync_ReturnsPushedEvent()
    {
        // Arrange
        var domainEvent = new TestDomainEvent();

        // Act
        await Queue.PushAsync(domainEvent, Token);

        var enumerator = Queue
            .PullAsync(Token)
            .GetAsyncEnumerator(Token);

        await enumerator.MoveNextAsync();

        // Assert
        Assert.Same(domainEvent, enumerator.Current);
    }

    /// <summary>
    /// The <see cref="InMemoryDomainEventQueue.PullAsync"/> method should
    /// suspend the enumeration, until the new <see cref="IDomainEvent"/> instance is pushed.
    /// </summary>
    [Fact]
    public async Task PullAsync_SuspendsUntilEventIsPushed()
    {
        // Arrange
        var domainEvent = new TestDomainEvent();

        var enumerator = Queue
            .PullAsync(Token)
            .GetAsyncEnumerator(Token);

        var moveNext = enumerator.MoveNextAsync();

        // Act
        await Queue.PushAsync(domainEvent, Token);

        // Assert
        var enumeratorAdvanced = await moveNext
            .ConfigureAwait(true);

        Assert.True(enumeratorAdvanced);
    }
    #endregion
}