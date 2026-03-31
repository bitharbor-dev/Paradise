using Paradise.WebApi.Infrastructure.Authentication.Caching;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Web.WebApi.Infrastructure.Authentication.Caching;

/// <summary>
/// Stub <see cref="IRefreshTokenCache"/> implementation.
/// </summary>
public sealed class StubRefreshTokenCache : IRefreshTokenCache
{
    #region Public methods
    /// <summary>
    /// <see cref="GetAsync"/> result.
    /// </summary>
    public Func<Task<RefreshTokenCacheEntry?>>? GetAsyncResult { get; set; }

    /// <summary>
    /// <see cref="RevokeAsync"/> result.
    /// </summary>
    public Func<Task>? RevokeAsyncResult { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task<RefreshTokenCacheEntry?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => GetAsyncResult!();

    /// <inheritdoc/>
    public Task RevokeAsync(Guid id, CancellationToken cancellationToken = default)
        => RevokeAsyncResult!();

    /// <inheritdoc/>
    public Task SetAsync(Guid id, RefreshTokenCacheEntry entry, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
    #endregion
}