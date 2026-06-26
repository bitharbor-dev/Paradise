using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Infrastructure.Communication;
using Paradise.ApplicationLogic.Infrastructure.Domain.Events.Identity;
using Paradise.ApplicationLogic.Infrastructure.EventListeners.Base;
using Paradise.ApplicationLogic.Options.Models;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Services.MessageTemplates;
using Paradise.Models;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;
using Paradise.Primitives.Web;

namespace Paradise.ApplicationLogic.Infrastructure.EventListeners.Domain.Identity;

/// <summary>
/// Reacts to the <see cref="UserRegisteredEvent"/> by sending the newly registered
/// user an email containing a confirmation link to verify their email address.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SendEmailAddressConfirmationLink"/> class.
/// </remarks>
/// <param name="serviceProvider">
/// The <see cref="IServiceProvider"/> instance used to resolve scoped dependencies.
/// </param>
internal sealed class SendEmailAddressConfirmationLink(IServiceProvider serviceProvider)
    : IdentityTokenEventListener<UserRegisteredEvent>(serviceProvider)
{
    #region Fields
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override async Task ProcessAsync(UserRegisteredEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var scope = _serviceProvider.CreateAsyncScope();

        await using (scope.ConfigureAwait(false))
        {
            var applicationOptions = scope.ServiceProvider.GetRequiredService<IOptions<ApplicationOptions>>();
            var emailTemplateOptions = scope.ServiceProvider.GetRequiredService<IOptions<EmailTemplateOptions>>();
            var timeProvider = scope.ServiceProvider.GetRequiredService<TimeProvider>();
            var communicationClient = scope.ServiceProvider.GetRequiredService<ICommunicationClient>();

            var timeout = applicationOptions.Value.Timeout.EmailConfirmationTimeout;
            var expiryDate = timeProvider.GetUtcNow().Add(timeout);

            var identityToken = new IdentityToken(domainEvent.EmailAddress,
                                                  domainEvent.EmailConfirmationToken,
                                                  null,
                                                  expiryDate);

            var link = CreateIdentityTokenLink(identityToken,
                                               applicationOptions.Value.ApiUrl,
                                               StaticRoutes.ConfirmEmailAddress,
                                               new() { ["culture"] = domainEvent.UserCulture.Name });

            var request = new SendEmailRequestModel(
                basicData: new([domainEvent.EmailAddress]),
                templateName: emailTemplateOptions.Value.EmailAddressConfirmationLinkTemplateName,
                culture: domainEvent.UserCulture,
                bodyArgs: [link]);

            await communicationClient.SendEmailAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }
    }
    #endregion
}