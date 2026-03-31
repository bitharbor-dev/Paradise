using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Localization;
using Paradise.Common.Extensions;
using Paradise.WebApi.Authentication.JwtBearer;
using Paradise.WebApi.Authorization;
using Paradise.WebApi.Infrastructure.Extensions;
using Paradise.WebApi.Infrastructure.TypeConverters;
using Paradise.WebApi.Services.Authentication;
using Paradise.WebApi.Services.Authentication.Implementation;
using Paradise.WebApi.Services.Background;
using System.ComponentModel;

namespace Paradise.WebApi.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IServiceCollection"/> <see langword="interface"/>.
/// </summary>
internal static class IServiceCollectionExtensions
{
    #region Public methods
    /// <summary>
    /// Registers authentication and authorization services.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configuration">
    /// The <see cref="IConfiguration"/> instance used to configure authentication and authorization.
    /// </param>
    /// <param name="environmentName">
    /// Current environment name.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration,
                                                                       string environmentName)
    {
        return services
            .AddJwtBearerAuthentication(configuration, typeof(JwtEvents), environmentName)
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddAuthorization();
    }

    /// <summary>
    /// Registers the default authorization result handler.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddAuthorizationResultHandler(this IServiceCollection services)
    {
        return services.AddSingleton<IAuthorizationMiddlewareResultHandler>(provider =>
        {
            var defaultHandler = new AuthorizationMiddlewareResultHandler();

            return new AuthorizationResultHandler(defaultHandler);
        });
    }

    /// <summary>
    /// Registers <see cref="LifecycleManagementService"/> as a hosted service.
    /// </summary>
    /// <remarks>
    /// This extension method is intended to hook into the application lifecycle
    /// to run startup and shutdown activities via <see cref="IHostedService"/>.
    /// </remarks>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddStartupAndShutdownActivities(this IServiceCollection services)
        => services.AddHostedService<LifecycleManagementService>();

    /// <summary>
    /// Registers the domain event dispatching service.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddDomainEventsDispatchingService(this IServiceCollection services)
        => services.AddHostedService<DomainEventsDispatchingService>();

    /// <summary>
    /// Adds the services and options for the request localization middleware.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="appSettings">
    /// The <see cref="IConfiguration"/> for retrieving localization options.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddRequestLocalization(this IServiceCollection services, IConfiguration appSettings)
    {
        TypeDescriptor.AddAttributes(typeof(RequestCulture), new TypeConverterAttribute(typeof(RequestCultureConverter)));

        return services.AddRequestLocalization(appSettings.BindSection);
    }
    #endregion
}