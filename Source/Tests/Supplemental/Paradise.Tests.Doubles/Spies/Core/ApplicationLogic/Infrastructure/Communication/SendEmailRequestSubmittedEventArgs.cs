using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;

namespace Paradise.Tests.Doubles.Spies.Core.ApplicationLogic.Infrastructure.Communication;

/// <summary>
/// Provides data for <see cref="SpyCommunicationClient.SendEmailRequested"/> event.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SendEmailRequestSubmittedEventArgs"/> class.
/// </remarks>
/// <param name="request">
/// Send email request.
/// </param>
public sealed class SendEmailRequestSubmittedEventArgs(SendEmailRequestModel request) : EventArgs
{
    #region Properties
    /// <summary>
    /// Send email request.
    /// </summary>
    public SendEmailRequestModel Request { get; } = request;
    #endregion
}