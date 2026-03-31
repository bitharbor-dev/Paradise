using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Implementation;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Authentication.JwtBearer.Implementation;

/// <summary>
/// <see cref="JwtManager"/> test class.
/// </summary>
public sealed partial class JwtManagerTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="TryParseToken_ReturnsFalseOnInvalidToken"/> method.
    /// </summary>
    public static TheoryData<string?> TryParseToken_ReturnsFalseOnInvalidToken_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               },
        { "Invalid JWT"     }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="JwtManager.IssueToken"/> method should
    /// generate a valid JWT, as well as output the token expiry date.
    /// </summary>
    [Fact]
    public void IssueToken()
    {
        // Arrange
        var expectedExpiryDate = Test.UtcNow.Add(Test.AuthOptions.AccessTokenLifetime);

        var sub = new Claim(JwtRegisteredClaimNames.Sub, "user");
        var sid = new Claim(JwtRegisteredClaimNames.Sid, "old-value");

        var refreshTokenId = Guid.Empty;

        // Act
        var token = Test.Target.IssueToken([sub, sid], refreshTokenId, out var expiryDate);

        // Assert
        Assert.Equal(expectedExpiryDate, expiryDate);

        var jwt = Test.JwtHandler.ReadJwtToken(token);
        Assert.Equal(Test.JwtOptions.TokenValidationParameters.ValidIssuer, jwt.Issuer);

        var audience = Assert.Single(jwt.Audiences);
        Assert.Equal(Test.JwtOptions.TokenValidationParameters.ValidAudience, audience);

        var overwrittenSid = Assert.Single(jwt.Claims, claim => claim.Type.Equals(sid.Type, StringComparison.Ordinal));
        Assert.Equal(refreshTokenId.ToString(), overwrittenSid.Value);

        Assert.Contains(jwt.Claims, claim => claim.Type.Equals(sub.Type, StringComparison.Ordinal)
                                          && claim.Value.Equals(sub.Value, StringComparison.Ordinal));
    }

    /// <summary>
    /// The <see cref="JwtManager.TryParseToken"/> method should
    /// return <see langword="true"/>, exposing the
    /// <see cref="ClaimsPrincipal"/> instance if the input
    /// token is a valid JWT.
    /// </summary>
    [Fact]
    public void TryParseToken_ReturnsTrueOnValidToken()
    {
        // Arrange
        const string ClaimType1 = "Type1";
        const string ClaimType2 = "Type2";

        var claim1 = new Claim(ClaimType1, "Value1");
        var claim2 = new Claim(ClaimType2, "Value2");

        var token = Test.CreateJwt([claim1, claim2]);

        // Act
        var result = Test.Target.TryParseToken(token, out var principal);

        // Assert
        Assert.True(result);
        Assert.NotNull(principal);

        Assert.Contains(principal.Claims, claim => claim.Type is ClaimType1);
        Assert.Contains(principal.Claims, claim => claim.Type is ClaimType2);
    }

    /// <summary>
    /// The <see cref="JwtManager.TryParseToken"/> method should
    /// return <see langword="false"/> if the input
    /// token is not a valid JWT.
    /// </summary>
    [Theory, MemberData(nameof(TryParseToken_ReturnsFalseOnInvalidToken_MemberData))]
    public void TryParseToken_ReturnsFalseOnInvalidToken(string? token)
    {
        // Arrange

        // Act
        var result = Test.Target.TryParseToken(token, out var principal);

        // Assert
        Assert.False(result);
        Assert.Null(principal);
    }

    /// <summary>
    /// The <see cref="JwtManager.TryParseToken"/> method should
    /// return <see langword="false"/> if the input
    /// token has different algorithm specified from the
    /// algorithm set in the token validation parameters.
    /// </summary>
    [Fact]
    public void TryParseToken_ReturnsFalseOnAlgorithmMismatch()
    {
        // Arrange
        var token = Test.CreateJwt([]);

        Test.JwtOptions.TokenValidationParameters.ValidAlgorithms = ["another"];

        // Act
        var result = Test.Target.TryParseToken(token, out var principal);

        // Assert
        Assert.False(result);
        Assert.Null(principal);
    }

    /// <summary>
    /// The <see cref="JwtManager.TryParseToken"/> method should
    /// return <see langword="true"/>, exposing the
    /// <see cref="ClaimsPrincipal"/> instance if the input
    /// token is an expired JWT, but the token lifetime validation is disabled.
    /// </summary>
    [Fact]
    public void TryParseToken_ReturnsTrueOnExpiredToken_WithDisabledLifetimeValidation()
    {
        // Arrange
        var checkExpiry = false;

        var token = Test.CreateJwt([]);

        Test.UtcNow += Test.AuthOptions.AccessTokenLifetime * 2;

        // Act
        var result = Test.Target.TryParseToken(token, out var principal, checkExpiry);

        // Assert
        Assert.True(result);
        Assert.NotNull(principal);
    }

    /// <summary>
    /// The <see cref="JwtManager.TryParseToken"/> method should
    /// return <see langword="false"/> if the input
    /// token is an expired JWT and the token lifetime validation is enabled.
    /// </summary>
    [Fact]
    public void TryParseToken_ReturnsFalseOnExpiredToken_WithEnabledLifetimeValidation()
    {
        // Arrange
        var checkExpiry = true;

        var token = Test.CreateJwt([]);

        Test.UtcNow += Test.AuthOptions.AccessTokenLifetime * 2;

        // Act
        var result = Test.Target.TryParseToken(token, out var principal, checkExpiry);

        // Assert
        Assert.False(result);
        Assert.Null(principal);
    }
    #endregion
}