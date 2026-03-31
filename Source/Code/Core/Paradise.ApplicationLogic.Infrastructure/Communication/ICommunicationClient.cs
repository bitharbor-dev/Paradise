using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;

namespace Paradise.ApplicationLogic.Infrastructure.Communication;

/// <summary>
/// Provides communication functionalities.
/// </summary>
public interface ICommunicationClient
{
    #region Methods
    /// <summary>
    /// Sends an email message.
    /// </summary>
    /// <param name="request">
    /// Contains necessary information to perform email sending operation.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="EmailModel"/> containing information about the message sent.
    /// </returns>
    Task<EmailModel> SendEmailAsync(EmailSendRequestModel request, CancellationToken cancellationToken = default);
    #endregion
}