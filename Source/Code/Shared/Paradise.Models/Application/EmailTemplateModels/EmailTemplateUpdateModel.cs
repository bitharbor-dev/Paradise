using System.ComponentModel.DataAnnotations;

namespace Paradise.Models.Application.EmailTemplateModels;

/// <summary>
/// Email template update schema.
/// </summary>
public sealed class EmailTemplateUpdateModel
{
    #region Properties
    /// <summary>
    /// Template text.
    /// </summary>
    [DataType(DataType.Html)]
    public string? TemplateText { get; set; }

    /// <summary>
    /// Email subject.
    /// </summary>
    [DataType(DataType.Text)]
    public string? Subject { get; set; }

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
    public ushort? PlaceholdersNumber { get; set; }

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
    public ushort? SubjectPlaceholdersNumber { get; set; }

    /// <summary>
    /// Indicates whether the email body is an HTML document.
    /// </summary>
    public bool? IsBodyHtml { get; set; }
    #endregion
}