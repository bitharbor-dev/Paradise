namespace Paradise.Domain.Tests.Users;

/// <summary>
/// Test class for the <see cref="User"/>.
/// </summary>
public sealed class UserTests
{
    #region Public methods
    /// <summary>
    /// <see cref="User.CancelDeletionRequest"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Sets the <see cref="User.DeletionRequestSubmitted"/> to <see langword="null"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void CancelDeletionRequest()
    {
        // Arrange
        var user = new User("test@email.com", "TestUser")
        {
            DeletionRequestSubmitted = DateTime.UtcNow
        };

        // Act
        user.CancelDeletionRequest();

        // Assert
        Assert.Null(user.DeletionRequestSubmitted);
    }

    /// <summary>
    /// <see cref="User.IsDeletionRequestOutdated"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/> since the
    /// <see cref="User.DeletionRequestSubmitted"/> is <see langword="null"/>
    /// or sum of the input <see cref="TimeSpan"/> value
    /// and <see cref="User.DeletionRequestSubmitted"/>
    /// is greater than <see cref="DateTime.UtcNow"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Theory, MemberData(nameof(IsDeletionRequestOutdated_ReturnsFalse_MemberData))]
    public void IsDeletionRequestOutdated_ReturnsFalse(DateTime? requestTime)
    {
        // Arrange
        var lifetime = TimeSpan.FromDays(1);

        var user = new User("test@email.com", "TestUser")
        {
            DeletionRequestSubmitted = requestTime
        };

        // Act
        var result = user.IsDeletionRequestOutdated(lifetime);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// <see cref="User.IsDeletionRequestOutdated"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="true"/> since the
    /// sum of the input <see cref="TimeSpan"/> value
    /// and <see cref="User.DeletionRequestSubmitted"/>
    /// is not greater than <see cref="DateTime.UtcNow"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void IsDeletionRequestOutdated_ReturnsTrue()
    {
        // Arrange
        var requestTime = DateTime.UtcNow.AddDays(-2);
        var lifetime = TimeSpan.FromDays(1);

        var user = new User("test@email.com", "TestUser")
        {
            DeletionRequestSubmitted = requestTime
        };

        // Act
        var result = user.IsDeletionRequestOutdated(lifetime);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// <see cref="User.IsEmailConfirmationPeriodExceeded"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/> since the sum of the input
    /// <see cref="TimeSpan"/> value and <see cref="User.Created"/>
    /// is greater than <see cref="DateTime.UtcNow"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void IsEmailConfirmationPeriodExceeded_ReturnsFalse()
    {
        // Arrange
        var requestTime = DateTime.UtcNow;
        var lifetime = TimeSpan.FromDays(1);

        var user = new User("test@email.com", "TestUser")
        {
            Created = requestTime
        };

        // Act
        var result = user.IsEmailConfirmationPeriodExceeded(lifetime);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// <see cref="User.IsEmailConfirmationPeriodExceeded"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="true"/> since the sum of the input
    /// <see cref="TimeSpan"/> value and <see cref="User.Created"/>
    /// is not greater than <see cref="DateTime.UtcNow"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void IsEmailConfirmationPeriodExceeded_ReturnsTrue()
    {
        // Arrange
        var requestTime = DateTime.UtcNow.AddDays(-2);
        var lifetime = TimeSpan.FromDays(1);

        var user = new User("test@email.com", "TestUser")
        {
            Created = requestTime
        };

        // Act
        var result = user.IsEmailConfirmationPeriodExceeded(lifetime);

        // Assert
        Assert.True(result);
    }
    #endregion

    #region Data generation
    /// <summary>
    /// Provides member data for <see cref="IsDeletionRequestOutdated_ReturnsFalse"/> method.
    /// </summary>
    public static TheoryData<DateTime?> IsDeletionRequestOutdated_ReturnsFalse_MemberData => new()
    {
        { null },
        { DateTime.UtcNow }
    };
    #endregion
}