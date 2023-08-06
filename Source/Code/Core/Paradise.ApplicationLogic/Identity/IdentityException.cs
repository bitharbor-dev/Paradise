using Microsoft.AspNetCore.Identity;
using Paradise.Localization.ModelsLocalization;
using System.Globalization;
using static System.Environment;

namespace Paradise.ApplicationLogic.Identity;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="IdentityException"/> class.
/// </remarks>
/// <param name="result">
/// The <see cref="IdentityResult"/> to be used
/// to format exception message.
/// </param>
public sealed class IdentityException(IdentityResult result) : Exception(string.Join(NewLine, result.Errors.Select(ErrorMessageSelector)))
{
    #region Private methods
    /// <summary>
    /// Converts the given <paramref name="error"/> into a <see cref="string"/>.
    /// </summary>
    /// <param name="error">
    /// The <see cref="IdentityError"/> to be converted.
    /// </param>
    /// <returns>
    /// A <see cref="string"/> representation
    /// of the given <paramref name="error"/> instance.
    /// </returns>
    private static string ErrorMessageSelector(IdentityError error)
    {
        var formatString = ModelsLocalizationMessages.ErrorToStringFormat;

        return string.Format(CultureInfo.CurrentCulture, formatString, error.Code, error.Description);
    }
    #endregion
}