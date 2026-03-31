using Azure.Communication.Email;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Spies.Azure.Communication.Email;

/// <summary>
/// Provides data for <see cref="SpyEmailClient.EmailSent"/> event.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailMessageSentEventArgs"/> class.
/// </remarks>
/// <param name="emailMessage">
/// Email message.
/// </param>
public sealed class EmailMessageSentEventArgs(EmailMessage emailMessage) : EventArgs
{
    #region Properties
    /// <summary>
    /// Email message.
    /// </summary>
    public EmailMessage EmailMessage { get; } = emailMessage;
    #endregion
}