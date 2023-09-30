using Paradise.ApplicationLogic.Services.Application;
using Paradise.DataAccess.Seed.Providers;
using Paradise.DependencyInjection;
using Paradise.Maintenance.Options;
using Paradise.Maintenance.Workers;
using Paradise.Options.Origins;

static void AddWorkerOptions<TOptions>(IServiceCollection services, IConfiguration configuration) where TOptions : class
    => services.AddOptions<TOptions>()
    .Bind(configuration.GetRequiredSection(typeof(TOptions).Name))
    .ValidateDataAnnotations()
    .ValidateOnStart();

static void AddWorkersOptions(IServiceCollection services, IConfiguration configuration)
{
    AddWorkerOptions<OutdatedTokensCleanupWorkerOptions>(services, configuration);
    AddWorkerOptions<PendingDeletionUsersResetWorkerOptions>(services, configuration);
    AddWorkerOptions<UnconfirmedUsersCleanupWorkerOptions>(services, configuration);
}

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    var servicesBuilder = new ServiceCollectionBuilder(services, JsonConfigurationOrigin.Default);

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