using Paradise.ApplicationLogic.Domain.MessageTemplates.Base;
using Paradise.Common.Extensions;
using Paradise.Domain.Base.Exceptions;
using Paradise.Localization.ExceptionsHandling;
using System.Text;

namespace Paradise.ApplicationLogic.Domain.MessageTemplates;

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
/// <param name="subject">
/// Email subject.
/// </param>
public sealed class EmailTemplate(string templateName, string templateText, string subject) : MessageTemplate(templateName, templateText)
{
    #region Properties
    /// <summary>
    /// Email subject.
    /// </summary>
    public string Subject { get; set; } = subject;

    /// <summary>
    /// Subject placeholder name to be replaced with values during a message formatting.
    /// </summary>
    /// <remarks>
    /// Each placeholder has the same name but with an index appended.
    /// <para>
    /// <c>
    /// SubjectPlaceholderName = "{parameter}";
    /// </c>
    /// <para>
    /// The template should have <c>"{parameter}0"</c>, <c>"{parameter}1"</c>, etc.
    /// </para>
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
                var message = ExceptionMessagesProvider.GetEmailTemplateSubjectInInvalidStateMessage();

                InvalidEntityStateException.Throw<EmailTemplate>(SubjectPlaceholdersNumber, message);
            }
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return TemplateName;
        yield return Culture;

        yield return TemplateText;
        yield return Subject;

        yield return PlaceholderName;
        yield return PlaceholdersNumber;

        yield return SubjectPlaceholderName;
        yield return SubjectPlaceholdersNumber;

        yield return IsBodyHtml;
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
    public string GetFormattedSubject(IList<object?> parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        ValidateSubjectBeforeFormatting(parameters);

        // If validation passed and the input parameters number is 0
        // - just return the initial subject text.
        if (parameters.Count is 0)
            return Subject;

        var builder = new StringBuilder(Subject);

        for (var index = 0; index < parameters.Count; index++)
        {
            var placeholder = string.Concat(SubjectPlaceholderName, index);

            if (!Subject.Contains(placeholder, StringComparison.OrdinalIgnoreCase))
            {
                var message = ExceptionMessagesProvider.GetPlaceholderNotExistsMessage(TemplateName, Culture, placeholder);

                throw new FormatException(message);
            }

            var value = parameters[index]?.ToString();

            builder.Replace(placeholder, value);
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
    /// Placeholder name to be replaced with values during a message formatting.
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
    /// Subject placeholder name to be replaced with values during a message formatting.
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

        if (isBodyHtml.HasValue)
        {
            if (IsBodyHtml != isBodyHtml.Value)
            {
                changesExists = true;
                IsBodyHtml = isBodyHtml.Value;
            }
        }

        if (!string.Equals(PlaceholderName, placeholderName, StringComparison.Ordinal))
        {
            changesExists = true;
            PlaceholderName = placeholderName;
        }

        if (placeholdersNumber.HasValue)
        {
            if (PlaceholdersNumber != placeholdersNumber.Value)
            {
                changesExists = true;
                PlaceholdersNumber = placeholdersNumber.Value;
            }
        }

        if (subject.IsNotNullOrWhiteSpace())
        {
            if (!string.Equals(Subject, subject, StringComparison.Ordinal))
            {
                changesExists = true;
                Subject = subject;
            }
        }

        if (!string.Equals(SubjectPlaceholderName, subjectPlaceholderName, StringComparison.Ordinal))
        {
            changesExists = true;
            SubjectPlaceholderName = subjectPlaceholderName;
        }

        if (subjectPlaceholdersNumber.HasValue)
        {
            if (SubjectPlaceholdersNumber != subjectPlaceholdersNumber.Value)
            {
                changesExists = true;
                SubjectPlaceholdersNumber = subjectPlaceholdersNumber.Value;
            }
        }

        if (templateText.IsNotNullOrWhiteSpace())
        {
            if (!string.Equals(TemplateText, templateText, StringComparison.Ordinal))
            {
                TemplateText = templateText;
                changesExists = true;
            }
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
    private void ValidateSubjectBeforeFormatting(IList<object?> parameters)
    {
        if (SubjectPlaceholderName is null)
        {
            // If placeholder name is not set, but placeholders number is not 0
            // - an exception to be thrown.
            if (SubjectPlaceholdersNumber is not 0)
            {
                var message = ExceptionMessagesProvider.GetEmailTemplateSubjectInInvalidStateMessage();

                InvalidEntityStateException.Throw<EmailTemplate>(SubjectPlaceholdersNumber, message);
            }

            // If placeholder name is not set, but input parameters number is not 0
            // - an exception to be thrown.
            if (parameters.Count is not 0)
            {
                var message = ExceptionMessagesProvider.GetEmailTemplateSubjectInInvalidStateMessage();

                throw new InvalidOperationException(message);
            }
        }

        // If the input parameters number is not equal to defined placeholders number
        // - an exception to be thrown.
        if (parameters.Count != SubjectPlaceholdersNumber)
        {
            var message = ExceptionMessagesProvider.GetInvalidParametersNumberMessage();

            throw new InvalidOperationException(message);
        }
    }
    #endregion
}