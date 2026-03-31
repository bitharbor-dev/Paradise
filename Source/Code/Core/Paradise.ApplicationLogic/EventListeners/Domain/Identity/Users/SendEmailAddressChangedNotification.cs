using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Infrastructure.Communication;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Services;
using Paradise.Domain.Base.Events;
using Paradise.Domain.Events.Identity.Users;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;

namespace Paradise.ApplicationLogic.EventListeners.Domain.Identity.Users;

/// <summary>
/// Reacts to <see cref="EmailAddressResetCompletedEvent"/> by
/// sending a notification email to the user and their old email address
/// informing them that the email address reset process has been completed.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SendEmailAddressChangedNotification"/> class.
/// </remarks>
/// <param name="serviceProvider">
/// The <see cref="IServiceProvider"/> instance used to resolve scoped dependencies.
/// </param>
internal sealed class SendEmailAddressChangedNotification(IServiceProvider serviceProvider)
    : IDomainEventListener<EmailAddressResetCompletedEvent>
{
    #region Public methods
    /// <inheritdoc/>
    public async Task ProcessAsync(EmailAddressResetCompletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var scope = serviceProvider.CreateAsyncScope();
        var emailTemplateOptions = scope.ServiceProvider.GetRequiredService<IOptions<EmailTemplateOptions>>();
        var communicationClient = scope.ServiceProvider.GetRequiredService<ICommunicationClient>();

        var request = new EmailSendRequestModel(
            basicData: new([domainEvent.OldEmailAddress, domainEvent.NewEmailAddress]),
            templateName: emailTemplateOptions.Value.EmailAddressChangedNotificationTemplateName,
            culture: domainEvent.UserCulture,
            bodyArgs: [domainEvent.UserName, domainEvent.NewEmailAddress]);

        await communicationClient.SendEmailAsync(request, cancellationToken)
            .ConfigureAwait(false);

        await scope.DisposeAsync()
            .ConfigureAwait(false);
    }
    #endregion
}