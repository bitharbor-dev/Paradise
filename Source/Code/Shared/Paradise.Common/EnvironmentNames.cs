using Paradise.Common.Extensions;
using Paradise.Localization.ExceptionsHandling;
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
    /// Default environment variable name for "EnvironmentName" in .NET or .NET Core applications.
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
    /// "Development.Docker" environment name.
    /// </summary>
    public const string DevelopmentDocker = "Development.Docker";

    /// <summary>
    /// "Staging.Docker" environment name.
    /// </summary>
    public const string StagingDocker = "Staging.Docker";

    /// <summary>
    /// "Production.Docker" environment name.
    /// </summary>
    public const string ProductionDocker = "Production.Docker";
    #endregion

    #region Fields
    private static readonly IEnumerable<string> _allowedEnvironments =
        [Development, Staging, Production, DevelopmentDocker, StagingDocker, ProductionDocker];
    #endregion

    #region Properties
    /// <summary>
    /// Gets the current environment name.
    /// </summary>
    public static string Current { get; } = GetCurrentEnvironmentName();
    #endregion

    #region Private methods
    /// <summary>
    /// Gets the current environment name.
    /// </summary>
    /// <returns>
    /// A <see langword="string"/> value containing environment name.
    /// </returns>
    private static string GetCurrentEnvironmentName()
    {
        var result = GetEnvironmentVariable(DotNetCoreEnvironmentVariableName)
                  ?? GetEnvironmentVariable(AspNetCoreEnvironmentVariableName);

        if (result.IsNullOrWhiteSpace() || !_allowedEnvironments.Contains(result))
        {
            var message = ExceptionMessagesProvider.GetInvalidEnvironmentNameMessage(_allowedEnvironments, result);

            throw new InvalidOperationException(message);
        }

        return result;
    }
    #endregion
}