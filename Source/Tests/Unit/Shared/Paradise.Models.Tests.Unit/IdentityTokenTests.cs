namespace Paradise.Models.Tests.Unit;

/// <summary>
/// <see cref="IdentityToken"/> test class.
/// </summary>
public sealed class IdentityTokenTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="Constructor_ThrowsOnInvalidEmailAddress"/> method.
    /// </summary>
    public static TheoryData<string> Constructor_ThrowsOnInvalidEmailAddress_MemberData { get; } = new()
    {
        { string.Empty              },
        { "Invalid email address"   }
    };

    /// <summary>
    /// Provides member data for <see cref="IsExpired"/> method.
    /// </summary>
    public static TheoryData<DateTimeOffset?, DateTimeOffset, bool> IsExpired_MemberData { get; } = new()
    {
        { null,                                 DateTimeOffset.UnixEpoch,   false   },
        { DateTimeOffset.UnixEpoch.AddDays(-1), DateTimeOffset.UnixEpoch,   true    },
        { DateTimeOffset.UnixEpoch,             DateTimeOffset.UnixEpoch,   false   },
        { DateTimeOffset.UnixEpoch.AddDays(1),  DateTimeOffset.UnixEpoch,   false   }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="IdentityToken(string, string, string?, DateTimeOffset?)"/> constructor should
    /// throw the <see cref="ArgumentException"/> if the input
    /// email address is invalid.
    /// </summary>
    /// <param name="emailAddress">
    /// Email address.
    /// </param>
    [Theory, MemberData(nameof(Constructor_ThrowsOnInvalidEmailAddress_MemberData))]
    public void Constructor_ThrowsOnInvalidEmailAddress(string emailAddress)
    {
        // Arrange
        var innerToken = "InnerToken";

        // Act & Assert
        Assert.Throws<ArgumentException>(()
            => new IdentityToken(emailAddress, innerToken));
    }

    /// <summary>
    /// The <see cref="IdentityToken.IsExpired"/> method should
    /// return the correct expiration state based on the expiry date
    /// and the provided current time.
    /// </summary>
    /// <param name="expiryDate">
    /// Token expiry date.
    /// <para>
    /// <see langword="null"/> value means that token never expires.
    /// </para>
    /// </param>
    /// <param name="currentTime">
    /// Current time.
    /// </param>
    /// <param name="expectedResult">
    /// Expected result.
    /// </param>
    [Theory, MemberData(nameof(IsExpired_MemberData))]
    public void IsExpired(DateTimeOffset? expiryDate, DateTimeOffset currentTime, bool expectedResult)
    {
        // Arrange
        var emailAddress = "test@email.com";
        var innerToken = "InnerToken";

        var identityToken = new IdentityToken(emailAddress, innerToken, expiryDate: expiryDate);

        // Act
        var result = identityToken.IsExpired(currentTime);

        // Assert
        Assert.Equal(expectedResult, result);
    }
    #endregion
}