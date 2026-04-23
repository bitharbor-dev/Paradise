using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Paradise.ApplicationLogic.Extensions;
using Paradise.Common;
using Paradise.Common.Extensions;
using Paradise.Domain.Base.Events.Extensions;
using System.Reflection;

var builder = FunctionsApplication.CreateBuilder(args);

var environmentName = builder.Environment.EnvironmentName;
EnvironmentNames.ThrowIfNotAllowedEnvironment(environmentName);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("options.json", false, true)
    .AddJsonFile($"options.{environmentName}.json", true, true)
    .AddUserSecrets(Assembly.GetExecutingAssembly(), true, true);

if (EnvironmentNames.IsProduction(environmentName))
{
    builder.Services
        .AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter(builder.Configuration.BindOptionalSection);
}

builder.Services.AddApplicationLogic(builder.Configuration, environmentName);
builder.Services.AddDomainEventsDispatching(options => options.MaxRetries = 0);

var host = builder.Build();

await host.RunAsync()
    .ConfigureAwait(false);