using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Paradise.WebApi.Extensions;
using Paradise.WebApi.Infrastructure.Filters.Validation;
using System.Net.Mime;

namespace Paradise.WebApi.Controllers.Base;

/// <summary>
/// Base API controller class.
/// </summary>
[ApiController, RequireHttps, ValidateModel]
[Produces(MediaTypeNames.Application.Json)]
public abstract class ApiControllerBase : ControllerBase
{
    #region Constants
    /// <summary>
    /// Request authorization header name.
    /// </summary>
    public const string AuthorizationHeaderName = "Authorization";
    #endregion

    #region Private protected methods
    /// <summary>
    /// Gets the currently authenticated user's Id.
    /// </summary>
    /// <returns>
    /// A <see cref="Guid"/> value representing the currently
    /// authenticated user's Id.
    /// </returns>
    private protected Guid GetCurrentUserId()
    {
        var identityOptions = HttpContext.RequestServices.GetRequiredService<IOptions<IdentityOptions>>();

        var idClaimType = identityOptions.Value.ClaimsIdentity.UserIdClaimType;

        return User.GetGuidClaim(idClaimType);
    }
    #endregion
}