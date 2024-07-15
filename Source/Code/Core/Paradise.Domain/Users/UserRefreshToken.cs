using Paradise.Domain.Base;
using Paradise.Domain.Base.Exceptions;

namespace Paradise.Domain.Users;

/// <summary>
/// Represents the user's refresh token.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserRefreshToken"/> class.
/// </remarks>
/// <param name="ownerId">
/// Refresh token owner's Id.
/// </param>
public sealed class UserRefreshToken(Guid ownerId) : ValueObject
{
    #region Properties
    /// <summary>
    /// Refresh token owner's Id.
    /// </summary>
    public Guid OwnerId { get; } = ownerId;

    /// <summary>
    /// Refresh token owner.
    /// </summary>
    public User? Owner { get; private set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override void ValidateState()
    {
        base.ValidateState();

        if (OwnerId == Guid.Empty)
            InvalidEntityStateException.Throw<UserRefreshToken>(OwnerId);
    }

    /// <summary>
    /// Gets the <see cref="bool"/> value
    /// indicating whether the current refresh token
    /// is outdated.
    /// </summary>
    /// <param name="lifetime">
    /// Refresh token lifetime.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if refresh token is outdated,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public bool IsOutdated(TimeSpan lifetime)
        => Created.Add(lifetime) < DateTime.UtcNow;

    /// <inheritdoc/>
    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Id;
        yield return Created;
        yield return OwnerId;
    }
    #endregion
}