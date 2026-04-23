using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.Extensions;
using Paradise.ApplicationLogic.Options.Models;
using Paradise.ApplicationLogic.Options.Models.DataAccess.Seed.Providers;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Communication.Email;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Services;
using Paradise.Domain.Base.Events;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Paradise.ApplicationLogic.Tests.Unit.Extensions;

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
            ApplicationOptions = new()
            {
                ApiUrl = new("https://localhost:5001")
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
            SmtpOptions = new()
            {
                Credentials = new("UserName", "endpoint=https://test.com/;accesskey=1234"),
                EnableSecureSocketsLayer = false,
                Host = "smtp.test",
                Port = 2525
            }
        };
        #endregion

        #region Public methods
        /// <summary>
        /// Builds a service provider configured with <see cref="IServiceCollectionExtensions.AddApplicationLogic"/>.
        /// </summary>
        /// <param name="environmentName">
        /// Current environment name.
        /// </param>
        /// <param name="retryOptionsBuilder">
        /// An action used to configure global domain event retry policy.
        /// </param>
        [SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance",
            Justification = "Intentional encapsulation.")]
        public IServiceProvider BuildApplicationLogicServiceProvider(string environmentName,
                                                                     Action<DomainEventRetryOptions>? retryOptionsBuilder = null)
        {
            var configuration = BuildConfiguration();

            var services = new ServiceCollection()
                .AddApplicationLogic(configuration, environmentName)
                .AddDomainEvents(retryOptionsBuilder);

            return services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });
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
        /// Configurable application options.
        /// </summary>
        public ApplicationOptions? ApplicationOptions { get; set; }

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
        /// Configurable connection strings.
        /// </summary>
        public Dictionary<string, string>? ConnectionStrings { get; set; }

        /// <summary>
        /// Configurable SMTP options instance.
        /// </summary>
        public SmtpOptions? SmtpOptions { get; set; }
        #endregion
    }
    #endregion
}