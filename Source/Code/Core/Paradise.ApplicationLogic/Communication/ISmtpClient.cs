using Paradise.Models.Application.CommunicationModels;
using Paradise.Options.Models.Communication;

namespace Paradise.ApplicationLogic.Communication;

/// <summary>
/// Provides emailing functionalities.
/// </summary>
public interface ISmtpClient
{
    #region Properties
    /// <summary>
    /// SMTP client options.
    /// </summary>
    SmtpOptions Options { get; }
    #endregion

    #region Methods
    /// <summary>
    /// Sends an email message using the data in the given <paramref name="model"/>.
    /// </summary>
    /// <param name="model">
    /// The <see cref="EmailModel"/>
    /// which data to be used to send an email message.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    Task SendAsync(EmailModel model, CancellationToken cancellationToken = default);
    #endregion
}