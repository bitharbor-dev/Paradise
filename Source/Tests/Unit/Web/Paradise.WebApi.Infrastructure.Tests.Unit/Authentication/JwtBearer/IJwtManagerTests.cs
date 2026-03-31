using Paradise.WebApi.Infrastructure.Authentication.JwtBearer;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Authentication.JwtBearer;

/// <summary>
/// <see cref="IJwtManager"/> test class.
/// </summary>
public sealed class IJwtManagerTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="RemoveTokenPrefixIfExists"/> method.
    /// </summary>
    public static TheoryData<string, string> RemoveTokenPrefixIfExists_MemberData { get; } = new()
    {
        { "Bearer token", "token" },
        { "bearer token", "token" },
        { "token", "token" },
        { "Bearer Bearer test", "Bearer test" }
    };

    /// <summary>
    /// Provides member data for <see cref="RemoveTokenPrefixIfExists_ThrowsOnInvalidToken"/> method.
    /// </summary>
    public static TheoryData<string?> RemoveTokenPrefixIfExists_ThrowsOnInvalidToken_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="IJwtManager.RemoveTokenPrefixIfExists"/> method should
    /// remove the "Bearer " part from the beginning of the input string.
    /// </summary>
    /// <param name="token">
    /// The token to be formatted.
    /// </param>
    /// <param name="expectedResult">
    /// Expected result.
    /// </param>
    [Theory, MemberData(nameof(RemoveTokenPrefixIfExists_MemberData))]
    public void RemoveTokenPrefixIfExists(string token, string expectedResult)
    {
        // Arrange

        // Act
        IJwtManager.RemoveTokenPrefixIfExists(ref token);

        // Assert
        Assert.Equal(expectedResult, token);
    }

    /// <summary>
    /// The <see cref="IJwtManager.RemoveTokenPrefixIfExists"/> method should
    /// throw the <see cref="ArgumentException"/> if the input
    /// <paramref name="token"/> is equal to <see langword="null"/>, <see cref="string.Empty"/> or whitespace-only.
    /// </summary>
    /// <param name="token">
    /// The token to be formatted.
    /// </param>
    [Theory, MemberData(nameof(RemoveTokenPrefixIfExists_ThrowsOnInvalidToken_MemberData))]
    public void RemoveTokenPrefixIfExists_ThrowsOnInvalidToken(string? token)
    {
        // Arrange
        var exceptionType = token is null
            ? typeof(ArgumentNullException)
            : typeof(ArgumentException);

        // Act & Assert
        Assert.Throws(exceptionType, ()
            => IJwtManager.RemoveTokenPrefixIfExists(ref token!));
    }
    #endregion
}