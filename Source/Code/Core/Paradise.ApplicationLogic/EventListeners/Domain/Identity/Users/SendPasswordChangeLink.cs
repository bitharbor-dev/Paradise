using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.EventListeners.Base;
using Paradise.ApplicationLogic.Infrastructure.Communication;
using Paradise.ApplicationLogic.Options.Models;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Services;
using Paradise.Common.Web;
using Paradise.Domain.Events.Identity.Users;
using Paradise.Models;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;

namespace Paradise.ApplicationLogic.EventListeners.Domain.Identity.Users;

/// <summary>
/// Reacts to <see cref="PasswordResetRequestedEvent"/> by sending an email
/// with a secure link to reset the password.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SendPasswordChangeLink"/> class.
/// </remarks>
/// <param name="serviceProvider">
/// The <see cref="IServiceProvider"/> instance used to resolve scoped dependencies.
/// </param>
internal sealed class SendPasswordChangeLink(IServiceProvider serviceProvider)
    : IdentityTokenEventListener<PasswordResetRequestedEvent>(serviceProvider)
{
    #region Fields
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override async Task ProcessAsync(PasswordResetRequestedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var scope = _serviceProvider.CreateAsyncScope();
        var applicationOptions = scope.ServiceProvider.GetRequiredService<IOptions<ApplicationOptions>>();
        var emailTemplateOptions = scope.ServiceProvider.GetRequiredService<IOptions<EmailTemplateOptions>>();
        var timeProvider = scope.ServiceProvider.GetRequiredService<TimeProvider>();
        var communicationClient = scope.ServiceProvider.GetRequiredService<ICommunicationClient>();

        var timeout = applicationOptions.Value.Timeout.ResetPasswordTimeout;
        var expiryDate = timeProvider.GetUtcNow().Add(timeout);

        var identityToken = new IdentityToken(domainEvent.EmailAddress,
                                              domainEvent.ChangePasswordToken,
                                              null,
                                              expiryDate);

        var link = CreateIdentityTokenLink(identityToken,
                                           applicationOptions.Value.ApiUrl,
                                           StaticRoutes.ResetPassword,
                                           new() { ["culture"] = domainEvent.UserCulture.Name });

        var request = new EmailSendRequestModel(
            basicData: new([domainEvent.EmailAddress]),
            templateName: emailTemplateOptions.Value.PasswordChangeLinkTemplateName,
            culture: domainEvent.UserCulture,
            bodyArgs: [link]);

        await communicationClient.SendEmailAsync(request, cancellationToken)
            .ConfigureAwait(false);

        await scope.DisposeAsync()
            .ConfigureAwait(false);
    }
    #endregion
}