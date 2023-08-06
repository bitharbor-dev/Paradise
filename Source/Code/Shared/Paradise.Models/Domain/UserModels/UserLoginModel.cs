using Paradise.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.UserModels;

/// <summary>
/// User login schema.
/// </summary>
[RequiredAtLeastOne(true, nameof(UserName), nameof(Email), nameof(Phone))]
public sealed class UserLoginModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="UserLoginModel"/> class.
    /// </summary>
    /// <param name="password">
    /// User's password.
    /// </param>
    [JsonConstructor]
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructors not working with constructor attributes.")]
    public UserLoginModel(string password)
        => Password = password;
    #endregion

    #region Properties
    /// <summary>
    /// User's user-name.
    /// </summary>
    [DataType(DataType.Text)]
    public string? UserName { get; set; }

    /// <summary>
    /// User's email address.
    /// </summary>
    [EmailAddress, DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

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
    #endregion
}