using Microsoft.AspNetCore.Authentication.JwtBearer;
using Paradise.WebApi.Services.Authentication;

namespace Paradise.WebApi.Authentication.JwtBearer;

/// <inheritdoc/>
internal sealed class JwtEvents : JwtBearerEvents
{
    #region Public methods
    /// <inheritdoc/>
    /// <remarks>
    /// This method is expected to be invoked only once per request.
    /// If that is not the case - check authentication schemes configuration.
    /// </remarks>
    public override async Task TokenValidated(TokenValidatedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var authenticationService = context
            .HttpContext
            .RequestServices
            .GetRequiredService<IAuthenticationService>();

        var checkResult = await authenticationService.CheckSessionAsync(context.Principal)
            .ConfigureAwait(false);

        if (!checkResult.IsSuccess)
            context.Fail(string.Empty);
    }
    #endregion
}