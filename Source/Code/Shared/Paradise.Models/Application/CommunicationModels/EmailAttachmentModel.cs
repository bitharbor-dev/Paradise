using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Paradise.Models.Application.CommunicationModels;

/// <summary>
/// Represents an email attachment.
/// </summary>
public sealed class EmailAttachmentModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailAttachmentModel"/> class.
    /// </summary>
    /// <param name="data">
    /// Attachment content.
    /// </param>
    /// <param name="fileName">
    /// Attachment file name.
    /// </param>
    /// <param name="mimeType">
    /// Attachment MIME type.
    /// </param>
    [JsonConstructor]
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructors not working with constructor attributes.")]
    public EmailAttachmentModel(byte[] data, string fileName, string mimeType)
    {
        Data = data;
        FileName = fileName;
        MimeType = mimeType;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Attachment content.
    /// </summary>
    public byte[] Data { get; set; }

    /// <summary>
    /// Attachment file name.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Attachment MIME type.
    /// </summary>
    public string MimeType { get; set; }
    #endregion
}