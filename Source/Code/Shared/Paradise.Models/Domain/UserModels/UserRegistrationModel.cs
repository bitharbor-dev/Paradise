using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.UserModels;

/// <summary>
/// User registration schema.
/// </summary>
public sealed class UserRegistrationModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="UserRegistrationModel"/> class.
    /// </summary>
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
    [JsonConstructor]
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructors not working with constructor attributes.")]
    public UserRegistrationModel(string email, string password, string passwordConfirmation, string userName)
    {
        UserName = userName;
        Email = email;
        Password = password;
        PasswordConfirmation = passwordConfirmation;
    }
    #endregion

    #region Properties
    /// <summary>
    /// User's user-name.
    /// </summary>
    [Required, DataType(DataType.Text)]
    public string UserName { get; set; }

    /// <summary>
    /// User's email address.
    /// </summary>
    [Required, EmailAddress, DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    /// <summary>
    /// User's phone number.
    /// </summary>
    [Phone, DataType(DataType.PhoneNumber)]
    public string? Phone { get; set; }

    /// <summary>
    /// User's password.
    /// </summary>
    [Required, DataType(DataType.Password)]
    public string Password { get; set; }

    /// <summary>
    /// User's password confirmation.
    /// </summary>
    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    public string PasswordConfirmation { get; set; }
    #endregion
}