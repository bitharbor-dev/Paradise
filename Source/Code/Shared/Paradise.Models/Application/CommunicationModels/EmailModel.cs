using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.Application.CommunicationModels;

/// <summary>
/// Represents an email message.
/// </summary>
public sealed class EmailModel : BaseEmailModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailModel"/> class.
    /// </summary>
    /// <param name="subject">
    /// Email subject.
    /// </param>
    /// <param name="body">
    /// Email body.
    /// </param>
    /// <param name="from">
    /// Sender email address.
    /// </param>
    /// <param name="to">
    /// Recipients' email addresses.
    /// </param>
    [JsonConstructor]
    public EmailModel(string subject, string body, string from, IEnumerable<string> to) : base(to)
    {
        Subject = subject;
        Body = body;
        From = from;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailModel"/> class.
    /// </summary>
    /// <param name="subject">
    /// Email subject.
    /// </param>
    /// <param name="body">
    /// Email body.
    /// </param>
    /// <param name="from">
    /// Sender email address.
    /// </param>
    /// <param name="baseModel">
    /// Basic email information.
    /// </param>
    public EmailModel(string subject, string body, string from, BaseEmailModel baseModel)
        : base(baseModel?.To ?? throw new ArgumentNullException(nameof(baseModel)))
    {
        Subject = subject;
        Body = body;
        From = from;

        Cc = baseModel.Cc;
        Bcc = baseModel.Bcc;
        Attachmetns = baseModel.Attachmetns;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Email subject.
    /// </summary>
    [Required, DataType(DataType.Text)]
    public string Subject { get; set; }

    /// <summary>
    /// Email body.
    /// </summary>
    [Required, DataType(DataType.Html)]
    public string Body { get; set; }

    /// <summary>
    /// Sender email address.
    /// </summary>
    [Required, EmailAddress, DataType(DataType.EmailAddress)]
    public string From { get; set; }

    /// <summary>
    /// Indicates whether the email body is an HTML document.
    /// </summary>
    public bool IsBodyHtml { get; set; }

    /// <summary>
    /// Sent date.
    /// </summary>
    [DataType(DataType.DateTime)]
    public DateTime Sent { get; set; }
    #endregion
}