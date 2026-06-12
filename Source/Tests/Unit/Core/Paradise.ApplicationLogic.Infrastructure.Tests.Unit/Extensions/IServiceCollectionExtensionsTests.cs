using Azure.Communication.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Infrastructure.Communication;
using Paradise.ApplicationLogic.Infrastructure.Communication.Email;
using Paradise.ApplicationLogic.Infrastructure.Communication.Email.Implementation;
using Paradise.ApplicationLogic.Infrastructure.DataProtection;
using Paradise.ApplicationLogic.Infrastructure.Domain.Events.Identity;
using Paradise.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.ApplicationLogic.Infrastructure.Extensions;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.ApplicationLogic.Infrastructure.Seed;
using Paradise.ApplicationLogic.Infrastructure.Services.Identity;
using Paradise.ApplicationLogic.Infrastructure.Services.MessageTemplates;
using Paradise.ApplicationLogic.Options.Models.DataAccess.Seed.Providers;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Communication.Email;
using Paradise.DataAccess.Seed.Providers;
using Paradise.Domain.Base.Events;
using Paradise.Primitives;
using Paradise.Tests.Extensibility;
using static System.Net.Mail.SmtpDeliveryMethod;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.Extensions;

/// <summary>
/// <see cref="IServiceCollectionExtensions"/> test class.
/// </summary>
public sealed partial class IServiceCollectionExtensionsTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="AddInfrastructure_InDevelopmentEnvironment"/> method.
    /// </summary>
    public static TheoryData<string> AddInfrastructure_InDevelopmentEnvironment_MemberData { get; } = new()
    {
        { EnvironmentNames.Development          },
        { EnvironmentNames.DevelopmentDocker    }
    };

    /// <summary>
    /// Provides member data for <see cref="AddInfrastructure_InNonDevelopmentEnvironment"/> method.
    /// </summary>
    public static TheoryData<string> AddInfrastructure_InNonDevelopmentEnvironment_MemberData { get; } = new()
    {
        { EnvironmentNames.Staging          },
        { EnvironmentNames.StagingDocker    },
        { EnvironmentNames.Production       },
        { EnvironmentNames.ProductionDocker }
    };

    /// <summary>
    /// Provides member data for <see cref="AddInfrastructure_AddsDomainEventsDispatching"/> method.
    /// </summary>
    public static TheoryData<string> AddInfrastructure_AddsDomainEventsDispatching_MemberData { get; }
        = [.. EnvironmentNames.AllowedEnvironments];

    /// <summary>
    /// Provides member data for <see cref="AddInfrastructure_AddsDomainEventsDispatching_WithGlobalRetryPolicy"/> method.
    /// </summary>
    public static TheoryData<string> AddInfrastructure_AddsDomainEventsDispatching_WithGlobalRetryPolicy_MemberData { get; }
        = [.. EnvironmentNames.AllowedEnvironments];
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddInfrastructure"/> method should
    /// configure the DI container to resolve core infrastructure services for development environment.
    /// </summary>
    /// <param name="environmentName">
    /// Current environment name.
    /// </param>
    [Theory, MemberData(nameof(AddInfrastructure_InDevelopmentEnvironment_MemberData))]
    public void AddInfrastructure_InDevelopmentEnvironment(string environmentName)
    {
        // Arrange
        var provider = Test.BuildInfrastructureServiceProvider(environmentName);

        // Act & Assert
        Assert.ServiceLifetime<IOptions<SmtpOptions>>(provider, ServiceLifetime.Singleton,
            options => Assert.Equivalent(Test.Options.SmtpOptions, options.Value));

        Assert.ServiceLifetime<ISmtpClient>(provider, ServiceLifetime.Scoped);

        Assert.ServiceLifetime<IEmailSender>(provider, ServiceLifetime.Scoped,
            emailSender => Assert.IsType<DefaultEmailSender>(emailSender));

        Assert.ServiceLifetime<ICommunicationClient>(provider, ServiceLifetime.Scoped);

        Assert.ServiceLifetime<IDataProtector>(provider, ServiceLifetime.Scoped);

        Assert.ServiceLifetime<IEmailTemplateService>(provider, ServiceLifetime.Scoped);

        Assert.ServiceLifetime<IOptions<JsonSeedDataProviderOptions>>(provider, ServiceLifetime.Singleton,
            options => Assert.Equivalent(Test.Options.JsonSeedDataProviderOptions, options.Value));

        Assert.ServiceLifetime<ISeedDataProvider>(provider, ServiceLifetime.Scoped);
        Assert.ServiceLifetime<IDatabaseSeeder>(provider, ServiceLifetime.Scoped);

        Assert.ServiceLifetime<IOptions<IdentityOptions>>(provider, ServiceLifetime.Singleton,
            options => Assert.Equivalent(Test.Options.IdentityOptions, options.Value));

        Assert.ServiceLifetime<IUserManager<User>>(provider, ServiceLifetime.Scoped);
        Assert.ServiceLifetime<IRoleManager<Role>>(provider, ServiceLifetime.Scoped);

        Assert.ServiceLifetime<IRoleService>(provider, ServiceLifetime.Scoped);
        Assert.ServiceLifetime<IUserService>(provider, ServiceLifetime.Scoped);
        Assert.ServiceLifetime<IUserRefreshTokenService>(provider, ServiceLifetime.Scoped);

        Assert.ServiceLifetimeEnumerable<IDomainEventListener<EmailAddressConfirmedEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<EmailAddressResetCompletedEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<EmailAddressResetRequestedEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<UserRegisteredEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<PasswordResetCompletedEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<PasswordResetRequestedEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<TwoFactorAuthenticationOccurringEvent>>(provider, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddInfrastructure"/> method should
    /// configure the DI container to resolve core infrastructure services for non-development environment.
    /// </summary>
    /// <param name="environmentName">
    /// Current environment name.
    /// </param>
    [Theory, MemberData(nameof(AddInfrastructure_InNonDevelopmentEnvironment_MemberData))]
    public void AddInfrastructure_InNonDevelopmentEnvironment(string environmentName)
    {
        // Arrange
        var provider = Test.BuildInfrastructureServiceProvider(environmentName);

        // Act & Assert
        Assert.ServiceLifetime<IOptions<SmtpOptions>>(provider, ServiceLifetime.Singleton,
            options => Assert.Equivalent(Test.Options.SmtpOptions, options.Value));

        Assert.ServiceLifetime<IEmailSender>(provider, ServiceLifetime.Scoped,
            emailSender => Assert.IsType<AzureEmailSender>(emailSender));

        Assert.ServiceLifetime<ICommunicationClient>(provider, ServiceLifetime.Scoped);

        Assert.ServiceLifetime<IDataProtector>(provider, ServiceLifetime.Scoped);

        Assert.ServiceLifetime<IEmailTemplateService>(provider, ServiceLifetime.Scoped);

        Assert.ServiceLifetime<IOptions<JsonSeedDataProviderOptions>>(provider, ServiceLifetime.Singleton,
            options => Assert.Equivalent(Test.Options.JsonSeedDataProviderOptions, options.Value));

        Assert.ServiceLifetime<ISeedDataProvider>(provider, ServiceLifetime.Scoped);
        Assert.ServiceLifetime<IDatabaseSeeder>(provider, ServiceLifetime.Scoped);

        Assert.ServiceLifetime<IOptions<IdentityOptions>>(provider, ServiceLifetime.Singleton,
            options => Assert.Equivalent(Test.Options.IdentityOptions, options.Value));

        Assert.ServiceLifetime<IUserManager<User>>(provider, ServiceLifetime.Scoped);
        Assert.ServiceLifetime<IRoleManager<Role>>(provider, ServiceLifetime.Scoped);

        Assert.ServiceLifetime<IRoleService>(provider, ServiceLifetime.Scoped);
        Assert.ServiceLifetime<IUserService>(provider, ServiceLifetime.Scoped);
        Assert.ServiceLifetime<IUserRefreshTokenService>(provider, ServiceLifetime.Scoped);

        Assert.ServiceLifetimeEnumerable<IDomainEventListener<EmailAddressConfirmedEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<EmailAddressResetCompletedEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<EmailAddressResetRequestedEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<UserRegisteredEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<PasswordResetCompletedEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<PasswordResetRequestedEvent>>(provider, ServiceLifetime.Singleton);
        Assert.ServiceLifetimeEnumerable<IDomainEventListener<TwoFactorAuthenticationOccurringEvent>>(provider, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddInfrastructure"/> method should
    /// register all configured domain event listeners.
    /// </summary>
    /// <param name="environmentName">
    /// Current environment name.
    /// </param>
    [Theory, MemberData(nameof(AddInfrastructure_AddsDomainEventsDispatching_MemberData))]
    public void AddInfrastructure_AddsDomainEventsDispatching(string environmentName)
    {
        // Arrange
        var provider = Test.BuildInfrastructureServiceProvider(environmentName);

        // Act
        var options = provider.GetService<IOptions<DomainEventRetryOptions>>()?.Value;

        // Assert
        Assert.NotNull(options);

        Assert.ServiceLifetimeEnumerable<IDomainEventDispatcher>(provider, ServiceLifetime.Singleton);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddInfrastructure"/> method should
    /// register all configured domain event listeners
    /// and configure global domain event retry policy.
    /// </summary>
    /// <param name="environmentName">
    /// Current environment name.
    /// </param>
    [Theory, MemberData(nameof(AddInfrastructure_AddsDomainEventsDispatching_WithGlobalRetryPolicy_MemberData))]
    public void AddInfrastructure_AddsDomainEventsDispatching_WithGlobalRetryPolicy(string environmentName)
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

        var provider = Test.BuildInfrastructureServiceProvider(environmentName, ConfigureOptions);

        // Act & Assert
        Assert.ServiceLifetime<IOptions<DomainEventRetryOptions>>(provider, ServiceLifetime.Singleton,
            options => Assert.Equivalent(expectedOptions, options.Value));
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddInfrastructure"/> method should
    /// configure the DI container to invoke the <see cref="ISmtpClient"/> factory which will
    /// setup the resulting instance to send emails over the network.
    /// </summary>
    [Fact]
    public void AddInfrastructure_SmtpClientFactory_ConfiguresNetworkSending()
    {
        // Arrange
        var provider = Test.BuildInfrastructureServiceProvider(EnvironmentNames.Development);

        // Act & Assert
        Assert.ServiceLifetime<ISmtpClient>(provider, ServiceLifetime.Scoped, client =>
        {
            var smtpClient = Assert.IsType<SmtpClient>(client);
            Assert.Equivalent(Test.Options.SmtpOptions!.Credentials, smtpClient.Credentials);
            Assert.Equal(Test.Options.SmtpOptions.Host, smtpClient.Host);
            Assert.Equal(Network, smtpClient.DeliveryMethod);
            Assert.Null(smtpClient.PickupDirectoryLocation);
        });
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddInfrastructure"/> method should
    /// configure the DI container to invoke the <see cref="ISmtpClient"/> factory which will
    /// setup the resulting instance to store emails locally instead of sending over the network.
    /// </summary>
    [Fact]
    public void AddInfrastructure_SmtpClientFactory_ConfiguresLocalEmailStorage()
    {
        // Arrange
        Test.Options.SmtpOptions!.LocalEmailStorage = Test.GetTemporaryDirectoryPath();

        var provider = Test.BuildInfrastructureServiceProvider(EnvironmentNames.Development);

        // Act & Assert
        Assert.ServiceLifetime<ISmtpClient>(provider, ServiceLifetime.Scoped, client =>
        {
            var smtpClient = Assert.IsType<SmtpClient>(client);
            Assert.Equal(SpecifiedPickupDirectory, smtpClient.DeliveryMethod);
            Assert.Equal(Test.Options.SmtpOptions.LocalEmailStorage, smtpClient.PickupDirectoryLocation);
        });
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddInfrastructure"/> method should
    /// configure the DI container to invoke the <see cref="ISmtpClient"/> factory which will
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// <see cref="SmtpOptions.Credentials"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void AddInfrastructure_SmtpClientFactory_ThrowsOnNullCredentials()
    {
        // Arrange
        Test.Options.SmtpOptions!.Credentials = null;

        var provider = Test.BuildInfrastructureServiceProvider(EnvironmentNames.Development);

        using var scope = provider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(scopedProvider.GetRequiredService<ISmtpClient>);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddInfrastructure"/> method should
    /// configure the DI container to invoke the <see cref="EmailClient"/> factory which will
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// <see cref="SmtpOptions.Credentials"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void AddInfrastructure_EmailClientFactory_ThrowsOnNullCredentials()
    {
        // Arrange
        Test.Options.SmtpOptions!.Credentials = null;

        var provider = Test.BuildInfrastructureServiceProvider(EnvironmentNames.Production);

        using var scope = provider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(scopedProvider.GetRequiredService<EmailClient>);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddInfrastructure"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// environment name is invalid.
    /// </summary>
    [Fact]
    public void AddInfrastructure_ThrowsOnInvalidEnvironmentName()
    {
        // Arrange
        var environmentName = "UnknownEnvironment";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(()
            => Test.BuildInfrastructureServiceProvider(environmentName));
    }
    #endregion
}