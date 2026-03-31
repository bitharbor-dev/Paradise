using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Paradise.Models;
using Paradise.WebApi.Authentication.JwtBearer;
using Paradise.WebApi.Infrastructure.Extensions;
using static Paradise.Models.ErrorCode;
using static Paradise.Models.OperationStatus;

namespace Paradise.WebApi.Authorization;

/// <summary>
/// Provides default, centralized response writing point for the web API.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthorizationResultHandler"/> class.
/// </remarks>
/// <param name="defaultHandler">
/// Default <see cref="IAuthorizationMiddlewareResultHandler"/> implementation.
/// </param>
internal sealed class AuthorizationResultHandler(IAuthorizationMiddlewareResultHandler defaultHandler)
    : IAuthorizationMiddlewareResultHandler
{
    #region Public methods
    /// <inheritdoc/>
    public Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        return authorizeResult.Challenged
            ? HandleUnauthorizedAsync(context)
            : authorizeResult.Forbidden
            ? HandleForbiddenAsync(context)
            : defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Writes a standardized unauthorized (401) response when authentication
    /// has failed or no valid authentication result is available.
    /// </summary>
    /// <remarks>
    /// If authentication previously failed during JWT token validation and a
    /// <see cref="Result"/> was stored in <see cref="HttpContext.Items"/>,
    /// that result is written to the response.
    /// Otherwise, a default unauthorized result is written to the response.
    /// </remarks>
    /// <param name="context">
    /// The current <see cref="HttpContext"/> instance.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    private static Task HandleUnauthorizedAsync(HttpContext context)
    {
        var key = JwtEvents.JwtEventsSessionCheckResult;

        if (context.Items.TryGetValue(key, out var value) && value is Result jwtEventsResult)
            return jwtEventsResult.AsActionResult().WriteResponseContentAsync(context.Response);

        var result = new Result(Unauthorized, UserUnauthorized);

        return result.AsActionResult().WriteResponseContentAsync(context.Response);
    }

    /// <summary>
    /// Writes a standardized forbidden (403) response when authorization
    /// has failed for an authenticated principal.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="HttpContext"/> instance.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    private static Task HandleForbiddenAsync(HttpContext context)
    {
        var result = new Result(Prohibited, AccessForbidden);

        return result.AsActionResult().WriteResponseContentAsync(context.Response);
    }
    #endregion
}