using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Common.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="string"/> <see langword="class"/>.
/// </summary>
public static class StringExtensions
{
    #region Public methods
    /// <inheritdoc cref="string.IsNullOrWhiteSpace(string?)"/>
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? value)
        => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Indicates whether a specified string is not <see langword="null"/>, empty,
    /// or consists only of white-space characters.
    /// </summary>
    /// <param name="value">
    /// The <see cref="string"/> to test.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="value"/> parameter is not
    /// <see langword="null"/> or <see cref="string.Empty"/>, or if <paramref name="value"/>
    /// does not consist exclusively of white-space characters.
    /// </returns>
    public static bool IsNotNullOrWhiteSpace([NotNullWhen(true)] this string? value)
        => !string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Replaces all '/' and '\' characters with <see cref="Path.DirectorySeparatorChar"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="string"/> to be sanitized.
    /// </param>
    /// <returns>
    /// A new <see cref="string"/> value equivalent to the given <paramref name="value"/>,
    /// except that all instances of '/' and '\' are replaced with <see cref="Path.DirectorySeparatorChar"/>.
    /// </returns>
    public static string SanitizePathSeparators(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
    }

    /// <summary>
    /// Checks if the given <paramref name="emailAddress"/> address is valid.
    /// </summary>
    /// <param name="emailAddress">
    /// The email address to be validated.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given <paramref name="emailAddress"/> address has a valid format,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool IsValidEmailAddress(this string emailAddress)
        => new EmailAddressAttribute().IsValid(emailAddress);

    /// <summary>
    /// Checks if the given <paramref name="phoneNumber"/> is valid.
    /// </summary>
    /// <param name="phoneNumber">
    /// Phone number to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given <paramref name="phoneNumber"/> has a valid format,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool IsValidPhoneNumber(this string phoneNumber)
        => new PhoneAttribute().IsValid(phoneNumber);

    /// <summary>
    /// Checks if the given <paramref name="userName"/> is valid.
    /// </summary>
    /// <param name="userName">
    /// User-name to be checked.
    /// </param>
    /// <param name="allowedCharacters">
    /// The list of allowed characters in the user-name used to
    /// validate <paramref name="userName"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given <paramref name="userName"/> has a valid format,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool IsValidUserName(this string userName, string allowedCharacters)
        => userName.IsNotNullOrWhiteSpace()
        && (allowedCharacters.IsNullOrWhiteSpace() || userName.All(allowedCharacters.Contains));
    #endregion
}