using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Services.Application;
using Paradise.Maintenance.Options;
using Paradise.Maintenance.Workers.Base;
using Paradise.Options.Models;

namespace Paradise.Maintenance.Workers;

/// <summary>
/// Worker class that deletes the users who have not confirmed
/// their email address and exceeded the confirmation period.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UnconfirmedUsersCleanupWorker"/> class.
/// </remarks>
/// <param name="serviceProvider">
/// Service provider to retrieve registered services.
/// </param>
internal sealed class UnconfirmedUsersCleanupWorker(IServiceProvider serviceProvider) : WorkerBase<UnconfirmedUsersCleanupWorkerOptions>(serviceProvider)
{
    #region Public methods
    /// <inheritdoc/>
    public override Task ExecuteAsync(IServiceProvider provider, CancellationToken cancellationToken)
    {
        var applicationOptions = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
        var emailConfirmationTokenLifetime = applicationOptions.Tokens.EmailConfirmationTokenLifetime;

        var databaseService = provider.GetRequiredService<IDatabaseService>();

        return databaseService.DeleteUnconfirmedUsersAsync(emailConfirmationTokenLifetime, cancellationToken);
    }
    #endregion
}