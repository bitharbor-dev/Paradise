using Microsoft.AspNetCore.Mvc;
using Paradise.ApplicationLogic.Services.Application;
using Paradise.DataAccess.Seed.Providers;
using Paradise.DependencyInjection;
using Paradise.Options.Origins;
using Paradise.WebApi.Filters.ExceptionHandling;
using Paradise.WebApi.Swagger;

static WebApplication CreateApp(string[] args, out IConfiguration appSettings)
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.AddEnvironmentVariables();

    var servicesBuilder = new ServiceCollectionBuilder(builder.Services, JsonConfigurationOrigin.Default);
    servicesBuilder.AddBearerAuth();

    servicesBuilder.ConfigureRequiredServices();

    builder.Services.AddApplicationInsightsTelemetry();

    builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
    builder.Services.AddControllers(options => options.Filters.Add(ExceptionFilter.Instance));

    builder.Services.AddSwaggerGen(options => options.Configure(builder.Configuration));

    appSettings = builder.Configuration;

    return builder.Build();
}

static void ConfigureApplication(WebApplication app, IConfiguration configuration)
{
    if (app.Environment.IsDevelopment())
        app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(options => options.Configure(configuration));

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.UseHttpsRedirection();
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

// Create WebApplication instance with configured DI.
var app = CreateApp(args, out var configuration);

// Configure the HTTP request pipeline.
ConfigureApplication(app, configuration);

// Seed application databases.
await SeedDatabasesAsync(app.Services)
    .ConfigureAwait(false);

await app.RunAsync()
    .ConfigureAwait(false);