using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Implementation;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Options;

/// <summary>
/// Contains configuration for <see cref="SymmetricSigningKeyProvider"/> class.
/// </summary>
internal sealed class SymmetricSigningKeyProviderOptions
{
    #region Properties
    /// <summary>
    /// A string key used to sign the JWT.
    /// </summary>
    [Required, NotNull]
    public string? Secret { get; set; }
    #endregion
}