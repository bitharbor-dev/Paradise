using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using static Paradise.Localization.ErrorHandling.ErrorMessages;
using static System.Text.CompositeFormat;

namespace Paradise.Models;

/// <summary>
/// Represents an application error.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ApplicationError"/> structure.
/// </remarks>
/// <param name="Code">
/// Error code.
/// </param>
/// <param name="Description">
/// Error description.
/// </param>
[method: JsonConstructor]
public readonly record struct ApplicationError(ErrorCode Code, string Description)
{
    #region Fields
    private static readonly CompositeFormat _toStringFormat = Parse(ApplicationErrorToStringFormat);
    #endregion

    #region Public methods
    /// <summary>
    /// Returns the string representation of the current instance.
    /// </summary>
    /// <returns>
    /// Human readable string, representing the error information:
    /// <c>
    /// 'Code: 1 | Description: Some error information.'
    /// </c>
    /// </returns>
    public override string ToString()
    {
        var codeNumber = (int)Code;

        return string.Format(CultureInfo.CurrentCulture,
                             _toStringFormat,
                             codeNumber,
                             Description);
    }
    #endregion

    #region Operators
    /// <summary>
    /// Implicitly converts the given <paramref name="error"/>
    /// into a <see cref="string"/> by calling <see cref="ToString"/> method.
    /// </summary>
    /// <param name="error">
    /// The <see cref="ApplicationError"/> to be converted into a <see cref="string"/>.
    /// </param>
    public static implicit operator string(ApplicationError error)
        => error.ToString();
    #endregion
}