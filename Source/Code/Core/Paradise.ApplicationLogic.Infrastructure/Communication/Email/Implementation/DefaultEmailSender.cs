using Paradise.ApplicationLogic.Options.Models.Infrastructure.Communication.Email;
using Paradise.Common.Extensions;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;
using System.Net.Mail;
using static Paradise.Localization.ExceptionHandling.ExceptionMessages;

namespace Paradise.ApplicationLogic.Infrastructure.Communication.Email.Implementation;

/// <summary>
/// Provides emailing functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DefaultEmailSender"/> class.
/// <para>
/// There is no <see cref="SmtpOptions"/> validation inside,
/// due to options validation being used on startup.
/// </para>
/// </remarks>
/// <param name="client">
/// The <see cref="ISmtpClient"/> instance used to send email messages.
/// </param>
internal sealed class DefaultEmailSender(ISmtpClient client) : IEmailSender
{
    #region Public methods
    /// <inheritdoc/>
    public async Task SendAsync(EmailModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

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

        using var mailMessage = new MailMessage
        {
            Body = model.Body,
            From = new(model.From),
            IsBodyHtml = model.IsBodyHtml,
            Subject = model.Subject
        };

        foreach (var recipient in model.To)
            mailMessage.To.Add(recipient);

        if (model.CarbonCopy is not null)
        {
            foreach (var recipient in model.CarbonCopy)
                mailMessage.CC.Add(recipient);
        }

        if (model.BlindCarbonCopy is not null)
        {
            foreach (var recipient in model.BlindCarbonCopy)
                mailMessage.Bcc.Add(recipient);
        }

        if (model.Attachments is not null)
        {
            foreach (var attachment in model.Attachments)
                mailMessage.Attachments.Add(new(attachment.Data, attachment.FileName, attachment.MimeType));
        }

        await client.SendMailAsync(mailMessage, cancellationToken)
            .ConfigureAwait(false);
    }
    #endregion
}