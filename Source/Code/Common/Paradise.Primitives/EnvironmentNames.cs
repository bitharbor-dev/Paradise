using Paradise.Localization.ExceptionHandling;

namespace Paradise.Primitives;

/// <summary>
/// Contains predefined application environment names.
/// </summary>
public static class EnvironmentNames
{
    #region Constants
    /// <summary>
    /// The "Development" environment name.
    /// </summary>
    public const string Development = "Development";

    /// <summary>
    /// The "Development.Docker" environment name.
    /// </summary>
    public const string DevelopmentDocker = "Development.Docker";

    /// <summary>
    /// The "Staging" environment name.
    /// </summary>
    public const string Staging = "Staging";

    /// <summary>
    /// The "Staging.Docker" environment name.
    /// </summary>
    public const string StagingDocker = "Staging.Docker";

    /// <summary>
    /// The "Production" environment name.
    /// </summary>
    public const string Production = "Production";

    /// <summary>
    /// The "Production.Docker" environment name.
    /// </summary>
    public const string ProductionDocker = "Production.Docker";
    #endregion

    #region Properties
    /// <summary>
    /// The list of allowed environment names.
    /// </summary>
    public static IEnumerable<string> AllowedEnvironments { get; } =
    [
        Development,
        DevelopmentDocker,
        Staging,
        StagingDocker,
        Production,
        ProductionDocker
    ];
    #endregion

    #region Public methods
    /// <summary>
    /// Indicates whether the given <paramref name="environmentName"/>
    /// belongs to the "Development" environments group.
    /// </summary>
    /// <param name="environmentName">
    /// The environment name to check.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given <paramref name="environmentName"/>
    /// belongs to the "Development" environments group,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool IsDevelopment(string environmentName)
        => environmentName is Development or DevelopmentDocker;

    /// <summary>
    /// Indicates whether the given <paramref name="environmentName"/>
    /// belongs to the "Staging" environments group.
    /// </summary>
    /// <param name="environmentName">
    /// The environment name to check.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given <paramref name="environmentName"/>
    /// belongs to the "Staging" environments group,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool IsStaging(string environmentName)
        => environmentName is Staging or StagingDocker;

    /// <summary>
    /// Indicates whether the given <paramref name="environmentName"/>
    /// belongs to the "Production" environments group.
    /// </summary>
    /// <param name="environmentName">
    /// The environment name to check.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given <paramref name="environmentName"/>
    /// belongs to the "Production" environments group,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool IsProduction(string environmentName)
        => environmentName is Production or ProductionDocker;

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if the given
    /// <paramref name="environmentName"/> is not in the list of allowed environment names.
    /// </summary>
    /// <param name="environmentName">
    /// The environment name to check.
    /// </param>
    public static void ThrowIfNotAllowedEnvironment(string environmentName)
    {
        if (!AllowedEnvironments.Contains(environmentName))
        {
            var message = ExceptionMessagesProvider.GetMessageInvalidEnvironmentName(environmentName, AllowedEnvironments);

            throw new InvalidOperationException(message);
        }
    }
    #endregion
}