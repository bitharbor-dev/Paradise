using System.Net.Mail;

namespace Paradise.ApplicationLogic.Infrastructure.Communication.Email;

/// <summary>
/// An abstraction over the <see cref="SmtpClient"/>.
/// </summary>
internal interface ISmtpClient : IDisposable
{
    #region Methods
    /// <inheritdoc cref="SmtpClient.SendMailAsync(MailMessage, CancellationToken)"/>
    Task SendMailAsync(MailMessage message, CancellationToken cancellationToken);
    #endregion
}