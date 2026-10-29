using Microsoft.IdentityModel.Tokens;

namespace Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys;

/// <summary>
/// An abstraction to provide JWT signing keys.
/// </summary>
internal interface IJwtSigningKeyProvider
{
    #region Properties
    /// <summary>
    /// The signature algorithm to apply.
    /// </summary>
    string JwtAlgorithm { get; }
    #endregion

    #region Methods
    /// <summary>
    /// Gets the key to sign JWTs.
    /// </summary>
    /// <returns>
    /// The <see cref="SecurityKey"/> to be used
    /// during JWTs creation or verification.
    /// </returns>
    SecurityKey GetSigningKey();
    #endregion
}