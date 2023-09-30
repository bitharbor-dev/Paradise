using Paradise.Common;

namespace Paradise.Options.Origins.Options;

/// <summary>
/// JSON configuration origin options.
/// </summary>
public sealed class JsonConfigurationOriginOptions
{
    #region Constants
    /// <summary>
    /// JSON file extension.
    /// </summary>
    private const string JsonFileExtension = "json";
    #endregion

    #region Properties
    /// <summary>
    /// Default <see cref="JsonConfigurationOriginOptions"/> instance.
    /// </summary>
    public static JsonConfigurationOriginOptions Default { get; } = new();

    /// <summary>
    /// Environment name.
    /// </summary>
    public string EnvironmentName { get; set; } = EnvironmentNames.Current;

    /// <summary>
    /// Configuration file path.
    /// </summary>
    public string FilePath { get; set; } = AppContext.BaseDirectory;

    /// <summary>
    /// Configuration file name.
    /// </summary>
    /// <remarks>
    /// Specify it without extension - ".json".
    /// </remarks>
    public string FileName { get; set; } = "options";

    /// <summary>
    /// Indicates whether the environment variables should be included
    /// into configuration.
    /// </summary>
    public bool AddEnvironmentVariables { get; set; } = true;

    /// <summary>
    /// Default configuration file name.
    /// </summary>
    public string DefaultConfigurationName
        => $"{FileName}.{JsonFileExtension}";

    /// <summary>
    /// Environment-specific configuration file name.
    /// </summary>
    public string EnvironmentConfigurationName
        => $"{FileName}.{EnvironmentName}.{JsonFileExtension}";
    #endregion
}