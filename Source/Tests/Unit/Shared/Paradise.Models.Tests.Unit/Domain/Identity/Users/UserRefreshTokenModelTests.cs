using Paradise.Models.Domain.Identity.Users;

namespace Paradise.Models.Tests.Unit.Domain.Identity.Users;

/// <summary>
/// <see cref="UserRefreshTokenModel"/> test class.
/// </summary>
public sealed class UserRefreshTokenModelTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="IsExpired"/> method.
    /// </summary>
    public static TheoryData<double, bool> IsExpired_MemberData { get; } = new()
    {
        { +1,   false   },
        { -1,   true    }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="UserRefreshTokenModel.IsExpired"/> method should
    /// return a boolean value indicating whether the refresh token is expired.
    /// </summary>
    /// <param name="delta">
    /// The difference between 'current time' and expiry date.
    /// </param>
    /// <param name="expectedResult">
    /// Expected result.
    /// </param>
    [Theory, MemberData(nameof(IsExpired_MemberData))]
    public void IsExpired(double delta, bool expectedResult)
    {
        // Arrange
        var currentTime = DateTimeOffset.UnixEpoch;
        var expiryDateUtc = currentTime.AddDays(delta);

        var model = new UserRefreshTokenModel(Guid.Empty, Guid.Empty, currentTime, true, expiryDateUtc);

        // Act
        var result = model.IsExpired(currentTime);

        // Assert
        Assert.Equal(expectedResult, result);
    }
    #endregion
}