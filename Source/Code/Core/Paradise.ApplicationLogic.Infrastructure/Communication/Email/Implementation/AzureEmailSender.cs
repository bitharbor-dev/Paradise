using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Communication.Email;
using Paradise.Common.Extensions;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;
using static Paradise.Localization.ExceptionHandling.ExceptionMessages;

namespace Paradise.ApplicationLogic.Infrastructure.Communication.Email.Implementation;

/// <summary>
/// Provides emailing functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AzureEmailSender"/> class.
/// <para>
/// There is no <see cref="SmtpOptions"/> validation inside,
/// due to options validation being used on startup.
/// </para>
/// </remarks>
/// <param name="smtpOptions">
/// The accessor used to access the <see cref="SmtpOptions"/>.
/// </param>
/// <param name="client">
/// The <see cref="EmailClient"/> instance used to send email messages.
/// </param>
internal sealed class AzureEmailSender(IOptions<SmtpOptions> smtpOptions,
                                       EmailClient client) : IEmailSender
{
    #region Public methods
    /// <inheritdoc/>
    public async Task SendAsync(EmailModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (smtpOptions.Value.LocalEmailStorage.IsNotNullOrWhiteSpace())
        {
            var message = GetMessageInvalidSmtpConfiguration();

            throw new NotSupportedException(message);
        }

        if (model.From.IsNullOrWhiteSpace() || !model.From.IsValidEmailAddress())
        {
            var message = GetMessageInvalidSmtpConfiguration();

            throw new InvalidOperationException(message);
        }

        if (!model.To.Any())
        {
            var message = GetMessageEmptyRecipientsList();

            throw new InvalidOperationException(message);
        }

        var content = new EmailContent(model.Subject);

        if (model.IsBodyHtml)
            content.Html = model.Body;
        else
            content.PlainText = model.Body;

        var recipients = new EmailRecipients(
            model.To.Select(recipient => new EmailAddress(recipient)),
            model.Cc?.Select(recipient => new EmailAddress(recipient)),
            model.Bcc?.Select(recipient => new EmailAddress(recipient)));

        var emailMessage = new EmailMessage(smtpOptions.Value.Credentials!.UserName, recipients, content);

        if (model.Attachments is not null)
        {
            foreach (var attachment in model.Attachments)
            {
                var attachmentContent = await BinaryData.FromStreamAsync(attachment.Data, cancellationToken)
                    .ConfigureAwait(false);

                emailMessage.Attachments.Add(new(attachment.FileName, attachment.MimeType, attachmentContent));
            }
        }

        await client.SendAsync(WaitUntil.Completed, emailMessage, cancellationToken)
            .ConfigureAwait(false);
    }
    #endregion
}