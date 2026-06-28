using Paradise.ApplicationLogic.Infrastructure.Communication;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;

namespace Paradise.Tests.Doubles.Spies.Core.ApplicationLogic.Infrastructure.Communication;

/// <summary>
/// Spy <see cref="ICommunicationClient"/> implementation.
/// </summary>
public sealed class SpyCommunicationClient : ICommunicationClient
{
    #region Public methods
    /// <inheritdoc/>
    public Task<EmailModel> SendEmailAsync(SendEmailRequestModel request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var subject = request.TemplateName;
        var body = request.BodyArgs is null
            ? string.Empty
            : string.Join(Environment.NewLine, request.BodyArgs.ToArray());
        var from = string.Empty;

        var result = new EmailModel(subject, body, from, request.BasicData);

        SendEmailRequested?.Invoke(this, new(request));

        return Task.FromResult(result);
    }
    #endregion

    #region Events
    /// <summary>
    /// Occurs when an email send request is submitted.
    /// </summary>
    public event EventHandler<SendEmailRequestSubmittedEventArgs>? SendEmailRequested;
    #endregion
}