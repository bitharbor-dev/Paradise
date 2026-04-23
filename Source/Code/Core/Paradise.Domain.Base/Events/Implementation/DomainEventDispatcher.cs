using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Paradise.Common.Extensions;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Paradise.Domain.Base.Events.Implementation;

/// <summary>
/// Provides domain events dispatching functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DomainEventDispatcher"/> class.
/// </remarks>
/// <param name="logger">
/// Logger.
/// </param>
/// <param name="retryOptions">
/// The accessor used to access the <see cref="DomainEventRetryOptions"/>.
/// </param>
/// <param name="serviceProvider">
/// The <see cref="IServiceProvider"/> to resolve dependencies.
/// </param>
/// <param name="eventSource">
/// The <see cref="IDomainEvent"/> source.
/// </param>
internal sealed class DomainEventDispatcher(ILogger<DomainEventDispatcher> logger,
                                            IOptions<DomainEventRetryOptions> retryOptions,
                                            IServiceProvider serviceProvider,
                                            IDomainEventSource eventSource) : IDomainEventDispatcher
{
    #region Fields
    private static readonly ConcurrentDictionary<Type, MethodInfo> _invocationCache = [];
    private readonly ConcurrentDictionary<Guid, Task> _processes = [];
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async Task StartDispatchingAsync(CancellationToken cancellationToken = default)
    {
        await foreach (var domainEvent in eventSource.PullAsync(cancellationToken).ConfigureAwait(false))
        {
            var listenerTask = InvokeListenersAsync(domainEvent, cancellationToken);
            var id = Guid.CreateVersion7();

            if (_processes.TryAdd(id, listenerTask))
            {
                _ = listenerTask.ContinueWith(__ =>
                {
                    _processes.TryRemove(id, out _);
                }, TaskScheduler.Default);
            }
        }
    }

    /// <summary>
    /// Waits asynchronously for all in-flight domain event processing (including retries) to complete.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> that completes when all currently executing listeners
    /// and their retries have finished.
    /// </returns>
    public Task WaitForCompletionAsync()
        => Task.WhenAll(_processes.Values);
    #endregion

    #region Private methods
    /// <summary>
    /// Dynamically invokes the appropriate generic dispatch method for the given event type.
    /// </summary>
    /// <param name="domainEvent">
    /// The event to dispatch.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    private Task InvokeListenersAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var method = _invocationCache.GetOrAdd(domainEvent.GetType(), GetDispatchInternalAsyncMethodInfo);

        return (Task)method.Invoke(this, [domainEvent, cancellationToken])!;
    }

    /// <summary>
    /// Resolves and constructs the generic method info for dispatching the specified event type.
    /// </summary>
    /// <param name="domainEventType">
    /// The concrete type of the domain event.
    /// </param>
    /// <returns>
    /// Constructed generic <see cref="MethodInfo"/> for dispatching the event.
    /// </returns>
    private static MethodInfo GetDispatchInternalAsyncMethodInfo(Type domainEventType)
    {
        var dispatcherType = typeof(DomainEventDispatcher);
        var methodName = nameof(DispatchInternalAsync);
        var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

        var nonGenericMethod = dispatcherType.GetMethod(methodName, bindingFlags);

        return nonGenericMethod!.MakeGenericMethod(domainEventType);
    }

    /// <summary>
    /// Dispatches a domain event to all registered listeners of the specific event type.
    /// </summary>
    /// <typeparam name="TEvent">
    /// The concrete type of the domain event.
    /// </typeparam>
    /// <param name="domainEvent">
    /// The event to dispatch.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    private async Task DispatchInternalAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken)
        where TEvent : IDomainEvent
    {
        var listeners = serviceProvider.GetServices<IDomainEventListener<TEvent>>();

        var orderedGroups = listeners
            .GroupBy(GetListenerProcessingOrder)
            .OrderBy(group => group.Key);

        foreach (var group in orderedGroups)
        {
            var tasks = group.Select(listener => ProcessInternalAsync(listener, domainEvent, cancellationToken));

            await Task.WhenAll(tasks)
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Retrieves the processing order for the specified domain event listener.
    /// </summary>
    /// <typeparam name="TEvent">
    /// The concrete type of the domain event.
    /// </typeparam>
    /// <param name="listener">
    /// The listener whose processing order should be determined.
    /// </param>
    /// <returns>
    /// The value of <see cref="IOrderedDomainEventListener{TEvent}.ProcessingOrder"/> if the
    /// listener implements <see cref="IOrderedDomainEventListener{TEvent}"/>,
    /// otherwise - <see cref="int.MaxValue"/>.
    /// </returns>
    [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Omitted for readability.")]
    private static int GetListenerProcessingOrder<TEvent>(IDomainEventListener<TEvent> listener) where TEvent : IDomainEvent
    {
        if (listener is IOrderedDomainEventListener<TEvent> orderedListener)
            return orderedListener.ProcessingOrder;

        return int.MaxValue;
    }

    /// <summary>
    /// Processes the <paramref name="domainEvent"/> by invoking the specified listener's handler method,
    /// applying retry logic in case of failure according to the configured retry options.
    /// </summary>
    /// <typeparam name="TEvent">
    /// The concrete type of the domain event.
    /// </typeparam>
    /// <param name="listener">
    /// The <see cref="IDomainEventListener{TEvent}"/> instance that will handle the event.
    /// </param>
    /// <param name="domainEvent">
    /// The <see cref="IDomainEvent"/> instance to be processed.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Background tasks should not throw.")]
    private async Task ProcessInternalAsync<TEvent>(IDomainEventListener<TEvent> listener,
                                                    TEvent domainEvent,
                                                    CancellationToken cancellationToken)
        where TEvent : IDomainEvent
    {
        var listenerType = listener.GetType();

        var listenerRetryOptions = (serviceProvider
            .GetKeyedService<IOptions<DomainEventRetryOptions>>(listenerType.AssemblyQualifiedName)
            ?? retryOptions).Value;

        ushort attempt = 0;
        while (attempt <= listenerRetryOptions.MaxRetries)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                await listener.ProcessAsync(domainEvent, cancellationToken)
                    .ConfigureAwait(false);

                break;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch when (attempt < listenerRetryOptions.MaxRetries)
            {
                try
                {
                    attempt++;

                    await listenerRetryOptions.DelayAsync(attempt, cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
            catch (Exception exception)
            {
                logger.LogUnhandledException(exception);

                break;
            }
        }
    }
    #endregion
}