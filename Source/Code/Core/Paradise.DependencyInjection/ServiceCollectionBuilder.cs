using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Paradise.ApplicationLogic.Authorization.JwtBearer;
using Paradise.ApplicationLogic.Communication;
using Paradise.ApplicationLogic.Communication.Implementation;
using Paradise.ApplicationLogic.Identity;
using Paradise.ApplicationLogic.Services.Application;
using Paradise.ApplicationLogic.Services.Application.Implementation;
using Paradise.ApplicationLogic.Services.Domain;
using Paradise.ApplicationLogic.Services.Domain.Implementation;
using Paradise.Common.Extensions;
using Paradise.DataAccess.Database;
using Paradise.DataAccess.Repositories;
using Paradise.DataAccess.Repositories.Application;
using Paradise.DataAccess.Repositories.Application.Implementation;
using Paradise.DataAccess.Repositories.Domain;
using Paradise.DataAccess.Repositories.Domain.Implementation;
using Paradise.DataAccess.Seed.Providers;
using Paradise.DataAccess.Seed.Providers.Implementation;
using Paradise.Domain.Roles;
using Paradise.Domain.Users;
using Paradise.Localization.ExceptionsHandling;
using Paradise.Options.Models;
using Paradise.Options.Models.Communication;
using Paradise.Options.Origins.Base;
using System.Text;
using System.Text.Json;

namespace Paradise.DependencyInjection;

/// <summary>
/// Provides a simple API for configuring application services.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ServiceCollectionBuilder"/> class.
/// </remarks>
/// <param name="services">
/// The service collection the builder will operate over.
/// </param>
/// <param name="configurationOrigin">
/// Configuration origin.
/// </param>
public sealed class ServiceCollectionBuilder(IServiceCollection services, IConfigurationOrigin configurationOrigin)
{
    #region Fields
    private readonly IConfiguration _configuration = configurationOrigin.GetConfiguration();
    #endregion

    #region Public methods
    /// <summary>
    /// Adds the default required service descriptors.
    /// </summary>
    public void ConfigureRequiredServices()
    {
        AddApplicationOptions();
        AddEmailTemplateOptions();
        AddSmtpOptions();
        AddSmtpClient();
        AddSeedDataProvider();
        AddJsonSerializerOptions();
        AddJwtBearerOptions();
        AddDbContexts();
        AddIdentity();
        AddRepositories();
        AddServices();
    }

    /// <summary>
    /// Adds the JWT Bearer authentication.
    /// </summary>
    /// <remarks>
    /// Since the <see cref="ServiceCollectionBuilder"/> is being used in both
    /// WebApi and Maintenance project - we need to keep this method separated,
    /// because it is ASP.NET Core-specific and is redundant in background worker project.
    /// </remarks>
    public void AddBearerAuth()
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                _configuration.GetRequiredSection(nameof(JwtBearerOptions)).Bind(options);
                AddSigningKey(options);

                options.Events = new()
                {
                    OnAuthenticationFailed = JwtEvents.OnAuthenticationFailed,
                    OnChallenge = JwtEvents.OnChallenge,
                    OnForbidden = JwtEvents.OnForbidden,
                    OnTokenValidated = JwtEvents.OnTokenValidated
                };
            });

        services.AddAuthorization();
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Adds <see cref="IOptions{T}"/> of <see cref="ApplicationOptions"/>.
    /// </summary>
    private void AddApplicationOptions()
        => AddOptionsInternal<ApplicationOptions>(null, true, true);

    /// <summary>
    /// Adds the <see cref="IOptions{T}"/> of <see cref="EmailTemplateOptions"/>.
    /// </summary>
    private void AddEmailTemplateOptions()
        => AddOptionsInternal<EmailTemplateOptions>(null, true, true);

    /// <summary>
    /// Adds the <see cref="IOptions{T}"/> of <see cref="SmtpOptions"/>.
    /// </summary>
    private void AddSmtpOptions()
        => AddOptionsInternal<SmtpOptions>(null, true, true);

    /// <summary>
    /// Adds the <see cref="IOptions{T}"/> of <see cref="JsonSerializerOptions"/>.
    /// </summary>
    private void AddJsonSerializerOptions()
        => AddOptionsInternal<JsonSerializerOptions>(null, true, true);

    /// <summary>
    /// Adds the <see cref="IOptions{T}"/> of <see cref="JwtBearerOptions"/>.
    /// </summary>
    private void AddJwtBearerOptions()
        => AddOptionsInternal<JwtBearerOptions>(AddSigningKey, true, true);

    /// <summary>
    /// Adds the <see cref="AzureSmtpClient"/>.
    /// </summary>
    private void AddSmtpClient()
        => services.AddScoped<ISmtpClient, AzureSmtpClient>();

    /// <summary>
    /// Adds the <see cref="JsonSeedDataProvider"/>.
    /// </summary>
    private void AddSeedDataProvider()
        => services.AddScoped<ISeedDataProvider, JsonSeedDataProvider>();

    /// <summary>
    /// Adds the database contexts.
    /// </summary>
    private void AddDbContexts()
    {
        var applicationContextConnectionString = _configuration
            .GetConnectionString(ApplicationContext.ConnectionStringName);

        var domainContextConnectionString = _configuration
            .GetConnectionString(DomainContext.ConnectionStringName);

        services.AddDbContext<IApplicationDataSource, ApplicationContext>(
            options => options.UseSqlServer(applicationContextConnectionString));

        services.AddDbContext<IDomainDataSource, DomainContext>(
            options => options.UseSqlServer(domainContextConnectionString));
    }

    /// <summary>
    /// Adds the identity services.
    /// </summary>
    private void AddIdentity()
    {
        void SetUpIdentity(IdentityOptions options)
            => _configuration.GetRequiredSection(nameof(IdentityOptions)).Bind(options);

        services.AddIdentity<User, Role>(SetUpIdentity)
            .AddEntityFrameworkStores<DomainContext>()
            .AddRoleManager<RoleManager<Role>>()
            .AddUserManager<UserManager>()
            .AddDefaultTokenProviders();
    }

    /// <summary>
    /// Adds the repositories.
    /// </summary>
    private void AddRepositories()
    {
        services.AddScoped<IEmailTemplatesRepository, EmailTemplatesRepository>();
        services.AddScoped<IUserRefreshTokensRepository, UserRefreshTokensRepository>();
    }

    /// <summary>
    /// Adds the default services.
    /// </summary>
    private void AddServices()
    {
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<ICommunicationService, CommunicationService>();
        services.AddScoped<IDatabaseService, DatabaseService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<IJsonWebTokenService, JsonWebTokenService>();
        services.AddDataProtection().UseCryptographicAlgorithms(new()
        {
            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
        });

        services.AddScoped<IDataProtectionService, DataProtectionService>();

        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, UserService>();
    }

    /// <summary>
    /// Adds the <see cref="IOptions{TOptions}"/> of <typeparamref name="TOptions"/>.
    /// </summary>
    /// <typeparam name="TOptions">
    /// Options type.
    /// </typeparam>
    /// <param name="postConfigure">
    /// Action used to configure the options.
    /// </param>
    /// <param name="validateOnStartup">
    /// Indicates whether the options would be
    /// validated on the application startup.
    /// </param>
    /// <param name="validateDataAnnotations">
    /// Indicates whether the options would be
    /// validated on the application startup.
    /// </param>
    private void AddOptionsInternal<TOptions>(Action<TOptions>? postConfigure = null, bool validateOnStartup = false, bool validateDataAnnotations = false)
        where TOptions : class
    {
        var configurationSection = _configuration.GetRequiredSection(typeof(TOptions).Name);

        var optionsBuilder = services.AddOptions<TOptions>().Bind(configurationSection);

        if (validateDataAnnotations)
            optionsBuilder.ValidateDataAnnotations();

        if (validateOnStartup)
            optionsBuilder.ValidateOnStart();

        if (postConfigure is not null)
            optionsBuilder.PostConfigure(postConfigure);
    }

    /// <summary>
    /// Adds the <see cref="TokenValidationParameters.IssuerSigningKey"/>
    /// to the given <see cref="JwtBearerOptions"/>.
    /// </summary>
    /// <param name="options">
    /// Options to which to add signing key.
    /// </param>
    private void AddSigningKey(JwtBearerOptions options)
    {
        var secret = _configuration
            .GetRequiredSection(nameof(ApplicationOptions))
            .GetValue<string>(nameof(ApplicationOptions.Secret));

        if (secret.IsNullOrWhiteSpace())
            throw new InvalidOperationException(ExceptionMessages.ApplicationSecretMissing);

        var bytes = Encoding.UTF8.GetBytes(secret);
        var key = new SymmetricSecurityKey(bytes);

        options.TokenValidationParameters.IssuerSigningKey = key;
    }
    #endregion
}