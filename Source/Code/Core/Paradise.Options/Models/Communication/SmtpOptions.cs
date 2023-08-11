using Paradise.Options.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mail;

namespace Paradise.Options.Models.Communication;

/// <summary>
/// SMTP client options.
/// </summary>
public sealed class SmtpOptions
{
    #region Constants
    /// <summary>
    /// Directory name for the email messages to be stored in.
    /// </summary>
    public const string EmailStorageDirectoryName = "EmailMessages";
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the credentials used to authenticate the sender.
    /// </summary>
    public NetworkCredential? Credentials { get; set; }

    /// <summary>
    /// Specify whether the <see cref="SmtpClient"/>
    /// uses Secure Sockets Layer (SSL)
    /// to encrypt the connection.
    /// </summary>
    public bool EnableSsl { get; set; }

    /// <summary>
    /// Gets or sets the name or IP address of the host used for SMTP transactions.
    /// </summary>
    [Required, NotNull]
    public string? Host { get; set; }

    /// <summary>
    /// Gets or sets the port used for SMTP transactions.
    /// </summary>
    [AllowedValues<int>(25, 465, 587, 2525)]
    public int Port { get; set; }

    /// <summary>
    /// Indicates whether the emails being send
    /// should be stored locally instead of sending.
    /// </summary>
    /// <remarks>
    /// This setting is very handy when you need to
    /// debug the application without Internet connection
    /// or if you don't have SMTP credentials to use.
    /// </remarks>
    public bool StoreEmailsInsteadOfSending { get; set; }

    /// <summary>
    /// Gets or sets a value that specifies the amount of time after which a synchronous
    /// Overload:System.Net.Mail.SmtpClient.Send call times out.
    /// </summary>
    public int Timeout { get; set; }
    #endregion
}