using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.Identity.Users;

/// <summary>
/// Represents a user's refresh token.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserRefreshTokenModel"/> class.
/// </remarks>
/// <param name="id">
/// Unique identifier.
/// </param>
/// <param name="ownerId">
/// Refresh token owner's Id.
/// </param>
/// <param name="created">
/// Creation date.
/// </param>
/// <param name="isActive">
/// Indicates whether the refresh token is active (was not revoked)
/// and can be used during authentication processes.
/// </param>
/// <param name="expiryDateUtc">
/// Refresh token expiry date.
/// </param>
[method: JsonConstructor]
public sealed class UserRefreshTokenModel(Guid id, Guid ownerId, DateTimeOffset created, bool isActive, DateTimeOffset expiryDateUtc)
{
    #region Properties
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public Guid Id { get; } = id;

    /// <summary>
    /// Refresh token owner's Id.
    /// </summary>
    public Guid OwnerId { get; } = ownerId;

    /// <summary>
    /// Creation date.
    /// </summary>
    public DateTimeOffset Created { get; } = created;

    /// <summary>
    /// Indicates whether the refresh token is active (was not revoked)
    /// and can be used during authentication processes.
    /// </summary>
    public bool IsActive { get; } = isActive;

    /// <summary>
    /// Refresh token expiry date.
    /// </summary>
    public DateTimeOffset ExpiryDateUtc { get; } = expiryDateUtc;
    #endregion

    #region Public methods
    /// <summary>
    /// Gets the <see cref="bool"/> value
    /// indicating whether the current refresh token
    /// is expired.
    /// </summary>
    /// <param name="currentTime">
    /// Current time.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if refresh token is expired,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public bool IsExpired(DateTimeOffset currentTime)
        => ExpiryDateUtc < currentTime;
    #endregion
}