namespace Paradise.WebApi.Infrastructure.Authentication.Caching;

/// <summary>
/// Provides refresh token caching functionalities.
/// </summary>
public interface IRefreshTokenCache
{
    #region Methods
    /// <summary>
    /// Retrieves a cached <see cref="RefreshTokenCacheEntry"/> by its identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the refresh token entry.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="RefreshTokenCacheEntry"/> if found,
    /// otherwise - <see langword="null"/>.
    /// </returns>
    Task<RefreshTokenCacheEntry?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a <see cref="RefreshTokenCacheEntry"/> in the cache with the specified expiration time.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the refresh token entry.
    /// </param>
    /// <param name="entry">
    /// The refresh token entry to cache.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task SetAsync(Guid id, RefreshTokenCacheEntry entry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a refresh token entry in the cache as revoked.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the refresh token entry to revoke.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task RevokeAsync(Guid id, CancellationToken cancellationToken = default);
    #endregion
}