using Paradise.WebApi.Infrastructure.Authentication.Caching;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Authentication.Caching;

/// <summary>
/// <see cref="RefreshTokenCacheEntry"/> test class.
/// </summary>
public sealed class RefreshTokenCacheEntryTests
{
    #region Properties
    /// <inheritdoc cref="DateTimeOffset.UnixEpoch"/>
    public static DateTimeOffset UnixEpoch { get; } = DateTimeOffset.UnixEpoch;

    /// <summary>
    /// Provides member data for <see cref="AsSpan"/> method.
    /// </summary>
    public static TheoryData<bool> AsSpan_MemberData { get; } = new()
    {
        { true  },
        { false }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry(ReadOnlySpan{byte})"/> constructor should
    /// correctly deserialize expiration timestamp and revoked flag.
    /// </summary>
    [Fact]
    public void Constructor_FromSpan()
    {
        var expiration = UnixEpoch.AddMinutes(10).Ticks;

        var buffer = new byte[RefreshTokenCacheEntry.EntryBytesSize];
        BitConverter.TryWriteBytes(buffer.AsSpan(0, 8), expiration);
        buffer[8] = 1;

        // Act
        var entry = new RefreshTokenCacheEntry(buffer);

        // Assert
        Assert.Equal(expiration, entry.ExpirationTimestamp);
        Assert.True(entry.IsRevoked);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry(long, bool)"/> constructor should
    /// assign expiration timestamp and revoked flag correctly.
    /// </summary>
    [Fact]
    public void Constructor_FromValues()
    {
        // Arrange
        var expiration = UnixEpoch.AddMinutes(5).Ticks;
        var isRevoked = false;

        // Act
        var entry = new RefreshTokenCacheEntry(expiration, isRevoked);

        // Assert
        Assert.Equal(expiration, entry.ExpirationTimestamp);
        Assert.Equal(isRevoked, entry.IsRevoked);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.AsSpan"/> method should
    /// correctly encode expiration timestamp and revoked flag.
    /// </summary>
    /// <param name="isRevoked">
    /// Indicates whether the refresh token was
    /// revoked by the user.
    /// </param>
    [Theory, MemberData(nameof(AsSpan_MemberData))]
    public void AsSpan(bool isRevoked)
    {
        // Arrange
        var expiration = DateTimeOffset.UnixEpoch.AddMinutes(1).Ticks;

        var entry = new RefreshTokenCacheEntry(expiration, isRevoked);

        // Act
        var span = entry.AsSpan();

        // Assert
        Assert.Equal(expiration, BitConverter.ToInt64(span[..8]));
        Assert.Equal(Convert.ToByte(isRevoked), span[8]);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.IsActive"/> method should
    /// return <see langword="true"/> when the token is not revoked and not expired.
    /// </summary>
    [Fact]
    public void IsActive_ReturnsTrueOnNonRevokedAndNonExpired()
    {
        // Arrange
        var currentTime = UnixEpoch.AddMinutes(1);

        var entry = new RefreshTokenCacheEntry(currentTime.AddMinutes(1).Ticks, false);

        // Act
        var result = entry.IsActive(currentTime);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.IsActive"/> method should
    /// return <see langword="false"/> when the token is revoked and not expired.
    /// </summary>
    [Fact]
    public void IsActive_ReturnsFalseOnRevokedAndNonExpired()
    {
        // Arrange
        var currentTime = UnixEpoch.AddMinutes(1);

        var entry = new RefreshTokenCacheEntry(currentTime.AddMinutes(1).Ticks, true);

        // Act
        var result = entry.IsActive(currentTime);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.IsActive"/> method should
    /// return <see langword="false"/> when the token is not revoked and expired.
    /// </summary>
    [Fact]
    public void IsActive_ReturnsFalseOnNonRevokedAndExpired()
    {
        // Arrange
        var currentTime = UnixEpoch.AddMinutes(1);

        var entry = new RefreshTokenCacheEntry(currentTime.AddMinutes(-1).Ticks, false);

        // Act
        var result = entry.IsActive(currentTime);

        // Assert
        Assert.False(result);
    }
    #endregion
}