using Paradise.WebApi.Infrastructure.Authentication.Caching;
using System.Diagnostics.CodeAnalysis;

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

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.Equals(RefreshTokenCacheEntry)"/> method should
    /// return <see langword="true"/> if both of the values compared
    /// have the same expiry date and revocation status.
    /// </summary>
    [Fact]
    public void Equals_ReturnsTrueOnEqualExpiryAndRevocation()
    {
        // Arrange
        var left = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);
        var right = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.Equals(RefreshTokenCacheEntry)"/> method should
    /// return <see langword="false"/> if both of the values compared
    /// have different expiry date.
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseOnNonEqualExpiry()
    {
        // Arrange
        var left = new RefreshTokenCacheEntry(UnixEpoch.Ticks + 1, false);
        var right = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.Equals(RefreshTokenCacheEntry)"/> method should
    /// return <see langword="false"/> if both of the values compared
    /// have different revocation status.
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseOnNonEqualRevocation()
    {
        // Arrange
        var left = new RefreshTokenCacheEntry(UnixEpoch.Ticks, true);
        var right = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.Equals(object?)"/> method should
    /// return <see langword="true"/> if both of the values compared
    /// have the same expiry date and revocation status.
    /// </summary>
    [Fact]
    public void Equals_Overload_ReturnsTrueOnEqualExpiryAndRevocation()
    {
        // Arrange
        var left = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);
        var right = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false) as object;

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.Equals(object?)"/> method should
    /// return <see langword="false"/> if one of the values being compared
    /// is <see langword="null"/>.
    /// </summary>
    [Fact, SuppressMessage("Maintainability", "CA1508:Avoid dead conditional code", Justification = "Test scenario.")]
    public void Equals_Overload_ReturnsFalseOnNull()
    {
        // Arrange
        var left = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);
        var right = null as object;

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.Equals(object?)"/> method should
    /// return <see langword="false"/> if both of the values being compared
    /// have different types.
    /// </summary>
    [Fact]
    public void Equals_Overload_ReturnsFalseOnNonEqualType()
    {
        // Arrange
        var left = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);
        var right = new object();

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.operator =="/> operator should
    /// return <see langword="true"/> if both of the values compared
    /// have the same expiry date and revocation status.
    /// </summary>
    [Fact]
    public void OperatorEquals_ReturnsTrueOnEqualExpiryAndRevocation()
    {
        // Arrange
        var left = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);
        var right = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);

        // Act
        var result = left == right;

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.operator =="/> operator should
    /// return <see langword="false"/> if both of the values compared
    /// have different expiry date.
    /// </summary>
    [Fact]
    public void OperatorEquals_ReturnsFalseOnNonEqualExpiry()
    {
        // Arrange
        var left = new RefreshTokenCacheEntry(UnixEpoch.Ticks + 1, false);
        var right = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);

        // Act
        var result = left == right;

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.operator =="/> operator should
    /// return <see langword="false"/> if both of the values compared
    /// have different revocation status.
    /// </summary>
    [Fact]
    public void OperatorEquals_ReturnsFalseOnNonEqualRevocation()
    {
        // Arrange
        var left = new RefreshTokenCacheEntry(UnixEpoch.Ticks, true);
        var right = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);

        // Act
        var result = left == right;

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.operator !="/> operator should
    /// return <see langword="false"/> if both of the values compared
    /// have the same expiry date and revocation status.
    /// </summary>
    [Fact]
    public void OperatorNotEquals_ReturnsFalseOnEqualExpiryAndRevocation()
    {
        // Arrange
        var left = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);
        var right = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);

        // Act
        var result = left != right;

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.operator !="/> operator should
    /// return <see langword="true"/> if both of the values compared
    /// have different expiry date.
    /// </summary>
    [Fact]
    public void OperatorNotEquals_ReturnsTrueOnNonEqualExpiry()
    {
        // Arrange
        var left = new RefreshTokenCacheEntry(UnixEpoch.Ticks + 1, false);
        var right = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);

        // Act
        var result = left != right;

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.operator !="/> operator should
    /// return <see langword="true"/> if both of the values compared
    /// have different revocation status.
    /// </summary>
    [Fact]
    public void OperatorNotEquals_ReturnsTrueOnNonEqualRevocation()
    {
        // Arrange
        var left = new RefreshTokenCacheEntry(UnixEpoch.Ticks, true);
        var right = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);

        // Act
        var result = left != right;

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.GetHashCode"/> method should
    /// return the same values if both of the values being compared
    /// have the same expiry date and revocation status.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsEqualValuesOnEqualExpiryAndRevocation()
    {
        // Arrange
        var left = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);
        var right = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);

        // Act
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // Assert
        Assert.Equal(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.GetHashCode"/> method should
    /// return different values if both of the values being compared
    /// have different expiry date.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsNonEqualValuesOnDifferentExpiry()
    {
        // Arrange
        var left = new RefreshTokenCacheEntry(UnixEpoch.Ticks + 1, false);
        var right = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);

        // Act
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // Assert
        Assert.NotEqual(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="RefreshTokenCacheEntry.GetHashCode"/> method should
    /// return different values if both of the values being compared
    /// have different revocation status.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsNonEqualValuesOnDifferentRevocation()
    {
        // Arrange
        var left = new RefreshTokenCacheEntry(UnixEpoch.Ticks, true);
        var right = new RefreshTokenCacheEntry(UnixEpoch.Ticks, false);

        // Act
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // Assert
        Assert.NotEqual(leftHash, rightHash);
    }
    #endregion
}