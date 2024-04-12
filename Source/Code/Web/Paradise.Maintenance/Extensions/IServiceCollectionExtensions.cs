using Paradise.Maintenance.Options.Base;

namespace Paradise.Maintenance.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
internal static class IServiceCollectionExtensions
{
    #region Public methods
    /// <summary>
    /// Adds worker options descriptor to the given <paramref name="services"/>.
    /// </summary>
    /// <typeparam name="TOptions">
    /// Worker options type.
    /// </typeparam>
    /// <param name="services">
    /// Target collection of service descriptors.
    /// </param>
    /// <param name="configuration">
    /// The source of configuration values.
    /// </param>
    public static void AddWorkerOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : WorkerOptionsBase
    {
        var optionsType = typeof(TOptions);
        var optionsName = optionsType.Name;

        var optionsConfiguration = configuration.GetRequiredSection(optionsName);

        services
            .AddOptions<TOptions>()
            .Bind(optionsConfiguration)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
    #endregion
}