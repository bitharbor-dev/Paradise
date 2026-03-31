using Paradise.Models;
using Paradise.Models.WebApi.Services.Authentication;
using System.Security.Claims;

namespace Paradise.WebApi.Services.Authentication;

/// <summary>
/// Provides authentication functionalities.
/// </summary>
public interface IAuthenticationService
{
    #region Methods
    /// <summary>
    /// Generates a new access token or two-factor authentication token in case it is enabled for the user.
    /// </summary>
    /// <param name="model">
    /// The <see cref="LoginModel"/> to be used to
    /// validate login data and generate an access token.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="AccessTokenModel"/>
    /// containing information about the access token or
    /// two-factor authentication token in case it is enabled for the user.
    /// </returns>
    Task<Result<AccessTokenModel>> LoginAsync(
        LoginModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a new access token for the user with two-factor authentication enabled.
    /// </summary>
    /// <param name="model">
    /// The <see cref="TwoFactorAuthenticationModel"/> to be used to
    /// validate the login data and generate an access token.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="AccessTokenModel"/>
    /// containing information about the access token.
    /// </returns>
    Task<Result<AccessTokenModel>> ConfirmLoginAsync(
        TwoFactorAuthenticationModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a new access token using the given <paramref name="oldAccessToken"/>.
    /// </summary>
    /// <param name="oldAccessToken">
    /// Old access token.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="AccessTokenModel"/>
    /// containing information about the access token.
    /// </returns>
    Task<Result<AccessTokenModel>> RenewTokenAsync(
        string oldAccessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates the refresh token associated with
    /// the given <paramref name="accessToken"/>
    /// to make it unusable during the authentication process.
    /// </summary>
    /// <param name="accessToken">
    /// Access token to be invalidated.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    Task<Result> LogoutAsync(
        string accessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates all user's refresh tokens
    /// to make them unusable during the authentication process.
    /// </summary>
    /// <param name="accessToken">
    /// Authorization token to be used to terminate
    /// all user sessions (currently active refresh tokens).
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    Task<Result> TerminateSessionsAsync(
        string accessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the <paramref name="principal"/>s session is expired or revoked.
    /// </summary>
    /// <param name="principal">
    /// The <see cref="ClaimsPrincipal"/> to check.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    Task<Result> CheckSessionAsync(
        ClaimsPrincipal? principal, CancellationToken cancellationToken = default);
    #endregion
}