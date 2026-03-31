using Paradise.WebApi.Infrastructure.Authentication.Caching;
using Paradise.WebApi.Infrastructure.Authentication.Caching.Implementation;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Authentication.Caching.Implementation;

/// <summary>
/// <see cref="RefreshTokenCache"/> test class.
/// </summary>
public sealed partial class RefreshTokenCacheTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="RefreshTokenCache.GetAsync"/> method should
    /// return an entry with the specified Id.
    /// </summary>
    [Fact]
    public async Task GetAsync()
    {
        // Arrange
        var id = Guid.Empty;
        Test.SetEntry(id, Test.UtcNow, false);

        // Act
        var result = await Test.Target.GetAsync(id, Token);

        // Assert
        Assert.NotNull(result);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCache.GetAsync"/> method should
    /// return <see langword="null"/>
    /// when no entry with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task GetAsync_ReturnsNullOnNonExistingEntry()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var result = await Test.Target.GetAsync(id, Token);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCache.SetAsync"/> method should
    /// store the entry.
    /// </summary>
    [Fact]
    public async Task SetAsync_StoresEntryWithCorrectExpiration()
    {
        // Arrange
        var id = Guid.Empty;
        var entry = new RefreshTokenCacheEntry(Test.UtcNow.Ticks, false);

        // Act
        await Test.Target.SetAsync(id, entry, Token);

        // Assert
        Assert.True(Test.TryGetEntry(id, out _));
    }

    /// <summary>
    /// The <see cref="RefreshTokenCache.RevokeAsync"/> method should
    /// revoke an entry with the specified Id.
    /// </summary>
    [Fact]
    public async Task RevokeAsync_StoresRevokedEntry()
    {
        // Arrange
        var id = Guid.Empty;
        Test.SetEntry(id, Test.UtcNow, false);

        // Act
        await Test.Target.RevokeAsync(id, Token);

        // Assert
        Assert.True(Test.TryGetEntry(id, out var entry));
        Assert.True(entry.Value.IsRevoked);
    }
    #endregion
}