using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Implementation;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Options;

/// <summary>
/// Contains configuration for <see cref="AsymmetricSigningKeyProvider"/> class.
/// </summary>
internal sealed class AsymmetricSigningKeyProviderOptions
{
    #region Properties
    /// <summary>
    /// High-entropy secret used to deterministically derive the private key.
    /// </summary>
    [Required, NotNull]
    public string? PrivateKey { get; set; }

    /// <summary>
    /// Key identifier.
    /// <para>
    /// Optional.
    /// </para>
    /// </summary>
    public string? KeyId { get; set; }
    #endregion
}