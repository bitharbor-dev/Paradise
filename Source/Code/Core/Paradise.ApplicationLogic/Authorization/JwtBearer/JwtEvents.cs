using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        => context
        .HttpContext
        .RequestServices
        .GetRequiredService<IAuthorizationService>()
        .OnAuthenticationFailedAsync(context.Response);

    /// <summary>
    /// Invoked if Authorization fails and results in a Forbidden response.
    /// </summary>
    /// <param name="context">
    /// Context object that contains event data.
    /// </param>
    public static Task OnForbidden(ForbiddenContext context)
        => context
        .HttpContext
        .RequestServices
        .GetRequiredService<IAuthorizationService>()
        .OnForbiddenAsync(context.Response);

    /// <summary>
    /// Invoked after the security token has passed validation
    /// and a <see cref="ClaimsIdentity"/> has been generated.
    /// </summary>
    /// <param name="context">
    /// Context object that contains event data.
    /// </param>
    public static Task OnTokenValidated(TokenValidatedContext context)
        => context
        .HttpContext
        .RequestServices
        .GetRequiredService<IAuthorizationService>()
        .OnTokenValidatedAsync(context.Response, context.Principal, context.SecurityToken, context.Fail);

    /// <summary>
    /// Invoked before a challenge is sent back to the caller.
    /// </summary>
    /// <param name="context">
    /// Context object that contains event data.
    /// </param>
    public static Task OnChallenge(JwtBearerChallengeContext context)
        => context
        .HttpContext
        .RequestServices
        .GetRequiredService<IAuthorizationService>()
        .OnChallengeAsync(context.Response, context.HandleResponse);
    #endregion
}