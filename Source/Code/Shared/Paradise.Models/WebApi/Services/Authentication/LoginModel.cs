using Paradise.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.WebApi.Services.Authentication;

/// <summary>
/// Login schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LoginModel"/> class.
/// </remarks>
/// <param name="password">
/// User's password.
/// </param>
[method: JsonConstructor]
[RequiredAtLeastOne(restrictEmptyOrWhitespaceStrings: true, nameof(UserName), nameof(EmailAddress), nameof(PhoneNumber))]
public sealed class LoginModel(string password)
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
    public string? EmailAddress { get; set; }

    /// <summary>
    /// User's phone number.
    /// </summary>
    [Phone, DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's password.
    /// </summary>
    [DataType(DataType.Password)]
    public string Password { get; } = password;
    #endregion
}