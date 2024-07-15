using Paradise.Domain.Base;
using Paradise.Domain.Base.Exceptions;
using Paradise.Localization.ExceptionsHandling;
using System.Globalization;
using System.Text;

namespace Paradise.ApplicationLogic.Domain.MessageTemplates.Base;

/// <summary>
/// Base message template class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MessageTemplate"/> class.
/// </remarks>
/// <param name="templateName">
/// Template name.
/// </param>
/// <param name="templateText">
/// Template text.
/// </param>
public abstract class MessageTemplate(string templateName, string templateText) : ValueObject
{
    #region Properties
    /// <summary>
    /// Template name.
    /// </summary>
    public string TemplateName { get; set; } = templateName;

    /// <summary>
    /// Template culture.
    /// </summary>
    public CultureInfo? Culture { get; set; }

    /// <summary>
    /// Template text.
    /// </summary>
    public string TemplateText { get; set; } = templateText;

    /// <summary>
    /// Placeholder name to be replaced with values during a message formatting.
    /// </summary>
    /// <remarks>
    /// Each placeholder has the same name but with an index appended.
    /// <para>
    /// <c>
    /// PlaceholderName = "{parameter}";
    /// </c>
    /// <para>
    /// The template should have <c>"{parameter}0"</c>, <c>"{parameter}1"</c>, etc.
    /// </para>
    /// </para>
    /// </remarks>
    public string? PlaceholderName { get; set; }

    /// <summary>
    /// The number of placeholders to be replaced with values during the message formatting.
    /// </summary>
    public ushort PlaceholdersNumber { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override void ValidateState()
    {
        base.ValidateState();

        if (PlaceholderName is null)
        {
            // If placeholder name is not set, but placeholders number is not 0
            // - an exception to be thrown.
            if (PlaceholdersNumber is not 0)
            {
                var message = ExceptionMessagesProvider.GetMessageTemplateTextInInvalidStateMessage();

                InvalidEntityStateException.Throw<MessageTemplate>(PlaceholdersNumber, message);
            }
        }
    }

    /// <summary>
    /// Gets the formatted template text with the given <paramref name="parameters"/>.
    /// </summary>
    /// <param name="parameters">
    /// <see cref="TemplateText"/> formatting values.
    /// </param>
    /// <returns>
    /// Formatted <see cref="TemplateText"/>.
    /// </returns>
    /// <exception cref="FormatException">
    /// <paramref name="parameters"/> number is not equal
    /// to the <see cref="PlaceholdersNumber"/>
    /// or
    /// <see cref="TemplateText"/> is <see langword="null"/> or empty
    /// or
    /// <see cref="TemplateText"/> does not contain the combined
    /// <see cref="PlaceholderName"/> with the corresponding index.
    /// </exception>
    public string GetFormattedText(IList<object?> parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        ValidateTextBeforeFormatting(parameters);

        // If all previous validations passed and the input parameters number is 0
        // - just return the initial template text.
        if (parameters.Count is 0)
            return TemplateText;

        var builder = new StringBuilder(TemplateText);

        for (var index = 0; index < parameters.Count; index++)
        {
            var placeholder = string.Concat(PlaceholderName, index);

            if (!TemplateText.Contains(placeholder, StringComparison.OrdinalIgnoreCase))
            {
                var message = ExceptionMessagesProvider.GetPlaceholderNotExistsMessage(TemplateName, Culture, placeholder);

                throw new FormatException(message);
            }

            var value = parameters[index]?.ToString();

            builder.Replace(placeholder, value);
        }

        return builder.ToString();
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Validates the placeholders number and
    /// <paramref name="parameters"/> number before
    /// performing the subject formatting.
    /// </summary>
    /// <param name="parameters">
    /// <see cref="TemplateText"/> formatting values.
    /// </param>
    private void ValidateTextBeforeFormatting(IList<object?> parameters)
    {
        if (PlaceholderName is null)
        {
            // If placeholder name is not set, but placeholders number is not 0
            // - an exception to be thrown.
            if (PlaceholdersNumber is not 0)
            {
                var message = ExceptionMessagesProvider.GetMessageTemplateTextInInvalidStateMessage();

                InvalidEntityStateException.Throw<MessageTemplate>(PlaceholdersNumber, message);
            }

            // If placeholder name is not set, but input parameters number is not 0
            // - an exception to be thrown.
            if (parameters.Count is not 0)
            {
                var message = ExceptionMessagesProvider.GetMessageTemplateTextInInvalidStateMessage();

                throw new InvalidOperationException(message);
            }
        }

        // If the input parameters number is not equal to defined placeholders number
        // - an exception to be thrown.
        if (parameters.Count != PlaceholdersNumber)
        {
            var message = ExceptionMessagesProvider.GetInvalidParametersNumberMessage();

            throw new InvalidOperationException(message);
        }
    }
    #endregion
}