using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates.Base;
using Paradise.Common.Extensions;
using Paradise.Domain.Base;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using static Paradise.Localization.ExceptionHandling.ExceptionMessages;

namespace Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;

/// <summary>
/// Represents an email message template.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailTemplate"/> class.
/// </remarks>
/// <param name="templateName">
/// Template name.
/// </param>
/// <param name="templateText">
/// Template text.
/// </param>
/// <param name="culture">
/// Template culture.
/// </param>
/// <param name="subject">
/// Email subject.
/// </param>
public sealed class EmailTemplate(string templateName, CultureInfo? culture, string templateText, string subject)
    : MessageTemplate(templateName, culture, templateText)
{
    #region Properties
    /// <summary>
    /// Email subject.
    /// </summary>
    public string Subject { get; set; } = subject;

    /// <summary>
    /// Subject placeholder name to be replaced with values during the message formatting.
    /// </summary>
    /// <remarks>
    /// Each placeholder has the same name but with an index appended.
    /// <para>
    /// <c>
    /// SubjectPlaceholderName = "{parameter}";
    /// </c>
    /// </para>
    /// <para>
    /// The template should have <c>"{parameter}0"</c>, <c>"{parameter}1"</c>, etc.
    /// </para>
    /// </remarks>
    public string? SubjectPlaceholderName { get; set; }

    /// <summary>
    /// The number of subject placeholders to be replaced with values during the message formatting.
    /// </summary>
    public ushort SubjectPlaceholdersNumber { get; set; }

    /// <summary>
    /// Indicates whether the email body is an HTML document.
    /// </summary>
    public bool IsBodyHtml { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override void ValidateState()
    {
        base.ValidateState();

        if (SubjectPlaceholderName is null)
        {
            // If placeholder name is not set, but placeholders number is not 0
            // - an exception to be thrown.
            if (SubjectPlaceholdersNumber is not 0)
            {
                var additionalInformation = GetMessageMessageTemplateFormattableTextInInvalidState();

                var message = new DomainStateError<EmailTemplate>(SubjectPlaceholdersNumber, additionalInformation);

                throw new InvalidOperationException(message);
            }
        }
        else
        {
            var placeholderOccurrences = Regex.Count(Subject, Regex.Escape(SubjectPlaceholderName));

            if (SubjectPlaceholdersNumber != placeholderOccurrences)
            {
                var additionalInformation = GetMessageMessageTemplateInvalidPlaceholdersNumber(
                    SubjectPlaceholdersNumber, (ushort)placeholderOccurrences);

                var message = new DomainStateError<EmailTemplate>(Subject, additionalInformation);

                throw new InvalidOperationException(message);
            }
        }
    }

    /// <summary>
    /// Gets the formatted template subject with the given <paramref name="parameters"/>.
    /// </summary>
    /// <param name="parameters">
    /// <see cref="Subject"/> formatting values.
    /// </param>
    /// <returns>
    /// Formatted <see cref="Subject"/>.
    /// </returns>
    /// <exception cref="FormatException">
    /// <paramref name="parameters"/> number is not equal
    /// to the <see cref="SubjectPlaceholdersNumber"/>
    /// or
    /// <see cref="Subject"/> does not contain the combined
    /// <see cref="SubjectPlaceholderName"/> with the corresponding index.
    /// </exception>
    public string GetFormattedSubject(IEnumerable<object?> parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        ValidateSubjectBeforeFormatting(parameters, out var parametersCount);

        // If validation passed and the input parameters number is 0
        // - just return the initial subject text.
        if (parametersCount is 0)
            return Subject;

        var builder = new StringBuilder(Subject);

        foreach (var (index, parameter) in parameters.Index())
        {
            var placeholder = string.Concat(SubjectPlaceholderName, index);

            if (!Subject.Contains(placeholder, StringComparison.OrdinalIgnoreCase))
            {
                var message = GetMessageMessageTemplatePlaceholderNotExists(TemplateName, Culture, placeholder);

                throw new FormatException(message);
            }

            builder.Replace(placeholder, parameter?.ToString());

        }

        return builder.ToString();
    }

    /// <summary>
    /// Preforms the partial update of the current <see cref="EmailTemplate"/> instance
    /// using the 'not-null' data from the input parameters.
    /// </summary>
    /// <param name="isBodyHtml">
    /// Indicates whether the email body is an HTML document.
    /// <para>
    /// Pass <see langword="null"/> to leave the old value.
    /// </para>
    /// </param>
    /// <param name="placeholderName">
    /// Placeholder name to be replaced with values during the message formatting.
    /// <para>
    /// Pass the current <see cref="MessageTemplate.PlaceholderName"/> to leave the old value.
    /// </para>
    /// </param>
    /// <param name="placeholdersNumber">
    /// The number of placeholders to be replaced with values during the message formatting.
    /// <para>
    /// Pass <see langword="null"/> to leave the old value.
    /// </para>
    /// </param>
    /// <param name="subject">
    /// Email subject.
    /// <para>
    /// Pass <see langword="null"/> to leave the old value.
    /// </para>
    /// </param>
    /// <param name="subjectPlaceholderName">
    /// Subject placeholder name to be replaced with values during the message formatting.
    /// <para>
    /// Pass the current <see cref="SubjectPlaceholderName"/> to leave the old value.
    /// </para>
    /// </param>
    /// <param name="subjectPlaceholdersNumber">
    /// The number of subject placeholders to be replaced with values during the message formatting.
    /// <para>
    /// Pass <see langword="null"/> to leave the old value.
    /// </para>
    /// </param>
    /// <param name="templateText">
    /// Template text.
    /// <para>
    /// Pass <see langword="null"/> to leave the old value.
    /// </para>
    /// </param>
    /// <returns>
    /// <see langword="true"/> if any changes were applied to the current <see cref="EmailTemplate"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public bool Update(bool? isBodyHtml, string? placeholderName, ushort? placeholdersNumber,
                       string? subject, string? subjectPlaceholderName, ushort? subjectPlaceholdersNumber,
                       string? templateText)
    {
        var changesExists = false;

        if (isBodyHtml.HasValue && IsBodyHtml != isBodyHtml.Value)
        {
            changesExists = true;
            IsBodyHtml = isBodyHtml.Value;
        }

        if (!string.Equals(PlaceholderName, placeholderName, StringComparison.Ordinal))
        {
            changesExists = true;
            PlaceholderName = placeholderName;
        }

        if (placeholdersNumber.HasValue && PlaceholdersNumber != placeholdersNumber.Value)
        {
            changesExists = true;
            PlaceholdersNumber = placeholdersNumber.Value;
        }

        if (subject.IsNotNullOrWhiteSpace() && !string.Equals(Subject, subject, StringComparison.Ordinal))
        {
            changesExists = true;
            Subject = subject;
        }

        if (!string.Equals(SubjectPlaceholderName, subjectPlaceholderName, StringComparison.Ordinal))
        {
            changesExists = true;
            SubjectPlaceholderName = subjectPlaceholderName;
        }

        if (subjectPlaceholdersNumber.HasValue && SubjectPlaceholdersNumber != subjectPlaceholdersNumber.Value)
        {
            changesExists = true;
            SubjectPlaceholdersNumber = subjectPlaceholdersNumber.Value;
        }

        if (templateText.IsNotNullOrWhiteSpace() && !string.Equals(TemplateText, templateText, StringComparison.Ordinal))
        {
            TemplateText = templateText;
            changesExists = true;
        }

        return changesExists;
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Validates the subject placeholders number and
    /// <paramref name="parameters"/> number before
    /// performing the subject formatting.
    /// </summary>
    /// <param name="parameters">
    /// <see cref="Subject"/> formatting values.
    /// </param>
    /// <param name="parametersCount">
    /// <paramref name="parameters"/> count.
    /// </param>
    private void ValidateSubjectBeforeFormatting(IEnumerable<object?> parameters, out int parametersCount)
    {
        parametersCount = parameters.Count();

        if (SubjectPlaceholderName is null)
        {
            // If placeholder name is not set, but placeholders number is not 0
            // - an exception to be thrown.
            if (SubjectPlaceholdersNumber is not 0)
            {
                var additionalInformation = GetMessageMessageTemplateFormattableTextInInvalidState();

                var message = new DomainStateError<EmailTemplate>(SubjectPlaceholdersNumber, additionalInformation);

                throw new InvalidOperationException(message);
            }

            // If placeholder name is not set, but input parameters number is not 0
            // - an exception to be thrown.
            if (parametersCount is not 0)
            {
                var message = GetMessageMessageTemplateFormattableTextInInvalidState();

                throw new InvalidOperationException(message);
            }
        }

        // If the input parameters number is not equal to defined placeholders number
        // - an exception to be thrown.
        if (parametersCount != SubjectPlaceholdersNumber)
        {
            var message = GetMessageMessageTemplateInvalidParametersNumber();

            throw new InvalidOperationException(message);
        }
    }
    #endregion
}