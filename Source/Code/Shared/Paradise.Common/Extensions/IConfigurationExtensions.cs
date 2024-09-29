using Microsoft.Extensions.Configuration;

namespace Paradise.Common.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IConfiguration"/> <see langword="interface"/>.
/// </summary>
public static class IConfigurationExtensions
{
    #region Public methods
    /// <summary>
    /// Attempts to bind the given object <paramref name="instance"/>
    /// to configuration values by matching property names against configuration keys in the configuration section
    /// with the name of the <paramref name="instance"/> type name.
    /// </summary>
    /// <typeparam name="T">
    /// Instance type which name is used as a section name.
    /// </typeparam>
    /// <param name="configuration">
    /// The configuration instance containing the target configuration section.
    /// </param>
    /// <param name="instance">
    /// The object to bind.
    /// </param>
    public static void BindSection<T>(this IConfiguration configuration, T instance)
    {
        var sectionName = typeof(T).Name;
        var section = configuration.GetRequiredSection(sectionName);

        section.Bind(instance);
    }
    #endregion
}