using Microsoft.IdentityModel.Tokens;
using Paradise.ApplicationLogic.Authorization.Models;
using System.Security.Claims;

namespace Paradise.ApplicationLogic.Services.Application;

/// <summary>
/// Provides authorization functionalities.
/// </summary>
public interface IAuthorizationService
{
    #region Methods
    /// <summary>
    /// Invoked if authentication fails during request processing.
    /// The exceptions will be re-thrown after this event unless suppressed.
    /// </summary>
    /// <param name="response">
    /// The response.
    /// </param>
    Task OnAuthenticationFailedAsync(IHttpResponseWrapper response);

    /// <summary>
    /// Invoked if Authorization fails and results in a Forbidden response.
    /// </summary>
    /// <param name="response">
    /// The response.
    /// </param>
    Task OnForbiddenAsync(IHttpResponseWrapper response);

    /// <summary>
    /// Invoked after the security token has passed validation
    /// and a <see cref="ClaimsIdentity"/> has been generated.
    /// </summary>
    /// <param name="response">
    /// The response.
    /// </param>
    /// <param name="principal">
    /// The <see cref="ClaimsPrincipal"/> containing the user claims.
    /// </param>
    /// <param name="securityToken">
    /// The validated security token.
    /// </param>
    /// <param name="failureDelegate">
    /// If invoked - indicates that there was a failure during authentication.
    /// </param>
    Task OnTokenValidatedAsync(IHttpResponseWrapper response, ClaimsPrincipal? principal, SecurityToken securityToken, Action<string> failureDelegate);

    /// <summary>
    /// Invoked before a challenge is sent back to the caller.
    /// </summary>
    /// <param name="response">
    /// The response.
    /// </param>
    /// <param name="handleResponseDelegate">
    /// Skips any default logic for this challenge.
    /// </param>
    Task OnChallengeAsync(IHttpResponseWrapper response, Action handleResponseDelegate);
    #endregion
}