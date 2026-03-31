using Paradise.Domain.Identity.Users;
using Paradise.Models.Domain.Identity.Users;

namespace Paradise.ApplicationLogic.DataConverters.Domain.Identity.Users;

/// <summary>
/// Contains extension methods for <see cref="UserRefreshToken"/> conversion operations.
/// </summary>
public static class UserRefreshTokenDataConverter
{
    #region Public methods
    /// <summary>
    /// Converts the <see cref="UserRefreshToken"/> into the <see cref="UserRefreshTokenModel"/>.
    /// </summary>
    /// <param name="refreshToken">
    /// The input <see cref="UserRefreshToken"/> to be converted.
    /// </param>
    /// <returns>
    /// A new <see cref="UserRefreshTokenModel"/> instance
    /// converted from the input <paramref name="refreshToken"/>.
    /// </returns>
    public static UserRefreshTokenModel ToModel(this UserRefreshToken refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        var id = refreshToken.Id;
        var ownerId = refreshToken.OwnerId;
        var created = refreshToken.Created;
        var isActive = refreshToken.IsActive;
        var expiryDateUtc = refreshToken.ExpiryDateUtc;

        return new(id, ownerId, created, isActive, expiryDateUtc);
    }
    #endregion
}