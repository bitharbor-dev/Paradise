using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.ApplicationLogic.Options.Models;

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
    /// Tokens and operations timeout options.
    /// </summary>
    public TimeoutOptions Timeout { get; set; } = TimeoutOptions.Default;
    #endregion
}