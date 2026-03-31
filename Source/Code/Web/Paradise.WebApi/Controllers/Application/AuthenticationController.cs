using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paradise.Common.Web;
using Paradise.Models;
using Paradise.Models.WebApi.Services.Authentication;
using Paradise.WebApi.Controllers.Base;
using Paradise.WebApi.Infrastructure.Extensions;
using Paradise.WebApi.Infrastructure.Filters.Metadata;
using Paradise.WebApi.Services.Authentication;
using System.ComponentModel.DataAnnotations;

namespace Paradise.WebApi.Controllers.Application;

/// <summary>
/// Contains authentication actions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthenticationController"/> class.
/// </remarks>
/// <param name="authenticationService">
/// Authentication service.
/// </param>
public sealed class AuthenticationController(IAuthenticationService authenticationService) : ApiControllerBase
{
    #region Public methods
    /// <summary>
    /// Generates a new user authorization token or
    /// two-factor authentication token in case it is enabled for the user.
    /// </summary>
    /// <param name="model">
    /// The <see cref="LoginModel"/> to be used to
    /// validate login data and generate an access token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="AccessTokenModel"/>
    /// containing information about the user authorization token or
    /// two-factor authentication token in case it is enabled for the user.
    /// </returns>
    [HttpPost(AuthenticationRoutes.Login), AllowAnonymous]
    [ResultResponse<AccessTokenModel>(OperationStatus.Success)]
    [ResultResponse<AccessTokenModel>(OperationStatus.Received)]
    [ResultResponse(OperationStatus.InvalidInput)]
    [ResultResponse(OperationStatus.Unauthorized)]
    public async Task<IActionResult> Login([FromBody, Required] LoginModel model)
        => (await authenticationService.LoginAsync(model, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Generates a new user authorization token
    /// for the user with two-factor authentication enabled.
    /// </summary>
    /// <param name="model">
    /// The <see cref="TwoFactorAuthenticationModel"/> to be used to
    /// validate the login data and generate an access token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="AccessTokenModel"/>
    /// containing information about the user authorization token.
    /// </returns>
    [HttpPut(AuthenticationRoutes.ConfirmLogin), AllowAnonymous]
    [ResultResponse<AccessTokenModel>(OperationStatus.Success)]
    [ResultResponse(OperationStatus.InvalidInput)]
    [ResultResponse(OperationStatus.Unauthorized)]
    [ResultResponse(OperationStatus.Missing)]
    [ResultResponse(OperationStatus.Blocked)]
    public async Task<IActionResult> ConfirmLogin([FromBody, Required] TwoFactorAuthenticationModel model)
        => (await authenticationService.ConfirmLoginAsync(model, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Generates a new user authorization token
    /// using the given <paramref name="accessToken"/>.
    /// </summary>
    /// <param name="accessToken">
    /// User authorization token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="AccessTokenModel"/>
    /// containing information about the user authorization token.
    /// </returns>
    [Authorize(AuthenticationSchemes = AuthenticationSchemeNames.DisableTokenLifetimeValidation)]
    [HttpGet(AuthenticationRoutes.RenewToken)]
    [ResultResponse<AccessTokenModel>(OperationStatus.Success)]
    [ResultResponse(OperationStatus.InvalidInput)]
    [ResultResponse(OperationStatus.Unauthorized)]
    [ResultResponse(OperationStatus.Missing)]
    public async Task<IActionResult> RenewToken([FromHeader(Name = AuthorizationHeaderName), Required] string accessToken)
        => (await authenticationService.RenewTokenAsync(accessToken, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Invalidates the given <paramref name="accessToken"/>
    /// to make it unusable during the authentication process.
    /// </summary>
    /// <param name="accessToken">
    /// Authorization token to be invalidated.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    [Authorize(AuthenticationSchemes = AuthenticationSchemeNames.Default)]
    [HttpDelete(AuthenticationRoutes.Logout)]
    [ResultResponse(OperationStatus.Success)]
    [ResultResponse(OperationStatus.InvalidInput)]
    [ResultResponse(OperationStatus.Unauthorized)]
    public async Task<IActionResult> Logout([FromHeader(Name = AuthorizationHeaderName), Required] string accessToken)
        => (await authenticationService.LogoutAsync(accessToken, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Invalidates all user's refresh tokens
    /// to make them unusable during the authentication process.
    /// </summary>
    /// <param name="accessToken">
    /// Authorization token to be used to terminate
    /// all user sessions (currently active refresh tokens).
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    [Authorize(AuthenticationSchemes = AuthenticationSchemeNames.Default)]
    [HttpDelete(AuthenticationRoutes.TerminateSessions)]
    [ResultResponse(OperationStatus.Success)]
    [ResultResponse(OperationStatus.InvalidInput)]
    [ResultResponse(OperationStatus.Unauthorized)]
    public async Task<IActionResult> TerminateSessions([FromHeader(Name = AuthorizationHeaderName), Required] string accessToken)
        => (await authenticationService.TerminateSessionsAsync(accessToken, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();
    #endregion
}