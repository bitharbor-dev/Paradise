using Paradise.Common.Extensions;
using Paradise.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Paradise.Models.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="ErrorCode"/> enumeration.
/// </summary>
public static class ErrorCodeExtensions
{
    #region Public methods
    /// <summary>
    /// Gets the formatted error description from the given
    /// <paramref name="errorCode"/> and <paramref name="args"/>.
    /// </summary>
    /// <param name="errorCode">
    /// <see cref="ErrorCode"/> that references
    /// the corresponding error description.
    /// </param>
    /// <param name="args">
    /// An object array that contains zero or more objects to format.
    /// </param>
    /// <returns>
    /// The formatted error description.
    /// </returns>
    public static string GetFormattedErrorDescription(this ErrorCode errorCode, params object?[] args)
    {
        var errorDisplayValue = errorCode.GetErrorDescriptionFormat();

        return errorDisplayValue.IsNullOrWhiteSpace()
            ? errorCode.ToString()
            : args.Length is not 0
            ? string.Format(CultureInfo.CurrentCulture, errorDisplayValue, args)
            : errorDisplayValue;
    }
    #endregion

    #region Internal methods
    /// <summary>
    /// Gets the <see cref="bool"/> value indicating whether
    /// the given <paramref name="errorCode"/> is critical.
    /// </summary>
    /// <param name="errorCode">
    /// Error code.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given <paramref name="errorCode"/> is critical,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    internal static bool GetIsCritical(this ErrorCode errorCode)
        => errorCode.GetAttributeOfType<IsCriticalAttribute>()?.Value ?? false;
    #endregion

    #region Private methods
    /// <summary>
    /// Gets the unformatted error description value.
    /// </summary>
    /// <param name="errorCode">
    /// <see cref="ErrorCode"/> that references
    /// the corresponding error description.
    /// </param>
    /// <returns>
    /// Unformatted error description value.
    /// </returns>
    private static string? GetErrorDescriptionFormat(this ErrorCode errorCode)
        => errorCode.GetAttributeOfType<DisplayAttribute>()?.GetName();

    /// <summary>
    /// Gets the attribute of type <typeparamref name="T"/>
    /// assigned to the given <paramref name="errorCode"/> instance.
    /// </summary>
    /// <typeparam name="T">
    /// Attribute type.
    /// </typeparam>
    /// <param name="errorCode">
    /// The <see cref="ErrorCode"/> instance from which
    /// the attribute to be retrieved.
    /// </param>
    /// <returns>
    /// The attribute of type <typeparamref name="T"/> or <see langword="null"/>.
    /// </returns>
    private static T? GetAttributeOfType<T>(this ErrorCode errorCode) where T : Attribute
        => errorCode.GetType()
        .GetMember(errorCode.ToString())
        .FirstOrDefault()?
        .GetCustomAttribute<T>();
    #endregion
}