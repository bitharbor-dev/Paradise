using Paradise.Models.WebApi.Services.Authentication;
using Paradise.WebApi.Infrastructure.Extensions;
using Paradise.WebApi.Services.Authentication;

namespace Paradise.WebApi.Endpoints.Handlers.Infrastructure;

/// <summary>
/// Contains authentication actions.
/// </summary>
internal static class AuthenticationHandlers
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
    /// <param name="service">
    /// Authentication service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="AccessTokenModel"/>
    /// containing information about the user authorization token or
    /// two-factor authentication token in case it is enabled for the user.
    /// </returns>
    public static Task<IResult> LoginAsync(LoginModel model, IAuthenticationService service, CancellationToken cancellationToken)
        => service.LoginAsync(model, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Generates a new user authorization token
    /// for the user with two-factor authentication enabled.
    /// </summary>
    /// <param name="model">
    /// The <see cref="TwoFactorAuthenticationModel"/> to be used to
    /// validate the login data and generate an access token.
    /// </param>
    /// <param name="service">
    /// Authentication service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="AccessTokenModel"/> containing information about the user authorization token.
    /// </returns>
    public static Task<IResult> ConfirmLoginAsync(TwoFactorAuthenticationModel model, IAuthenticationService service, CancellationToken cancellationToken)
        => service.ConfirmLoginAsync(model, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Generates a new user authorization token
    /// using the given <paramref name="accessToken"/>.
    /// </summary>
    /// <param name="accessToken">
    /// User authorization token.
    /// </param>
    /// <param name="service">
    /// Authentication service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="AccessTokenModel"/> containing information about the user authorization token.
    /// </returns>
    public static Task<IResult> RenewTokenAsync(string accessToken, IAuthenticationService service, CancellationToken cancellationToken)
        => service.RenewTokenAsync(accessToken, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Invalidates the given <paramref name="accessToken"/>
    /// to make it unusable during the authentication process.
    /// </summary>
    /// <param name="accessToken">
    /// Authorization token to be invalidated.
    /// </param>
    /// <param name="service">
    /// Authentication service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> instance containing errors data if any occurs.
    /// </returns>
    public static Task<IResult> LogoutAsync(string accessToken, IAuthenticationService service, CancellationToken cancellationToken)
        => service.LogoutAsync(accessToken, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Invalidates all user's refresh tokens
    /// to make them unusable during the authentication process.
    /// </summary>
    /// <param name="accessToken">
    /// Authorization token to be used to terminate
    /// all user sessions (currently active refresh tokens).
    /// </param>
    /// <param name="service">
    /// Authentication service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> instance containing errors data if any occurs.
    /// </returns>
    public static Task<IResult> TerminateSessionsAsync(string accessToken, IAuthenticationService service, CancellationToken cancellationToken)
        => service.TerminateSessionsAsync(accessToken, cancellationToken).AsHttpResultAsync();
    #endregion
}