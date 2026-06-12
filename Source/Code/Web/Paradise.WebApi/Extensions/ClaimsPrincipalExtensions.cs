using Paradise.Primitives.Extensions;
using System.Security.Claims;

namespace Paradise.WebApi.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="ClaimsPrincipal"/> <see langword="class"/>.
/// </summary>
internal static class ClaimsPrincipalExtensions
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
    public static Guid GetGuidClaim(this ClaimsPrincipal principal, string type)
    {
        ArgumentNullException.ThrowIfNull(principal);

        var guid = principal.FindFirstValue(type);

        return guid.IsNotNullOrWhiteSpace() ? Guid.Parse(guid) : Guid.Empty;
    }
    #endregion
}