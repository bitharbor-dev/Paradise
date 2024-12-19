using Paradise.Common.Extensions;
using Paradise.Localization.ExceptionsHandling;
using System.Security;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Paradise.DataAccess.Seed.Models.Application.MessageTemplates;

/// <summary>
/// Email template seed data schema.
/// </summary>
public sealed partial class SeedEmailTemplateModel
{
    #region Fields
    private static readonly Regex RegexBetweenTags = GetRegexBetweenTags();
    private static readonly Regex RegexLineBreaks = GetRegexLineBreaks();
    private static readonly Regex RegexCaretBreaks = GetRegexCaretBreaks();
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="SeedEmailTemplateModel"/> class.
    /// </summary>
    /// <param name="templateName">
    /// Template name.
    /// </param>
    /// <param name="subject">
    /// Email subject.
    /// </param>
    /// <param name="templateText">
    /// Template text.
    /// </param>
    /// <param name="templateTextSourcePath">
    /// Template text source path.
    /// </param>
    /// <remarks>
    /// <para>
    /// If <paramref name="templateText"/> is not <see langword="null"/> -
    /// it is assigned directly to <see cref="TemplateText"/> property.
    /// </para>
    /// <para>
    /// If <paramref name="templateTextSourcePath"/> is not <see langword="null"/> -
    /// it is being used to read value for <see cref="TemplateText"/> from the file
    /// located by the <paramref name="templateTextSourcePath"/>.
    /// </para>
    /// <para>
    /// If both <paramref name="templateText"/> and <paramref name="templateTextSourcePath"/>
    /// are not <see langword="null"/> - <paramref name="templateText"/>
    /// is assigned directly to <see cref="TemplateText"/> property and
    /// <paramref name="templateTextSourcePath"/> is ignored.
    /// </para>
    /// <para>
    /// If both <paramref name="templateText"/> and <paramref name="templateTextSourcePath"/>
    /// are <see langword="null"/> - <see cref="ArgumentException"/> is thrown.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown if both <paramref name="templateText"/>
    /// and <paramref name="templateTextSourcePath"/>
    /// are <see langword="null"/>.
    /// </exception>
    [JsonConstructor]
    public SeedEmailTemplateModel(string templateName, string subject, string? templateText = null, string? templateTextSourcePath = null)
    {
        TemplateName = templateName;
        Subject = subject;

        if (templateText is not null)
        {
            TemplateText = templateText;
        }
        else if (templateTextSourcePath is not null)
        {
            TemplateText = GetMinifiedHtml(templateTextSourcePath);
            TemplateTextSourcePath = templateTextSourcePath;
        }
        else
        {
            var message = ExceptionMessagesProvider.GetTemplateTextOrSourcePathIsRequiredMessage();

            throw new ArgumentException(message);
        }
    }
    #endregion

    #region Properties
    /// <summary>
    /// Template name.
    /// </summary>
    public string TemplateName { get; set; }

    /// <summary>
    /// Template culture LCID.
    /// </summary>
    public int? CultureId { get; set; }

    /// <summary>
    /// Email subject.
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// Template text.
    /// </summary>
    public string TemplateText { get; set; }

    /// <summary>
    /// Template text source path.
    /// </summary>
    public string? TemplateTextSourcePath { get; set; }

    /// <summary>
    /// Indicates whether the email body is an HTML document.
    /// </summary>
    public bool IsBodyHtml { get; set; }

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
    #endregion

    #region Private methods
    /// <summary>
    /// Reads the HTML file by it's <paramref name="relativeHtmlPath"/>
    /// and returns it's minified content.
    /// </summary>
    /// <param name="relativeHtmlPath">
    /// Relative HTML file path.
    /// Would be perpended by <see cref="AppContext.BaseDirectory"/>.
    /// </param>
    /// <returns>
    /// Minified HTML string.
    /// </returns>
    /// <exception cref="PathTooLongException">
    /// The specified path, file name, or both exceed the system-defined maximum length.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// The specified path is invalid (for example, it is on an unmapped drive).
    /// </exception>
    /// <exception cref="IOException">
    /// An I/O error occurred while opening the file.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// <paramref name="relativeHtmlPath"/> specified a file that is read-only
    /// or
    /// this operation is not supported on the current platform.
    /// or
    /// <paramref name="relativeHtmlPath"/> specified a directory
    /// or
    /// the caller does not have the required permission.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// The file specified in path was not found.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <paramref name="relativeHtmlPath"/> is in an invalid format.
    /// </exception>
    /// <exception cref="SecurityException">
    /// The caller does not have the required permission.
    /// </exception>
    private static string GetMinifiedHtml(string relativeHtmlPath)
    {
        var fullPath = Path.Combine(AppContext.BaseDirectory, relativeHtmlPath.SanitizePathSeparators());
        var html = File.ReadAllText(fullPath);

        html = RegexBetweenTags.Replace(html, string.Empty);
        html = RegexLineBreaks.Replace(html, string.Empty);
        html = RegexCaretBreaks.Replace(html, string.Empty);

        return html;
    }

    [GeneratedRegex("(?<=\\s)\\s+(?![^<>]*</pre>)", RegexOptions.Compiled)]
    private static partial Regex GetRegexBetweenTags();

    [GeneratedRegex("\n(?![^<]*</pre>)", RegexOptions.Compiled)]
    private static partial Regex GetRegexLineBreaks();

    [GeneratedRegex("\r(?![^<]*</pre>)", RegexOptions.Compiled)]
    private static partial Regex GetRegexCaretBreaks();
    #endregion
}