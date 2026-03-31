using Microsoft.Azure.Functions.Worker;
using Paradise.ApplicationLogic.Services.Identity.Users;

namespace Paradise.Maintenance.Functions;

/// <summary>
/// Resets the users who are pending deletion.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PendingDeletionUsersResetter"/> class.
/// </remarks>
/// <param name="userService">
/// User service.
/// </param>
public sealed class PendingDeletionUsersResetter(IUserService userService)
{
    #region Constants
    /// <summary>
    /// Schedule - every 24 hours.
    /// </summary>
    private const string Schedule = "0 0 0 * * *";
    #endregion

    #region Public methods
    /// <summary>
    /// Resets the users who are pending deletion.
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
    [Function(nameof(PendingDeletionUsersResetter))]
    public Task RunAsync([TimerTrigger(Schedule)] TimerInfo _, CancellationToken cancellationToken)
        => userService.CancelExpiredDeletionRequestsAsync(cancellationToken);
    #endregion
}