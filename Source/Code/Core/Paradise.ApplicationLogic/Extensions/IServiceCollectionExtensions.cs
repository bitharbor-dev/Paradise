using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.Infrastructure.Extensions;
using Paradise.ApplicationLogic.Options.Extensions;
using Paradise.ApplicationLogic.Options.Models;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Services.MessageTemplates;
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
        => services;
    #endregion
}