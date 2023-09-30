using Paradise.Common.Extensions;
using System.Security.Claims;

namespace Paradise.ApplicationLogic.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="ClaimsPrincipal"/> class.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    #region Public methods
    /// <summary>
    /// Gets the first claim with the given <paramref name="type"/> contained
    /// in the input <paramref name="principal"/>
    /// and attempts to parse it as a <see cref="Guid"/>.
    /// </summary>
    /// <param name="principal">
    /// The input <see cref="ClaimsPrincipal"/> to get the claim from.
    /// </param>
    /// <param name="type">
    /// Claim type.
    /// </param>
    /// <returns>
    /// <see cref="Guid"/> value from the input <paramref name="principal"/> object.
    /// </returns>
    public static Guid GetGuidClaim(this ClaimsPrincipal? principal, string type)
    {
        if (principal is null)
            return Guid.Empty;

        var guid = principal.FindFirstValue(type);

        return guid.IsNotNullOrWhiteSpace() ? Guid.Parse(guid) : Guid.Empty;
    }

    /// <summary>
    /// Gets the list of claims with the given <paramref name="type"/>
    /// from the input <paramref name="principal"/>.
    /// </summary>
    /// <param name="principal">
    /// The input <see cref="ClaimsPrincipal"/> to get claims from.
    /// </param>
    /// <param name="type">
    /// The type of claims to be returned.
    /// </param>
    /// <returns>
    /// The list of claims with the given <paramref name="type"/>.
    /// </returns>
    public static IEnumerable<string> FindValues(this ClaimsPrincipal principal, string type)
    {
        ArgumentNullException.ThrowIfNull(principal);

        return principal.FindAll(type).Select(claim => claim.Value);
    }
    #endregion
}