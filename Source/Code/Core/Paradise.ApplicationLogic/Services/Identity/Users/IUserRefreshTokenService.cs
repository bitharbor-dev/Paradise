using Paradise.Models;
using Paradise.Models.Domain.Identity.Users;

namespace Paradise.ApplicationLogic.Services.Identity.Users;

/// <summary>
/// Provides users' refresh tokens management functionalities.
/// </summary>
public interface IUserRefreshTokenService
{
    #region Methods
    /// <summary>
    /// Gets the refresh token with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    /// The Id of the refresh token to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserRefreshTokenModel"/>
    /// containing information about the refresh token found.
    /// </returns>
    Task<Result<UserRefreshTokenModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new refresh token for the user with the given <paramref name="userId"/>.
    /// </summary>
    /// <param name="userId">
    /// The Id of the user for whom to create a new refresh token.
    /// </param>
    /// <param name="lifetime">
    /// Refresh token lifetime.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserRefreshTokenModel"/>
    /// containing information about the refresh token created.
    /// </returns>
    Task<Result<UserRefreshTokenModel>> CreateAsync(Guid userId, TimeSpan lifetime,
                                                    CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates the refresh token with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="userId">
    /// The Id of the user who owns the refresh token.
    /// </param>
    /// <param name="id">
    /// The Id of the refresh token deactivate.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserRefreshTokenModel"/>
    /// containing information about the refresh token deactivated.
    /// </returns>
    Task<Result<UserRefreshTokenModel>> DeactivateAsync(Guid userId, Guid id,
                                                        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates all refresh tokens which belong to the user with the given <paramref name="userId"/>.
    /// </summary>
    /// <param name="userId">
    /// The Id of the user whose refresh tokens to be deactivated.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="UserRefreshTokenModel"/>
    /// containing information about the refresh tokens, which belong
    /// to the user with the given <paramref name="userId"/> and were deactivated.
    /// </returns>
    Task<Result<IEnumerable<UserRefreshTokenModel>>> DeactivateAllAsync(Guid userId,
                                                                        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes expired refresh tokens, effectively ending the expired sessions.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="int"/>
    /// representing the number of deleted refresh tokens.
    /// </returns>
    Task<Result<int>> DeleteExpiredAsync(CancellationToken cancellationToken = default);
    #endregion
}