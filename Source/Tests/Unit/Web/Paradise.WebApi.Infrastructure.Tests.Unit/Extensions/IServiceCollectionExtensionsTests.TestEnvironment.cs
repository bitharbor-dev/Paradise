using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Options;
using Paradise.WebApi.Infrastructure.Extensions;
using Paradise.WebApi.Infrastructure.Options;
using Paradise.WebApi.Infrastructure.Tests.Unit.Authentication.JwtBearer.Implementation;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Extensions;

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
        #region Properties
        /// <summary>
        /// Application configuration which helps resolving the registered services.
        /// </summary>
        public OptionsContainer Options { get; } = new()
        {
            AuthenticationOptions = new()
            {
                AccessTokenLifetime = TimeSpan.FromMinutes(5),
                RefreshTokenLifetime = TimeSpan.FromDays(90),
                TwoFactorTokenLifetime = TimeSpan.FromDays(1),
                TwoFactorVerificationCodeLength = 6
            },
            JwtBearerOptions = new()
            {
                Events = null!,
                TokenValidationParameters = new()
                {
                    AuthenticationType = "Bearer",
                    ClockSkew = TimeSpan.Zero,
                    PropertyBag = new Dictionary<string, object>()
                    {
                        [nameof(IJwtSigningKeyProvider.JwtAlgorithm)] = SecurityAlgorithms.HmacSha256
                    },
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidAudience = nameof(JwtManagerTests),
                    ValidIssuer = nameof(JwtManagerTests)
                }
            },
            SymmetricSigningKeyProviderOptions = new()
            {
                Secret = "Ug5xCXJaNaxfx78KdQxQZDAmniAbZw6V"
            },
            AsymmetricSigningKeyProviderOptions = new()
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
        /// <see cref="IServiceCollectionExtensions.AddJwtBearerAuthentication"/> registration method.
        /// </summary>
        /// <param name="environmentName">
        /// Current environment name.
        /// </param>
        /// <returns>
        /// A configured <see cref="IServiceProvider"/>.
        /// </returns>
        [SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance",
            Justification = "Intentional encapsulation.")]
        public IServiceProvider BuildWebInfrastructureServiceProvider(string environmentName)
        {
            var configuration = BuildConfiguration();

            var services = new ServiceCollection()
                .AddJwtBearerAuthentication(configuration, typeof(JwtBearerEvents), environmentName);

            return services.BuildServiceProvider();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Builds the <see cref="IConfiguration"/> instance representing
        /// the current <see cref="Options"/> instance.
        /// </summary>
        /// <returns>
        /// The <see cref="IConfiguration"/> representation of the <see cref="Options"/>.
        /// </returns>
        private IConfiguration BuildConfiguration()
        {
            using var configurationStream = new MemoryStream();
            JsonSerializer.Serialize(configurationStream, Options);

            configurationStream.Position = 0;

            return new ConfigurationBuilder()
                .AddJsonStream(configurationStream)
                .Build();
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
        /// Configurable authentication options instance.
        /// </summary>
        public AuthenticationOptions? AuthenticationOptions { get; set; }

        /// <summary>
        /// Configurable JWT bearer options instance.
        /// </summary>
        public JwtBearerOptions? JwtBearerOptions { get; set; }

        /// <summary>
        /// Configurable secret-based signing key provider options instance.
        /// </summary>
        public SymmetricSigningKeyProviderOptions? SymmetricSigningKeyProviderOptions { get; set; }

        /// <summary>
        /// Configurable Azure KeyVault signing key provider options instance.
        /// </summary>
        public AsymmetricSigningKeyProviderOptions? AsymmetricSigningKeyProviderOptions { get; set; }
        #endregion
    }
    #endregion
}