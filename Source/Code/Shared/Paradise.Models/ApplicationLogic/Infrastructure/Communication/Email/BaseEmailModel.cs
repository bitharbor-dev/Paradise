using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;

/// <summary>
/// Contains basic email message information.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BaseEmailModel"/> class.
/// </remarks>
/// <param name="to">
/// Recipients' email addresses.
/// </param>
[method: JsonConstructor]
public class BaseEmailModel(IEnumerable<string> to)
{
    #region Properties
    /// <summary>
    /// Recipients' email addresses.
    /// </summary>
    [Required, MinLength(1)]
    public IEnumerable<string> To { get; } = to;

    /// <summary>
    /// Carbon copy recipients' email addresses.
    /// </summary>
    public IEnumerable<string>? CarbonCopy { get; set; }

    /// <summary>
    /// Blind carbon copy recipients' email addresses.
    /// </summary>
    public IEnumerable<string>? BlindCarbonCopy { get; set; }

    /// <summary>
    /// Attachments.
    /// </summary>
    public IEnumerable<EmailAttachmentModel>? Attachments { get; set; }
    #endregion
}