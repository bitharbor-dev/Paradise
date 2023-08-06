using Microsoft.AspNetCore.Identity;
using Paradise.Common.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Paradise.ApplicationLogic.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="string"/> class.
/// </summary>
public static class StringExtensions
{
    #region Public methods
    /// <summary>
    /// Checks if the given <paramref name="email"/> address is valid.
    /// </summary>
    /// <param name="email">
    /// The email address to be validated.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given <paramref name="email"/> address has a valid format,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool IsValidEmailAddress(this string email)
        => new EmailAddressAttribute().IsValid(email);

    /// <summary>
    /// Checks if the given <paramref name="phone"/> is valid.
    /// </summary>
    /// <param name="phone">
    /// Phone number to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given <paramref name="phone"/> has a valid format,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool IsValidPhoneNumber(this string phone)
        => new PhoneAttribute().IsValid(phone);

    /// <summary>
    /// Checks if the given <paramref name="userName"/> is valid.
    /// </summary>
    /// <param name="userName">
    /// User-name to be checked.
    /// </param>
    /// <param name="options">
    /// The <see cref="IdentityOptions"/> to be used to
    /// validate the <paramref name="userName"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given <paramref name="userName"/> has a valid format,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool IsValidUserName(this string userName, IdentityOptions options)
    {
        if (userName.IsNullOrWhiteSpace())
            return false;

        var allowedCharacters = options.User.AllowedUserNameCharacters;

        return string.IsNullOrWhiteSpace(allowedCharacters) || userName.All(allowedCharacters.Contains);
    }
    #endregion
}