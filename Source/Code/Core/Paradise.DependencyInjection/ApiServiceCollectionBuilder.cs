using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.Authorization.JwtBearer;
using Paradise.ApplicationLogic.Authorization.JwtBearer.Keys;
using Paradise.ApplicationLogic.Communication;
using Paradise.ApplicationLogic.Communication.Implementation;
using Paradise.ApplicationLogic.Services.Application;
using Paradise.ApplicationLogic.Services.Application.Implementation;
using Paradise.ApplicationLogic.Services.Domain;
using Paradise.ApplicationLogic.Services.Domain.Implementation;
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

        AddOptions<EmailTemplateOptions>(null, true, true);
        AddOptions<SmtpOptions>(null, true, true);
        AddOptions<JwtBearerOptions>(SetJwtIssuerSigningKey, true, true);

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
                Configuration.GetRequiredSection(nameof(JwtBearerOptions)).Bind(options);
                SetJwtIssuerSigningKey(options);

                options.Events = new()
                {
                    OnAuthenticationFailed = JwtEvents.OnAuthenticationFailed,
                    OnChallenge = JwtEvents.OnChallenge,
                    OnForbidden = JwtEvents.OnForbidden,
                    OnTokenValidated = JwtEvents.OnTokenValidated
                };
            });

        Services.AddAuthorization();

        Services.AddScoped<IAuthorizationService, AuthorizationService>();
    }

    private void SetJwtIssuerSigningKey(JwtBearerOptions options)
        => options.TokenValidationParameters.IssuerSigningKey = signingKeyProvider.GetSigningKey();
    #endregion
}