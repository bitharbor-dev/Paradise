using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Infrastructure.Communication.Email;
using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Communication.Email;
using Paradise.Common.Extensions;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;
using System.Globalization;
using System.Text;
using static Paradise.Localization.ExceptionHandling.ExceptionMessages;

namespace Paradise.ApplicationLogic.Infrastructure.Communication.Implementation;

/// <summary>
/// Provides communication functionalities.
/// </summary>
internal sealed class CommunicationClient : ICommunicationClient
{
    #region Fields
    private readonly TimeProvider _timeProvider;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplatesRepository _emailTemplatesRepository;

    private readonly string _userName;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="CommunicationClient"/> class.
    /// </summary>
    /// <param name="smtpOptions">
    /// The accessor used to access the <see cref="SmtpOptions"/>.
    /// </param>
    /// <param name="timeProvider">
    /// Time provider.
    /// </param>
    /// <param name="emailSender">
    /// The <see cref="IEmailSender"/> instance to be used to
    /// perform emailing operations.
    /// </param>
    /// <param name="emailTemplatesRepository">
    /// Email templates repository.
    /// </param>
    public CommunicationClient(IOptions<SmtpOptions> smtpOptions,
                               TimeProvider timeProvider,
                               IEmailSender emailSender,
                               IEmailTemplatesRepository emailTemplatesRepository)
    {
        _timeProvider = timeProvider;
        _emailSender = emailSender;
        _emailTemplatesRepository = emailTemplatesRepository;

        if (smtpOptions.Value.Credentials is null)
        {
            var message = GetMessageInvalidSmtpConfiguration();

            throw new InvalidOperationException(message);
        }

        if (smtpOptions.Value.Credentials.UserName.IsNullOrWhiteSpace())
        {
            var message = GetMessageInvalidSmtpConfiguration();

            throw new InvalidOperationException(message);
        }

        _userName = smtpOptions.Value.Credentials.UserName;
    }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async Task<EmailModel> SendEmailAsync(EmailSendRequestModel request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!request.BasicData.To.Any())
        {
            var message = GetMessageEmptyRecipientsList();

            throw new InvalidOperationException(message);
        }

        ValidateRecipients(request.BasicData.To);

        if (request.BasicData.CarbonCopy is not null)
            ValidateRecipients(request.BasicData.CarbonCopy);

        if (request.BasicData.BlindCarbonCopy is not null)
            ValidateRecipients(request.BasicData.BlindCarbonCopy);

        var template = await FindEmailTemplateAsync(request, cancellationToken)
            .ConfigureAwait(false);

        var emailMessage = CreateMessage(template, request);

        await _emailSender.SendAsync(emailMessage, cancellationToken)
            .ConfigureAwait(false);

        return emailMessage;
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
        var errorBuilder = new StringBuilder();

        foreach (var address in recipients)
        {
            if (!address.IsValidEmailAddress())
            {
                var message = GetMessageInvalidEmailAddress(address);

                errorBuilder.AppendLine(message);
            }
        }

        if (errorBuilder.Length is not 0)
            throw new InvalidOperationException(errorBuilder.ToString());
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
    /// The <see cref="EmailTemplate"/> found.
    /// </returns>
    private async Task<EmailTemplate> FindEmailTemplateAsync(EmailSendRequestModel request, CancellationToken cancellationToken = default)
    {
        var templateName = request.TemplateName;
        var culture = request.Culture;

        var template = await _emailTemplatesRepository.GetByNameAndCultureAsync(templateName, culture, cancellationToken)
            .ConfigureAwait(false);

        if (template is null && request.UseNullOrInvariantCultureAsFallback)
        {
            culture = null;

            template = await _emailTemplatesRepository.GetByNameAndCultureAsync(templateName, culture, cancellationToken)
                .ConfigureAwait(false);

            culture = CultureInfo.InvariantCulture;

            template ??= await _emailTemplatesRepository.GetByNameAndCultureAsync(templateName, culture, cancellationToken)
                .ConfigureAwait(false);
        }

        if (template is null)
        {
            var message = GetMessageMessageTemplateNotFound(request.TemplateName, request.Culture);

            throw new InvalidOperationException(message);
        }

        return template;
    }

    /// <summary>
    /// Creates the <see cref="EmailModel"/> using the given
    /// <paramref name="template"/>, <paramref name="request"/> and
    /// <see cref="_userName"/>.
    /// </summary>
    /// <param name="template">
    /// The <see cref="EmailTemplate"/> to be used to
    /// format the email body.
    /// </param>
    /// <param name="request">
    /// Contains other data, necessary for proper
    /// <see cref="EmailModel"/> creation.
    /// </param>
    /// <returns>
    /// Prepared for sending <see cref="EmailModel"/> instance.
    /// </returns>
    private EmailModel CreateMessage(EmailTemplate template, EmailSendRequestModel request)
    {
        var body = template.GetFormattedText(request.BodyArgs ?? []);
        var subject = template.GetFormattedSubject(request.SubjectArgs ?? []);

        return new(subject, body, _userName, request.BasicData)
        {
            IsBodyHtml = template.IsBodyHtml,
            Sent = _timeProvider.GetUtcNow()
        };
    }
    #endregion
}