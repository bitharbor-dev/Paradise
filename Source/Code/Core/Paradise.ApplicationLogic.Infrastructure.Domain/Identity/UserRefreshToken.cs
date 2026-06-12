using Paradise.Domain.Base;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.ApplicationLogic.Infrastructure.Domain.Identity;

/// <summary>
/// Represents the user's refresh token.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserRefreshToken"/> class.
/// </remarks>
/// <param name="ownerId">
/// Refresh token owner's Id.
/// </param>
/// <param name="expiryDateUtc">
/// Refresh token expiry date.
/// </param>
public sealed class UserRefreshToken(Guid ownerId, DateTimeOffset expiryDateUtc) : ValueObject
{
    #region Constants
    private const string AutoPropertyJustificationMessage = "Field is used by EF, so the property can remain read-only.";
    #endregion

    #region Fields
    [SuppressMessage("Style", "IDE0032:Use auto property", Justification = AutoPropertyJustificationMessage)]
    private readonly Guid _ownerId = ownerId;

    [SuppressMessage("Style", "IDE0032:Use auto property", Justification = AutoPropertyJustificationMessage)]
    private readonly DateTimeOffset _expiryDateUtc = expiryDateUtc;
    #endregion

    #region Properties
    /// <summary>
    /// Refresh token owner's Id.
    /// </summary>
    [SuppressMessage("Style", "IDE0032:Use auto property", Justification = AutoPropertyJustificationMessage)]
    public Guid OwnerId
        => _ownerId;

    /// <summary>
    /// Indicates whether the refresh token is active (was not revoked)
    /// and can be used during authentication processes.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Refresh token expiry date.
    /// </summary>
    [SuppressMessage("Style", "IDE0032:Use auto property", Justification = AutoPropertyJustificationMessage)]
    public DateTimeOffset ExpiryDateUtc
        => _expiryDateUtc;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override void ValidateState()
    {
        base.ValidateState();

        if (OwnerId == Guid.Empty)
            throw new InvalidOperationException(new DomainStateError(GetType(), OwnerId));
    }

    /// <inheritdoc/>
    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Id;
        yield return Created;
        yield return OwnerId;
    }
    #endregion
}