using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.Services.Application;
using System.Security.Claims;

namespace Paradise.ApplicationLogic.Authorization.JwtBearer;

/// <summary>
/// Specifies the events which the <see cref="JwtBearerHandler"/>
/// invokes to enable developer control over the authentication process.
/// </summary>
public static class JwtEvents
{
    #region Public methods
    /// <summary>
    /// Invoked if authentication fails during request processing.
    /// The exceptions will be re-thrown after this event unless suppressed.
    /// </summary>
    /// <param name="context">
    /// Context object that contains event data.
    /// </param>
    public static Task OnAuthenticationFailed(AuthenticationFailedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var authorizationService = GetAuthorizationService(context.HttpContext);

        return authorizationService.OnAuthenticationFailedAsync(context.Response);
    }

    /// <summary>
    /// Invoked if Authorization fails and results in a Forbidden response.
    /// </summary>
    /// <param name="context">
    /// Context object that contains event data.
    /// </param>
    public static Task OnForbidden(ForbiddenContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var authorizationService = GetAuthorizationService(context.HttpContext);

        return authorizationService.OnForbiddenAsync(context.Response);
    }

    /// <summary>
    /// Invoked after the security token has passed validation
    /// and a <see cref="ClaimsIdentity"/> has been generated.
    /// </summary>
    /// <param name="context">
    /// Context object that contains event data.
    /// </param>
    public static Task OnTokenValidated(TokenValidatedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var authorizationService = GetAuthorizationService(context.HttpContext);

        return authorizationService.OnTokenValidatedAsync(context.Response,
                                                          context.Principal,
                                                          context.SecurityToken,
                                                          context.Fail);
    }

    /// <summary>
    /// Invoked before a challenge is sent back to the caller.
    /// </summary>
    /// <param name="context">
    /// Context object that contains event data.
    /// </param>
    public static Task OnChallenge(JwtBearerChallengeContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var authorizationService = GetAuthorizationService(context.HttpContext);

        return authorizationService.OnChallengeAsync(context.Response,
                                                     context.HandleResponse);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Gets the <see cref="IAuthorizationService"/> instance from the current
    /// <see cref="HttpContext"/> service collection.
    /// </summary>
    /// <param name="httpContext">
    /// The <see cref="HttpContext"/> to be used to
    /// retrieve an <see cref="IAuthorizationService"/> instance.
    /// </param>
    /// <returns>
    /// A new <see cref="IAuthorizationService"/> instance.
    /// </returns>
    private static IAuthorizationService GetAuthorizationService(HttpContext httpContext)
    {
        var serviceProvider = httpContext.RequestServices;

        return serviceProvider.GetRequiredService<IAuthorizationService>();
    }
    #endregion
}