using Paradise.ApplicationLogic.Services.Application;
using Paradise.DataAccess.Seed.Providers;
using Paradise.DependencyInjection;
using Paradise.Maintenance.Extensions;
using Paradise.Maintenance.Options;
using Paradise.Maintenance.Workers;
using Paradise.Options.Origins;

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    var servicesBuilder = new WorkerServiceCollectionBuilder(services, JsonConfigurationOrigin.Default);

    servicesBuilder.ConfigureRequiredServices();

    var configuration = context.Configuration;

    services.AddWorkerOptions<OutdatedTokensCleanupWorkerOptions>(configuration);
    services.AddWorkerOptions<PendingDeletionUsersResetWorkerOptions>(configuration);
    services.AddWorkerOptions<UnconfirmedUsersCleanupWorkerOptions>(configuration);

    services.AddHostedService<OutdatedTokensCleanupWorker>();
    services.AddHostedService<PendingDeletionUsersResetWorker>();
    services.AddHostedService<UnconfirmedUsersCleanupWorker>();

    services.AddApplicationInsightsTelemetryWorkerService();
}

static async Task SeedDatabasesAsync(IServiceProvider serviceProvider)
{
    var scope = serviceProvider.CreateAsyncScope();
    var dbService = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
    var dataProvider = scope.ServiceProvider.GetRequiredService<ISeedDataProvider>();

    await dbService
        .EnsureDatabasesCreatedAsync()
        .ConfigureAwait(false);

    await dbService
        .SeedRolesAsync(dataProvider.GetSeedRoles())
        .ConfigureAwait(false);

    await dbService
        .SeedUsersAsync(dataProvider.GetSeedUsers())
        .ConfigureAwait(false);

    await dbService
        .SeedEmailTemplatesAsync(dataProvider.GetSeedEmailTemplates())
        .ConfigureAwait(false);

    await scope
        .DisposeAsync()
        .ConfigureAwait(false);
}

// Create Host instance with configured DI.
var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(builder => builder.AddEnvironmentVariables())
    .ConfigureServices(ConfigureServices)
    .Build();

// Seed application databases.
await SeedDatabasesAsync(host.Services)
    .ConfigureAwait(false);

// Note: use "RunAsync(CancellationToken)" for proper workers stopping.
await host.RunAsync()
    .ConfigureAwait(false);