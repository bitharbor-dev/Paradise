using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.Infrastructure.DataProtection;
using Paradise.ApplicationLogic.Options.Models;
using Paradise.ApplicationLogic.Options.Models.DataAccess.Seed.Providers;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Communication.Email;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Services;
using Paradise.ApplicationLogic.Services.Identity.Roles;
using Paradise.ApplicationLogic.Services.Identity.Users;
using Paradise.Domain.Base.Events;
using Paradise.Tests.Miscellaneous.Json.Converters;
using Paradise.WebApi.Extensions;
using Paradise.WebApi.Infrastructure.Options;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Paradise.WebApi.Tests.Unit.Extensions;

public sealed partial class IServiceCollectionExtensionsTests
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="IServiceCollectionExtensionsTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Fields
        private static readonly JsonSerializerOptions _jsonSerializerOptions = GetJsonSerializerOptions();
        private static readonly ServiceProviderOptions _serviceProviderOptions = new()
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        };

        private readonly IServiceCollection _services = new ServiceCollection();
        #endregion

        #region Properties
        /// <summary>
        /// Application configuration which helps resolving the registered services.
        /// </summary>
        public OptionsContainer Options { get; } = new()
        {
            ApplicationOptions = new()
            {
                ApiUrl = new("https://localhost:5001")
            },
            AuthenticationOptions = new()
            {
                AccessTokenLifetime = TimeSpan.FromMinutes(5),
                RefreshTokenLifetime = TimeSpan.FromDays(90),
                TwoFactorTokenLifetime = TimeSpan.FromDays(1),
                TwoFactorVerificationCodeLength = 6
            },
            ConnectionStrings = new()
            {
                ["DomainConnectionString"] = "",
                ["InfrastructureConnectionString"] = ""
            },
            EmailTemplateOptions = new()
            {
                EmailAddressChangedNotificationTemplateName = "EmailAddressChangedNotification",
                EmailAddressChangeLinkTemplateName = "EmailAddressChangeLink",
                EmailAddressChangingNotificationTemplateName = "EmailAddressChangingNotification",
                EmailAddressConfirmationLinkTemplateName = "EmailAddressConfirmationLink",
                PasswordChangedNotificationTemplateName = "PasswordChangedNotification",
                PasswordChangeLinkTemplateName = "PasswordChangeLink",
                TwoFactorVerificationTemplateName = "TwoFactorVerification"
            },
            IdentityOptions = new(),
            JsonSeedDataProviderOptions = new(),
            JsonSerializerOptions = new(),
            JwtBearerOptions = new(),
            RequestLocalizationOptions = new()
            {
                DefaultRequestCulture = new("en-US"),
                SupportedCultures = [new("en-US")],
                SupportedUICultures = [new("en-US")]
            },
            SmtpOptions = new()
            {
                Credentials = new("UserName", "endpoint=https://test.com/;accesskey=1234"),
                EnableSecureSocketsLayer = false,
                Host = "smtp.test",
                Port = 2525
            },
            SymmetricSigningKeyProviderOptions = new
            {
                Secret = "Ug5xCXJaNaxfx78KdQxQZDAmniAbZw6V"
            },
            AsymmetricSigningKeyProviderOptions = new
            {
                PrivateKey = "MIICXQIBAAKBgQCNgomP9R3QYNDP35i5OyWs2eyXeRj4x+AtmjE/12XOitu0l5Od"
                           + "qwYvWNQogxNPxjNr65xaZMZbXJz7EGq++JjP52yq8BJgN/VnW6cJP7jhBljNaozs"
                           + "F5zvmEAT1IIo7sz+G/xYijkr3wwG6S8kN1EBIWpo7RkDEJG4EDtk17+4rwIDAQAB"
                           + "AoGAUknccKgbJDeIdbkSeHRanj9Dg3nZ+aFRTXNivDsnaon45PVX09HGEPZYuQ4v"
                           + "xq387P7ftvjvF+WtK5oKWO76/NaKP+TLPZHwJM3puJOrTaDrwhI6otLyA+fVWZgl"
                           + "8dDcgjeQ2T9WDNhHZMRu7yUIQwWtbFVl2jbJLyAw6ce+iEECQQD1cEIzGmEmP/3p"
                           + "LDNHba2eaoeLpl6H81gT8bCdLNOcseZSEiUfPEwFKTYgumwtSuW5QOteUVjFoJKQ"
                           + "s198EHO/AkEAk5lnvKphkK7MCATckgrW5Hm0vBYXdW6e7xBnPIyT0GCVeIGbAysd"
                           + "orG7Mclmt/RmTmeSk+JIOlcyoM8eOLY3EQJAPHTFaa8SxQg4NApWKz8B6CaXcret"
                           + "S1GOnYMIHP8gtNVBRXAAwtvoYdEP6yngYZu0UFiEYXwqIKv3zjrQx0+KIwJBAIPl"
                           + "HfJWPwFPcjvoPEK1NPrOV1eMVkI2LAhtnBNbe+tFo8wf5SmbqcvtDt6anxPbbmC5"
                           + "5R4Jo4meyjsxWkxLaEECQQDnIX/OVc7ckaYZJiHjxEosOF7D2io+qFWWWSMZMyEP"
                           + "mctLm7sOCsCn/Ik/q7H81rI4P4Lr/HWMywwdH7LMz1SC"
            }
        };
        #endregion

        #region Public methods
        /// <summary>
        /// Builds a service provider using the current <see cref="Options"/> and the
        /// <see cref="IServiceCollectionExtensions.AddAuthenticationAndAuthorization"/> registration method.
        /// </summary>
        /// <param name="environmentName">
        /// Current environment name.
        /// </param>
        /// <returns>
        /// A configured <see cref="IServiceProvider"/>.
        /// </returns>
        public ServiceProvider BuildAuthenticationAndAuthorizationServiceProvider(string environmentName)
        {
            var configuration = BuildConfiguration();

            _services.AddLogging();
            _services.AddSingleton(UrlEncoder.Default);

            _services.AddSingleton(_ => (EndpointDataSource)null!);

            _services.AddScoped(_ => (IDomainEventSink)null!);
            _services.AddScoped(_ => (IDataProtector)null!);
            _services.AddScoped(_ => (IRoleService)null!);
            _services.AddScoped(_ => (IUserService)null!);
            _services.AddScoped(_ => (IUserRefreshTokenService)null!);

            _services.AddAuthenticationAndAuthorization(configuration, environmentName);
            return _services.BuildServiceProvider(_serviceProviderOptions);
        }

        /// <summary>
        /// Builds a service provider using the
        /// <see cref="IServiceCollectionExtensions.AddAuthorizationResultHandler"/> registration method.
        /// </summary>
        /// <returns>
        /// A configured <see cref="IServiceProvider"/>.
        /// </returns>
        public ServiceProvider BuilderAuthorizationResultHandlerServiceProvider()
        {
            _services.AddAuthorizationResultHandler();
            return _services.BuildServiceProvider(_serviceProviderOptions);
        }

        /// <summary>
        /// Builds a service provider using the
        /// <see cref="IServiceCollectionExtensions.AddDomainEventsDispatchingService"/> registration method.
        /// </summary>
        /// <returns>
        /// A configured <see cref="IServiceProvider"/>.
        /// </returns>
        public ServiceProvider BuildDomainEventsDispatchingServiceProvider()
        {
            _services.AddSingleton(_ => (IDomainEventDispatcher)null!);

            _services.AddDomainEventsDispatchingService();
            return _services.BuildServiceProvider(_serviceProviderOptions);
        }

        /// <summary>
        /// Builds a service provider using the
        /// <see cref="IServiceCollectionExtensions.AddRequestLocalization"/> registration method.
        /// </summary>
        /// <returns>
        /// A configured <see cref="IServiceProvider"/>.
        /// </returns>
        public ServiceProvider BuildRequestLocalizationServiceProvider()
        {
            var configuration = BuildConfiguration();

            _services.AddRequestLocalization(configuration);
            return _services.BuildServiceProvider(_serviceProviderOptions);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Creates an <see cref="IConfiguration"/> instance from the current <see cref="Options"/>.
        /// </summary>
        /// <returns>
        /// The configuration instance.
        /// </returns>
        private IConfiguration BuildConfiguration()
        {
            using var configurationStream = new MemoryStream();
            JsonSerializer.Serialize(configurationStream, Options, _jsonSerializerOptions);

            configurationStream.Position = 0;

            return new ConfigurationBuilder()
                .AddJsonStream(configurationStream)
                .Build();
        }

        /// <summary>
        /// Creates a configured <see cref="JsonSerializerOptions"/> instance
        /// with custom converters required for application-specific serialization.
        /// </summary>
        /// <returns>
        /// A configured <see cref="JsonSerializerOptions"/> instance.
        /// </returns>
        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new CultureInfoConverter());
            options.Converters.Add(new JwtBearerOptionsConverter());

            return options;
        }
        #endregion
    }

    /// <summary>
    /// Replicates the application options data structure for proper
    /// conversion into <see cref="IConfiguration"/> instances.
    /// </summary>
    private sealed class OptionsContainer
    {
        #region Properties
        /// <summary>
        /// Configurable application options.
        /// </summary>
        public ApplicationOptions? ApplicationOptions { get; set; }

        /// <summary>
        /// Configurable authentication options instance.
        /// </summary>
        public AuthenticationOptions? AuthenticationOptions { get; set; }

        /// <summary>
        /// Configurable connection strings.
        /// </summary>
        public Dictionary<string, string>? ConnectionStrings { get; set; }

        /// <summary>
        /// Configurable email template options.
        /// </summary>
        public EmailTemplateOptions? EmailTemplateOptions { get; set; }

        /// <summary>
        /// Configurable Identity options instance.
        /// </summary>
        public IdentityOptions? IdentityOptions { get; set; }

        /// <summary>
        /// Configurable JSON seed data provider options instance.
        /// </summary>
        public JsonSeedDataProviderOptions? JsonSeedDataProviderOptions { get; set; }

        /// <summary>
        /// Configurable JSON serializer options.
        /// </summary>
        public JsonSerializerOptions? JsonSerializerOptions { get; set; }

        /// <summary>
        /// Configurable JWT bearer options instance.
        /// </summary>
        public JwtBearerOptions? JwtBearerOptions { get; set; }

        /// <summary>
        /// Configurable request localization options instance.
        /// </summary>
        public RequestLocalizationOptions? RequestLocalizationOptions { get; set; }

        /// <summary>
        /// Configurable SMTP options instance.
        /// </summary>
        public SmtpOptions? SmtpOptions { get; set; }

        /// <summary>
        /// Configurable secret-based signing key provider options instance.
        /// </summary>
        public object? SymmetricSigningKeyProviderOptions { get; set; }

        /// <summary>
        /// Configurable Azure KeyVault signing key provider options instance.
        /// </summary>
        public object? AsymmetricSigningKeyProviderOptions { get; set; }
        #endregion
    }
    #endregion
}