using Microsoft.AspNetCore.Authentication.JwtBearer;
using Paradise.WebApi.Services.Authentication;

namespace Paradise.WebApi.Authentication.JwtBearer;

/// <inheritdoc/>
internal sealed class JwtEvents : JwtBearerEvents
{
    #region Constants
    /// <summary>
    /// A key to access the session check result in <see cref="HttpContext.Items"/>.
    /// </summary>
    public const string JwtEventsSessionCheckResult = nameof(JwtEventsSessionCheckResult);
    #endregion

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

        context.HttpContext.Items.Add(JwtEventsSessionCheckResult, checkResult);

        if (!checkResult.IsSuccess)
            context.Fail(string.Empty);
    }
    #endregion
}