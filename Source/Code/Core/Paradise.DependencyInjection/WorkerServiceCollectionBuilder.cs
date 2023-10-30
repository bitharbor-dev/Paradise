using Microsoft.Extensions.DependencyInjection;
using Paradise.DependencyInjection.Base;
using Paradise.Options.Origins.Base;

namespace Paradise.DependencyInjection;

/// <summary>
/// Configures web API services.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WorkerServiceCollectionBuilder"/> class.
/// </remarks>
/// <param name="services">
/// The service collection the builder will operate over.
/// </param>
/// <param name="configurationOrigin">
/// Configuration origin.
/// </param>
public sealed class WorkerServiceCollectionBuilder(IServiceCollection services,
                                                   IConfigurationOrigin configurationOrigin)
    : ServiceCollectionBuilderCore(services, configurationOrigin)
{

}