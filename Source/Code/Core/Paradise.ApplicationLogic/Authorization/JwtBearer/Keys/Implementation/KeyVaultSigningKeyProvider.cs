using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Paradise.Common.Extensions;
using Paradise.Options.Models;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using static Paradise.Localization.DataValidation.ValidationMessagesProvider;

namespace Paradise.ApplicationLogic.Authorization.JwtBearer.Keys.Implementation;

/// <summary>
/// Provides JWT signing keys based on a key contained in the Azure Key Vault.
/// </summary>
public sealed class KeyVaultSigningKeyProvider : IJwtSigningKeyProvider
{
    #region Fields
    private readonly Uri _vaultUrl;
    private readonly string _certificateName;
    private readonly string _tenantId;
    private readonly string _clientId;
    private readonly string _clientSecret;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="KeyVaultSigningKeyProvider"/> class.
    /// </summary>
    /// <param name="configuration">
    /// The <see cref="IConfiguration"/> instance to get
    /// the Azure Key Vault credentials from.
    /// </param>
    public KeyVaultSigningKeyProvider(IConfiguration configuration)
    {
        var options = configuration.GetRequiredInstance<JwtKeyVaultOptions>();
        ThrowIfNull(options.VaultUrl);
        ThrowIfNull(options.CertificateName);
        ThrowIfNull(options.TenantId);
        ThrowIfNull(options.ClientId);
        ThrowIfNull(options.ClientSecret);

        _vaultUrl = options.VaultUrl;
        _certificateName = options.CertificateName;
        _tenantId = options.TenantId;
        _clientId = options.ClientId;
        _clientSecret = options.ClientSecret;
    }
    #endregion

    #region Properties
    /// <inheritdoc/>
    public string JwtAlgorithm { get; } = SecurityAlgorithms.RsaSha256;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public SecurityKey GetSigningKey()
    {
        var credentials = new ClientSecretCredential(_tenantId, _clientId, _clientSecret);

        var certificateClient = new CertificateClient(_vaultUrl, credentials);

        var certificateResponse = certificateClient.DownloadCertificate(new DownloadCertificateOptions(_certificateName)
        {
            KeyStorageFlags = X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable
        });

        var certificate = certificateResponse.Value;

        return new X509SecurityKey(certificate);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if the given <paramref name="value"/>
    /// is equals to <see langword="null"/>.
    /// </summary>
    /// <param name="value">
    /// The value to check.
    /// </param>
    /// <param name="parameterName">
    /// Parameter name.
    /// </param>
    private static void ThrowIfNull([NotNull] object? value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value is null)
        {
            var message = GetObjectIsNullMessage(parameterName!);

            throw new InvalidOperationException(message);
        }
    }
    #endregion
}