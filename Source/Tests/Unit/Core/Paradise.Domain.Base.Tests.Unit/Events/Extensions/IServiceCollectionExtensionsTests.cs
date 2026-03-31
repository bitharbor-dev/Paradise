using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.Domain.Base.Events;
using Paradise.Domain.Base.Events.Extensions;
using Paradise.Domain.Base.Events.Implementation;
using Paradise.Tests.Miscellaneous;
using Paradise.Tests.Miscellaneous.TestDoubles.Dummies.Core.Domain.Events;

namespace Paradise.Domain.Base.Tests.Unit.Events.Extensions;

/// <summary>
/// <see cref="IServiceCollectionExtensions"/> test class.
/// </summary>
public sealed class IServiceCollectionExtensionsTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="AddDomainEventListener"/> method.
    /// </summary>
    public static TheoryData<bool> AddDomainEventListener_MemberData { get; } = new()
    {
        { true  },
        { false }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddDomainEventsDispatching"/> method should
    /// register all domain event dispatching related service into DI container.
    /// </summary>
    [Fact]
    public void AddDomainEventsDispatching()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddDomainEventsDispatching(o => { });

        var provider = services.BuildServiceProvider();

        // Act & Assert
        Assert.ServiceLifetime<IDomainEventDispatcher>(provider, ServiceLifetime.Singleton);

        Assert.ServiceLifetime<IDomainEventSink>(provider, ServiceLifetime.Singleton,
            sink => Assert.IsType<InMemoryDomainEventQueue>(sink));

        Assert.ServiceLifetime<IDomainEventSource>(provider, ServiceLifetime.Singleton,
            source => Assert.IsType<InMemoryDomainEventQueue>(source));

        Assert.ServiceLifetime<IOptions<DomainEventRetryOptions>>(provider, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddDomainEventListener{TListener, TEvent}"/> method should
    /// register a domain event listener for the specified domain event as well as
    /// configure the listener-specific <see cref="DomainEventRetryOptions"/>, if configuration provided.
    /// </summary>
    /// <param name="useKeyedOptions">
    /// Indicates whether the listener-specific <see cref="DomainEventRetryOptions"/> should be configured.
    /// </param>
    [Theory, MemberData(nameof(AddDomainEventListener_MemberData))]
    public void AddDomainEventListener(bool useKeyedOptions)
    {
        // Arrange
        Action<DomainEventRetryOptions>? configureOptions = useKeyedOptions ? options => { } : null;

        var provider = new ServiceCollection()
            .AddDomainEventListener<DummyDomainEventListener, DummyDomainEvent>(configureOptions)
            .BuildServiceProvider();

        // Act & Assert
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<DummyDomainEvent>>(provider, ServiceLifetime.Singleton, listeners =>
        {
            var listener = Assert.Single(listeners);
            Assert.IsType<DummyDomainEventListener>(listener);
        });

        if (useKeyedOptions)
        {
            var optionsKey = typeof(DummyDomainEventListener).AssemblyQualifiedName;

            Assert.ServiceLifetimeKeyed<IOptions<DomainEventRetryOptions>>(provider, ServiceLifetime.Transient, optionsKey);
        }
    }
    #endregion
}