using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Paradise.WebApi.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="HttpContext"/> <see langword="class"/>.
/// </summary>
internal static class HttpContextExtensions
{
    #region Public methods
    /// <summary>
    /// Gets the currently authenticated user's Id.
    /// </summary>
    /// <param name="context">
    /// HTTP context.
    /// </param>
    /// <returns>
    /// A <see cref="Guid"/> value representing the currently
    /// authenticated user's Id.
    /// </returns>
    public static Guid GetUserId(this HttpContext context)
    {
        var identityOptions = context.RequestServices.GetRequiredService<IOptions<IdentityOptions>>();

        var idClaimType = identityOptions.Value.ClaimsIdentity.UserIdClaimType;

        return context.User.GetGuidClaim(idClaimType);
    }
    #endregion
}