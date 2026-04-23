using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.Domain.Base.Events;
using Paradise.Domain.Base.Events.Implementation;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.Domain.Events;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.Extensions.Logging;
using Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.Domain.Events;
using Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Core.Domain.Events;
using OptionsBuilder = Microsoft.Extensions.Options.Options;

namespace Paradise.Domain.Base.Tests.Unit.Events.Implementation;

public sealed partial class DomainEventDispatcherTests
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();

    /// <summary>
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </summary>
    public CancellationToken Token { get; } = TestContext.Current.CancellationToken;
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="DomainEventDispatcherTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Fields
        private readonly FakeLogger<DomainEventDispatcher> _logger = new();
        private readonly DomainEventRetryOptions _options = new();
        private readonly ServiceCollection _services = new();
        private readonly StubDomainEventSource _eventSource = new();
        #endregion

        #region Public methods
        /// <summary>
        /// Enqueues a new <see cref="IDomainEvent"/> instance for further dispatching.
        /// </summary>
        /// <returns>
        /// A queued event.
        /// </returns>
        public SpyDomainEvent EnqueueEvent()
        {
            var domainEvent = new SpyDomainEvent();
            _eventSource.Enqueue(domainEvent);

            return domainEvent;
        }

        /// <summary>
        /// Registers a <see cref="FakePrimaryDomainEventListener"/> in the dependency injection container,
        /// with optional listener-specific retry options configuration.
        /// </summary>
        /// <param name="configureOptions">
        /// Optional configuration delegate for <see cref="DomainEventRetryOptions"/> associated
        /// with the specified listener.
        /// <para>
        /// If provided, the options are registered as a keyed singleton tied to the listener type.
        /// </para>
        /// </param>
        /// <param name="processAsyncOverride">
        /// An overriding delegate for <see cref="IDomainEventListener{TEvent}.ProcessAsync(TEvent, CancellationToken)"/> method.
        /// </param>
        /// <param name="processingOrder">
        /// Defines the processing order of this event listener.
        /// </param>
        public void AddPrimaryDomainEventListener(Action<DomainEventRetryOptions>? configureOptions = null,
                                                  Func<SpyDomainEvent, CancellationToken, Task>? processAsyncOverride = null,
                                                  int? processingOrder = null)
        {
            if (processingOrder.HasValue)
            {
                AddDomainEventListener<FakeOrderedPrimaryDomainEventListener>(
                    configureOptions, processAsyncOverride, processingOrder);
            }
            else
            {
                AddDomainEventListener<FakePrimaryDomainEventListener>(
                    configureOptions, processAsyncOverride);
            }
        }

        /// <summary>
        /// Registers a <see cref="FakeSecondaryDomainEventListener"/> in the dependency injection container,
        /// with optional listener-specific retry options configuration.
        /// </summary>
        /// <param name="configureOptions">
        /// Optional configuration delegate for <see cref="DomainEventRetryOptions"/> associated
        /// with the specified listener.
        /// <para>
        /// If provided, the options are registered as a keyed singleton tied to the listener type.
        /// </para>
        /// </param>
        /// <param name="processAsyncOverride">
        /// An overriding delegate for <see cref="IDomainEventListener{TEvent}.ProcessAsync(TEvent, CancellationToken)"/> method.
        /// </param>
        /// <param name="processingOrder">
        /// Defines the processing order of this event listener.
        /// </param>
        public void AddSecondaryDomainEventListener(Action<DomainEventRetryOptions>? configureOptions = null,
                                                    Func<SpyDomainEvent, CancellationToken, Task>? processAsyncOverride = null,
                                                    int? processingOrder = null)
        {
            if (processingOrder.HasValue)
            {
                AddDomainEventListener<FakeOrderedSecondaryDomainEventListener>(
                    configureOptions, processAsyncOverride, processingOrder);
            }
            else
            {
                AddDomainEventListener<FakeSecondaryDomainEventListener>(
                    configureOptions, processAsyncOverride);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEventDispatcher"/> class.
        /// </summary>
        /// <returns>
        /// System under test.
        /// </returns>
        public DomainEventDispatcher CreateDispatcher()
            => new(_logger, OptionsBuilder.Create(_options), _services.BuildServiceProvider(), _eventSource);
        #endregion

        #region Private methods
        /// <summary>
        /// Registers a domain event listener in the dependency injection container,
        /// with optional listener-specific retry options configuration.
        /// </summary>
        /// <typeparam name="TListener">
        /// The listener type that will react to the domain event.
        /// </typeparam>
        /// <remarks>
        /// <para>
        /// This method adds <typeparamref name="TListener"/> as a scoped service that implements
        /// <see cref="IDomainEventListener{TEvent}"/>.
        /// </para>
        /// <para>
        /// If <paramref name="configureOptions"/> is specified,
        /// retry options are registered in the options system using the listener type as a key.
        /// </para>
        /// </remarks>
        /// <param name="configureOptions">
        /// Optional configuration delegate for <see cref="DomainEventRetryOptions"/> associated
        /// with the specified <typeparamref name="TListener"/>.
        /// <para>
        /// If provided, the options are registered as a keyed singleton tied to the listener type.
        /// </para>
        /// </param>
        /// <param name="processAsyncOverride">
        /// An overriding delegate for <see cref="IDomainEventListener{TEvent}.ProcessAsync(TEvent, CancellationToken)"/> method.
        /// </param>
        /// <param name="processingOrder">
        /// Defines the processing order of this event listener.
        /// </param>
        private void AddDomainEventListener<TListener>(Action<DomainEventRetryOptions>? configureOptions = null,
                                                       Func<SpyDomainEvent, CancellationToken, Task>? processAsyncOverride = null,
                                                       int? processingOrder = null)
            where TListener : class, IDomainEventListener<SpyDomainEvent>, new()
        {
            if (configureOptions is not null)
            {
                var optionsKey = typeof(TListener).AssemblyQualifiedName;

                _services.AddOptions();

                _services.Configure(optionsKey, configureOptions);

                _services.AddKeyedTransient(optionsKey, (provider, key) =>
                {
                    var monitor = provider.GetRequiredService<IOptionsMonitor<DomainEventRetryOptions>>();

                    return OptionsBuilder.Create(monitor.Get((string)key!));
                });
            }

            _services.AddSingleton<IDomainEventListener<SpyDomainEvent>>(_ =>
            {
                var listener = new TListener();

                if (processAsyncOverride is not null)
                {
                    if (listener is not FakeDomainEventListener fakeListener)
                        throw new InvalidOperationException();

                    fakeListener.ProcessAsyncResult = processAsyncOverride;
                }

                if (processingOrder.HasValue)
                {
                    if (listener is not FakeOrderedDomainEventListener fakeOrderedListener)
                    {
                        var message = $"{typeof(TListener)} does not implement configurable ordered interface.";

                        throw new InvalidOperationException(message);
                    }

                    fakeOrderedListener.SetProcessingOrder(processingOrder.Value);
                }

                return listener;
            });
        }
        #endregion
    }
    #endregion
}