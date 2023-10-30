using Microsoft.IdentityModel.Tokens;

namespace Paradise.ApplicationLogic.Authorization.JwtBearer.Keys;

/// <summary>
/// An abstraction to provide JWT signing keys.
/// </summary>
public interface IJwtSigningKeyProvider
{
    #region Methods
    /// <summary>
    /// Gets the key to sign a JWTs.
    /// </summary>
    /// <returns>
    /// The <see cref="SecurityKey"/> to be used
    /// during JWTs creation or verification.
    /// </returns>
    SecurityKey GetSigningKey();
    #endregion
}