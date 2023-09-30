using Microsoft.Extensions.Configuration;
using Paradise.Options.Origins.Base;
using Paradise.Options.Origins.Options;

namespace Paradise.Options.Origins;

/// <summary>
/// A JSON file configuration origin implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="JsonConfigurationOrigin"/> class.
/// </remarks>
/// <param name="options">
/// Configuration origin options.
/// </param>
public sealed class JsonConfigurationOrigin(JsonConfigurationOriginOptions? options = null) : IConfigurationOrigin<JsonConfigurationOriginOptions>
{
    #region Fields
    private IConfiguration? _configuration;
    #endregion

    #region Properties
    /// <summary>
    /// Default <see cref="JsonConfigurationOrigin"/> instance.
    /// </summary>
    public static JsonConfigurationOrigin Default { get; } = new(JsonConfigurationOriginOptions.Default);

    /// <inheritdoc/>
    public JsonConfigurationOriginOptions Options { get; } = options ?? JsonConfigurationOriginOptions.Default;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public IConfiguration GetConfiguration()
    {
        if (_configuration is not null)
            return _configuration;

        var builder = new ConfigurationBuilder()
            .SetBasePath(Options.FilePath);

        if (Options.AddEnvironmentVariables)
            builder.AddEnvironmentVariables();

        builder.AddJsonFile(Options.DefaultConfigurationName, false, true);
        builder.AddJsonFile(Options.EnvironmentConfigurationName, true, true);

        _configuration = builder.Build();

        return _configuration;
    }
    #endregion
}