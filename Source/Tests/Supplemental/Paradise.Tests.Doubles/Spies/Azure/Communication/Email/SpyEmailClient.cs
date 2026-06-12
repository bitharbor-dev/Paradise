using Azure;
using Azure.Communication.Email;

namespace Paradise.Tests.Doubles.Spies.Azure.Communication.Email;

/// <summary>
/// Spy <see cref="EmailClient"/> implementation.
/// </summary>
public sealed class SpyEmailClient : EmailClient
{
    #region Public methods
    /// <inheritdoc/>
    public override EmailSendOperation Send(WaitUntil wait, EmailMessage message,
                                            CancellationToken cancellationToken = default)
    {
        EmailSent?.Invoke(this, new(message));

        return new EmailSendOperation(Guid.NewGuid().ToString(), this);
    }

    /// <inheritdoc/>
    public override EmailSendOperation Send(WaitUntil wait, EmailMessage message,
                                            Guid operationId, CancellationToken cancellationToken = default)
    {
        EmailSent?.Invoke(this, new(message));

        return new EmailSendOperation(Guid.NewGuid().ToString(), this);
    }

    /// <inheritdoc/>
    public override EmailSendOperation Send(WaitUntil wait, string senderAddress,
                                            string recipientAddress, string subject,
                                            string htmlContent, string? plainTextContent = null,
                                            CancellationToken cancellationToken = default)
    {
        var content = new EmailContent(subject)
        {
            Html = htmlContent,
            PlainText = plainTextContent
        };

        var message = new EmailMessage(senderAddress, recipientAddress, content);

        EmailSent?.Invoke(this, new(message));

        return new EmailSendOperation(Guid.NewGuid().ToString(), this);
    }

    /// <inheritdoc/>
    public override EmailSendOperation Send(WaitUntil wait, string senderAddress,
                                            string recipientAddress, string subject,
                                            string htmlContent, Guid operationId,
                                            string? plainTextContent = null,
                                            CancellationToken cancellationToken = default)
    {
        var content = new EmailContent(subject)
        {
            Html = htmlContent,
            PlainText = plainTextContent
        };

        var message = new EmailMessage(senderAddress, recipientAddress, content);

        EmailSent?.Invoke(this, new(message));

        return new EmailSendOperation(operationId.ToString(), this);
    }

    /// <inheritdoc/>
    public override Task<EmailSendOperation> SendAsync(WaitUntil wait, EmailMessage message,
                                                       CancellationToken cancellationToken = default)
    {
        EmailSent?.Invoke(this, new(message));

        return Task.FromResult(new EmailSendOperation(Guid.NewGuid().ToString(), this));
    }

    /// <inheritdoc/>
    public override Task<EmailSendOperation> SendAsync(WaitUntil wait, EmailMessage message,
                                                       Guid operationId, CancellationToken cancellationToken = default)
    {
        EmailSent?.Invoke(this, new(message));

        return Task.FromResult(new EmailSendOperation(operationId.ToString(), this));
    }

    /// <inheritdoc/>
    public override Task<EmailSendOperation> SendAsync(WaitUntil wait, string senderAddress,
                                                       string recipientAddress, string subject,
                                                       string htmlContent, string? plainTextContent = null,
                                                       CancellationToken cancellationToken = default)
    {
        var content = new EmailContent(subject)
        {
            Html = htmlContent,
            PlainText = plainTextContent
        };

        var message = new EmailMessage(senderAddress, recipientAddress, content);

        EmailSent?.Invoke(this, new(message));

        return Task.FromResult(new EmailSendOperation(Guid.NewGuid().ToString(), this));
    }

    /// <inheritdoc/>
    public override Task<EmailSendOperation> SendAsync(WaitUntil wait, string senderAddress,
                                                       string recipientAddress, string subject,
                                                       string htmlContent, Guid operationId,
                                                       string? plainTextContent = null,
                                                       CancellationToken cancellationToken = default)
    {
        var content = new EmailContent(subject)
        {
            Html = htmlContent,
            PlainText = plainTextContent
        };

        var message = new EmailMessage(senderAddress, recipientAddress, content);

        EmailSent?.Invoke(this, new(message));

        return Task.FromResult(new EmailSendOperation(operationId.ToString(), this));
    }
    #endregion

    #region Events
    /// <summary>
    /// Occurs when an email message is sent.
    /// </summary>
    public event EventHandler<EmailMessageSentEventArgs>? EmailSent;
    #endregion
}