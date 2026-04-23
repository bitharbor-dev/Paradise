using Azure.Communication.Email;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Infrastructure.Communication;
using Paradise.ApplicationLogic.Infrastructure.Communication.Email;
using Paradise.ApplicationLogic.Infrastructure.Communication.Email.Implementation;
using Paradise.ApplicationLogic.Infrastructure.Communication.Implementation;
using Paradise.ApplicationLogic.Infrastructure.DataProtection.Implementation;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.ApplicationLogic.Infrastructure.Identity.Implementation;
using Paradise.ApplicationLogic.Infrastructure.Seed;
using Paradise.ApplicationLogic.Infrastructure.Seed.Implementation;
using Paradise.ApplicationLogic.Infrastructure.Services;
using Paradise.ApplicationLogic.Infrastructure.Services.Implementation;
using Paradise.ApplicationLogic.Options.Extensions;
using Paradise.ApplicationLogic.Options.Models.DataAccess.Seed.Providers;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Communication.Email;
using Paradise.Common.Extensions;
using Paradise.DataAccess.Extensions;
using Paradise.DataAccess.Seed.Providers;
using Paradise.DataAccess.Seed.Providers.Implementation;
using Paradise.Domain.Identity.Roles;
using Paradise.Domain.Identity.Users;
using static Paradise.Common.EnvironmentNames;
using static Paradise.Localization.ExceptionHandling.ExceptionMessages;

namespace Paradise.ApplicationLogic.Infrastructure.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IServiceCollection"/> <see langword="interface"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    #region Constants
    private const string ApplicationName = "Paradise";
    #endregion

    #region Public methods
    /// <summary>
    /// Registers the core infrastructure services required by the application,
    /// including data access, communication, identity, and supporting utilities.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method composes the application's infrastructure by registering:
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// Domain events dispatching.
    /// </item>
    /// <item>
    /// Database access and persistence.
    /// </item>
    /// <item>
    /// Communication services (SMTP, email client).
    /// </item>
    /// <item>
    /// Data protection services.
    /// </item>
    /// <item>
    /// ASP.NET Core Identity services.
    /// </item>
    /// <item>
    /// Database seeding services.
    /// </item>
    /// <item>
    /// Logging services.
    /// </item>
    /// </list>
    /// <para>
    /// This provides a single entry point for registering all infrastructure-level
    /// dependencies, simplifying application startup configuration.
    /// </para>
    /// </remarks>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configuration">
    /// The <see cref="IConfiguration"/> instance used to configure infrastructure components.
    /// </param>
    /// <param name="environmentName">
    /// Current environment name.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration,
                                                       string environmentName)
    {
        return services
            .AddDataAccess(configuration)
            .AddCommunication(configuration, environmentName)
            .AddDataProtector()
            .AddSeeding(configuration)
            .AddIdentityServices(configuration)
            .AddLogging();
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Registers the communication-related services, including SMTP client configuration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The method chooses which <see cref="IEmailSender"/> implementation to register based on the
    /// current environment.
    /// </para>
    /// <para>
    /// Additionally, <see cref="SmtpOptions"/> are bound from configuration and validated on startup
    /// using data annotations.
    /// </para>
    /// <para>
    /// This method requires the <see cref="DataAccess.Extensions.IServiceCollectionExtensions.AddDataAccess"/> to be called.
    /// </para>
    /// </remarks>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configuration">
    /// The application <see cref="IConfiguration"/> used to bind the <see cref="SmtpOptions"/>.
    /// </param>
    /// <param name="environmentName">
    /// Current environment name.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    private static IServiceCollection AddCommunication(this IServiceCollection services, IConfiguration configuration,
                                                       string environmentName)
    {
        services.AddOptions<SmtpOptions>(configuration, validateOnStartup: true, validateDataAnnotations: true);

        switch (environmentName)
        {
            case Development:
            case DevelopmentDocker:
                {
                    services.AddScoped<ISmtpClient>(provider =>
                    {
                        var options = provider.GetRequiredService<IOptions<SmtpOptions>>().Value;

                        return GetSmtpClient(options);
                    });

                    services.AddScoped<IEmailSender, DefaultEmailSender>();

                    break;
                }
            case Staging:
            case StagingDocker:
            case Production:
            case ProductionDocker:
                {
                    services.AddScoped(provider =>
                    {
                        var options = provider.GetRequiredService<IOptions<SmtpOptions>>().Value;

                        return GetEmailClient(options);
                    });

                    services.AddScoped<IEmailSender, AzureEmailSender>();

                    break;
                }
            default:
                {
                    var message = GetMessageInvalidEnvironmentName(environmentName, AllowedEnvironments);

                    throw new InvalidOperationException(message);
                }
        }

        return services.AddScoped<ICommunicationClient, CommunicationClient>();
    }

    /// <summary>
    /// Registers data protection services for the application, including key storage
    /// and a custom <see cref="DataProtector"/> wrapper.
    /// </summary>
    /// <remarks>
    /// This method requires the <see cref="DataAccess.Extensions.IServiceCollectionExtensions.AddDataAccess"/> to be called.
    /// </remarks>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    private static IServiceCollection AddDataProtector(this IServiceCollection services)
    {
        services
            .AddDataProtection()
            .SetApplicationName(ApplicationName)
            .PersistKeysToDataAccess();

        return services.AddScoped<DataProtection.IDataProtector, DataProtector>();
    }

    /// <summary>
    /// Registers application data seeding services.
    /// </summary>
    /// <remarks>
    /// This method requires the <see cref="DataAccess.Extensions.IServiceCollectionExtensions.AddDataAccess"/> to be called.
    /// </remarks>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configuration">
    /// The application <see cref="IConfiguration"/> used to bind the <see cref="JsonSeedDataProviderOptions"/>.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    private static IServiceCollection AddSeeding(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddScoped<IEmailTemplateService, EmailTemplateService>()
            .AddOptions<JsonSeedDataProviderOptions>(configuration, validateOnStartup: true, validateDataAnnotations: true)
            .AddScoped<ISeedDataProvider>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<JsonSeedDataProviderOptions>>().Value;

                var path = options.ResolveSeedDirectoryPath();

                return new JsonSeedDataProvider(path);
            })
            .AddScoped<IDatabaseSeeder, DatabaseSeeder>();
    }

    /// <summary>
    /// Registers ASP.NET Core Identity services, including user, role, and store support.
    /// </summary>
    /// <remarks>
    /// This setup enables the use of <see cref="UserManager{TUser}"/>, <see cref="RoleManager{TRole}"/>,
    /// and related Identity services without requiring cookie authentication, making it suitable
    /// for API-based scenarios.
    /// <para>
    /// This method requires the <see cref="DataAccess.Extensions.IdentityBuilderExtensions.AddDataAccessStores"/> to be called.
    /// </para>
    /// </remarks>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configuration">
    /// The <see cref="IConfiguration"/> instance used to bind the <see cref="IdentityOptions"/>.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    private static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<IdentityOptions>(configuration, validateOnStartup: true, validateDataAnnotations: true)
            .AddIdentityCore<User>(configuration.BindSection)
            .AddRoles<Role>()
            .AddUserManager<Identity.Implementation.UserManager<User>>()
            .AddRoleManager<Identity.Implementation.RoleManager<Role>>()
            .AddDataAccessStores()
            .AddDefaultTokenProviders();

        services.AddScoped<IUserManager<User>, Identity.Implementation.UserManager<User>>();
        services.AddScoped<IRoleManager<Role>, Identity.Implementation.RoleManager<Role>>();

        return services;
    }

    /// <summary>
    /// Creates and configures an <see cref="SmtpClient"/> instance
    /// based on the given <paramref name="options"/>.
    /// </summary>
    /// <param name="options">
    /// The <see cref="SmtpOptions"/> used to configure the client,
    /// including host, port, SSL, credentials,
    /// and whether to store emails instead of sending them.
    /// </param>
    /// <returns>
    /// A fully configured <see cref="SmtpClient"/> instance ready for use.
    /// </returns>
    /// <remarks>
    /// <para>
    /// If <see cref="SmtpOptions.LocalEmailStorage"/> is set, the client is configured
    /// to use <see cref="System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory"/> and will write outgoing
    /// emails as <c>.eml</c> files into a local directory under the application's base path.
    /// </para>
    /// <para>
    /// If <see cref="SmtpOptions.LocalEmailStorage"/> is not set, the client is configured
    /// for network delivery using the specified host, port, SSL setting, and credentials.
    /// A <see langword="null"/> <see cref="SmtpOptions.Credentials"/> value will trigger a failure.
    /// </para>
    /// </remarks>
    private static SmtpClient GetSmtpClient(SmtpOptions options)
    {
        var client = new SmtpClient();

        if (options.LocalEmailStorage.IsNotNullOrWhiteSpace())
        {
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory;
            client.PickupDirectoryLocation = options.InitializeLocalEmailStorage();
        }
        else
        {
            client.Host = options.Host;
            client.Port = options.Port;
            client.EnableSsl = options.EnableSecureSocketsLayer;
            client.Credentials = options.Credentials;

            if (client.Credentials is null)
            {
                var message = GetMessageInvalidSmtpConfiguration();

                throw new InvalidOperationException(message);
            }
        }

        return client;
    }

    /// <summary>
    /// Creates and configures an <see cref="EmailClient"/> instance
    /// based on the given <paramref name="options"/>.
    /// </summary>
    /// <param name="options">
    /// The <see cref="SmtpOptions"/> used to configure the client.
    /// </param>
    /// <returns>
    /// A fully configured <see cref="EmailClient"/> instance ready for use.
    /// </returns>
    private static EmailClient GetEmailClient(SmtpOptions options)
    {
        if (options.Credentials is null)
        {
            var message = GetMessageInvalidSmtpConfiguration();

            throw new InvalidOperationException(message);
        }

        return new EmailClient(options.Credentials.Password);
    }
    #endregion
}