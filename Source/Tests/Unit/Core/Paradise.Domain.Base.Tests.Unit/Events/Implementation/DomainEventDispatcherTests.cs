using Paradise.Domain.Base.Events;
using Paradise.Domain.Base.Events.Implementation;
using Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.Domain.Events;

namespace Paradise.Domain.Base.Tests.Unit.Events.Implementation;

/// <summary>
/// <see cref="DomainEventDispatcher"/> test class.
/// </summary>
public sealed partial class DomainEventDispatcherTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="StartDispatchingAsync_ProcessesWithSingleListener"/> method.
    /// </summary>
    public static TheoryData<ushort> StartDispatchingAsync_ProcessesWithSingleListener_MemberData { get; } = new()
    {
        { 1 },
        { 3 }
    };

    /// <summary>
    /// Provides member data for <see cref="StartDispatchingAsync_StopsWhenCancelled"/> method.
    /// </summary>
    public static TheoryData<bool, int, bool> StartDispatchingAsync_StopsWhenCancelled_MemberData { get; } = new()
    {
        { true,     0,  false   },
        { false,    1,  true    },
        { false,    1,  false   }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="DomainEventDispatcher.StartDispatchingAsync"/> method should
    /// process all consumed events via single event listener.
    /// </summary>
    /// <param name="numberOfEvents">
    /// The number of events to process.
    /// </param>
    [Theory, MemberData(nameof(StartDispatchingAsync_ProcessesWithSingleListener_MemberData))]
    public async Task StartDispatchingAsync_ProcessesWithSingleListener(ushort numberOfEvents)
    {
        // Arrange
        Test.AddPrimaryDomainEventListener();

        var dispatcher = Test.CreateDispatcher();

        var events = new List<SpyDomainEvent>(numberOfEvents);

        for (var i = 0; i < numberOfEvents; i++)
            events.Add(Test.EnqueueEvent());

        // Act
        await dispatcher.StartDispatchingAsync(Token);
        await dispatcher.WaitForCompletionAsync();

        // Assert
        Assert.All(events, domainEvent => Assert.Equal(1, domainEvent.Invocations));
    }

    /// <summary>
    /// The <see cref="DomainEventDispatcher.StartDispatchingAsync"/> method should
    /// process all consumed events via multiple event listeners.
    /// </summary>
    [Fact]
    public async Task StartDispatchingAsync_ProcessesWithMultipleListeners()
    {
        // Arrange
        var listenerAInvoked = false;
        var listenerBInvoked = false;

        Test.AddPrimaryDomainEventListener(processAsyncOverride: (e, c) =>
        {
            listenerAInvoked = true;

            e.Invocations++;
            return Task.CompletedTask;
        });

        Test.AddSecondaryDomainEventListener(processAsyncOverride: (e, c) =>
        {
            listenerBInvoked = true;

            e.Invocations++;
            return Task.CompletedTask;
        });

        var dispatcher = Test.CreateDispatcher();

        var domainEvent = Test.EnqueueEvent();

        // Act
        await dispatcher.StartDispatchingAsync(Token);
        await dispatcher.WaitForCompletionAsync();

        // Assert
        Assert.True(listenerAInvoked);
        Assert.True(listenerBInvoked);
        Assert.Equal(2, domainEvent.Invocations);
    }

    /// <summary>
    /// The <see cref="DomainEventDispatcher.StartDispatchingAsync"/> method should
    /// retry to invoke a listener on a particular event after previous attempt failed.
    /// </summary>
    [Fact]
    public async Task StartDispatchingAsync_RetriesAfterFailure()
    {
        // Arrange
        ushort retries = 3;
        var totalProcessings = retries + 1;

        Test.AddPrimaryDomainEventListener(configureOptions: options =>
        {
            options.MaxRetries = retries;
            options.BaseDelay = TimeSpan.Zero;
            options.UseExponentialBackOff = false;
        },
        processAsyncOverride: (e, c) =>
        {
            e.Invocations++;

            throw new InvalidOperationException();
        });

        var dispatcher = Test.CreateDispatcher();

        var domainEvent = Test.EnqueueEvent();

        // Act
        await dispatcher.StartDispatchingAsync(Token);
        await dispatcher.WaitForCompletionAsync();

        // Assert
        Assert.Equal(totalProcessings, domainEvent.Invocations);
    }

    /// <summary>
    /// The <see cref="DomainEventDispatcher.StartDispatchingAsync"/> method should
    /// stop dispatching the events when input cancellation token signals operation cancellation.
    /// </summary>
    /// <remarks>
    /// TODO: Split into multiple, more simple tests.
    /// </remarks>
    /// <param name="isGracefulCancellation">
    /// Indicates whetter the cancellation should happen before
    /// looping over the queued events.
    /// </param>
    /// <param name="expectedInvocations">
    /// Expected number of <see cref="IDomainEventListener{TEvent}.ProcessAsync"/>
    /// invocations over a particular domain event.
    /// </param>
    /// <param name="cancelsFromListener">
    /// Indicates whether the cancellation should happen inside
    /// the processing listener.
    /// </param>
    [Theory, MemberData(nameof(StartDispatchingAsync_StopsWhenCancelled_MemberData))]
    public async Task StartDispatchingAsync_StopsWhenCancelled(bool isGracefulCancellation,
                                                               int expectedInvocations,
                                                               bool cancelsFromListener)
    {
        // Arrange
        using var tokenSource = new CancellationTokenSource();

        if (isGracefulCancellation)
            await tokenSource.CancelAsync();

        Test.AddPrimaryDomainEventListener(configureOptions: options =>
        {
            options.MaxRetries = 3;
            options.BaseDelay = TimeSpan.Zero;
            options.UseExponentialBackOff = false;

            if (!cancelsFromListener && !isGracefulCancellation)
                options.Delaying += (s, e) => tokenSource.Cancel();
        },
        processAsyncOverride: async (e, c) =>
        {
            e.Invocations++;

            if (cancelsFromListener)
            {
                await tokenSource.CancelAsync()
                    .ConfigureAwait(true);

                c.ThrowIfCancellationRequested();
            }

            throw new InvalidOperationException();
        });

        var dispatcher = Test.CreateDispatcher();

        var domainEvent = Test.EnqueueEvent();

        // Act
        await dispatcher.StartDispatchingAsync(tokenSource.Token);
        await dispatcher.WaitForCompletionAsync();

        // Assert
        Assert.Equal(expectedInvocations, domainEvent.Invocations);
    }

    /// <summary>
    /// The <see cref="DomainEventDispatcher.StartDispatchingAsync"/> method should
    /// not dispatch the events for which no listener has been registered.
    /// </summary>
    [Fact]
    public async Task StartDispatchingAsync_IgnoresNonMappedEvents()
    {
        // Arrange
        var dispatcher = Test.CreateDispatcher();

        var domainEvent = Test.EnqueueEvent();

        // Act
        await dispatcher.StartDispatchingAsync(Token);
        await dispatcher.WaitForCompletionAsync();

        // Assert
        Assert.Equal(0, domainEvent.Invocations);
    }

    /// <summary>
    /// The <see cref="DomainEventDispatcher.StartDispatchingAsync"/> method should
    /// concurrently dispatch listeners with the same <see cref="IOrderedDomainEventListener{TEvent}.ProcessingOrder"/>.
    /// </summary>
    [Fact]
    public async Task StartDispatchingAsync_InvokesConcurrentlyOnSameOrder()
    {
        // Arrange
        var entered = 0;
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Task ProcessAsync(SpyDomainEvent e, CancellationToken c)
        {
            e.Invocations++;
            Interlocked.Increment(ref entered);
            return gate.Task;
        }

        Test.AddPrimaryDomainEventListener(processingOrder: 1, processAsyncOverride: ProcessAsync);
        Test.AddSecondaryDomainEventListener(processingOrder: 1, processAsyncOverride: ProcessAsync);

        var dispatcher = Test.CreateDispatcher();
        var domainEvent = Test.EnqueueEvent();

        // Act
        await dispatcher.StartDispatchingAsync(Token);

        while (Volatile.Read(ref entered) is not 2)
            await Task.Yield();

        gate.SetResult();

        // Assert
        Assert.Equal(2, entered);
        Assert.Equal(2, domainEvent.Invocations);
    }

    /// <summary>
    /// The <see cref="DomainEventDispatcher.StartDispatchingAsync"/> method should
    /// sequentially dispatch listeners with the different <see cref="IOrderedDomainEventListener{TEvent}.ProcessingOrder"/>.
    /// </summary>
    [Fact]
    public async Task StartDispatchingAsync_InvokesSequentiallyOnDifferentOrder()
    {
        // Arrange
        var magicNumbers = new List<int>();

        Test.AddPrimaryDomainEventListener(processingOrder: 1, processAsyncOverride: (e, c) =>
        {
            magicNumbers.Add(1);

            e.Invocations++;
            return Task.CompletedTask;
        });

        Test.AddSecondaryDomainEventListener(processingOrder: 2, processAsyncOverride: (e, c) =>
        {
            magicNumbers.Add(2);

            e.Invocations++;
            return Task.CompletedTask;
        });

        var dispatcher = Test.CreateDispatcher();

        var domainEvent = Test.EnqueueEvent();

        // Act
        await dispatcher.StartDispatchingAsync(Token);
        await dispatcher.WaitForCompletionAsync();

        // Assert
        Assert.Collection(magicNumbers, processingOrder => Assert.Equal(1, processingOrder),
                                        processingOrder => Assert.Equal(2, processingOrder));
        Assert.Equal(2, domainEvent.Invocations);
    }
    #endregion
}