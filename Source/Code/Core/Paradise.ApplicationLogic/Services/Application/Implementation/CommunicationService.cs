using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Communication;
using Paradise.ApplicationLogic.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Exceptions;
using Paradise.ApplicationLogic.Extensions;
using Paradise.DataAccess.Repositories.Application;
using Paradise.Models;
using Paradise.Models.Application.CommunicationModels;
using Paradise.Options.Models.Communication;
using System.Globalization;
using static Paradise.ApplicationLogic.Exceptions.ResultException;
using static Paradise.Models.ErrorCode;
using static System.Net.HttpStatusCode;

namespace Paradise.ApplicationLogic.Services.Application.Implementation;

/// <summary>
/// Provides messaging functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CommunicationService"/> class.
/// </remarks>
/// <param name="smtpOptions">
/// The accessor used to access the <see cref="SmtpOptions"/>.
/// </param>
/// <param name="smtpClient">
/// SMTP client instance to be used to
/// perform emailing operations.
/// </param>
/// <param name="emailTemplatesRepository">
/// Email templates repository.
/// </param>
public sealed class CommunicationService(IOptions<SmtpOptions> smtpOptions,
                                         ISmtpClient smtpClient,
                                         IEmailTemplatesRepository emailTemplatesRepository)
    : ICommunicationService
{
    #region Fields
    private readonly SmtpOptions _smtpOptions = smtpOptions.Value;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async Task<Result<EmailModel>> SendEmailAsync(EmailSendRequestModel request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        _smtpOptions.Credentials.ThrowIfNull(ServiceUnavailable, InvalidSmtpConfiguration);

        _smtpOptions.Credentials.UserName.ThrowIfNullOrWhiteSpace(ServiceUnavailable, InvalidSmtpConfiguration, SenderEmailIsMissing);

        request.BasicData.To.ThrowIfEmpty(BadRequest, EmptyRecipientsList);

        ValidateRecipients(request.BasicData.To);

        if (request.BasicData.Cc is not null)
            ValidateRecipients(request.BasicData.Cc);

        if (request.BasicData.Bcc is not null)
            ValidateRecipients(request.BasicData.Bcc);

        var template = await FindEmailTemplateAsync(request, cancellationToken)
            .ConfigureAwait(false);

        template.ThrowIfNull(NotFound, MessageTemplateNotFound, request.TemplateName, request.Culture?.Name);

        var message = CreateMessage(template, request, _smtpOptions.Credentials.UserName);

        await smtpClient.SendAsync(message, cancellationToken)
            .ConfigureAwait(false);

        EmailMessageSent?.Invoke(this, new(request.TemplateName, request.Culture, request.BodyArgs, request.SubjectArgs, message));

        return new(message, OK);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Validates the recipients' email addresses.
    /// </summary>
    /// <param name="recipients">
    /// Email addresses to be validated.
    /// </param>
    private static void ValidateRecipients(IEnumerable<string> recipients)
    {
        var exception = new ResultException();

        foreach (var address in recipients)
        {
            if (!address.IsValidEmailAddress())
                exception.AddError(BadRequest, InvalidEmail, address);
        }

        if (exception.HaveErrors)
            throw exception;
    }

    /// <summary>
    /// Asynchronously finds an <see cref="EmailTemplate"/>
    /// with the name and culture specified in the given <paramref name="request"/>.
    /// <para>
    /// If such <see cref="EmailTemplate"/> does not exist and
    /// <see cref="EmailSendRequestModel.UseNullOrInvariantCultureAsFallback"/>
    /// is <see langword="true"/> - attempts to find another
    /// <see cref="EmailTemplate"/> with <see cref="CultureInfo.InvariantCulture"/>
    /// or <see langword="null"/> culture.
    /// </para>
    /// </summary>
    /// <param name="request">
    /// Contains <see cref="EmailTemplate"/>
    /// lookup criteria.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="EmailTemplate"/> found or <see langword="null"/>.
    /// </returns>
    private async Task<EmailTemplate?> FindEmailTemplateAsync(EmailSendRequestModel request, CancellationToken cancellationToken = default)
    {
        var templateName = request.TemplateName;
        var culture = request.Culture;

        var template = await emailTemplatesRepository
            .GetByNameAndCultureAsync(templateName, culture, cancellationToken)
            .ConfigureAwait(false);

        if (template is null && request.UseNullOrInvariantCultureAsFallback)
        {
            culture = null;

            template = await emailTemplatesRepository
                .GetByNameAndCultureAsync(templateName, culture, cancellationToken)
                .ConfigureAwait(false);

            culture = CultureInfo.InvariantCulture;

            template ??= await emailTemplatesRepository
                .GetByNameAndCultureAsync(templateName, culture, cancellationToken)
                .ConfigureAwait(false);
        }

        return template;
    }

    /// <summary>
    /// Creates the <see cref="EmailModel"/> using the given
    /// <paramref name="template"/>, <paramref name="request"/> and
    /// <paramref name="from"/>.
    /// </summary>
    /// <param name="template">
    /// The <see cref="EmailTemplate"/> to be used to
    /// format the email body.
    /// </param>
    /// <param name="request">
    /// Contains other data, necessary for proper
    /// <see cref="EmailModel"/> creation.
    /// </param>
    /// <param name="from">
    /// Sender email address.
    /// </param>
    /// <returns>
    /// Prepared for sending <see cref="EmailModel"/> instance.
    /// </returns>
    private static EmailModel CreateMessage(EmailTemplate template, EmailSendRequestModel request, string from)
    {
        string? body = null;
        string? subject = null;

        try
        {
            body = template.GetFormattedText(request.BodyArgs ?? []);
            subject = template.GetFormattedSubject(request.SubjectArgs ?? []);
        }
        catch (InvalidOperationException e)
        {
            Throw(ServiceUnavailable, MessageTemplateInvalidPlaceholdersNumber, e.Message);
        }
        catch (FormatException e)
        {
            Throw(ServiceUnavailable, MessageTemplateMissingPlaceholder, e.Message);
        }

        return new(subject, body, from, request.BasicData)
        {
            IsBodyHtml = template.IsBodyHtml,
            Sent = DateTime.UtcNow
        };
    }
    #endregion

    #region Events
    /// <inheritdoc/>
    public event EventHandler<EmailMessageSentEventArgs>? EmailMessageSent;
    #endregion
}