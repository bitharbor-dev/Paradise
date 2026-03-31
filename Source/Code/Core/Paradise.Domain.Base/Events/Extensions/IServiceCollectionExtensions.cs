using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.Domain.Base.Events.Implementation;

namespace Paradise.Domain.Base.Events.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IServiceCollection"/> <see langword="interface"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    #region Public methods
    /// <summary>
    /// Registers the services required for domain event dispatching, including
    /// the dispatcher and event sink/source integration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method configures the default in-memory domain event dispatching pipeline:
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// Registers a singleton implementation of <see cref="IDomainEventDispatcher"/>.
    /// </item>
    /// <item>
    /// Registers a singleton implementations of
    /// <see cref="IDomainEventSink"/> and <see cref="IDomainEventSource"/>.
    /// </item>
    /// <item>
    /// Registers <see cref="DomainEventRetryOptions"/> in the options system to allow
    /// configuration of retry behavior.
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configureOptions">
    /// An action used to configure global domain event retry policy.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddDomainEventsDispatching(this IServiceCollection services,
                                                                Action<DomainEventRetryOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddLogging()
            .Configure(configureOptions)
            .AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>()
            .AddSingleton<InMemoryDomainEventQueue>()
            .AddSingleton<IDomainEventSink>(provider => provider.GetRequiredService<InMemoryDomainEventQueue>())
            .AddSingleton<IDomainEventSource>(provider => provider.GetRequiredService<InMemoryDomainEventQueue>());
    }

    /// <summary>
    /// Registers a domain event listener in the dependency injection container,
    /// with optional listener-specific retry options configuration.
    /// </summary>
    /// <typeparam name="TListener">
    /// The listener type that will react to the domain event.
    /// </typeparam>
    /// <typeparam name="TEvent">
    /// The domain event type that the listener reacts to.
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
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configureOptions">
    /// Optional configuration delegate for <see cref="DomainEventRetryOptions"/> associated
    /// with the specified <typeparamref name="TListener"/>.
    /// <para>
    /// If provided, the options are registered as a keyed singleton tied to the listener type.
    /// </para>
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddDomainEventListener<TListener, TEvent>(this IServiceCollection services,
                                                                               Action<DomainEventRetryOptions>? configureOptions = null)
        where TListener : class, IDomainEventListener<TEvent>
        where TEvent : IDomainEvent
    {
        ArgumentNullException.ThrowIfNull(services);

        if (configureOptions is not null)
        {
            var optionsKey = typeof(TListener).AssemblyQualifiedName;

            services.AddOptions();

            services.Configure(optionsKey, configureOptions);

            services.AddKeyedTransient(optionsKey, (provider, key) =>
            {
                var monitor = provider.GetRequiredService<IOptionsMonitor<DomainEventRetryOptions>>();

                return Options.Create(monitor.Get((string)key!));
            });
        }

        return services.AddSingleton<IDomainEventListener<TEvent>, TListener>();
    }
    #endregion
}