using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Infrastructure.Communication;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Services;
using Paradise.Domain.Base.Events;
using Paradise.Domain.Events.Identity.Users;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;

namespace Paradise.ApplicationLogic.EventListeners.Domain.Identity.Users;

/// <summary>
/// Reacts to <see cref="TwoFactorAuthenticationOccurringEvent"/> by
/// sending an email to the user
/// containing the two-factor authentication code.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SendTwoFactorAuthenticationCode"/> class.
/// </remarks>
/// <param name="serviceProvider">
/// The <see cref="IServiceProvider"/> instance used to resolve scoped dependencies.
/// </param>
internal sealed class SendTwoFactorAuthenticationCode(IServiceProvider serviceProvider)
    : IDomainEventListener<TwoFactorAuthenticationOccurringEvent>
{
    #region Public methods
    /// <inheritdoc/>
    public async Task ProcessAsync(TwoFactorAuthenticationOccurringEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var scope = serviceProvider.CreateAsyncScope();
        var emailTemplateOptions = scope.ServiceProvider.GetRequiredService<IOptions<EmailTemplateOptions>>();
        var communicationClient = scope.ServiceProvider.GetRequiredService<ICommunicationClient>();

        var request = new EmailSendRequestModel(
            basicData: new([domainEvent.EmailAddress]),
            templateName: emailTemplateOptions.Value.TwoFactorVerificationTemplateName,
            culture: domainEvent.UserCulture,
            bodyArgs: [domainEvent.VerificationCode]);

        await communicationClient.SendEmailAsync(request, cancellationToken)
            .ConfigureAwait(false);

        await scope.DisposeAsync()
            .ConfigureAwait(false);
    }
    #endregion
}