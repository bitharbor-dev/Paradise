using Paradise.ApplicationLogic.Services.Application;
using Paradise.DataAccess.Seed.Providers;
using Paradise.DependencyInjection;
using Paradise.Maintenance.Options;
using Paradise.Maintenance.Workers;
using Paradise.Options.Origins;

static void AddWorkersOptions(IServiceCollection services, IConfiguration configuration)
{
    services.AddOptions<OutdatedTokensCleanupWorkerOptions>()
        .Bind(configuration.GetRequiredSection(nameof(OutdatedTokensCleanupWorkerOptions)))
        .ValidateDataAnnotations()
        .ValidateOnStart();

    services.AddOptions<PendingDeletionUsersResetWorkerOptions>()
        .Bind(configuration.GetRequiredSection(nameof(PendingDeletionUsersResetWorkerOptions)))
        .ValidateDataAnnotations()
        .ValidateOnStart();

    services.AddOptions<UnconfirmedUsersCleanupWorkerOptions>()
        .Bind(configuration.GetRequiredSection(nameof(UnconfirmedUsersCleanupWorkerOptions)))
        .ValidateDataAnnotations()
        .ValidateOnStart();
}

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    var servicesBuilder = new ServiceCollectionBuilder(services, new JsonConfigurationOrigin());

    servicesBuilder.ConfigureRequiredServices();

    AddWorkersOptions(services, context.Configuration);

    services.AddHostedService<OutdatedTokensCleanupWorker>();
    services.AddHostedService<PendingDeletionUsersResetWorker>();
    services.AddHostedService<UnconfirmedUsersCleanupWorker>();

    services.AddApplicationInsightsTelemetryWorkerService();
}

static async Task SeedDatabasesAsync(IServiceProvider serviceProvider)
{
    await using var scope = serviceProvider.CreateAsyncScope();
    var dbService = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
    var dataProvider = scope.ServiceProvider.GetRequiredService<ISeedDataProvider>();

    await dbService.EnsureDatabasesCreatedAsync();
    await dbService.SeedRolesAsync(dataProvider.GetSeedRoles());
    await dbService.SeedUsersAsync(dataProvider.GetSeedUsers());
    await dbService.SeedEmailTemplatesAsync(dataProvider.GetSeedEmailTemplates());
}

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(builder => builder.AddEnvironmentVariables())
    .ConfigureServices(ConfigureServices)
    .Build();

// Seed application databases.
await SeedDatabasesAsync(host.Services);

await host.RunAsync();