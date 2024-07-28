using Paradise.Common;

namespace Paradise.Options.Origins.Options;

/// <summary>
/// Configuration origin options.
/// </summary>
public sealed class ConfigurationOriginOptions
{
    #region Constants
    /// <summary>
    /// JSON file extension.
    /// </summary>
    private const string JsonFileExtension = "json";

    /// <summary>
    /// Default options file name.
    /// </summary>
    private const string DefaultOptionsFileName = "options";
    #endregion

    #region Properties
    /// <summary>
    /// Default <see cref="ConfigurationOriginOptions"/> instance.
    /// </summary>
    public static ConfigurationOriginOptions Default { get; } = new();

    /// <summary>
    /// Environment name.
    /// </summary>
    public string EnvironmentName { get; set; } = EnvironmentNames.Current;

    /// <summary>
    /// JSON configuration file path.
    /// </summary>
    public string JsonFilePath { get; set; } = AppContext.BaseDirectory;

    /// <summary>
    /// JSON configuration file name.
    /// </summary>
    /// <remarks>
    /// Specify it without extension - ".json".
    /// </remarks>
    public string JsonFileName { get; set; } = DefaultOptionsFileName;

    /// <summary>
    /// Indicates whether the environment variables should be included
    /// into configuration.
    /// </summary>
    public bool AddEnvironmentVariables { get; set; } = true;

    /// <summary>
    /// Indicates whether the CLI arguments should be included
    /// into configuration.
    /// </summary>
    public bool AddCmmandLineArguments { get; set; } = true;

    /// <summary>
    /// Default JSON configuration file name.
    /// </summary>
    public string DefaultJsonConfigurationName
        => $"{JsonFileName}.{JsonFileExtension}";

    /// <summary>
    /// Environment-specific JSON configuration file name.
    /// </summary>
    public string EnvironmentJsonConfigurationName
        => $"{JsonFileName}.{EnvironmentName}.{JsonFileExtension}";
    #endregion
}