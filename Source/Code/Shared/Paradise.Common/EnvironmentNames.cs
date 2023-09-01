using Paradise.Common.Extensions;
using Paradise.Localization.ExceptionsHandling;
using System.Globalization;
using static System.Environment;

namespace Paradise.Common;

/// <summary>
/// Contains predefined application environment names.
/// </summary>
public static class EnvironmentNames
{
    #region Constants
    /// <summary>
    /// Default environment variable name for "EnvironmentName" in ASP.NET Core applications.
    /// </summary>
    private const string AspNetCoreEnvironmentVariableName = "ASPNETCORE_ENVIRONMENT";

    /// <summary>
    /// Default environment variable name for "EnvironmentName" in .NET Core applications.
    /// </summary>
    private const string DotNetCoreEnvironmentVariableName = "DOTNET_ENVIRONMENT";

    /// <summary>
    /// "Development" environment name.
    /// </summary>
    public const string Development = "Development";

    /// <summary>
    /// "Staging" environment name.
    /// </summary>
    public const string Staging = "Staging";

    /// <summary>
    /// "Production" environment name.
    /// </summary>
    public const string Production = "Production";

    /// <summary>
    /// "Docker.Development" environment name.
    /// </summary>
    public const string DockerDevelopment = "Docker.Development";

    /// <summary>
    /// "Docker.Staging" environment name.
    /// </summary>
    public const string DockerStaging = "Docker.Staging";

    /// <summary>
    /// "Docker.Production" environment name.
    /// </summary>
    public const string DockerProduction = "Docker.Production";
    #endregion

    #region Fields
    private static readonly IEnumerable<string> _allowedEnvironments = new[]
    {
        Development,
        Staging,
        Production,
        DockerDevelopment,
        DockerStaging,
        DockerProduction
    };

    private static string? _current;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the current environment name.
    /// </summary>
    public static string Current
    {
        get
        {
            if (_current is null)
            {
                _current = GetEnvironmentVariable(DotNetCoreEnvironmentVariableName)
                        ?? GetEnvironmentVariable(AspNetCoreEnvironmentVariableName);

                if (_current.IsNullOrWhiteSpace() || !_allowedEnvironments.Contains(_current))
                {
                    var environments = string.Join(", ", _allowedEnvironments);

                    var message = string.Format(CultureInfo.CurrentCulture, ExceptionMessages.InvalidEnvironmentName, _current, environments);

                    throw new InvalidOperationException(message);
                }
            }

            return _current;
        }
    }
    #endregion
}