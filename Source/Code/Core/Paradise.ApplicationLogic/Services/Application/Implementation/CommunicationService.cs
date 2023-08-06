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
    private readonly ISmtpClient _smtpClient = smtpClient;
    private readonly IEmailTemplatesRepository _emailTemplatesRepository = emailTemplatesRepository;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async Task<Result<EmailModel>> SendEmailAsync(EmailSendRequestModel request, CancellationToken cancellationToken = default)
    {
        _smtpOptions.Credentials.UserName.ThrowIfNullOrWhiteSpace(ServiceUnavailable, InvalidSmtpConfiguration, SenderEmailIsMissing);

        var basicData = request.BasicData;

        basicData.To.ThrowIfEmpty(BadRequest, EmptyRecipientsList);

        ValidateRecipients(basicData.To);

        if (basicData.Cc is not null)
            ValidateRecipients(basicData.Cc);

        if (basicData.Bcc is not null)
            ValidateRecipients(basicData.Bcc);

        var template = await FindEmailTemplateAsync(
            request.TemplateName, request.Culture, request.UseNullOrInvariantCultureAsFallback, cancellationToken);

        template.ThrowIfNull(NotFound, MessageTemplateNotFound, request.TemplateName, request.Culture?.Name);

        var message = CreateMessage(template, _smtpOptions.Credentials.UserName, basicData, request.SubjectArgs, request.BodyArgs);

        await _smtpClient.SendAsync(message, cancellationToken);

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
    /// Asynchronously finds the <see cref="EmailTemplate"/>
    /// with the given <paramref name="name"/> and <paramref name="culture"/>.
    /// <para>
    /// If such <see cref="EmailTemplate"/> does not exist and
    /// <paramref name="useNullOrInvariantCultureAsFallback"/> is <see langword="true"/>,
    /// attempts to find another one with <see cref="CultureInfo.InvariantCulture"/> or
    /// <see langword="null"/> culture.
    /// </para>
    /// </summary>
    /// <param name="name">
    /// Template name.
    /// </param>
    /// <param name="culture">
    /// Template culture.
    /// </param>
    /// <param name="useNullOrInvariantCultureAsFallback">
    /// Indicates whether the <see cref="CultureInfo.InvariantCulture"/>
    /// or <see langword="null"/> culture should be used
    /// in case the template with the specified <paramref name="culture"/>
    /// does not exist.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="EmailTemplate"/> found or <see langword="null"/>.
    /// </returns>
    private async Task<EmailTemplate?> FindEmailTemplateAsync(string name, CultureInfo? culture,
                                                              bool useNullOrInvariantCultureAsFallback = true,
                                                              CancellationToken cancellationToken = default)
    {
        var template = await _emailTemplatesRepository
            .GetByNameAndCultureAsync(name, culture, cancellationToken);

        if (template is null && useNullOrInvariantCultureAsFallback)
        {
            template = await _emailTemplatesRepository
                .GetByNameAndCultureAsync(name, null, cancellationToken);

            template ??= await _emailTemplatesRepository
                .GetByNameAndCultureAsync(name, CultureInfo.InvariantCulture, cancellationToken);
        }

        return template;
    }

    /// <summary>
    /// Creates the <see cref="EmailModel"/> using the given
    /// <paramref name="template"/>, <paramref name="from"/>,
    /// <paramref name="model"/>, <paramref name="subjectArgs"/>
    /// and <paramref name="bodyArgs"/>.
    /// </summary>
    /// <param name="template">
    /// The <see cref="EmailTemplate"/> to be used
    /// to format the email body.
    /// </param>
    /// <param name="from">
    /// Sender email address.
    /// </param>
    /// <param name="model">
    /// Basic email information.
    /// </param>
    /// <param name="subjectArgs">
    /// An object array that contains zero or more objects to format the email template subject.
    /// </param>
    /// <param name="bodyArgs">
    /// An object array that contains zero or more objects to format the email template body.
    /// </param>
    /// <returns>
    /// Prepared for sending <see cref="EmailModel"/> instance.
    /// </returns>
    private static EmailModel CreateMessage(EmailTemplate template, string from, BaseEmailModel model,
                                            IList<object?>? subjectArgs, IList<object?>? bodyArgs)
    {
        string? body = null;
        string? subject = null;

        try
        {
            body = template.GetFormattedText(bodyArgs ?? Array.Empty<object>());
            subject = template.GetFormattedSubject(subjectArgs ?? Array.Empty<object>());
        }
        catch (IndexOutOfRangeException e)
        {
            Throw(ServiceUnavailable, MessageTemplateInvalidPlaceholdersNumber, e.Message);
        }
        catch (FormatException e)
        {
            Throw(ServiceUnavailable, MessageTemplateMissingPlaceholder, e.Message);
        }

        return new(body, from, subject, model)
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