using Microsoft.Extensions.Configuration;
using static Paradise.Localization.ExceptionHandling.ExceptionMessages;

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
    /// <para>
    /// Fails on non-existing configuration section.
    /// </para>
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
        ArgumentNullException.ThrowIfNull(configuration);

        var sectionName = typeof(T).Name;
        var section = configuration.GetRequiredSection(sectionName);

        section.Bind(instance);
    }

    /// <summary>
    /// Attempts to bind the given object <paramref name="instance"/>
    /// to configuration values by matching property names against configuration keys in the configuration section
    /// with the name of the <paramref name="instance"/> type name.
    /// <para>
    /// Skips on non-existing configuration section.
    /// </para>
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
    public static void BindOptionalSection<T>(this IConfiguration configuration, T instance)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var sectionName = typeof(T).Name;
        var section = configuration.GetSection(sectionName);

        if (!section.Exists())
            return;

        section.Bind(instance);
    }

    /// <summary>
    /// Gets the value of type <typeparamref name="T"/>
    /// from the input <paramref name="configuration"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Value type.
    /// </typeparam>
    /// <param name="configuration">
    /// The configuration instance containing the target configuration section.
    /// </param>
    /// <returns>
    /// The value of type <typeparamref name="T"/>,
    /// retrieved from the input <paramref name="configuration"/>.
    /// </returns>
    public static T GetRequiredInstance<T>(this IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var instanceType = typeof(T);

        var instance = configuration.GetSection(instanceType.Name).Get<T>();

        if (instance is null)
        {
            var message = GetMessageFailedToCreateInstanceOfType(instanceType);

            throw new InvalidOperationException(message);
        }

        return instance;
    }
    #endregion
}