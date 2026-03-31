using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Paradise.ApplicationLogic.Extensions;
using Paradise.Common;
using System.Reflection;

var builder = FunctionsApplication.CreateBuilder(args);

var environmentName = builder.Environment.EnvironmentName;
EnvironmentNames.ThrowIfNotAllowedEnvironment(environmentName);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("options.json", false, true)
    .AddJsonFile($"options.{environmentName}.json", true, true)
    .AddUserSecrets(Assembly.GetExecutingAssembly(), true, true);

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddApplicationLogic(builder.Configuration, environmentName);

var host = builder.Build();

await host.RunAsync()
    .ConfigureAwait(false);