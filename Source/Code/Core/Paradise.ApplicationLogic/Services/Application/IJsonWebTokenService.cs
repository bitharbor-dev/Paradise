using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Paradise.ApplicationLogic.Services.Application;

/// <summary>
/// Provides JSON web token generation
/// and validation functionalities.
/// </summary>
public interface IJsonWebTokenService
{
    #region Methods
    /// <summary>
    /// Generates a JWT.
    /// </summary>
    /// <param name="claims">
    /// The list of claims to be included in JWT.
    /// <para>
    /// All JTI claims would be ignored and a single
    /// one with the value of <paramref name="refreshTokenId"/>
    /// would be used instead.
    /// </para>
    /// </param>
    /// <param name="refreshTokenId">
    /// The Id of the refresh token to be used to
    /// bound with the newly generated JWT.
    /// </param>
    /// <param name="expiryDate">
    /// Token expiry date.
    /// </param>
    /// <returns>
    /// A JWT, including the given list of <paramref name="claims"/>
    /// and additional JTI claim with the value of <paramref name="refreshTokenId"/>.
    /// </returns>
    string GenerateToken(IEnumerable<Claim> claims, Guid refreshTokenId, out DateTime expiryDate);

    /// <summary>
    /// Validates the format of the given <paramref name="token"/>.
    /// </summary>
    /// <param name="token">
    /// The JWT to be validated.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given <paramref name="token"/> has a valid format,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    bool ValidateFormat(string? token);

    /// <summary>
    /// Validates the given <paramref name="token"/>
    /// and writes it into <paramref name="jwtSecurityToken"/>
    /// in case of success.
    /// </summary>
    /// <param name="token">
    /// The token to be validated and read.
    /// </param>
    /// <param name="jwtSecurityToken">
    /// The <see cref="JwtSecurityToken"/> into which
    /// to write the <paramref name="token"/>.
    /// </param>
    /// <param name="claimsPrincipal">
    /// The <see cref="ClaimsPrincipal"/> instance
    /// retrieved from the given <paramref name="token"/>.
    /// </param>
    /// <param name="checkExpiry">
    /// Indicates whether the method will return <see langword="false"/>
    /// if the <paramref name="token"/> is expired.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if validation succeeds, otherwise - <see langword="false"/>.
    /// </returns>
    bool TryParseToken(string? token, [NotNullWhen(true)] out JwtSecurityToken? jwtSecurityToken, [NotNullWhen(true)] out ClaimsPrincipal? claimsPrincipal, bool checkExpiry = true);

    /// <summary>
    /// Removes the 'Bearer ' prefix from the given
    /// <paramref name="token"/> if such prefix found.
    /// </summary>
    /// <param name="token">
    /// The token to be formatted.
    /// </param>
    public static void RemoveTokenPrefixIfExists(ref string token)
    {
        if (token.StartsWith(JwtBearerDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
            token = token[(JwtBearerDefaults.AuthenticationScheme.Length + 1)..];
    }
    #endregion
}