using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.UserModels;

/// <summary>
/// User registration schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserRegistrationModel"/> class.
/// </remarks>
/// <param name="userName">
/// User's user-name.
/// </param>
/// <param name="email">
/// User's email address.
/// </param>
/// <param name="password">
/// User's password.
/// </param>
/// <param name="passwordConfirmation">
/// User's password confirmation.
/// </param>
[method: JsonConstructor]
public sealed class UserRegistrationModel(string email, string password, string passwordConfirmation, string userName)
{
    #region Properties
    /// <summary>
    /// User's user-name.
    /// </summary>
    [Required, DataType(DataType.Text)]
    public string UserName { get; set; } = userName;

    /// <summary>
    /// User's email address.
    /// </summary>
    [Required, EmailAddress, DataType(DataType.EmailAddress)]
    public string Email { get; set; } = email;

    /// <summary>
    /// User's phone number.
    /// </summary>
    [Phone, DataType(DataType.PhoneNumber)]
    public string? Phone { get; set; }

    /// <summary>
    /// User's password.
    /// </summary>
    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = password;

    /// <summary>
    /// User's password confirmation.
    /// </summary>
    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    public string PasswordConfirmation { get; set; } = passwordConfirmation;
    #endregion
}