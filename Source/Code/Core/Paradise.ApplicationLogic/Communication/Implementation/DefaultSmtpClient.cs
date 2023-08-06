using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Extensions;
using Paradise.Models.Application.CommunicationModels;
using Paradise.Options.Models.Communication;
using System.Net.Mail;
using static Paradise.Models.ErrorCode;
using static System.Net.HttpStatusCode;

namespace Paradise.ApplicationLogic.Communication.Implementation;

/// <summary>
/// Provides emailing functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DefaultSmtpClient"/> class.
/// <para>
/// There is no <see cref="SmtpOptions"/> validation inside,
/// due to options validation being used on startup.
/// </para>
/// </remarks>
/// <param name="smtpOptions">
/// The accessor used to access the <see cref="SmtpOptions"/>.
/// </param>
public sealed class DefaultSmtpClient(IOptions<SmtpOptions> smtpOptions) : ISmtpClient
{
    #region Properties
    /// <inheritdoc/>
    public SmtpOptions Options { get; } = smtpOptions.Value;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async Task SendAsync(EmailModel model, CancellationToken cancellationToken = default)
    {
        model.From.ThrowIfEmptyOrWhiteSpace(ServiceUnavailable, InvalidSmtpConfiguration);

        model.From.IsValidEmailAddress().ThrowIfFalse(ServiceUnavailable, InvalidSmtpConfiguration);

        model.To.ThrowIfEmpty(BadRequest, EmptyRecipientsList);

        using var client = new SmtpClient(Options.Host, Options.Port);
        client.EnableSsl = Options.EnableSsl;
        client.Credentials = Options.Credentials;

        using var message = new MailMessage
        {
            Body = model.Body,
            From = new(model.From),
            IsBodyHtml = model.IsBodyHtml,
            Subject = model.Subject
        };

        foreach (var recipient in model.To)
            message.To.Add(recipient);

        if (model.Cc is not null)
        {
            foreach (var recipient in model.Cc)
                message.CC.Add(recipient);
        }

        if (model.Bcc is not null)
        {
            foreach (var recipient in model.Bcc)
                message.Bcc.Add(recipient);
        }

        if (model.Attachmetns is not null)
        {
            foreach (var attachment in model.Attachmetns)
                message.Attachments.Add(new(new MemoryStream(attachment.Data), attachment.FileName, attachment.MimeType));
        }

        await client.SendMailAsync(message, cancellationToken);
    }
    #endregion
}