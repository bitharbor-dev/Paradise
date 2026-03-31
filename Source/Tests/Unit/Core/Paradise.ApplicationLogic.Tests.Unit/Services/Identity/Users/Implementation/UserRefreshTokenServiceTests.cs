using Paradise.ApplicationLogic.Services.Identity.Users.Implementation;
using Paradise.Models;

namespace Paradise.ApplicationLogic.Tests.Unit.Services.Identity.Users;

/// <summary>
/// <see cref="UserRefreshTokenService"/> test class.
/// </summary>
public sealed partial class UserRefreshTokenServiceTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="UserRefreshTokenService.GetByIdAsync"/> method should
    /// return a refresh token with the specified Id.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync()
    {
        // Arrange
        var user = await Test.AddUserAsync();
        var refreshToken = await Test.AddRefreshTokenAsync(user);

        // Act
        var result = await Test.Target.GetByIdAsync(refreshToken.Id, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(OperationStatus.Success, result.Status);

        Assert.Equal(refreshToken.Id, result.Value.Id);
    }

    /// The <see cref="UserRefreshTokenService.GetByIdAsync"/> method should
    /// fail to retrieve a refresh token
    /// when no refresh token with the specified Id exists.
    [Fact]
    public async Task GetByIdAsync_FailsOnNonExistingToken()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var result = await Test.Target.GetByIdAsync(id, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(OperationStatus.Missing, result.Status);
    }

    /// <summary>
    /// The <see cref="UserRefreshTokenService.CreateAsync"/> method should
    /// create a new refresh token.
    /// </summary>
    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var user = await Test.AddUserAsync();
        var lifetime = TimeSpan.FromHours(1);

        // Act
        var result = await Test.Target.CreateAsync(user.Id, lifetime, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(OperationStatus.Created, result.Status);

        Assert.Equal(user.Id, result.Value.OwnerId);
    }

    /// <summary>
    /// The <see cref="UserRefreshTokenService.CreateAsync"/> method should
    /// fail to create a refresh token
    /// when no user with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task CreateAsync_FailsOnNonExistingUser()
    {
        // Arrange
        var userId = Guid.Empty;
        var lifetime = TimeSpan.FromDays(1);

        // Act
        var result = await Test.Target.CreateAsync(userId, lifetime, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(OperationStatus.Missing, result.Status);
    }

    /// <summary>
    /// The <see cref="UserRefreshTokenService.DeactivateAsync"/> method should
    /// deactivate a refresh token with the specified Id.
    /// </summary>
    [Fact]
    public async Task DeactivateAsync()
    {
        // Arrange
        var user = await Test.AddUserAsync();
        var refreshToken = await Test.AddRefreshTokenAsync(user, isActive: true);

        // Act
        var result = await Test.Target.DeactivateAsync(user.Id, refreshToken.Id, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(OperationStatus.Success, result.Status);

        Assert.False(result.Value.IsActive);
    }

    /// The <see cref="UserRefreshTokenService.DeactivateAsync"/> method should
    /// fail to deactivate a refresh token
    /// when no refresh token with the specified Id exists
    /// in the scope of the user with the specified user Id..
    [Fact]
    public async Task DeactivateAsync_FailsOnNonExistingToken()
    {
        // Arrange
        var user = await Test.AddUserAsync();
        var id = Guid.Empty;

        // Act
        var result = await Test.Target.DeactivateAsync(user.Id, id, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(OperationStatus.Missing, result.Status);
    }

    /// <summary>
    /// The <see cref="UserRefreshTokenService.DeactivateAllAsync"/> method should
    /// deactivate all user's refresh tokens.
    /// </summary>
    [Fact]
    public async Task DeactivateAllAsync()
    {
        // Arrange
        var user = await Test.AddUserAsync();
        await Test.AddRefreshTokenAsync(user, isActive: true);
        await Test.AddRefreshTokenAsync(user, isActive: true);

        // Act
        var result = await Test.Target.DeactivateAllAsync(user.Id, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(OperationStatus.Success, result.Status);

        Assert.All(result.Value, token => Assert.False(token.IsActive));
    }

    /// <summary>
    /// The <see cref="UserRefreshTokenService.DeleteExpiredAsync"/> method should
    /// delete expired refresh tokens.
    /// </summary>
    [Fact]
    public async Task DeleteExpiredAsync()
    {
        // Arrange
        Test.UtcNow = DateTimeOffset.UnixEpoch;

        var user = await Test.AddUserAsync();
        await Test.AddRefreshTokenAsync(user, Test.UtcNow - TimeSpan.FromDays(1));
        await Test.AddRefreshTokenAsync(user, Test.UtcNow - TimeSpan.FromDays(1));
        await Test.AddRefreshTokenAsync(user, Test.UtcNow + TimeSpan.FromDays(1));

        // Act
        var result = await Test.Target.DeleteExpiredAsync(Token);

        // Assert
        Assert.Equal(2, result.Value);
        Assert.Equal(OperationStatus.Success, result.Status);
    }
    #endregion
}