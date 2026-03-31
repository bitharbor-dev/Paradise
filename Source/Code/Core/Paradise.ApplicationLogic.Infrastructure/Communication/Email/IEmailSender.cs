using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;

namespace Paradise.ApplicationLogic.Infrastructure.Communication.Email;

/// <summary>
/// Provides emailing functionalities.
/// </summary>
internal interface IEmailSender
{
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
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task SendAsync(EmailModel model, CancellationToken cancellationToken = default);
    #endregion
}