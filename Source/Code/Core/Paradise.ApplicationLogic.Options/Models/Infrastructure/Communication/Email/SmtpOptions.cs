using Paradise.Common.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mail;

namespace Paradise.ApplicationLogic.Options.Models.Infrastructure.Communication.Email;

/// <summary>
/// SMTP client options.
/// </summary>
public sealed class SmtpOptions
{
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
    public bool EnableSecureSocketsLayer { get; set; }

    /// <summary>
    /// Gets or sets the name or IP address of the host used for SMTP transactions.
    /// </summary>
    [Required, NotNull]
    public string? Host { get; set; }

    /// <summary>
    /// Gets or sets the port used for SMTP transactions.
    /// </summary>
    [AllowedValues(25, 465, 587, 2525)]
    public int Port { get; set; }

    /// <summary>
    /// If set, indicates that the emails should be stored locally
    /// by the specified path, instead of sending.
    /// </summary>
    public string? LocalEmailStorage { get; set; }
    #endregion

    #region Public methods
    /// <summary>
    /// Creates a directory specified by the <see cref="LocalEmailStorage"/>.
    /// </summary>
    /// <returns>
    /// <see langword="null"/> if the <see cref="LocalEmailStorage"/>
    /// is equal to <see langword="null"/>, <see cref="string.Empty"/> or whitespace-only,
    /// otherwise - resolves the path, creates the directory if it does not exist,
    /// and returns the fully qualified directory path.
    /// </returns>
    public string? InitializeLocalEmailStorage()
    {
        const string ApplicationRootPlaceholder = "{ApplicationRoot}";

        if (LocalEmailStorage.IsNullOrWhiteSpace())
            return null;

        string? path;

        if (LocalEmailStorage.StartsWith(ApplicationRootPlaceholder, StringComparison.Ordinal))
        {
            var remainder = LocalEmailStorage[ApplicationRootPlaceholder.Length..]
                .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            path = Path.Combine(AppContext.BaseDirectory, remainder);
        }
        else
        {
            path = LocalEmailStorage;
        }

        return Directory.CreateDirectory(path).FullName;
    }
    #endregion
}