using Microsoft.Extensions.Options;
using Paradise.Models;
using Paradise.Models.WebApi.Services.Authentication;
using Paradise.WebApi.Client.Base;
using System.Text.Json;
using static Paradise.Common.Web.AuthenticationRoutes;

namespace Paradise.WebApi.Client.Application;

/// <summary>
/// Contains all web API requests to the EmailTemplatesController.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthenticationApiClient"/> class.
/// </remarks>
/// <param name="jsonSerializerOptions">
/// The accessor used to access the <see cref="JsonSerializerOptions"/>.
/// </param>
/// <param name="httpClient">
/// <see cref="HttpClient"/> instance the <see cref="AuthenticationApiClient"/>
/// will operate over.
/// </param>
public sealed class AuthenticationApiClient(IOptionsMonitor<JsonSerializerOptions> jsonSerializerOptions, HttpClient httpClient)
    : ApiClientBase(jsonSerializerOptions, httpClient)
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
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="AccessTokenModel"/>
    /// containing information about the user authorization token or
    /// two-factor authentication token in case it is enabled for the user.
    /// </returns>
    public Task<Result<AccessTokenModel>> LoginAsync(LoginModel model, CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(Login);

        return PostAsync<AccessTokenModel>(route, false, model, cancellationToken);
    }

    /// <summary>
    /// Generates a new user authorization token
    /// for the user with two-factor authentication enabled.
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
    /// containing information about the user authorization token.
    /// </returns>
    public Task<Result<AccessTokenModel>> ConfirmLoginAsync(
        TwoFactorAuthenticationModel model, CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(ConfirmLogin);

        return PutAsync<AccessTokenModel>(route, false, model, cancellationToken);
    }

    /// <summary>
    /// Generates a new user authorization token
    /// using the present access token.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="AccessTokenModel"/>
    /// containing information about the user authorization token.
    /// </returns>
    public Task<Result<AccessTokenModel>> RenewTokenAsync(CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(RenewToken);

        return GetAsync<AccessTokenModel>(route, true, cancellationToken);
    }

    /// <summary>
    /// Invalidates the present access token
    /// to make it unusable during the authentication process.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    public Task<Result> LogoutAsync(CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(Logout);

        return DeleteAsync(route, true, cancellationToken);
    }

    /// <summary>
    /// Invalidates all user's refresh tokens
    /// to make them unusable during the authentication process.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    public Task<Result> TerminateSessionsAsync(CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(TerminateSessions);

        return DeleteAsync(route, true, cancellationToken);
    }
    #endregion
}