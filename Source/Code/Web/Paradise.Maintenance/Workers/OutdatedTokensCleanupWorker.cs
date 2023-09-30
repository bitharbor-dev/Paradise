using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Services.Application;
using Paradise.Maintenance.Options;
using Paradise.Maintenance.Workers.Base;
using Paradise.Options.Models;

namespace Paradise.Maintenance.Workers;

/// <summary>
/// Worker class that deletes the outdated user tokens.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OutdatedTokensCleanupWorkerOptions"/> class.
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
internal sealed class OutdatedTokensCleanupWorker(ILogger<OutdatedTokensCleanupWorker> logger,
                                                  IServiceProvider serviceProvider,
                                                  IOptionsMonitor<OutdatedTokensCleanupWorkerOptions> optionsMonitor)
    : WorkerBase<OutdatedTokensCleanupWorkerOptions>(logger, serviceProvider, optionsMonitor)
{
    #region Public methods
    /// <inheritdoc/>
    public override Task DoWorkAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var applicationOptions = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;

        var databaseService = provider.GetRequiredService<IDatabaseService>();

        return databaseService.DeleteOutdatedTokensAsync(applicationOptions.Authentication.RefreshTokenLifetime, cancellationToken);
    }
    #endregion
}