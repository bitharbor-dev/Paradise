using Paradise.Models;
using Paradise.Models.WebApi.Services.Authentication;
using Paradise.WebApi.Services.Authentication;
using System.Security.Claims;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Web.WebApi.Services.Authentication;

/// <summary>
/// Stub <see cref="IAuthenticationService"/> implementation.
/// </summary>
public sealed class StubAuthenticationService : IAuthenticationService
{
    #region Properties
    /// <summary>
    /// <see cref="LoginAsync"/> result.
    /// </summary>
    public Func<Task<Result<AccessTokenModel>>>? LoginAsyncResult { get; set; }

    /// <summary>
    /// <see cref="ConfirmLoginAsync"/> result.
    /// </summary>
    public Func<Task<Result<AccessTokenModel>>>? ConfirmLoginAsyncResult { get; set; }

    /// <summary>
    /// <see cref="RenewTokenAsync"/> result.
    /// </summary>
    public Func<Task<Result<AccessTokenModel>>>? RenewTokenAsyncResult { get; set; }

    /// <summary>
    /// <see cref="LogoutAsync"/> result.
    /// </summary>
    public Func<Task<Result>>? LogoutAsyncResult { get; set; }

    /// <summary>
    /// <see cref="TerminateSessionsAsync"/> result.
    /// </summary>
    public Func<Task<Result>>? TerminateSessionsAsyncResult { get; set; }

    /// <summary>
    /// <see cref="CheckSessionAsync"/> result.
    /// </summary>
    public Func<Task<Result>>? CheckSessionAsyncResult { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task<Result<AccessTokenModel>> LoginAsync(LoginModel model, CancellationToken cancellationToken = default)
        => LoginAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<AccessTokenModel>> ConfirmLoginAsync(TwoFactorAuthenticationModel model, CancellationToken cancellationToken = default)
        => ConfirmLoginAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<AccessTokenModel>> RenewTokenAsync(string oldAccessToken, CancellationToken cancellationToken = default)
        => RenewTokenAsyncResult!();

    /// <inheritdoc/>
    public Task<Result> LogoutAsync(string accessToken, CancellationToken cancellationToken = default)
        => LogoutAsyncResult!();

    /// <inheritdoc/>
    public Task<Result> TerminateSessionsAsync(string accessToken, CancellationToken cancellationToken = default)
        => TerminateSessionsAsyncResult!();

    /// <inheritdoc/>
    public Task<Result> CheckSessionAsync(ClaimsPrincipal? principal, CancellationToken cancellationToken = default)
        => CheckSessionAsyncResult!();
    #endregion
}