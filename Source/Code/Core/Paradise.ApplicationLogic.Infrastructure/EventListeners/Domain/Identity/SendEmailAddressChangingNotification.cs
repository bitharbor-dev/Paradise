using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Infrastructure.Communication;
using Paradise.ApplicationLogic.Infrastructure.Domain.Events.Identity;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Services.MessageTemplates;
using Paradise.Domain.Base.Events;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;

namespace Paradise.ApplicationLogic.Infrastructure.EventListeners.Domain.Identity;

/// <summary>
/// Reacts to <see cref="EmailAddressResetRequestedEvent"/> by sending a notification
/// to the user's current email address that their email is about to be changed.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SendEmailAddressChangingNotification"/> class.
/// </remarks>
/// <param name="serviceProvider">
/// The <see cref="IServiceProvider"/> instance used to resolve scoped dependencies.
/// </param>
internal sealed class SendEmailAddressChangingNotification(IServiceProvider serviceProvider)
    : IOrderedDomainEventListener<EmailAddressResetRequestedEvent>
{
    #region Properties
    /// <inheritdoc/>
    public int ProcessingOrder { get; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async Task ProcessAsync(EmailAddressResetRequestedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var scope = serviceProvider.CreateAsyncScope();

        await using (scope.ConfigureAwait(false))
        {
            var emailTemplateOptions = scope.ServiceProvider.GetRequiredService<IOptions<EmailTemplateOptions>>();
            var communicationClient = scope.ServiceProvider.GetRequiredService<ICommunicationClient>();

            var request = new SendEmailRequestModel(
                basicData: new([domainEvent.CurrentEmailAddress]),
                templateName: emailTemplateOptions.Value.EmailAddressChangingNotificationTemplateName,
                culture: domainEvent.UserCulture,
                bodyArgs: [domainEvent.UserName, domainEvent.NewEmailAddress]);

            await communicationClient.SendEmailAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }
    }
    #endregion
}