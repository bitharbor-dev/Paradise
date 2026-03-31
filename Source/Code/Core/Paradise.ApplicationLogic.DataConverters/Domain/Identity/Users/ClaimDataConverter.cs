using Paradise.Models.Domain.Identity.Users;
using System.Security.Claims;

namespace Paradise.ApplicationLogic.DataConverters.Domain.Identity.Users;

/// <summary>
/// Contains extension methods for <see cref="Claim"/> conversion operations.
/// </summary>
public static class ClaimDataConverter
{
    #region Public methods
    /// <summary>
    /// Converts the <see cref="Claim"/> into the <see cref="UserClaimModel"/>.
    /// </summary>
    /// <param name="claim">
    /// The input <see cref="Claim"/> to be converted.
    /// </param>
    /// <returns>
    /// A new <see cref="UserClaimModel"/> instance
    /// converted from the input <paramref name="claim"/>.
    /// </returns>
    public static UserClaimModel ToModel(this Claim claim)
    {
        ArgumentNullException.ThrowIfNull(claim);

        var type = claim.Type;
        var value = claim.Value;

        return new(type, value);
    }
    #endregion
}