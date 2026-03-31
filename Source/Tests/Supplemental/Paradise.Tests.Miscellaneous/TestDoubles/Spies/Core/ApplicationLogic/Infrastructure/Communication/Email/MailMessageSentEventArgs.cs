using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.ApplicationLogic.Infrastructure.Communication.Email;

/// <summary>
/// Provides data for <see cref="SpySmtpClient.MailSent"/> or
/// <see cref="SpyEmailSender.MailSent"/> events.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MailMessageSentEventArgs"/> class.
/// </remarks>
/// <param name="emailModel">
/// Email model.
/// </param>
public sealed class MailMessageSentEventArgs(EmailModel emailModel) : EventArgs
{
    #region Properties
    /// <summary>
    /// Email message.
    /// </summary>
    public EmailModel EmailModel { get; } = emailModel;
    #endregion
}