using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Paradise.Common.Extensions;
using Paradise.WebApi.Infrastructure.Options;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Implementation;

/// <summary>
/// Provides JSON web token generation and validation functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="JwtManager"/> class.
/// </remarks>
/// <param name="authenticationOptions">
/// The accessor used to access the <see cref="AuthenticationOptions"/>.
/// </param>
/// <param name="jwtBearerOptions">
/// The accessor used to access the <see cref="JwtBearerOptions"/>.
/// </param>
/// <param name="timeProvider">
/// Time provider.
/// </param>
internal sealed class JwtManager(IOptions<AuthenticationOptions> authenticationOptions,
                                 IOptions<JwtBearerOptions> jwtBearerOptions,
                                 TimeProvider timeProvider) : IJwtManager
{
    #region Fields
    private static readonly JwtSecurityTokenHandler _handler = new();
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public string IssueToken(IEnumerable<Claim> claims, Guid refreshTokenId, out DateTimeOffset expiryDate)
    {
        // Removing the Sid claim from the input claims
        // and adding a new one with the value of refreshTokenId
        // to track refresh token in the future.
        var overwrittenClaims = claims.Where(claim => claim.Type is not JwtRegisteredClaimNames.Sid).ToList();
        overwrittenClaims.Add(new(JwtRegisteredClaimNames.Sid, refreshTokenId.ToString()));

        var creationTime = timeProvider.GetUtcNow();
        var accessTokenLifetime = authenticationOptions.Value.AccessTokenLifetime;

        expiryDate = creationTime.Add(accessTokenLifetime);

        var tokenParameters = jwtBearerOptions.Value.TokenValidationParameters;
        var algorithm = tokenParameters.ValidAlgorithms.Single();

        var token = _handler.CreateJwtSecurityToken(new()
        {
            Audience = tokenParameters.ValidAudience,
            Subject = new(overwrittenClaims),
            IssuedAt = creationTime.UtcDateTime,
            NotBefore = creationTime.UtcDateTime,
            Expires = expiryDate.UtcDateTime,
            Issuer = tokenParameters.ValidIssuer,
            SigningCredentials = new(tokenParameters.IssuerSigningKey, algorithm)
        });

        return _handler.WriteToken(token);
    }

    /// <inheritdoc/>
    public bool TryParseToken(string? token, [NotNullWhen(true)] out ClaimsPrincipal? claimsPrincipal, bool checkExpiry = true)
    {
        claimsPrincipal = null;

        if (token.IsNullOrWhiteSpace())
            return false;

        try
        {
            var tokenParameters = jwtBearerOptions.Value.TokenValidationParameters.Clone();

            tokenParameters.ValidateLifetime = checkExpiry;

            claimsPrincipal = _handler.ValidateToken(token, tokenParameters, out _);

            return true;
        }
        catch (SecurityTokenException)
        {
            return false;
        }
        catch (SecurityTokenArgumentException)
        {
            return false;
        }
    }
    #endregion
}