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
/// <param name="logger">
/// Logger.
/// </param>
/// <param name="serviceProvider">
/// Service provider to retrieve registered services.
/// </param>
/// <param name="optionsMonitor">
/// Worker options.
/// </param>
internal sealed class UnconfirmedUsersCleanupWorker(ILogger<UnconfirmedUsersCleanupWorker> logger,
                                                    IServiceProvider serviceProvider,
                                                    IOptionsMonitor<UnconfirmedUsersCleanupWorkerOptions> optionsMonitor)
    : WorkerBase<UnconfirmedUsersCleanupWorkerOptions>(logger, serviceProvider, optionsMonitor)
{
    #region Public methods
    /// <inheritdoc/>
    public override Task DoWorkAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var applicationOptions = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;

        var databaseService = provider.GetRequiredService<IDatabaseService>();

        return databaseService.DeleteUnconfirmedUsersAsync(applicationOptions.Tokens.EmailConfirmationTokenLifetime, cancellationToken);
    }
    #endregion
}