using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;

/// <summary>
/// Represents an email template.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailTemplateModel"/> class.
/// </remarks>
/// <param name="id">
/// Email template Id.
/// </param>
/// <param name="created">
/// Creation date.
/// </param>
/// <param name="modified">
/// Last changed date.
/// </param>
/// <param name="templateName">
/// Template name.
/// </param>
/// <param name="cultureId">
/// Template culture LCID.
/// </param>
/// <param name="templateText">
/// Template text.
/// </param>
/// <param name="subject">
/// Email subject.
/// </param>
/// <param name="placeholderName">
/// Placeholder name to be replaced with values during the message formatting.
/// </param>
/// <param name="placeholdersNumber">
/// The number of placeholders to be replaced with values during the message formatting.
/// </param>
/// <param name="subjectPlaceholderName">
/// Subject placeholder name to be replaced with values during the message formatting.
/// </param>
/// <param name="subjectPlaceholdersNumber">
/// The number of subject placeholders to be replaced with values during the message formatting.
/// </param>
/// <param name="isBodyHtml">
/// Indicates whether the email body is an HTML document.
/// </param>
[method: JsonConstructor]
public sealed class EmailTemplateModel(Guid id, DateTimeOffset created, DateTimeOffset modified,
                                       string templateName, int? cultureId, string templateText, string subject,
                                       string? placeholderName, ushort placeholdersNumber,
                                       string? subjectPlaceholderName, ushort subjectPlaceholdersNumber, bool isBodyHtml)
{
    #region Properties
    /// <summary>
    /// Email template Id.
    /// </summary>
    public Guid Id { get; } = id;

    /// <summary>
    /// Creation date.
    /// </summary>
    [DataType(DataType.DateTime)]
    public DateTimeOffset Created { get; } = created;

    /// <summary>
    /// Last changed date.
    /// </summary>
    [DataType(DataType.DateTime)]
    public DateTimeOffset Modified { get; } = modified;

    /// <summary>
    /// Template name.
    /// </summary>
    [DataType(DataType.Text)]
    public string TemplateName { get; } = templateName;

    /// <summary>
    /// Template culture LCID.
    /// </summary>
    public int? CultureId { get; } = cultureId;

    /// <summary>
    /// Template text.
    /// </summary>
    [DataType(DataType.Html)]
    public string TemplateText { get; } = templateText;

    /// <summary>
    /// Email subject.
    /// </summary>
    [DataType(DataType.Text)]
    public string Subject { get; } = subject;

    /// <summary>
    /// Placeholder name to be replaced with values during the message formatting.
    /// </summary>
    /// <remarks>
    /// Each placeholder has the same name but with an index appended.
    /// <para>
    /// <c>
    /// PlaceholderName = "{parameter}";
    /// </c>
    /// </para>
    /// <para>
    /// The template should have <c>"{parameter}0"</c>, <c>"{parameter}1"</c>, etc.
    /// </para>
    /// </remarks>
    [DataType(DataType.Text)]
    public string? PlaceholderName { get; } = placeholderName;

    /// <summary>
    /// The number of placeholders to be replaced with values during the message formatting.
    /// </summary>
    public ushort PlaceholdersNumber { get; } = placeholdersNumber;

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
    [DataType(DataType.Text)]
    public string? SubjectPlaceholderName { get; } = subjectPlaceholderName;

    /// <summary>
    /// The number of subject placeholders to be replaced with values during the message formatting.
    /// </summary>
    public ushort SubjectPlaceholdersNumber { get; } = subjectPlaceholdersNumber;

    /// <summary>
    /// Indicates whether the email body is an HTML document.
    /// </summary>
    public bool IsBodyHtml { get; } = isBodyHtml;
    #endregion
}