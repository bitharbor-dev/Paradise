using Paradise.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.UserModels;

/// <summary>
/// User login schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserLoginModel"/> class.
/// </remarks>
/// <param name="password">
/// User's password.
/// </param>
[method: JsonConstructor]
[RequiredAtLeastOne(true, nameof(UserName), nameof(Email), nameof(Phone))]
public sealed class UserLoginModel(string password)
{
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
    public string Password { get; set; } = password;
    #endregion
}