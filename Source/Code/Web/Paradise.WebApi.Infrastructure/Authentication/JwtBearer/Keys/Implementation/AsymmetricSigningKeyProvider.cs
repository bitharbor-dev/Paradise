using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Options;
using System.Security.Cryptography;

namespace Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Implementation;

/// <summary>
/// Provides asymmetric JWT signing keys.
/// </summary>
internal sealed class AsymmetricSigningKeyProvider : IJwtSigningKeyProvider
{
    #region Fields
    private readonly AsymmetricSigningKeyProviderOptions _options;

    private RsaSecurityKey? _key;
    #endregion

    #region Constructors
    public AsymmetricSigningKeyProvider(IOptions<AsymmetricSigningKeyProviderOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options.Value.PrivateKey);

        _options = options.Value;
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
        if (_key is not null)
            return _key;

        var privateKey = Convert.FromBase64String(_options.PrivateKey);

        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(privateKey, out _);

        var paramters = rsa.ExportParameters(true);

        _key = new(paramters)
        {
            KeyId = _options.KeyId
        };

        return _key;
    }
    #endregion
}