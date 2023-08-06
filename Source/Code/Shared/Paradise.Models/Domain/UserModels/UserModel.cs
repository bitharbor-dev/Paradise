using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.UserModels;

/// <summary>
/// Represents an application user.
/// </summary>
public sealed class UserModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="UserModel"/> class.
    /// </summary>
    /// <param name="email">
    /// User's email address.
    /// </param>
    /// <param name="userName">
    /// User's user-name.
    /// </param>
    [JsonConstructor]
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructors not working with constructor attributes.")]
    public UserModel(string email, string userName)
    {
        Email = email;
        UserName = userName;
    }
    #endregion

    #region Properties
    /// <summary>
    /// User Id.
    /// </summary>
    public Guid Id { get; set; }

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
    /// User phone number.
    /// </summary>
    [Phone, DataType(DataType.PhoneNumber)]
    public string? Phone { get; set; }

    /// <summary>
    /// Indicates whether the user's email address has been confirmed.
    /// </summary>
    public bool IsEmailConfirmed { get; set; }

    /// <summary>
    /// Indicates whether the user is pending deletion.
    /// </summary>
    public bool IsPendingDeletion { get; set; }

    /// <summary>
    /// Indicates whether two-factor authentication is enabled for the user.
    /// </summary>
    public bool IsTwoFactorEnabled { get; set; }

    /// <summary>
    /// User registration date.
    /// </summary>
    [DataType(DataType.DateTime)]
    public DateTime RegistrationDate { get; set; }
    #endregion
}