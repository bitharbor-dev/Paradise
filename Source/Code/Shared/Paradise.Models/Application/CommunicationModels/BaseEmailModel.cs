using Paradise.Common;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Paradise.Models.Application.CommunicationModels;

/// <summary>
/// Contains basic email message information.
/// </summary>
public class BaseEmailModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEmailModel"/> class.
    /// </summary>
    /// <param name="to">
    /// Recipients' email addresses.
    /// </param>
    [JsonConstructor]
    [SuppressMessage(SuppressionOfIDE0290.Category, SuppressionOfIDE0290.CheckId, Justification = SuppressionOfIDE0290.Justification)]
    public BaseEmailModel(IEnumerable<string> to)
        => To = to;
    #endregion

    #region Properties
    /// <summary>
    /// Recipients' email addresses.
    /// </summary>
    [Required, MinLength(1)]
    public IEnumerable<string> To { get; set; }

    /// <summary>
    /// Copy recipients' email addresses.
    /// </summary>
    public IEnumerable<string>? Cc { get; set; }

    /// <summary>
    /// Blind copy recipients' email addresses.
    /// </summary>
    public IEnumerable<string>? Bcc { get; set; }

    /// <summary>
    /// Attachments.
    /// </summary>
    public IEnumerable<EmailAttachmentModel>? Attachmetns { get; set; }
    #endregion
}