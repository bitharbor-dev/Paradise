using Microsoft.Azure.Functions.Worker;
using Paradise.ApplicationLogic.Services.Identity.Users;

namespace Paradise.Maintenance.Functions;

/// <summary>
/// Deletes the outdated user refresh tokens.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserRefreshTokenCleaner"/> class.
/// </remarks>
/// <param name="userRefreshTokenService">
/// User refresh token service.
/// </param>
public sealed class UserRefreshTokenCleaner(IUserRefreshTokenService userRefreshTokenService)
{
    #region Constants
    /// <summary>
    /// Schedule - every 24 hours.
    /// </summary>
    private const string Schedule = "0 0 0 * * *";
    #endregion

    #region Public methods
    /// <summary>
    /// Deletes the outdated user refresh tokens.
    /// </summary>
    /// <param name="_">
    /// Function timer.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    [Function(nameof(UserRefreshTokenCleaner))]
    public Task RunAsync([TimerTrigger(Schedule)] TimerInfo _, CancellationToken cancellationToken)
        => userRefreshTokenService.DeleteExpiredAsync(cancellationToken);
    #endregion
}