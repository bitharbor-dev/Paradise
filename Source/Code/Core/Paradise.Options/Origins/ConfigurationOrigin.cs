using Microsoft.Extensions.Configuration;
using Paradise.Options.Origins.Base;
using Paradise.Options.Origins.Options;

namespace Paradise.Options.Origins;

/// <summary>
/// Default <see cref="IConfigurationOrigin{TOptions}"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ConfigurationOrigin"/> class.
/// </remarks>
/// <param name="options">
/// Configuration origin options.
/// </param>
public sealed class ConfigurationOrigin(ConfigurationOriginOptions? options = null) : IConfigurationOrigin<ConfigurationOriginOptions>
{
    #region Fields
    private IConfiguration? _configuration;
    #endregion

    #region Properties
    /// <summary>
    /// Default <see cref="ConfigurationOrigin"/> instance.
    /// </summary>
    public static ConfigurationOrigin Default { get; } = new(ConfigurationOriginOptions.Default);

    /// <inheritdoc/>
    public ConfigurationOriginOptions Options { get; } = options ?? ConfigurationOriginOptions.Default;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public IConfiguration GetConfiguration()
    {
        if (_configuration is not null)
            return _configuration;

        var builder = new ConfigurationBuilder()
            .SetBasePath(Options.JsonFilePath);

        if (Options.AddEnvironmentVariables)
            builder.AddEnvironmentVariables();

        if (Options.AddCmmandLineArguments)
            builder.AddCommandLine(Environment.GetCommandLineArgs());

        builder.AddJsonFile(Options.DefaultJsonConfigurationName, false, true);
        builder.AddJsonFile(Options.EnvironmentJsonConfigurationName, true, true);

        _configuration = builder.Build();

        return _configuration;
    }
    #endregion
}