using Microsoft.Extensions.Caching.Distributed;
using System.Globalization;
using System.Text;

namespace Paradise.WebApi.Infrastructure.Authentication.Caching.Implementation;

/// <summary>
/// Provides refresh token caching functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RefreshTokenCache"/> class.
/// </remarks>
/// <param name="timeProvider">
/// Time provider.
/// </param>
/// <param name="cache">
/// The <see cref="IDistributedCache"/> instance used to
/// access the backing cache.
/// </param>
internal sealed class RefreshTokenCache(TimeProvider timeProvider,
                                        IDistributedCache cache) : IRefreshTokenCache
{
    #region Fields
    private static readonly CompositeFormat _entryKeyFormat = CompositeFormat.Parse("rt:{0:N}");
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async Task<RefreshTokenCacheEntry?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var key = GetEntryKey(id);

        var data = await cache.GetAsync(key, cancellationToken)
            .ConfigureAwait(false);

        return data is null || data.Length is not RefreshTokenCacheEntry.EntryBytesSize
            ? null
            : new(data);
    }

    /// <inheritdoc/>
    public Task SetAsync(Guid id, RefreshTokenCacheEntry entry, CancellationToken cancellationToken = default)
    {
        var options = GetOptions(timeProvider.GetUtcNow());

        var key = GetEntryKey(id);

        return cache.SetAsync(key, entry.AsSpan().ToArray(), options, cancellationToken);
    }

    /// <inheritdoc/>
    public Task RevokeAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var options = GetOptions(timeProvider.GetUtcNow());

        var key = GetEntryKey(id);

        var entry = new RefreshTokenCacheEntry(default, true);

        return cache.SetAsync(key, entry.AsSpan().ToArray(), options, cancellationToken);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Builds a cache entry key string for the specified identifier
    /// using the predefined composite format and invariant culture.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the refresh token entry.
    /// </param>
    /// <returns>
    /// A string representing the formatted cache key for the specified identifier.
    /// </returns>
    private static string GetEntryKey(Guid id)
        => string.Format(CultureInfo.InvariantCulture, _entryKeyFormat, id);

    /// <summary>
    /// Builds <see cref="DistributedCacheEntryOptions"/> for a cache entry
    /// using the specified UTC timestamp as the base for absolute expiration.
    /// </summary>
    /// <param name="utcNow">
    /// Current UTC time used as the reference point for absolute expiration.
    /// </param>
    /// <returns>
    /// A <see cref="DistributedCacheEntryOptions"/> instance with
    /// <see cref="DistributedCacheEntryOptions.AbsoluteExpiration"/>
    /// set to <paramref name="utcNow"/> plus
    /// <see cref="RefreshTokenCacheEntry.EntryLifetime"/> minutes.
    /// </returns>
    private static DistributedCacheEntryOptions GetOptions(DateTimeOffset utcNow) => new()
    {
        AbsoluteExpiration = utcNow.AddMinutes(RefreshTokenCacheEntry.EntryLifetime)
    };
    #endregion
}