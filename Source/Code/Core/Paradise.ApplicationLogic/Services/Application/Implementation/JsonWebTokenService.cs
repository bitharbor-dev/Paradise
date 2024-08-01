using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Paradise.Common.Extensions;
using Paradise.Options.Models;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Paradise.ApplicationLogic.Services.Application.Implementation;

/// <summary>
/// Provides JSON web token generation
/// and validation functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="JsonWebTokenService"/> class.
/// </remarks>
/// <param name="applicationOptions">
/// The accessor used to access the <see cref="ApplicationOptions"/>.
/// </param>
/// <param name="jwtBearerOptions">
/// The accessor used to access the <see cref="JwtBearerOptions"/>.
/// </param>
public sealed class JsonWebTokenService(IOptions<ApplicationOptions> applicationOptions,
                                        IOptions<JwtBearerOptions> jwtBearerOptions)
    : IJsonWebTokenService
{
    #region Fields
    private readonly ApplicationOptions _applicationOptions = applicationOptions.Value;
    private readonly JwtBearerOptions _jwtBearerOptions = jwtBearerOptions.Value;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public string GenerateToken(IEnumerable<Claim> claims, Guid refreshTokenId, out DateTime expiryDate)
    {
        // Removing the Jti claim from the input claims
        // and adding a new one with the value of refreshTokenId
        // to track refresh token in the future.
        var overwrittenClaims = claims.Where(claim => claim.Type is not JwtRegisteredClaimNames.Jti).ToList();
        overwrittenClaims.Add(new(JwtRegisteredClaimNames.Jti, refreshTokenId.ToString()));

        var creationTime = DateTime.UtcNow;
        var accessTokenLifetime = _applicationOptions.Authentication.AccessTokenLifetime;

        expiryDate = creationTime.Add(accessTokenLifetime);

        var tokenParameters = _jwtBearerOptions.TokenValidationParameters;

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(new()
        {
            Audience = tokenParameters.ValidAudience,
            Subject = new(overwrittenClaims),
            IssuedAt = creationTime,
            NotBefore = creationTime,
            Expires = expiryDate,
            Issuer = tokenParameters.ValidIssuer,
            SigningCredentials = new(tokenParameters.IssuerSigningKey, SecurityAlgorithms.HmacSha256)
        });

        return handler.WriteToken(token);
    }

    /// <inheritdoc/>
    public bool ValidateFormat(string? token)
    {
        if (token.IsNullOrWhiteSpace())
            return false;

        IJsonWebTokenService.RemoveTokenPrefixIfExists(ref token);

        return new JwtSecurityTokenHandler().CanReadToken(token);
    }

    /// <inheritdoc/>
    public bool TryParseToken(string? token, [NotNullWhen(true)] out JwtSecurityToken? jwtSecurityToken,
                              [NotNullWhen(true)] out ClaimsPrincipal? claimsPrincipal, bool checkExpiry = true)
    {
        jwtSecurityToken = null;
        claimsPrincipal = null;

        if (token.IsNullOrWhiteSpace())
            return false;

        try
        {
            var tokenParameters = _jwtBearerOptions.TokenValidationParameters;

            tokenParameters.ValidateLifetime = checkExpiry;

            IJsonWebTokenService.RemoveTokenPrefixIfExists(ref token);

            var handler = new JwtSecurityTokenHandler();
            claimsPrincipal = handler.ValidateToken(token, tokenParameters, out var securityToken);
            jwtSecurityToken = securityToken as JwtSecurityToken;

            return jwtSecurityToken is not null
                && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
    #endregion
}