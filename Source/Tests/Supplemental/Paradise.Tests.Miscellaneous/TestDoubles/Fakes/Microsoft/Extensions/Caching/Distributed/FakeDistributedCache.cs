using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.Extensions.Caching.Distributed;

/// <summary>
/// Fake <see cref="IDistributedCache"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeDistributedCache"/> class.
/// </remarks>
/// <param name="timeProvider">
/// Time provider used for expiration checks.
/// </param>
public sealed class FakeDistributedCache(TimeProvider timeProvider) : IDistributedCache
{
    #region Fields
    private readonly ConcurrentDictionary<string, Entry> _entries = [];
    #endregion

    #region Public methods
    /// <inheritdoc />
    public byte[]? Get(string key)
        => GetInternal(key);

    /// <inheritdoc />
    public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        return Task.FromResult(GetInternal(key));
    }

    /// <inheritdoc />
    public void Refresh(string key)
        => RefreshInternal(key);

    /// <inheritdoc />
    public Task RefreshAsync(string key, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        RefreshInternal(key);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Remove(string key)
        => RemoveInternal(key);

    /// <inheritdoc />
    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        RemoveInternal(key);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        => SetInternal(key, value, options);

    /// <inheritdoc />
    public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        SetInternal(key, value, options);

        return Task.CompletedTask;
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Retrieves a cache entry by key and updates its sliding expiration if applicable.
    /// </summary>
    /// <param name="key">
    /// Cache entry key.
    /// </param>
    /// <returns>
    /// Cached value if present and not expired; otherwise <see langword="null"/>.
    /// </returns>
    private byte[]? GetInternal(string? key)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (!TryGetEntry(key, out var entry))
            return null;

        entry.Touch(timeProvider.GetUtcNow());

        return entry.Value;
    }

    /// <summary>
    /// Refreshes the sliding expiration of an existing cache entry.
    /// </summary>
    /// <param name="key">
    /// Cache entry key.
    /// </param>
    private void RefreshInternal(string? key)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (TryGetEntry(key, out var entry))
            entry.Touch(timeProvider.GetUtcNow());
    }

    /// <summary>
    /// Removes a cache entry if it exists.
    /// </summary>
    /// <param name="key">
    /// Cache entry key.
    /// </param>
    private void RemoveInternal(string? key)
    {
        ArgumentNullException.ThrowIfNull(key);

        _entries.TryRemove(key, out _);
    }

    /// <summary>
    /// Stores a cache entry using the specified options.
    /// </summary>
    /// <param name="key">
    /// Cache entry key.
    /// </param>
    /// <param name="value">
    /// Value to cache.
    /// </param>
    /// <param name="options">
    /// Cache entry configuration options.
    /// </param>
    private void SetInternal(string? key, byte[]? value, DistributedCacheEntryOptions? options)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(options);

        var created = timeProvider.GetUtcNow();
        var absoluteExpiration = GetAbsoluteExpiration(options, created);

        _entries[key] = new(value, absoluteExpiration, options.SlidingExpiration, created);
    }

    /// <summary>
    /// Resolves the absolute expiration timestamp based on cache entry options.
    /// </summary>
    /// <param name="options">
    /// Cache entry configuration options.
    /// </param>
    /// <param name="created">
    /// Cache entry creation timestamp.
    /// </param>
    /// <returns>
    /// Absolute expiration timestamp if configured; otherwise <see langword="null"/>.
    /// </returns>
    [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Omitted for readability.")]
    private static DateTimeOffset? GetAbsoluteExpiration(DistributedCacheEntryOptions options, DateTimeOffset created)
    {
        if (options.AbsoluteExpiration.HasValue)
            return options.AbsoluteExpiration.Value;

        if (options.AbsoluteExpirationRelativeToNow.HasValue)
            return created.Add(options.AbsoluteExpirationRelativeToNow.Value);

        return null;
    }

    /// <summary>
    /// Attempts to retrieve a non-expired cache entry.
    /// </summary>
    /// <param name="key">
    /// Cache entry key.
    /// </param>
    /// <param name="entry">
    /// Retrieved entry if found and not expired.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the entry exists and is valid; otherwise <see langword="false"/>.
    /// </returns>
    private bool TryGetEntry(string key, [NotNullWhen(true)] out Entry? entry)
    {
        if (!_entries.TryGetValue(key, out entry))
            return false;

        if (entry.IsExpired(timeProvider.GetUtcNow()))
        {
            _entries.TryRemove(key, out _);

            return false;
        }

        return true;
    }
    #endregion

    #region Nested types
    /// <summary>
    /// Represents a cached value and its expiration metadata.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="Entry"/> class.
    /// </remarks>
    /// <param name="value">
    /// Cached value.
    /// </param>
    /// <param name="absoluteExpiration">
    /// An absolute expiration date for the cache entry.
    /// </param>
    /// <param name="slidingExpiration">
    /// Indicates how long a cache entry can be inactive before it will be removed.
    /// </param>
    /// <param name="created">
    /// Cache entry creation time.
    /// </param>
    private sealed class Entry(byte[] value, DateTimeOffset? absoluteExpiration, TimeSpan? slidingExpiration, DateTimeOffset created)
    {
        #region Fields
        private DateTimeOffset _lastAccessUtc = created;
        #endregion

        #region Properties
        /// <summary>
        /// Cached value.
        /// </summary>
        public byte[] Value { get; } = value;
        #endregion

        #region Public methods
        /// <summary>
        /// Determines whether the entry is expired at the specified time.
        /// </summary>
        /// <param name="currentTime">
        /// Current UTC time.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the entry is expired,
        /// otherwise - <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Omitted for readability.")]
        public bool IsExpired(DateTimeOffset currentTime)
        {
            if (absoluteExpiration.HasValue && currentTime >= absoluteExpiration.Value)
                return true;

            if (slidingExpiration.HasValue && currentTime - _lastAccessUtc >= slidingExpiration.Value)
                return true;

            return false;
        }

        /// <summary>
        /// Updates the last access timestamp for sliding expiration tracking.
        /// </summary>
        /// <param name="currentTime">
        /// Current UTC time.
        /// </param>
        public void Touch(DateTimeOffset currentTime)
        {
            if (slidingExpiration.HasValue)
                _lastAccessUtc = currentTime;
        }
        #endregion
    }
    #endregion
}