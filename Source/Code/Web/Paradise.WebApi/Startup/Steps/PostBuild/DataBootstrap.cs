using Paradise.ApplicationLogic.Infrastructure.Seed;
using Paradise.DataAccess.Seed.Providers;

namespace Paradise.WebApi.Startup.Steps.PostBuild;

/// <summary>
/// Configures the HTTP request processing pipeline.
/// </summary>
internal sealed class DataBootstrap : IPostBuildStep
{
    #region Public methods
    /// <inheritdoc/>
    public Task ExecuteAsync(PostBuildContext context)
        => SeedDatabaseAsync(context.App.Services, context.App.Lifetime.ApplicationStopping);
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