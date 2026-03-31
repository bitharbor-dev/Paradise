using Paradise.Common.Extensions;
using Paradise.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Paradise.Models.Extensions;

/// <summary>
/// Contains extension methods for enumerations.
/// </summary>
public static class EnumExtensions
{
    #region Public methods
    /// <summary>
    /// Gets the formatted value of the <see cref="DisplayAttribute.Name"/> property
    /// from the attribute assigned to the given <paramref name="enum"/>.
    /// </summary>
    /// <typeparam name="T">
    /// <see langword="enum"/> type.
    /// </typeparam>
    /// <param name="enum">
    /// Target <see langword="enum"/>.
    /// </param>
    /// <param name="arguments">
    /// An object array that contains zero or more objects to format.
    /// </param>
    /// <returns>
    /// The formatted display value.
    /// </returns>
    public static string GetFormattedDisplayValue<T>(this T @enum, params object?[] arguments)
        where T : Enum
    {
        var errorDisplayValue = @enum.GetDisplayValue();

        return errorDisplayValue.IsNullOrWhiteSpace()
            ? @enum.ToString()
            : arguments is not null && arguments.Length is not 0
            ? string.Format(CultureInfo.CurrentCulture, errorDisplayValue, arguments)
            : errorDisplayValue;
    }
    #endregion

    #region Internal methods
    /// <summary>
    /// Gets the value of the <see cref="IsCriticalAttribute.Value"/> property
    /// from the attribute assigned to the given <paramref name="enum"/>.
    /// </summary>
    /// <typeparam name="T">
    /// <see langword="enum"/> type.
    /// </typeparam>
    /// <param name="enum">
    /// Target <see langword="enum"/>.
    /// </param>
    /// <returns>
    /// The value of the <see cref="IsCriticalAttribute.Value"/> property
    /// from the attribute assigned to the given <paramref name="enum"/>
    /// or <see langword="false"/> if not assigned.
    /// </returns>
    internal static bool GetIsCritical<T>(this T @enum)
        where T : Enum
        => @enum.GetAttributeOfType<IsCriticalAttribute, T>()?.Value ?? false;
    #endregion

    #region Private methods
    /// <summary>
    /// Gets the value of the <see cref="DisplayAttribute.Name"/> property
    /// from the attribute assigned to the given <paramref name="enum"/>.
    /// </summary>
    /// <typeparam name="T">
    /// <see langword="enum"/> type.
    /// </typeparam>
    /// <param name="enum">
    /// Target <see langword="enum"/>.
    /// </param>
    /// <returns>
    /// The value of the <see cref="DisplayAttribute.Name"/> property
    /// from the attribute assigned to the given <paramref name="enum"/>
    /// or <see cref="string.Empty"/>.
    /// </returns>
    private static string GetDisplayValue<T>(this T @enum)
        where T : Enum
    {
        return @enum.GetAttributeOfType<DisplayAttribute, T>()?
            .GetName() ?? string.Empty;
    }

    /// <summary>
    /// Gets the attribute of type <typeparamref name="TAttribute"/>
    /// assigned to the given <paramref name="enum"/>.
    /// </summary>
    /// <typeparam name="TAttribute">
    /// Attribute type.
    /// </typeparam>
    /// <typeparam name="TTarget">
    /// Enum type.
    /// </typeparam>
    /// <param name="enum">
    /// Target <see langword="enum"/>.
    /// </param>
    /// <returns>
    /// The attribute of type <typeparamref name="TAttribute"/> or <see langword="null"/>.
    /// </returns>
    private static TAttribute? GetAttributeOfType<TAttribute, TTarget>(this TTarget @enum)
        where TAttribute : Attribute
        where TTarget : Enum
    {
        return @enum.GetType()
            .GetMember(@enum.ToString())[0]
            .GetCustomAttribute<TAttribute>();
    }
    #endregion
}