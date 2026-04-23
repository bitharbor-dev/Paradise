using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Extensions;
using Paradise.ApplicationLogic.Options.Models;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Services;
using Paradise.ApplicationLogic.Services.Identity.Roles;
using Paradise.ApplicationLogic.Services.Identity.Users;
using Paradise.Common;
using Paradise.Domain.Base.Events;
using Paradise.Domain.Events.Identity.Users;
using Paradise.Tests.Miscellaneous;
using System.Text.Json;

namespace Paradise.ApplicationLogic.Tests.Unit.Extensions;

/// <summary>
/// <see cref="IServiceCollectionExtensions"/> test class.
/// </summary>
public sealed partial class IServiceCollectionExtensionsTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="AddApplicationLogic"/> method.
    /// </summary>
    public static TheoryData<string> AddApplicationLogic_MemberData { get; } = [.. EnvironmentNames.AllowedEnvironments];

    /// <summary>
    /// Provides member data for <see cref="AddDomainEvents"/> method.
    /// </summary>
    public static TheoryData<string> AddDomainEvents_MemberData { get; } = [.. EnvironmentNames.AllowedEnvironments];

    /// <summary>
    /// Provides member data for <see cref="AddDomainEvents_WithGlobalRetryPolicy"/> method.
    /// </summary>
    public static TheoryData<string> AddDomainEvents_WithGlobalRetryPolicy_MemberData { get; } = [.. EnvironmentNames.AllowedEnvironments];
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddApplicationLogic"/> method should
    /// configure the DI container to resolve core application logic services for the specified environment.
    /// </summary>
    /// <param name="environmentName">
    /// Current environment name.
    /// </param>
    [Theory, MemberData(nameof(AddApplicationLogic_MemberData))]
    public void AddApplicationLogic(string environmentName)
    {
        // Arrange
        var provider = Test.BuildApplicationLogicServiceProvider(environmentName);

        // Act & Assert
        Assert.ServiceLifetime<IOptions<ApplicationOptions>>(provider, ServiceLifetime.Singleton,
            options => Assert.Equivalent(Test.Options.ApplicationOptions, options.Value));

        Assert.ServiceLifetime<IOptions<EmailTemplateOptions>>(provider, ServiceLifetime.Singleton,
            options => Assert.Equivalent(Test.Options.EmailTemplateOptions, options.Value));

        Assert.ServiceLifetime<IOptions<JsonSerializerOptions>>(provider, ServiceLifetime.Singleton,
            options => Assert.Equivalent(Test.Options.JsonSerializerOptions, options.Value));

        Assert.ServiceLifetime<IRoleService>(provider, ServiceLifetime.Scoped);
        Assert.ServiceLifetime<IUserService>(provider, ServiceLifetime.Scoped);
        Assert.ServiceLifetime<IUserRefreshTokenService>(provider, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddDomainEvents"/> method should
    /// register all configured domain event listeners.
    /// </summary>
    /// <param name="environmentName">
    /// Current environment name.
    /// </param>
    [Theory, MemberData(nameof(AddDomainEvents_MemberData))]
    public void AddDomainEvents(string environmentName)
    {
        // Arrange
        var provider = Test.BuildApplicationLogicServiceProvider(environmentName);

        // Act
        var options = provider.GetService<IOptions<DomainEventRetryOptions>>()?.Value;

        // Assert
        Assert.NotNull(options);

        Assert.ServiceLifetimeEnumerable<IDomainEventDispatcher>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<EmailAddressConfirmedEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<EmailAddressResetCompletedEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<EmailAddressResetRequestedEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<UserRegisteredEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<PasswordResetCompletedEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<PasswordResetRequestedEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<TwoFactorAuthenticationOccurringEvent>>(provider, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddDomainEvents"/> method should
    /// register all configured domain event listeners
    /// and configure global domain event retry policy.
    /// </summary>
    /// <param name="environmentName">
    /// Current environment name.
    /// </param>
    [Theory, MemberData(nameof(AddDomainEvents_WithGlobalRetryPolicy_MemberData))]
    public void AddDomainEvents_WithGlobalRetryPolicy(string environmentName)
    {
        // Arrange
        static void ConfigureOptions(DomainEventRetryOptions options)
        {
            options.BaseDelay = TimeSpan.FromSeconds(2);
            options.MaxRetries = 10;
            options.UseExponentialBackOff = false;
        }

        var expectedOptions = new DomainEventRetryOptions();
        ConfigureOptions(expectedOptions);

        var provider = Test.BuildApplicationLogicServiceProvider(environmentName, ConfigureOptions);

        // Act & Assert
        Assert.ServiceLifetime<IOptions<DomainEventRetryOptions>>(provider, ServiceLifetime.Singleton,
            options => Assert.Equivalent(expectedOptions, options.Value));
    }
    #endregion
}