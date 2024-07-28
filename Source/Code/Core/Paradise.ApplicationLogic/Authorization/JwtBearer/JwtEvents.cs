using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.Authorization.Models;
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
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public static Task OnAuthenticationFailed(AuthenticationFailedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var authorizationService = context.HttpContext.GetAuthorizationService();

        var wrapper = context.CreateWrapper();
        var delegates = context.CreateDelegates();

        return authorizationService.OnAuthenticationFailedAsync(wrapper, delegates);
    }

    /// <summary>
    /// Invoked before a challenge is sent back to the caller.
    /// </summary>
    /// <param name="context">
    /// Context object that contains event data.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public static Task OnChallenge(JwtBearerChallengeContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var authorizationService = context.HttpContext.GetAuthorizationService();

        var wrapper = CreateWrapper(context);

        return authorizationService.OnChallengeAsync(wrapper, context.HandleResponse);
    }

    /// <summary>
    /// Invoked if Authorization fails and results in a Forbidden response.
    /// </summary>
    /// <param name="context">
    /// Context object that contains event data.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public static Task OnForbidden(ForbiddenContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var authorizationService = context.HttpContext.GetAuthorizationService();

        var wrapper = context.CreateWrapper();
        var delegates = context.CreateDelegates();

        return authorizationService.OnForbiddenAsync(wrapper, delegates);
    }

    /// <summary>
    /// Invoked when a protocol message is first received.
    /// </summary>
    /// <param name="context">
    /// Context object that contains event data.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public static Task OnMessageReceived(MessageReceivedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var authorizationService = context.HttpContext.GetAuthorizationService();

        var wrapper = context.CreateWrapper();
        var delegates = context.CreateDelegates();

        return authorizationService.OnMessageReceivedAsync(wrapper,
                                                           delegates,
                                                           token => context.Token = token);
    }

    /// <summary>
    /// Invoked after the security token has passed validation
    /// and a <see cref="ClaimsIdentity"/> has been generated.
    /// </summary>
    /// <param name="context">
    /// Context object that contains event data.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public static Task OnTokenValidated(TokenValidatedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var authorizationService = context.HttpContext.GetAuthorizationService();

        var wrapper = context.CreateWrapper();
        var delegates = context.CreateDelegates();

        return authorizationService.OnTokenValidatedAsync(wrapper,
                                                          context.Principal,
                                                          context.SecurityToken,
                                                          delegates);
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
    private static IAuthorizationService GetAuthorizationService(this HttpContext httpContext)
    {
        var serviceProvider = httpContext.RequestServices;

        return serviceProvider.GetRequiredService<IAuthorizationService>();
    }

    /// <summary>
    /// Creates a new <see cref="HttpResponseWrapper"/> instance
    /// from the given <paramref name="context"/>.
    /// </summary>
    /// <param name="context">
    /// The <see cref="BaseContext{TOptions}"/> which
    /// <see cref="BaseContext{TOptions}.Response"/> to be wrapped.
    /// </param>
    /// <returns>
    /// A new <see cref="HttpResponseWrapper"/> instance.
    /// </returns>
    private static HttpResponseWrapper CreateWrapper(this BaseContext<JwtBearerOptions> context)
        => new(context.Response);

    /// <summary>
    /// Creates a new <see cref="ResultContextDelegates"/> instance
    /// from the given <paramref name="context"/>.
    /// </summary>
    /// <param name="context">
    /// The <see cref="ResultContext{TOptions}"/> which methods to be used
    /// during authentication process.
    /// </param>
    /// <returns>
    /// A new <see cref="ResultContextDelegates"/> instance.
    /// </returns>
    private static ResultContextDelegates CreateDelegates(this ResultContext<JwtBearerOptions> context)
        => new(context.Success, context.NoResult, context.Fail, context.Fail);
    #endregion
}