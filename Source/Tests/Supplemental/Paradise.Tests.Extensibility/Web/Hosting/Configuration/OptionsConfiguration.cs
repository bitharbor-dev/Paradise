using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Options.Models;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Communication.Email;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Services.MessageTemplates;
using Paradise.Tests.Extensibility.Web.Hosting.Configuration.Base;

namespace Paradise.Tests.Extensibility.Web.Hosting.Configuration;

/// <summary>
/// Configures application options used by the test host.
/// </summary>
public sealed class OptionsConfiguration : IWebApplicationServicesConfiguration
{
    #region Fields
    private readonly IOptions<EmailTemplateOptions> _emailTemplateOptions;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="OptionsConfiguration"/> class.
    /// </summary>
    /// <param name="emailTemplateOptions">
    /// The accessor used to access the <see cref="EmailTemplateOptions"/>.
    /// </param>
    public OptionsConfiguration(out IOptions<EmailTemplateOptions> emailTemplateOptions)
    {
        emailTemplateOptions = _emailTemplateOptions = Options.Create(new EmailTemplateOptions()
        {
            EmailAddressChangedNotificationTemplateName = nameof(EmailTemplateOptions.EmailAddressChangedNotificationTemplateName),
            EmailAddressChangeLinkTemplateName = nameof(EmailTemplateOptions.EmailAddressChangeLinkTemplateName),
            EmailAddressChangingNotificationTemplateName = nameof(EmailTemplateOptions.EmailAddressChangingNotificationTemplateName),
            EmailAddressConfirmationLinkTemplateName = nameof(EmailTemplateOptions.EmailAddressConfirmationLinkTemplateName),
            PasswordChangedNotificationTemplateName = nameof(EmailTemplateOptions.PasswordChangedNotificationTemplateName),
            PasswordChangeLinkTemplateName = nameof(EmailTemplateOptions.PasswordChangeLinkTemplateName),
            TwoFactorVerificationTemplateName = nameof(EmailTemplateOptions.TwoFactorVerificationTemplateName)
        });
    }
    #endregion

    #region Properties
    /// <summary>
    /// Web API base URL.
    /// </summary>
    /// <remarks>
    /// Defaults to <see href="http://localhost"/>.
    /// </remarks>
    public Uri ApiUrl { get; set; } = new("http://localhost");
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        services
            .RemoveAll<IOptions<EmailTemplateOptions>>()
            .AddSingleton(_emailTemplateOptions);

        services.PostConfigureAll<ApplicationOptions>(options => options.ApiUrl = ApiUrl);
        services.PostConfigureAll<SmtpOptions>(options => options.LocalEmailStorage = SmtpOptions.ApplicationRootPlaceholder);
    }
    #endregion
}