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
/// <param name="serviceProvider">
/// Service provider to retrieve registered services.
/// </param>
internal sealed class OutdatedTokensCleanupWorker(IServiceProvider serviceProvider) : WorkerBase<OutdatedTokensCleanupWorkerOptions>(serviceProvider)
{
    #region Public methods
    /// <inheritdoc/>
    public override Task ExecuteAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var applicationOptions = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
        var refreshTokenLifetime = applicationOptions.Authentication.RefreshTokenLifetime;

        var databaseService = provider.GetRequiredService<IDatabaseService>();

        return databaseService.DeleteOutdatedTokensAsync(refreshTokenLifetime, cancellationToken);
    }
    #endregion
}