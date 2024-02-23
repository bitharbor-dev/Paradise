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

        if (SubjectPlaceholderName is null)
        {
            if (SubjectPlaceholdersNumber is not 0)
            {
                var message = ExceptionMessagesProvider.GetEmailTemplateSubjectInInvalidStateMessage();

                InvalidEntityStateException.Throw<EmailTemplate>(SubjectPlaceholdersNumber, message);
            }

            if (parameters.Count is not 0)
            {
                var message = ExceptionMessagesProvider.GetEmailTemplateSubjectInInvalidStateMessage();

                throw new ArgumentException(message);
            }
        }

        if (parameters.Count != SubjectPlaceholdersNumber)
        {
            var message = ExceptionMessagesProvider.GetInvalidParametersNumberMessage();

            throw new IndexOutOfRangeException(message);
        }

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
    /// Preforms the partial update of the current <see cref="EmailTemplate"/>
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

        if (PlaceholderName != placeholderName)
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
            if (Subject != subject)
            {
                changesExists = true;
                Subject = subject;
            }
        }

        if (SubjectPlaceholderName != subjectPlaceholderName)
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
            if (TemplateText != templateText)
            {
                TemplateText = templateText;
                changesExists = true;
            }
        }

        return changesExists;
    }

    /// <inheritdoc/>
    public override void ValidateState()
    {
        if (SubjectPlaceholderName is null)
        {
            if (SubjectPlaceholdersNumber is not 0)
            {
                var message = ExceptionMessagesProvider.GetEmailTemplateSubjectInInvalidStateMessage();

                InvalidEntityStateException.Throw<EmailTemplate>(SubjectPlaceholdersNumber, message);
            }
        }

        base.ValidateState();
    }
    #endregion
}