using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Options;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using static Paradise.Localization.DataValidation.ValidationMessages;

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
        ThrowIfNull(options.Value.PrivateKey);

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
            var message = GetMessageObjectIsNull(parameterName!);

            throw new InvalidOperationException(message);
        }
    }
    #endregion
}