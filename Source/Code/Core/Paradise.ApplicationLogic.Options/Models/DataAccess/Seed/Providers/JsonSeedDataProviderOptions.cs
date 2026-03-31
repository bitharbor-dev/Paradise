using Paradise.Common.Extensions;

namespace Paradise.ApplicationLogic.Options.Models.DataAccess.Seed.Providers;

/// <summary>
/// JSON seed data provider options.
/// </summary>
public sealed class JsonSeedDataProviderOptions
{
    #region Properties
    /// <summary>
    /// The path to the directory which contains seed data JSON files.
    /// </summary>
    /// <remarks>
    /// Can start with <c>{ApplicationRoot}</c> to specify the application's running directory.
    /// <para>
    /// Do not use directly, call <see cref="ResolveSeedDirectoryPath"/> to get the final value.
    /// </para>
    /// </remarks>
    public string? SeedDirectoryPath { get; set; }
    #endregion

    #region Public methods
    /// <summary>
    /// Resolves the <see cref="SeedDirectoryPath"/> value by checking if it starts
    /// with <c>{ApplicationRoot}</c> placeholder
    /// and replaces it with <see cref="AppContext.BaseDirectory"/> value.
    /// </summary>
    /// <returns>
    /// The <see cref="SeedDirectoryPath"/> final value.
    /// </returns>
    public string? ResolveSeedDirectoryPath()
    {
        const string ApplicationRootPlaceholder = "{ApplicationRoot}";

        if (SeedDirectoryPath.IsNullOrWhiteSpace())
            return null;

        if (SeedDirectoryPath.StartsWith(ApplicationRootPlaceholder, StringComparison.Ordinal))
        {
            var remainder = SeedDirectoryPath[ApplicationRootPlaceholder.Length..]
                .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            return Path.Combine(AppContext.BaseDirectory, remainder);
        }
        else
        {
            return SeedDirectoryPath;
        }
    }
    #endregion
}