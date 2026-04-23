using Paradise.ApplicationLogic.Infrastructure.Communication.Email;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;
using System.Net.Mail;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.ApplicationLogic.Infrastructure.Communication.Email;

/// <summary>
/// Spy <see cref="ISmtpClient"/> implementation.
/// </summary>
public sealed class SpySmtpClient : ISmtpClient
{
    #region Public methods
    /// <inheritdoc/>
    public Task SendMailAsync(MailMessage message, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);

        var baseModel = new BaseEmailModel(message.To.Select(recipient => recipient.Address))
        {
            Attachments = message.Attachments.Select(a => new EmailAttachmentModel(a.ContentStream, a.Name!, a.ContentType.MediaType)).ToList(),
            BlindCarbonCopy = message.Bcc.Select(recipient => recipient.Address).ToList(),
            CarbonCopy = message.CC.Select(recipient => recipient.Address).ToList()
        };

        var model = new EmailModel(message.Subject, message.Body, message.From?.Address!, baseModel)
        {
            Body = message.Body,
            IsBodyHtml = message.IsBodyHtml
        };

        MailSent?.Invoke(this, new(model));

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Dispose() { }
    #endregion

    #region Events
    /// <summary>
    /// Occurs when an email message is sent.
    /// </summary>
    public event EventHandler<MailMessageSentEventArgs>? MailSent;
    #endregion
}