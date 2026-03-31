using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.EventListeners.Base;
using Paradise.ApplicationLogic.Infrastructure.Communication;
using Paradise.ApplicationLogic.Options.Models;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Services;
using Paradise.Common.Web;
using Paradise.Domain.Base.Events;
using Paradise.Domain.Events.Identity.Users;
using Paradise.Models;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;

namespace Paradise.ApplicationLogic.EventListeners.Domain.Identity.Users;

/// <summary>
/// Reacts to <see cref="EmailAddressResetRequestedEvent"/> by sending an email
/// to the new address with a secure link to confirm and reset the email address.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SendEmailAddressChangeLink"/> class.
/// </remarks>
/// <param name="serviceProvider">
/// The <see cref="IServiceProvider"/> instance used to resolve scoped dependencies.
/// </param>
internal sealed class SendEmailAddressChangeLink(IServiceProvider serviceProvider)
    : IdentityTokenEventListener<EmailAddressResetRequestedEvent>(serviceProvider), IOrderedDomainEventListener<EmailAddressResetRequestedEvent>
{
    #region Fields
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    #endregion

    #region Properties
    /// <inheritdoc/>
    public int ProcessingOrder { get; } = 1;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override async Task ProcessAsync(EmailAddressResetRequestedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var scope = _serviceProvider.CreateAsyncScope();
        var applicationOptions = scope.ServiceProvider.GetRequiredService<IOptions<ApplicationOptions>>();
        var emailTemplateOptions = scope.ServiceProvider.GetRequiredService<IOptions<EmailTemplateOptions>>();
        var timeProvider = scope.ServiceProvider.GetRequiredService<TimeProvider>();
        var communicationClient = scope.ServiceProvider.GetRequiredService<ICommunicationClient>();

        var timeout = applicationOptions.Value.Timeout.ResetEmailAddressTimeout;
        var expiryDate = timeProvider.GetUtcNow().Add(timeout);

        var identityToken = new IdentityToken(domainEvent.CurrentEmailAddress,
                                              domainEvent.ChangeEmailAddressToken,
                                              domainEvent.NewEmailAddress,
                                              expiryDate);

        var link = CreateIdentityTokenLink(identityToken,
                                           applicationOptions.Value.ApiUrl,
                                           StaticRoutes.ResetEmailAddress,
                                           new() { ["culture"] = domainEvent.UserCulture.Name });

        var request = new EmailSendRequestModel(
            basicData: new([domainEvent.NewEmailAddress]),
            templateName: emailTemplateOptions.Value.EmailAddressChangeLinkTemplateName,
            culture: domainEvent.UserCulture,
            bodyArgs: [domainEvent.UserName, link]);

        await communicationClient.SendEmailAsync(request, cancellationToken)
            .ConfigureAwait(false);

        await scope.DisposeAsync()
            .ConfigureAwait(false);
    }
    #endregion
}