namespace Paradise.Domain.Tests.Users;

/// <summary>
/// Test class for the <see cref="UserRefreshToken"/>.
/// </summary>
public sealed class UserRefreshTokenTests
{
    #region Public methods
    /// <summary>
    /// <see cref="UserRefreshToken.IsOutdated"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/> since the sum of the input
    /// <see cref="TimeSpan"/> value and <see cref="UserRefreshToken"/> creation time
    /// is greater than <see cref="DateTime.UtcNow"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void IsOutdated_ReturnsFalse()
    {
        // Arrange
        var creationTime = DateTime.UtcNow;
        var lifetime = TimeSpan.FromDays(1);

        var userRefreshToken = new UserRefreshToken(Guid.Empty)
        {
            Created = creationTime
        };

        // Act
        var result = userRefreshToken.IsOutdated(lifetime);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// <see cref="UserRefreshToken.IsOutdated"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="true"/> since the sum of the input
    /// <see cref="TimeSpan"/> value and <see cref="UserRefreshToken"/> creation time
    /// is not greater than <see cref="DateTime.UtcNow"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void IsOutdated_ReturnsTrue()
    {
        // Arrange
        var creationTime = DateTime.UtcNow.AddDays(-2);
        var lifetime = TimeSpan.FromDays(1);

        var userRefreshToken = new UserRefreshToken(Guid.Empty)
        {
            Created = creationTime
        };

        // Act
        var result = userRefreshToken.IsOutdated(lifetime);

        // Assert
        Assert.True(result);
    }
    #endregion
}