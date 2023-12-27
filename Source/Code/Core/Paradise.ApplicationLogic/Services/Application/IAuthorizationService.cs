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
    /// <param name="delegates">
    /// Authentication context delegates container.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task OnAuthenticationFailedAsync(IHttpResponseWrapper response, ResultContextDelegates delegates);

    /// <summary>
    /// Invoked before a challenge is sent back to the caller.
    /// </summary>
    /// <param name="response">
    /// The response.
    /// </param>
    /// <param name="handleResponseDelegate">
    /// Skips any default logic for this challenge.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task OnChallengeAsync(IHttpResponseWrapper response, Action handleResponseDelegate);

    /// <summary>
    /// Invoked when a protocol message is first received.
    /// </summary>
    /// <param name="response">
    /// The response.
    /// </param>
    /// <param name="delegates">
    /// Authentication context delegates container.
    /// </param>
    /// <param name="setTokenDelegate">
    /// Sets the authentication token from an external source.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task OnMessageReceivedAsync(IHttpResponseWrapper response, ResultContextDelegates delegates, Action<string?> setTokenDelegate);

    /// <summary>
    /// Invoked if Authorization fails and results in a Forbidden response.
    /// </summary>
    /// <param name="response">
    /// The response.
    /// </param>
    /// <param name="delegates">
    /// Authentication context delegates container.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task OnForbiddenAsync(IHttpResponseWrapper response, ResultContextDelegates delegates);

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
    /// <param name="delegates">
    /// Authentication context delegates container.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task OnTokenValidatedAsync(IHttpResponseWrapper response, ClaimsPrincipal? principal, SecurityToken securityToken, ResultContextDelegates delegates);
    #endregion
}