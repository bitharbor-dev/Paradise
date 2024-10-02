namespace Paradise.Options.Models;

/// <summary>
/// JWT Key Vault options.
/// </summary>
public sealed class JwtKeyVaultOptions
{
    #region Properties
    /// <summary>
    /// Key Vault URL.
    /// </summary>
    public Uri? VaultUrl { get; set; }

    /// <summary>
    /// JWT signing certificate name.
    /// </summary>
    public string? CertificateName { get; set; }

    /// <summary>
    /// Tenant Id.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Client Id.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Client secret.
    /// </summary>
    public string? ClientSecret { get; set; }
    #endregion
}