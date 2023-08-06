using Paradise.Models;
using Paradise.Models.Application.CommunicationModels;
using System.Globalization;

namespace Paradise.ApplicationLogic.Services.Application;

/// <summary>
/// Provides messaging functionalities.
/// </summary>
public interface ICommunicationService
{
    #region Methods
    /// <summary>
    /// Sends a message to the given email addresses.
    /// </summary>
    /// <param name="request">
    /// Contains necessary information to perform email sending operation.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where the
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailModel"/>
    /// containing information about the message sent.
    /// </returns>
    Task<Result<EmailModel>> SendEmailAsync(EmailSendRequestModel request, CancellationToken cancellationToken = default);
    #endregion

    #region Events
    /// <summary>
    /// Occurs when the email message is successfully sent.
    /// </summary>
    event EventHandler<EmailMessageSentEventArgs>? EmailMessageSent;
    #endregion
}

/// <summary>
/// Provides data for <see cref="ICommunicationService.EmailMessageSent"/> event.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailMessageSentEventArgs"/> class.
/// </remarks>
/// <param name="templateName">
/// Template name.
/// </param>
/// <param name="culture">
/// Template culture.
/// </param>
/// <param name="bodyArgs">
/// An object array that contains zero or more objects to format the email template body.
/// </param>
/// <param name="subjectArgs">
/// An object array that contains zero or more objects to format the email template subject.
/// </param>
/// <param name="message">
/// Email message which was sent.
/// </param>
public sealed class EmailMessageSentEventArgs(string templateName, CultureInfo? culture, IEnumerable<object?>? bodyArgs,
                                              IEnumerable<object?>? subjectArgs, EmailModel message)
    : EventArgs
{
    #region Properties
    /// <summary>
    /// Template name.
    /// </summary>
    public string TemplateName { get; } = templateName;

    /// <summary>
    /// Template culture.
    /// </summary>
    public CultureInfo? Culture { get; } = culture;

    /// <summary>
    /// An object array that contains zero or more objects to format the email template body.
    /// </summary>
    public IEnumerable<object?>? BodyArgs { get; } = bodyArgs;

    /// <summary>
    /// An object array that contains zero or more objects to format the email template subject.
    /// </summary>
    public IEnumerable<object?>? SubjectArgs { get; } = subjectArgs;

    /// <summary>
    /// Email message which was sent.
    /// </summary>
    public EmailModel Message { get; set; } = message;
    #endregion
}