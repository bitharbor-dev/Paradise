using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Paradise.Models.Application.EmailTemplateModels;

/// <summary>
/// Email template creation schema.
/// </summary>
public sealed class EmailTemplateCreationModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailTemplateCreationModel"/> class.
    /// </summary>
    /// <param name="templateName">
    /// Template name.
    /// </param>
    /// <param name="templateText">
    /// Template text.
    /// </param>
    /// <param name="subject">
    /// Email subject.
    /// </param>
    [JsonConstructor]
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructors not working with constructor attributes.")]
    public EmailTemplateCreationModel(string templateName, string subject, string templateText)
    {
        TemplateName = templateName;
        Subject = subject;
        TemplateText = templateText;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Template name.
    /// </summary>
    [Required, DataType(DataType.Text)]
    public string TemplateName { get; set; }

    /// <summary>
    /// Template culture LCID.
    /// </summary>
    public int? CultureId { get; set; }

    /// <summary>
    /// Template text.
    /// </summary>
    [Required, DataType(DataType.Html)]
    public string TemplateText { get; set; }

    /// <summary>
    /// Email subject.
    /// </summary>
    [Required, DataType(DataType.Text)]
    public string Subject { get; set; }

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
    [DataType(DataType.Text)]
    public string? PlaceholderName { get; set; }

    /// <summary>
    /// The number of placeholders to be replaced with values during the message formatting.
    /// </summary>
    public ushort PlaceholdersNumber { get; set; }

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
    [DataType(DataType.Text)]
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
}