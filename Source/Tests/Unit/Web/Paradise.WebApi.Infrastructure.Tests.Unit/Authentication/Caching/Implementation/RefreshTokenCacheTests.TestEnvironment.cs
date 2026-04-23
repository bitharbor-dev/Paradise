using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Time.Testing;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.Extensions.Caching.Distributed;
using Paradise.WebApi.Infrastructure.Authentication.Caching;
using Paradise.WebApi.Infrastructure.Authentication.Caching.Implementation;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Authentication.Caching.Implementation;

public sealed partial class RefreshTokenCacheTests
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();

    /// <summary>
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </summary>
    public CancellationToken Token { get; } = TestContext.Current.CancellationToken;
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="RefreshTokenCacheTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Fields
        private static readonly CompositeFormat _entryKeyFormat = CompositeFormat.Parse("rt:{0:N}");

        private readonly FakeTimeProvider _timeProvider;
        private readonly FakeDistributedCache _distributedCache;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _timeProvider = new();
            _distributedCache = new(_timeProvider);

            Target = new(_timeProvider, _distributedCache);
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public RefreshTokenCache Target { get; }

        /// <summary>
        /// Gets or sets the current UTC time.
        /// </summary>
        public DateTimeOffset UtcNow
        {
            get => _timeProvider.GetUtcNow();
            set => _timeProvider.SetUtcNow(value);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates and stores a <see cref="RefreshTokenCacheEntry"/> in the
        /// <see cref="IDistributedCache"/> for the specified token identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the refresh token entry.
        /// </param>
        /// <param name="expiryDate">
        /// The UTC date and time when the refresh token should expire.
        /// </param>
        /// <param name="isRevoked">
        /// Indicates whether the refresh token should be marked as revoked.
        /// </param>
        /// <returns>
        /// The <see cref="RefreshTokenCacheEntry"/> that was created and stored.
        /// </returns>
        public RefreshTokenCacheEntry SetEntry(Guid id, DateTimeOffset expiryDate, bool isRevoked)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = _timeProvider.GetUtcNow().AddMinutes(RefreshTokenCacheEntry.EntryLifetime)
            };

            var key = GetEntryKey(id);

            var entry = new RefreshTokenCacheEntry(expiryDate.Ticks, isRevoked);

            _distributedCache.Set(key, entry.AsSpan().ToArray(), options);

            return entry;
        }

        /// <summary>
        /// Attempts to retrieve a <see cref="RefreshTokenCacheEntry"/> from the
        /// distributed cache for the specified identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the refresh token entry.
        /// </param>
        /// <param name="entry">
        /// The <see cref="RefreshTokenCacheEntry"/> if found,
        /// otherwise - <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a cache entry exists for the specified identifier,
        /// otherwise - <see langword="false"/>.
        /// </returns>
        public bool TryGetEntry(Guid id, [NotNullWhen(true)] out RefreshTokenCacheEntry? entry)
        {
            var key = GetEntryKey(id);

            var data = _distributedCache.Get(key);

            entry = data is null ? null : new(data);

            return entry is not null;
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
        #endregion
    }
    #endregion
}