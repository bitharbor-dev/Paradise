using Paradise.Domain.Base;
using Paradise.Domain.Base.Exceptions;
using Paradise.Localization.ExceptionsHandling;
using System.Globalization;
using System.Text;

namespace Paradise.ApplicationLogic.Domain.MessageTemplates.Base;

/// <summary>
/// Base message template class.
/// </summary>
public abstract class MessageTemplate : ValueObject
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageTemplate"/> class.
    /// </summary>
    /// <param name="templateName">
    /// Template name.
    /// </param>
    /// <param name="templateText">
    /// Template text.
    /// </param>
    protected MessageTemplate(string templateName, string templateText)
    {
        TemplateName = templateName;
        TemplateText = templateText;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Template name.
    /// </summary>
    public string TemplateName { get; set; }

    /// <summary>
    /// Template culture.
    /// </summary>
    public CultureInfo? Culture { get; set; }

    /// <summary>
    /// Template text.
    /// </summary>
    public string TemplateText { get; set; }

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
        if (PlaceholderName is null)
        {
            if (PlaceholdersNumber is not 0)
            {
                InvalidEntityStateException.Throw<MessageTemplate>(PlaceholdersNumber,
                                                                   ExceptionMessages.MessageTemplateTextInInvalidState);
            }

            if (parameters.Count is not 0)
                throw new ArgumentException(ExceptionMessages.MessageTemplateTextInInvalidState);
        }

        if (parameters.Count != PlaceholdersNumber)
            throw new IndexOutOfRangeException(ExceptionMessages.InvalidParametersNumber);

        if (parameters.Count is 0)
            return TemplateText;

        var builder = new StringBuilder(TemplateText);

        for (var index = 0; index < parameters.Count; index++)
        {
            var placeholder = string.Concat(PlaceholderName, index);

            if (!TemplateText.Contains(placeholder, StringComparison.OrdinalIgnoreCase))
            {
                var message = string.Format(CultureInfo.CurrentCulture,
                                            ExceptionMessages.PlaceholderNotExists,
                                            TemplateName,
                                            Culture?.Name,
                                            placeholder);

                throw new FormatException(message);
            }

            var value = parameters[index]?.ToString();

            builder.Replace(placeholder, value);
        }

        return builder.ToString();
    }

    /// <inheritdoc/>
    public override void ValidateState()
    {
        if (PlaceholderName is null)
        {
            if (PlaceholdersNumber is not 0)
            {
                InvalidEntityStateException.Throw<MessageTemplate>(PlaceholdersNumber,
                                                                   ExceptionMessages.MessageTemplateTextInInvalidState);
            }
        }

        base.ValidateState();
    }
    #endregion
}