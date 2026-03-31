using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.Identity.Users;

/// <summary>
/// User registration schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserRegistrationModel"/> class.
/// </remarks>
/// <param name="userName">
/// User's user-name.
/// </param>
/// <param name="emailAddress">
/// User's email address.
/// </param>
/// <param name="phoneNumber">
/// User's phone number.
/// </param>
/// <param name="password">
/// User's password.
/// </param>
/// <param name="passwordConfirmation">
/// User's password confirmation.
/// </param>
[method: JsonConstructor]
public sealed class UserRegistrationModel(string userName, string emailAddress, string? phoneNumber,
                                          string password, string passwordConfirmation)
{
    #region Properties
    /// <summary>
    /// User's user-name.
    /// </summary>
    [DataType(DataType.Text)]
    public string UserName { get; } = userName;

    /// <summary>
    /// User's email address.
    /// </summary>
    [EmailAddress, DataType(DataType.EmailAddress)]
    public string EmailAddress { get; } = emailAddress;

    /// <summary>
    /// User's phone number.
    /// </summary>
    [Phone, DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; } = phoneNumber;

    /// <summary>
    /// User's password.
    /// </summary>
    [DataType(DataType.Password)]
    public string Password { get; } = password;

    /// <summary>
    /// User's password confirmation.
    /// </summary>
    [DataType(DataType.Password), Compare(nameof(Password))]
    public string PasswordConfirmation { get; } = passwordConfirmation;
    #endregion
}