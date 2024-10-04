using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.Authorization.JwtBearer;
using Paradise.ApplicationLogic.Authorization.JwtBearer.Keys;
using Paradise.ApplicationLogic.Communication;
using Paradise.ApplicationLogic.Communication.Implementation;
using Paradise.ApplicationLogic.Services.Application;
using Paradise.ApplicationLogic.Services.Application.Implementation;
using Paradise.ApplicationLogic.Services.Domain.Roles;
using Paradise.ApplicationLogic.Services.Domain.Roles.Implementation;
using Paradise.ApplicationLogic.Services.Domain.Users;
using Paradise.ApplicationLogic.Services.Domain.Users.Implementation;
using Paradise.Common.Extensions;
using Paradise.DependencyInjection.Base;
using Paradise.Options.Models.Communication;
using Paradise.Options.Origins.Base;

namespace Paradise.DependencyInjection;

/// <summary>
/// Configures web API services.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ApiServiceCollectionBuilder"/> class.
/// </remarks>
/// <param name="services">
/// The service collection the builder will operate over.
/// </param>
/// <param name="configurationOrigin">
/// Configuration origin.
/// </param>
/// <param name="signingKeyProvider">
/// JWT signing key provider.
/// </param>
public sealed class ApiServiceCollectionBuilder(IServiceCollection services,
                                                IConfigurationOrigin configurationOrigin,
                                                IJwtSigningKeyProvider signingKeyProvider)
    : ServiceCollectionBuilderCore(services, configurationOrigin)
{
    #region Protected methods
    /// <inheritdoc/>
    protected override void AddMiscellaneous()
    {
        base.AddMiscellaneous();

        AddOptions<EmailTemplateOptions>(
            postConfigure: null,
            validateOnStartup: true,
            validateDataAnnotations: true);

        AddOptions<SmtpOptions>(
            postConfigure: null,
            validateOnStartup: true,
            validateDataAnnotations: true);

        AddOptions<JwtBearerOptions>(
            postConfigure: SetSigningKeyAndAlgorithm,
            validateOnStartup: true,
            validateDataAnnotations: true);

        Services.AddScoped<ISmtpClient, AzureSmtpClient>();

        AddJwtBearerAuth();
    }

    /// <inheritdoc/>
    protected override void AddServices()
    {
        base.AddServices();

        Services.AddScoped<ICommunicationService, CommunicationService>();
        Services.AddScoped<IJsonWebTokenService, JsonWebTokenService>();

        Services.AddScoped<IDataProtectionService, DataProtectionService>();

        Services.AddScoped<IRoleService, RoleService>();
        Services.AddScoped<IUserService, UserService>();
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Adds the JWT Bearer authentication.
    /// </summary>
    private void AddJwtBearerAuth()
    {
        Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                Configuration.BindSection(options);
                SetSigningKeyAndAlgorithm(options);

                options.Events = new()
                {
                    OnAuthenticationFailed = JwtEvents.OnAuthenticationFailed,
                    OnChallenge = JwtEvents.OnChallenge,
                    OnForbidden = JwtEvents.OnForbidden,
                    OnMessageReceived = JwtEvents.OnMessageReceived,
                    OnTokenValidated = JwtEvents.OnTokenValidated
                };
            });

        Services.AddAuthorization();

        Services.AddScoped<IAuthorizationService, AuthorizationService>();
    }

    /// <summary>
    /// Sets the JWT signing key and algorithm to the given
    /// <paramref name="options"/> instance.
    /// </summary>
    /// <param name="options">
    /// The <see cref="JwtBearerOptions"/> to update with key and algorithm.
    /// </param>
    private void SetSigningKeyAndAlgorithm(JwtBearerOptions options)
    {
        var algorithmPropertyName = nameof(IJwtSigningKeyProvider.JwtAlgorithm);
        var algorithmPropertyValue = signingKeyProvider.JwtAlgorithm;

        var parameters = options.TokenValidationParameters;

        parameters.PropertyBag = new Dictionary<string, object?>
        {
            { algorithmPropertyName, algorithmPropertyValue }
        };

        parameters.IssuerSigningKey = signingKeyProvider.GetSigningKey();
    }
    #endregion
}