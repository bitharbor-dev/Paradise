using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.Services.Identity.Users;

namespace Paradise.Maintenance;

/// <summary>
/// A container class for all Azure Functions methods.
/// </summary>
/// <param name="serviceProvider">
/// The <see cref="IServiceProvider"/> to resolve methods dependencies.
/// </param>
internal sealed class FunctionsContainer(IServiceProvider serviceProvider)
{
    #region Constants
    /// <summary>
    /// <see cref="ResetUsersPendingDeletionAsync"/> schedule - every 24 hours.
    /// </summary>
    private const string ResetUsersPendingDeletionSchedule = "0 0 0 * * *";

    /// <summary>
    /// <see cref="DeleteUnconfirmedUsersAsync"/> schedule - every 24 hours.
    /// </summary>
    private const string DeleteUnconfirmedUsersSchedule = "0 0 0 * * *";

    /// <summary>
    /// <see cref="DeleteExpiredRefreshTokensAsync"/> schedule - every 24 hours.
    /// </summary>
    private const string DeleteExpiredRefreshTokensSchedule = "0 0 0 * * *";
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
    [Function(nameof(ResetUsersPendingDeletionAsync))]
    public async Task ResetUsersPendingDeletionAsync([TimerTrigger(ResetUsersPendingDeletionSchedule)] TimerInfo _,
                                                     CancellationToken cancellationToken)
    {
        var scope = serviceProvider.CreateAsyncScope();

        try
        {
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            await userService.CancelExpiredDeletionRequestsAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            await scope.DisposeAsync()
                .ConfigureAwait(false);
        }
    }

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
    [Function(nameof(DeleteUnconfirmedUsersAsync))]
    public async Task DeleteUnconfirmedUsersAsync([TimerTrigger(DeleteUnconfirmedUsersSchedule)] TimerInfo _,
                                                  CancellationToken cancellationToken)
    {
        var scope = serviceProvider.CreateAsyncScope();

        try
        {
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            await userService.DeleteUnconfirmedUsersAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            await scope.DisposeAsync()
                .ConfigureAwait(false);
        }
    }

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
    [Function(nameof(DeleteExpiredRefreshTokensAsync))]
    public async Task DeleteExpiredRefreshTokensAsync([TimerTrigger(DeleteExpiredRefreshTokensSchedule)] TimerInfo _,
                                                      CancellationToken cancellationToken)
    {
        var scope = serviceProvider.CreateAsyncScope();

        try
        {
            var userRefreshTokenService = scope.ServiceProvider.GetRequiredService<IUserRefreshTokenService>();

            await userRefreshTokenService.DeleteExpiredAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            await scope.DisposeAsync()
                .ConfigureAwait(false);
        }
    }
    #endregion
}