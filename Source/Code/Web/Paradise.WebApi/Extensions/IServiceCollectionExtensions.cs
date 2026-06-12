using Microsoft.AspNetCore.Localization;
using Paradise.Primitives.Extensions;
using Paradise.WebApi.Authentication.JwtBearer;
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
            .AddJwtBearerAuthentication<JwtEvents>(configuration, environmentName)
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddAuthorization();
    }

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
        RegisterTypeConverter<RequestCulture, RequestCultureConverter>();

        return services.AddRequestLocalization(appSettings.BindSection);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Registers a <see cref="TypeConverter"/> for the specified target type by adding a
    /// <see cref="TypeConverterAttribute"/> to the type descriptor metadata.
    /// </summary>
    /// <typeparam name="TTarget">
    /// The type for which the converter should be registered.
    /// </typeparam>
    /// <typeparam name="TConverter">
    /// The <see cref="TypeConverter"/> implementation associated with <typeparamref name="TTarget"/>.
    /// </typeparam>
    private static void RegisterTypeConverter<TTarget, TConverter>()
        where TConverter : TypeConverter
    {
        var targetType = typeof(TTarget);
        var converterType = typeof(TConverter);

        var attribute = new TypeConverterAttribute(converterType);

        TypeDescriptor.AddAttributes(targetType, attribute);
    }
    #endregion
}