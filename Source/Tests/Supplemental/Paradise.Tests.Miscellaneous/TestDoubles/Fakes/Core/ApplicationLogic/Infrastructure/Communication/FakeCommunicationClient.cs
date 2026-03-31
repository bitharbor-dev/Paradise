using Paradise.ApplicationLogic.Infrastructure.Communication;
using Paradise.ApplicationLogic.Infrastructure.Communication.Email;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;
using System.Globalization;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.ApplicationLogic.Infrastructure.Communication;

/// <summary>
/// Fake <see cref="ICommunicationClient"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeCommunicationClient"/> class.
/// </remarks>
/// <param name="timeProvider">
/// Time provider.
/// </param>
/// <param name="emailSender">
/// Email sender.
/// </param>
/// <param name="emailTemplatesRepository">
/// Email templates repository.
/// </param>
internal sealed class FakeCommunicationClient(TimeProvider timeProvider,
                                              IEmailSender emailSender,
                                              IEmailTemplatesRepository emailTemplatesRepository) : ICommunicationClient
{
    #region Properties
    /// <summary>
    /// Default sender email address.
    /// </summary>
    public string From { get; set; } = "test@email.com";
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async Task<EmailModel> SendEmailAsync(EmailSendRequestModel request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var templateName = request.TemplateName;
        var culture = request.Culture;

        var template = await emailTemplatesRepository.GetByNameAndCultureAsync(templateName, culture, cancellationToken)
            .ConfigureAwait(false);

        if (template is null && request.UseNullOrInvariantCultureAsFallback)
        {
            culture = null;

            template = await emailTemplatesRepository.GetByNameAndCultureAsync(templateName, culture, cancellationToken)
                .ConfigureAwait(false);

            culture = CultureInfo.InvariantCulture;

            template ??= await emailTemplatesRepository.GetByNameAndCultureAsync(templateName, culture, cancellationToken)
                .ConfigureAwait(false);
        }

        if (template is null)
            throw new InvalidOperationException();

        var body = template.GetFormattedText(request.BodyArgs ?? []);
        var subject = template.GetFormattedSubject(request.SubjectArgs ?? []);

        var emailMessage = new EmailModel(subject, body, From, request.BasicData)
        {
            IsBodyHtml = template.IsBodyHtml,
            Sent = timeProvider.GetUtcNow()
        };

        await emailSender.SendAsync(emailMessage, cancellationToken)
            .ConfigureAwait(false);

        return emailMessage;
    }
    #endregion
}