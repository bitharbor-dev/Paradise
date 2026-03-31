using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Spies.Microsoft.AspNetCore.Authorization;

/// <summary>
/// Spy <see cref="IAuthorizationMiddlewareResultHandler"/> implementation.
/// </summary>
public sealed class SpyAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    #region Properties
    /// <summary>
    /// Indicates whether the <see cref="HandleAsync"/> method was invoked.
    /// </summary>
    public bool Handled { get; private set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        Handled = true;

        return Task.CompletedTask;
    }
    #endregion
}