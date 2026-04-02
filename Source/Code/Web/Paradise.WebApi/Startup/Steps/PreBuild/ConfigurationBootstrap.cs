using System.Reflection;

namespace Paradise.WebApi.Startup.Steps.PreBuild;

/// <summary>
/// Configures application configuration sources.
/// </summary>
internal sealed class ConfigurationBootstrap : IPreBuildStep
{
    #region Public methods
    /// <inheritdoc/>
    public Task ExecuteAsync(PreBuildContext context)
    {
        var environmentName = context.Builder.Environment.EnvironmentName;

        context.Builder.Configuration
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("options.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"options.{environmentName}.json", optional: true, reloadOnChange: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true, reloadOnChange: true);

        return Task.CompletedTask;
    }
    #endregion
}