using Paradise.ApplicationLogic.Infrastructure.Seed;
using Paradise.DataAccess.Seed.Providers;

namespace Paradise.WebApi.Services.Background;

/// <summary>
/// A background services which performs
/// the application startup and shutdown activities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LifecycleManagementService"/> class.
/// </remarks>
/// <param name="services">
/// The <see cref="IServiceProvider"/> to resolve lifecycle actions dependencies.
/// </param>
internal sealed class LifecycleManagementService(IServiceProvider services) : IHostedService
{
    #region Public methods
    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
        => SeedDatabaseAsync(services, cancellationToken);

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
    #endregion

    #region Private methods
    /// <summary>
    /// Performs database seeding using the services resolved via the given <paramref name="provider"/>.
    /// </summary>
    /// <param name="provider">
    /// The <see cref="IServiceProvider"/> to resolve seeding services.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    private static async Task SeedDatabaseAsync(IServiceProvider provider, CancellationToken cancellationToken)
    {
        var scope = provider.CreateAsyncScope();

        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        var seedDataProvider = scope.ServiceProvider.GetRequiredService<ISeedDataProvider>();

        await seeder.EnsureStorageAvailableAsync(cancellationToken)
            .ConfigureAwait(false);

        await seeder.SeedRolesAsync(seedDataProvider.GetSeedRoles(), cancellationToken)
            .ConfigureAwait(false);

        await seeder.SeedUsersAsync(seedDataProvider.GetSeedUsers(), cancellationToken)
            .ConfigureAwait(false);

        await seeder.SeedEmailTemplatesAsync(seedDataProvider.GetSeedEmailTemplates(), cancellationToken)
            .ConfigureAwait(false);

        await scope.DisposeAsync()
            .ConfigureAwait(false);
    }
    #endregion
}