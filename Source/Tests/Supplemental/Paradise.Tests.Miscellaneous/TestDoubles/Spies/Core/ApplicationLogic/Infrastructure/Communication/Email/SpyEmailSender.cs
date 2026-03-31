using Paradise.ApplicationLogic.Infrastructure.Communication.Email;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.ApplicationLogic.Infrastructure.Communication.Email;

/// <summary>
/// Fake <see cref="IEmailSender"/> implementation.
/// </summary>
public sealed class SpyEmailSender : IEmailSender
{
    #region Public methods
    /// <inheritdoc/>
    public Task SendAsync(EmailModel model, CancellationToken cancellationToken = default)
    {
        MailSent?.Invoke(this, new(model));

        return Task.CompletedTask;
    }
    #endregion

    #region Events
    /// <summary>
    /// Occurs when an email message is sent.
    /// </summary>
    public event EventHandler<MailMessageSentEventArgs>? MailSent;
    #endregion
}