using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Paradise.WebApi.Infrastructure.Authentication.JwtBearer;

/// <summary>
/// Provides JSON web token generation and validation functionalities.
/// </summary>
public interface IJwtManager
{
    #region Constants
    private const string BearerTokenPrefix = $"{JwtBearerDefaults.AuthenticationScheme} ";
    #endregion

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
    /// and additional SID claim with the value of <paramref name="refreshTokenId"/>.
    /// </returns>
    string IssueToken(IEnumerable<Claim> claims, Guid refreshTokenId, out DateTimeOffset expiryDate);

    /// <summary>
    /// Validates the given <paramref name="token"/>.
    /// </summary>
    /// <param name="token">
    /// The token to be validated and read.
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
    bool TryParseToken(string? token, [NotNullWhen(true)] out ClaimsPrincipal? claimsPrincipal, bool checkExpiry = true);

    /// <summary>
    /// Removes the 'Bearer ' prefix from the given
    /// <paramref name="token"/> if such prefix found.
    /// </summary>
    /// <param name="token">
    /// The token to be formatted.
    /// </param>
    static void RemoveTokenPrefixIfExists(ref string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        if (token.StartsWith(BearerTokenPrefix, StringComparison.OrdinalIgnoreCase))
            token = token[BearerTokenPrefix.Length..];
    }
    #endregion
}