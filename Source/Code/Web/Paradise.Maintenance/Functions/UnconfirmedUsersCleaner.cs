using Microsoft.Azure.Functions.Worker;
using Paradise.ApplicationLogic.Services.Identity.Users;

namespace Paradise.Maintenance.Functions;

/// <summary>
/// Deletes the users who have not confirmed
/// their email address and exceeded the confirmation period.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UnconfirmedUsersCleaner"/> class.
/// </remarks>
/// <param name="userService">
/// User service.
/// </param>
public sealed class UnconfirmedUsersCleaner(IUserService userService)
{
    #region Constants
    /// <summary>
    /// Schedule - every 24 hours.
    /// </summary>
    private const string Schedule = "0 0 0 * * *";
    #endregion

    #region Public methods
    /// <summary>
    /// Deletes the users who have not confirmed
    /// their email address and exceeded the confirmation period.
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
    [Function(nameof(UnconfirmedUsersCleaner))]
    public Task RunAsync([TimerTrigger(Schedule)] TimerInfo _, CancellationToken cancellationToken)
        => userService.DeleteUnconfirmedUsersAsync(cancellationToken);
    #endregion
}