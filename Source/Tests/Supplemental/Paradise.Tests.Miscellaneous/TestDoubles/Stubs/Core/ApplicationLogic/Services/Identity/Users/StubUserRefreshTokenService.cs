using Paradise.ApplicationLogic.Services.Identity.Users;
using Paradise.Models;
using Paradise.Models.Domain.Identity.Users;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Core.ApplicationLogic.Services.Identity.Users;

/// <summary>
/// Stub <see cref="IUserRefreshTokenService"/> implementation.
/// </summary>
internal class StubUserRefreshTokenService : IUserRefreshTokenService
{
    #region Properties
    /// <summary>
    /// <see cref="GetByIdAsync"/> result.
    /// </summary>
    public Func<Task<Result<UserRefreshTokenModel>>>? GetByIdAsyncResult { get; set; }

    /// <summary>
    /// <see cref="CreateAsync"/> result.
    /// </summary>
    public Func<Task<Result<UserRefreshTokenModel>>>? CreateAsyncResult { get; set; }

    /// <summary>
    /// <see cref="DeactivateAsync"/> result.
    /// </summary>
    public Func<Task<Result<UserRefreshTokenModel>>>? DeactivateAsyncResult { get; set; }

    /// <summary>
    /// <see cref="DeactivateAllAsync"/> result.
    /// </summary>
    public Func<Task<Result<IEnumerable<UserRefreshTokenModel>>>>? DeactivateAllAsyncResult { get; set; }

    /// <summary>
    /// <see cref="DeleteExpiredAsync"/> result.
    /// </summary>
    public Func<Task<Result<int>>>? DeleteExpiredAsyncResult { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task<Result<UserRefreshTokenModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => GetByIdAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<UserRefreshTokenModel>> CreateAsync(Guid userId, TimeSpan lifetime, CancellationToken cancellationToken = default)
        => CreateAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<UserRefreshTokenModel>> DeactivateAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
        => DeactivateAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<IEnumerable<UserRefreshTokenModel>>> DeactivateAllAsync(Guid userId, CancellationToken cancellationToken = default)
        => DeactivateAllAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<int>> DeleteExpiredAsync(CancellationToken cancellationToken = default)
        => DeleteExpiredAsyncResult!();
    #endregion
}