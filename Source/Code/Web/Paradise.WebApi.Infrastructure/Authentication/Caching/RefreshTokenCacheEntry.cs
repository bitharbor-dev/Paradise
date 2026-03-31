namespace Paradise.WebApi.Infrastructure.Authentication.Caching;

/// <summary>
/// Represents a cached refresh token entry used during
/// fast refresh token lookups in authentication scenarios.
/// </summary>
public readonly struct RefreshTokenCacheEntry : IEquatable<RefreshTokenCacheEntry>
{
    #region Constants
    /// <summary>
    /// Defines the size of a single <see cref="RefreshTokenCacheEntry"/> in bytes.
    /// </summary>
    public const ushort EntryBytesSize = 9;

    /// <summary>
    /// Defines the lifetime (minutes) of a single entry within the cache.
    /// </summary>
    public const double EntryLifetime = 5;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenCacheEntry"/> class.
    /// </summary>
    /// <param name="data">
    /// The <see cref="ReadOnlySpan{T}"/> of <see langword="byte"/>
    /// to be converted into a <see cref="RefreshTokenCacheEntry"/>.
    /// </param>
    public RefreshTokenCacheEntry(ReadOnlySpan<byte> data)
    {
        ExpirationTimestamp = BitConverter.ToInt64(data[..8]);
        IsRevoked = data[8] is 1;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenCacheEntry"/> class.
    /// </summary>
    /// <param name="expirationTimestamp">
    /// Refresh token expiration timestamp in ticks.
    /// </param>
    /// <param name="isRevoked">
    /// Indicates whether the refresh token was
    /// revoked by the user.
    /// </param>
    public RefreshTokenCacheEntry(long expirationTimestamp, bool isRevoked)
    {
        ExpirationTimestamp = expirationTimestamp;
        IsRevoked = isRevoked;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Refresh token expiration timestamp in ticks.
    /// <para>
    /// This value indicating the token expiration itself,
    /// not it's cached representation.
    /// </para>
    /// </summary>
    public long ExpirationTimestamp { get; }

    /// <summary>
    /// Indicates whether the refresh token was
    /// revoked by the user.
    /// </summary>
    public bool IsRevoked { get; }
    #endregion

    #region Public methods
    /// <summary>
    /// Gets the <see langword="bool"/> value indicating whether the
    /// refresh token represented by the current entry is expired or revoked.
    /// </summary>
    /// <param name="currentTime">
    /// Current time.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the refresh token represented by the current entry
    /// neither expired, nor revoked, otherwise - <see langword="false"/>.
    /// </returns>
    public bool IsActive(DateTimeOffset currentTime)
        => !IsRevoked && ExpirationTimestamp > currentTime.Ticks;

    /// <summary>
    /// Converts the current entry into its binary representation
    /// as a <see cref="ReadOnlySpan{T}"/> of <see langword="byte"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="ReadOnlySpan{T}"/> containing the binary representation
    /// of the current entry.
    /// </returns>
    public ReadOnlySpan<byte> AsSpan()
    {
        var result = new byte[EntryBytesSize].AsSpan();

        BitConverter.TryWriteBytes(result[..8], ExpirationTimestamp);
        result[8] = IsRevoked ? (byte)1 : (byte)0;

        return result;
    }

    /// <inheritdoc/>
    public bool Equals(RefreshTokenCacheEntry other)
        => other.IsRevoked == IsRevoked
        && other.ExpirationTimestamp == ExpirationTimestamp;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is RefreshTokenCacheEntry entry && Equals(entry);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(IsRevoked, ExpirationTimestamp);
    #endregion

    #region Operators
    /// <summary>
    /// Compares the given <paramref name="left"/> and <paramref name="right"/>
    /// objects for equality.
    /// </summary>
    /// <param name="left">
    /// The first <see cref="RefreshTokenCacheEntry"/> to be compared.
    /// </param>
    /// <param name="right">
    /// The second <see cref="RefreshTokenCacheEntry"/> to be compared.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> equals <paramref name="right"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool operator ==(RefreshTokenCacheEntry left, RefreshTokenCacheEntry right)
        => left.Equals(right);

    /// <summary>
    /// Compares the given <paramref name="left"/> and <paramref name="right"/>
    /// objects for inequality.
    /// </summary>
    /// <param name="left">
    /// The first <see cref="RefreshTokenCacheEntry"/> to be compared.
    /// </param>
    /// <param name="right">
    /// The second <see cref="RefreshTokenCacheEntry"/> to be compared.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> does not equal <paramref name="right"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool operator !=(RefreshTokenCacheEntry left, RefreshTokenCacheEntry right)
        => !(left == right);
    #endregion
}