using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.EventListeners.Domain.Identity.Users;
using Paradise.ApplicationLogic.Infrastructure.Extensions;
using Paradise.ApplicationLogic.Options.Extensions;
using Paradise.ApplicationLogic.Options.Models;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Services;
using Paradise.ApplicationLogic.Services.Identity.Roles;
using Paradise.ApplicationLogic.Services.Identity.Roles.Implementation;
using Paradise.ApplicationLogic.Services.Identity.Users;
using Paradise.ApplicationLogic.Services.Identity.Users.Implementation;
using Paradise.Domain.Base.Events;
using Paradise.Domain.Base.Events.Extensions;
using Paradise.Domain.Events.Identity.Users;
using System.Text.Json;

namespace Paradise.ApplicationLogic.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IServiceCollection"/> <see langword="interface"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    #region Public methods
    /// <summary>
    /// Registers application-level services, options and infrastructure.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configuration">
    /// The <see cref="IConfiguration"/> instance used to configure dependencies.
    /// </param>
    /// <param name="environmentName">
    /// Current environment name.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddApplicationLogic(this IServiceCollection services, IConfiguration configuration,
                                                         string environmentName)
    {
        return services
            .AddOptions<JsonSerializerOptions>(configuration, validateOnStartup: true, validateDataAnnotations: true)
            .AddOptions<ApplicationOptions>(configuration, validateOnStartup: true, validateDataAnnotations: true)
            .AddOptions<EmailTemplateOptions>(configuration, validateOnStartup: true, validateDataAnnotations: true)
            .AddInfrastructure(configuration, environmentName)
            .AddDomainServices();
    }

    /// <summary>
    /// Registers the infrastructure for domain event handling by configuring both
    /// the event dispatching mechanism and the registered event listeners.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configureOptions">
    /// An action used to configure global domain event retry policy.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddDomainEvents(this IServiceCollection services,
                                                     Action<DomainEventRetryOptions>? configureOptions = null)
    {
        static void UseDefaultRetryOptions(DomainEventRetryOptions options)
        {
            options.MaxRetries = 3;
            options.BaseDelay = TimeSpan.FromSeconds(2);
            options.UseExponentialBackOff = true;
        }

        return services
            .AddDomainEventsDispatching(configureOptions ?? UseDefaultRetryOptions)
            .AddEventListeners();
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Registers core domain services required by the application.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    private static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IRoleService, RoleService>()
            .AddScoped<IUserRefreshTokenService, UserRefreshTokenService>()
            .AddScoped<IUserService, UserService>();
    }

    /// <summary>
    /// Registers domain event listeners that react to the domain events.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    private static IServiceCollection AddEventListeners(this IServiceCollection services)
    {
        return services
            .AddDomainEventListener<AssignDefaultUserRoles, EmailAddressConfirmedEvent>()
            .AddDomainEventListener<SendEmailAddressChangedNotification, EmailAddressResetCompletedEvent>()
            .AddDomainEventListener<SendEmailAddressChangeLink, EmailAddressResetRequestedEvent>()
            .AddDomainEventListener<SendEmailAddressChangingNotification, EmailAddressResetRequestedEvent>()
            .AddDomainEventListener<SendEmailAddressConfirmationLink, UserRegisteredEvent>()
            .AddDomainEventListener<SendPasswordChangedNotification, PasswordResetCompletedEvent>()
            .AddDomainEventListener<SendPasswordChangeLink, PasswordResetRequestedEvent>()
            .AddDomainEventListener<SendTwoFactorAuthenticationCode, TwoFactorAuthenticationOccurringEvent>();
    }
    #endregion
}