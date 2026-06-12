using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Options;
using System.Text;

namespace Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Implementation;

/// <summary>
/// Provides symmetric JWT signing keys.
/// </summary>
internal sealed class SymmetricSigningKeyProvider : IJwtSigningKeyProvider
{
    #region Fields
    private readonly SymmetricSigningKeyProviderOptions _options;

    private SymmetricSecurityKey? _key;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="SymmetricSigningKeyProvider"/> class.
    /// </summary>
    /// <param name="options">
    /// The accessor used to access the <see cref="SymmetricSigningKeyProviderOptions"/>.
    /// </param>
    public SymmetricSigningKeyProvider(IOptions<SymmetricSigningKeyProviderOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options.Value.Secret);

        _options = options.Value;
    }
    #endregion

    #region Properties
    /// <inheritdoc/>
    public string JwtAlgorithm { get; } = SecurityAlgorithms.HmacSha256;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public SecurityKey GetSigningKey()
    {
        if (_key is not null)
            return _key;

        var bytes = Encoding.UTF8.GetBytes(_options.Secret);
        _key = new(bytes);

        return _key;
    }
    #endregion
}