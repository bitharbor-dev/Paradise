using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.Options.Extensions;
using Paradise.Common.Extensions;
using Paradise.Common.Web;
using Paradise.WebApi.Infrastructure.Authentication.Caching;
using Paradise.WebApi.Infrastructure.Authentication.Caching.Implementation;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Implementation;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Implementation;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Options;
using Paradise.WebApi.Infrastructure.Options;
using static Paradise.Common.EnvironmentNames;
using static Paradise.Localization.ExceptionHandling.ExceptionMessages;

namespace Paradise.WebApi.Infrastructure.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IServiceCollection"/> <see langword="interface"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    #region Public methods
    /// <summary>
    /// Adds the JWT Bearer authentication.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configuration">
    /// The <see cref="IConfiguration"/> for authentication configuration.
    /// </param>
    /// <param name="eventsType">
    /// JWT Bearer events type.
    /// </param>
    /// <param name="environmentName">
    /// Current environment name.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration,
                                                                Type eventsType, string environmentName)
    {
        void UseDefaultJwtBearerConfiguration(JwtBearerOptions options)
        {
            configuration.BindSection(options);

            options.EventsType = eventsType;
        }

        void JwtBearerOptionsPostConfigure(JwtBearerOptions options, IServiceProvider provider)
        {
            var signingKeyProvider = provider.GetRequiredService<IJwtSigningKeyProvider>();

            options.TokenValidationParameters.IssuerSigningKey = signingKeyProvider.GetSigningKey();
            options.TokenValidationParameters.ValidAlgorithms = [signingKeyProvider.JwtAlgorithm];
        }

        services.AddOptions<JwtBearerOptions>(configuration,
                                              configurationSectionPath: null,
                                              postConfigure: JwtBearerOptionsPostConfigure,
                                              validateOnStartup: true,
                                              validateDataAnnotations: true);

        services.AddOptions<AuthenticationOptions>(configuration,
                                                   configurationSectionPath: null,
                                                   postConfigure: null,
                                                   validateOnStartup: true,
                                                   validateDataAnnotations: true);

        services
            .AddJwtSigningKeyProvider(configuration, environmentName)
            .AddAuthentication()
            .AddJwtBearer(AuthenticationSchemeNames.Default, UseDefaultJwtBearerConfiguration)
            .AddJwtBearer(AuthenticationSchemeNames.DisableTokenLifetimeValidation, options =>
            {
                UseDefaultJwtBearerConfiguration(options);

                SetValidateLifetime(options, false);
            });

        services.PostConfigure<JwtBearerOptions>(AuthenticationSchemeNames.Default, JwtBearerOptionsPostConfigure);
        services.PostConfigure<JwtBearerOptions>(AuthenticationSchemeNames.DisableTokenLifetimeValidation, JwtBearerOptionsPostConfigure);

        return services
            .AddScoped(eventsType)
            .AddScoped<IJwtManager, JwtManager>()
            .AddDistributedMemoryCache()
            .AddSingleton<IRefreshTokenCache, RefreshTokenCache>();
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Adds the singleton environment-dependent <see cref="IJwtSigningKeyProvider"/> implementation.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configuration">
    /// The <see cref="IConfiguration"/> for retrieving singing key provider options.
    /// </param>
    /// <param name="environmentName">
    /// Current environment name.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    private static IServiceCollection AddJwtSigningKeyProvider(this IServiceCollection services,
                                                               IConfiguration configuration,
                                                               string environmentName)
    {
        switch (environmentName)
        {
            case Development:
            case DevelopmentDocker:
                {
                    services.AddOptions<SymmetricSigningKeyProviderOptions>(configuration,
                                                                            validateOnStartup: true,
                                                                            validateDataAnnotations: true);

                    services.AddSingleton<IJwtSigningKeyProvider, SymmetricSigningKeyProvider>();

                    break;
                }
            case Staging:
            case StagingDocker:
            case Production:
            case ProductionDocker:
                {
                    services.AddOptions<AsymmetricSigningKeyProviderOptions>(configuration,
                                                                             validateOnStartup: true,
                                                                             validateDataAnnotations: true);

                    services.AddSingleton<IJwtSigningKeyProvider, AsymmetricSigningKeyProvider>();

                    break;
                }
            default:
                {
                    var message = GetMessageInvalidEnvironmentName(environmentName, AllowedEnvironments);

                    throw new InvalidOperationException(message);
                }
        }

        return services;
    }

    /// <summary>
    /// Configures the <see cref="JwtBearerOptions.TokenValidationParameters"/> to
    /// validate or skip the JWT lifetime validation.
    /// </summary>
    /// <param name="options">
    /// Target <see cref="JwtBearerOptions"/>.
    /// </param>
    /// <param name="validateLifetime">
    /// Indicates whether the JWT lifetime will be validated.
    /// </param>
    private static void SetValidateLifetime(JwtBearerOptions options, bool validateLifetime)
        => options.TokenValidationParameters.ValidateLifetime = validateLifetime;
    #endregion
}