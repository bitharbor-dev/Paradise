using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Options;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using static Paradise.Localization.DataValidation.ValidationMessages;

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
        ThrowIfNull(options.Value.Secret);

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