using Microsoft.AspNetCore.Identity;
using Paradise.Localization.ModelsLocalization;
using System.Globalization;
using static System.Environment;

namespace Paradise.ApplicationLogic.Identity;

/// <summary>
/// <inheritdoc/>
/// </summary>
public sealed class IdentityException : Exception
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityException"/> class.
    /// </summary>
    internal IdentityException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityException"/> class.
    /// </summary>
    /// <param name="result">
    /// The <see cref="IdentityResult"/> to be used
    /// to format exception message.
    /// </param>
    public IdentityException(IdentityResult result) : base(CreateExceptionMessage(result)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">
    /// <inheritdoc/>
    /// </param>
    internal IdentityException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityException"/> class
    /// with a specified error message and a reference to the inner exception that is the
    /// cause of this exception.
    /// </summary>
    /// <param name="message">
    /// <inheritdoc/>
    /// </param>
    /// <param name="innerException">
    /// <inheritdoc/>
    /// </param>
    internal IdentityException(string message, Exception innerException) : base(message, innerException) { }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates an exception message from the given <paramref name="result"/>.
    /// </summary>
    /// <param name="result">
    /// The <see cref="IdentityResult"/> to be used
    /// to format exception message.
    /// </param>
    /// <returns>
    /// A <see cref="string"/> value containing the exception message.
    /// </returns>
    private static string CreateExceptionMessage(IdentityResult result)
        => string.Join(NewLine, result.Errors.Select(ErrorMessageSelector));

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
        var messageFormat = ModelsLocalizationMessages.ErrorToStringFormat;

        return string.Format(CultureInfo.CurrentCulture,
                             messageFormat,
                             error.Code,
                             error.Description);
    }
    #endregion
}