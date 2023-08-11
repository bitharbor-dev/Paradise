using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Extensions;
using Paradise.Models.Application.CommunicationModels;
using Paradise.Options.Models.Communication;
using static Paradise.Models.ErrorCode;
using static System.Net.HttpStatusCode;

namespace Paradise.ApplicationLogic.Communication.Implementation;

/// <summary>
/// Provides emailing functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AzureSmtpClient"/> class.
/// <para>
/// There is no <see cref="SmtpOptions"/> validation inside,
/// due to options validation being used on startup.
/// </para>
/// </remarks>
/// <param name="smtpOptions">
/// The accessor used to access the <see cref="SmtpOptions"/>.
/// </param>
public sealed class AzureSmtpClient(IOptions<SmtpOptions> smtpOptions) : ISmtpClient
{
    #region Properties
    /// <inheritdoc/>
    public SmtpOptions Options { get; } = smtpOptions.Value;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async Task SendAsync(EmailModel model, CancellationToken cancellationToken = default)
    {
        if (Options.StoreEmailsInsteadOfSending)
        {
            var localClient = new LocalSmtpClient(smtpOptions);
            await localClient.SendAsync(model, cancellationToken);

            return;
        }

        Options.Credentials.ThrowIfNull(ServiceUnavailable, InvalidSmtpConfiguration);

        model.From.ThrowIfEmptyOrWhiteSpace(ServiceUnavailable, InvalidSmtpConfiguration);

        model.From.IsValidEmailAddress().ThrowIfFalse(ServiceUnavailable, InvalidSmtpConfiguration);

        model.To.ThrowIfEmpty(BadRequest, EmptyRecipientsList);

        var client = new EmailClient(Options.Credentials.Password);
        var content = new EmailContent(model.Subject);

        if (model.IsBodyHtml)
            content.Html = model.Body;
        else
            content.PlainText = model.Body;

        var recipients = new EmailRecipients(
            model.To.Select(recipient => new EmailAddress(recipient)),
            model.Cc?.Select(recipient => new EmailAddress(recipient)),
            model.Bcc?.Select(recipient => new EmailAddress(recipient)));

        var message = new EmailMessage(Options.Credentials.UserName, recipients, content);

        if (model.Attachmetns is not null)
        {
            foreach (var attachment in model.Attachmetns)
                message.Attachments.Add(new(attachment.FileName, attachment.MimeType, new(attachment.Data)));
        }

        await client.SendAsync(WaitUntil.Completed, message, cancellationToken);
    }
    #endregion
}