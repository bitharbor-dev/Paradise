using Paradise.Common;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Paradise.Models.Application.EmailTemplateModels;

/// <summary>
/// Represents an email template.
/// </summary>
public sealed class EmailTemplateModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailTemplateModel"/> class.
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
    [SuppressMessage(SuppressionOfIDE0290.Category, SuppressionOfIDE0290.CheckId, Justification = SuppressionOfIDE0290.Justification)]
    public EmailTemplateModel(string templateName, string templateText, string subject)
    {
        TemplateName = templateName;
        TemplateText = templateText;
        Subject = subject;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Email template Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Creation date.
    /// </summary>
    [DataType(DataType.DateTime)]
    public DateTime Created { get; set; }

    /// <summary>
    /// Last changed date.
    /// </summary>
    [DataType(DataType.DateTime)]
    public DateTime Modified { get; set; }

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
    [Required]
    public bool IsBodyHtml { get; set; }
    #endregion
}