using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Paradise.ApplicationLogic.Tests.Services.Application;

/// <summary>
/// Test class for the <see cref="JsonWebTokenService"/> methods.
/// </summary>
public sealed class JsonWebTokenServiceTests
{
    #region Constants
    /// <summary>
    /// A <see cref="string"/> containing the encryption key.
    /// </summary>
    private const string Secret = "Ug5xCXJaNaxfx78KdQxQZDAmniAbZw6V";
    /// <summary>
    /// Valid test JWT.
    /// </summary>
    private const string ValidToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.e30.Et9HFtf9R3GEMA0IICOfFMVXY7kkTX1wr4qCyhIf58U";
    /// <summary>
    /// Invalid test JWT.
    /// </summary>
    private const string InvalidToken = "ABCD1234";
    /// <summary>
    /// Empty test JWT.
    /// </summary>
    private const string EmptyToken = "";
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonWebTokenServiceTests"/> class.
    /// </summary>
    public JsonWebTokenServiceTests()
    {
        var applicationOptions = GetApplicationOptions(Secret);
        var jwtBearerOptions = GetJwtBearerOptions(Secret);

        Options = jwtBearerOptions.Value;

        Service = new(applicationOptions, jwtBearerOptions);
    }
    #endregion

    #region Properties
    /// <summary>
    /// JWT Bearer options.
    /// </summary>
    public JwtBearerOptions Options { get; }

    /// <summary>
    /// A <see cref="JsonWebTokenService"/> instance to be tested.
    /// </summary>
    public JsonWebTokenService Service { get; }
    #endregion

    #region Public methods
    /// <summary>
    /// <see cref="JsonWebTokenService.GenerateToken"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns a <see cref="string"/> value containing the JWT.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GenerateToken()
    {
        // Arrange
        var claims = Array.Empty<Claim>();

        // Act
        var token = Service.GenerateToken(claims, Guid.Empty, out _);

        // Assert
        Assert.True(new JwtSecurityTokenHandler().CanReadToken(token));
    }

    /// <summary>
    /// <see cref="JsonWebTokenService.TryParseToken"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="true"/>
    /// and not-null values in the output parameters
    /// due to the first argument passed into the method is a valid JWT.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void TryParseToken()
    {
        // Arrange
        var parameters = Options.TokenValidationParameters;

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.CreateJwtSecurityToken(new()
        {
            Audience = parameters?.ValidAudience,
            Issuer = parameters?.ValidIssuer,
            SigningCredentials = new(parameters?.IssuerSigningKey, SecurityAlgorithms.HmacSha256)
        });

        var token = handler.WriteToken(jwt);

        // Act
        var result = Service.TryParseToken(token, out var jwtSecurityToken, out var claimsPrincipal, false);

        // Assert
        Assert.True(result);
        Assert.NotNull(jwtSecurityToken);
        Assert.NotNull(claimsPrincipal);
    }

    /// <summary>
    /// <see cref="JsonWebTokenService.TryParseToken"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/>
    /// and null values in the output parameters
    /// due to the first argument passed into the method is expired JWT.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void TryParseToken_ReturnsFalseOnExpiredToken()
    {
        // Arrange
        var checkExpiry = true;

        var parameters = Options.TokenValidationParameters;

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.CreateJwtSecurityToken(new()
        {
            Audience = parameters?.ValidAudience,
            Expires = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
            Issuer = parameters?.ValidIssuer,
            NotBefore = DateTime.UtcNow.Subtract(TimeSpan.FromDays(2)),
            SigningCredentials = new(parameters?.IssuerSigningKey, SecurityAlgorithms.HmacSha256)
        });

        var token = handler.WriteToken(jwt);

        // Act
        var result = Service.TryParseToken(token, out var jwtSecurityToken, out var claimsPrincipal, checkExpiry);

        // Assert
        Assert.False(result);
        Assert.Null(jwtSecurityToken);
        Assert.Null(claimsPrincipal);
    }

    /// <summary>
    /// <see cref="JsonWebTokenService.TryParseToken"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/>
    /// and null values in the output parameters
    /// due to the first argument passed into the method is not a valid JWT.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void TryParseToken_ReturnsFalseOnInvalidToken()
    {
        // Arrange
        var token = InvalidToken;

        // Act
        var result = Service.TryParseToken(token, out var jwtSecurityToken, out var claimsPrincipal, true);

        // Assert
        Assert.False(result);
        Assert.Null(jwtSecurityToken);
        Assert.Null(claimsPrincipal);
    }

    /// <summary>
    /// <see cref="JsonWebTokenService.ValidateFormat"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="true"/>
    /// due to the argument passed into the method is a valid JWT.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void ValidateFormat()
    {
        // Arrange
        var token = ValidToken;

        // Act
        var result = Service.ValidateFormat(token);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// <see cref="JsonWebTokenService.ValidateFormat"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/>
    /// due to the argument passed into the method
    /// is <see cref="string.Empty"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void ValidateFormat_ReturnsFalseOnEmptyString()
    {
        // Arrange
        var token = EmptyToken;

        // Act
        var result = Service.ValidateFormat(token);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// <see cref="JsonWebTokenService.ValidateFormat"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/>
    /// due to the argument passed into the method is not a valid JWT.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void ValidateFormat_ReturnsFalseOnInvalidTokenFormat()
    {
        // Arrange
        var token = InvalidToken;

        // Act
        var result = Service.ValidateFormat(token);

        // Assert
        Assert.False(result);
    }
    #endregion
}