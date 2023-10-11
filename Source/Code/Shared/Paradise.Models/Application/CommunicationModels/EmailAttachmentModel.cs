using System.Text.Json.Serialization;

namespace Paradise.Models.Application.CommunicationModels;

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
public sealed class EmailAttachmentModel(byte[] data, string fileName, string mimeType)
{
    #region Properties
    /// <summary>
    /// Attachment content.
    /// </summary>
    public byte[] Data { get; set; } = data;

    /// <summary>
    /// Attachment file name.
    /// </summary>
    public string FileName { get; set; } = fileName;

    /// <summary>
    /// Attachment MIME type.
    /// </summary>
    public string MimeType { get; set; } = mimeType;
    #endregion
}