using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.Infrastructure.Services.Identity;

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
    /// NCRONTAB expression - at midnight every day.
    /// </summary>
    private const string Daily = "0 0 0 * * *";
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
    public async Task ResetUsersPendingDeletionAsync([TimerTrigger(Daily)] TimerInfo _, CancellationToken cancellationToken)
    {
        var scope = serviceProvider.CreateAsyncScope();

        await using (scope.ConfigureAwait(false))
        {
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            await userService.CancelExpiredDeletionRequestsAsync(cancellationToken)
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
    public async Task DeleteUnconfirmedUsersAsync([TimerTrigger(Daily)] TimerInfo _, CancellationToken cancellationToken)
    {
        var scope = serviceProvider.CreateAsyncScope();

        await using (scope.ConfigureAwait(false))
        {
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            await userService.DeleteUnconfirmedUsersAsync(cancellationToken)
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
    public async Task DeleteExpiredRefreshTokensAsync([TimerTrigger(Daily)] TimerInfo _, CancellationToken cancellationToken)
    {
        var scope = serviceProvider.CreateAsyncScope();
        await using (scope.ConfigureAwait(false))
        {
            var userRefreshTokenService = scope.ServiceProvider.GetRequiredService<IUserRefreshTokenService>();

            await userRefreshTokenService.DeleteExpiredAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
    #endregion
}