using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;

/// <summary>
/// Represents an email attachment.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailAttachmentModel"/> class.
/// </remarks>
/// <param name="data">
/// Attachment content.
/// </param>
/// <param name="fileName">
/// Attachment file name.
/// </param>
/// <param name="mimeType">
/// Attachment MIME type.
/// </param>
[method: JsonConstructor]
public sealed class EmailAttachmentModel(Stream data, string fileName, string mimeType)
{
    #region Properties
    /// <summary>
    /// Attachment content.
    /// </summary>
    [DataType(DataType.Upload)]
    public Stream Data { get; } = data;

    /// <summary>
    /// Attachment file name.
    /// </summary>
    public string FileName { get; } = fileName;

    /// <summary>
    /// Attachment MIME type.
    /// </summary>
    public string MimeType { get; } = mimeType;
    #endregion
}