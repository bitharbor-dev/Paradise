using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Options.Models;

/// <summary>
/// Application options.
/// </summary>
public sealed class ApplicationOptions
{
    #region Properties
    /// <summary>
    /// Web API base URL.
    /// </summary>
    [Required, NotNull]
    public Uri? ApiUrl { get; set; }

    /// <summary>
    /// Authentication options.
    /// </summary>
    public AuthenticationOptions Authentication { get; set; } = AuthenticationOptions.Default;

    /// <summary>
    /// Value to be used during data encryption.
    /// </summary>
    [Required, NotNull]
    public string? Secret { get; set; }

    /// <summary>
    /// Application tokens options.
    /// </summary>
    public TokenOptions Tokens { get; set; } = TokenOptions.Default;
    #endregion
}