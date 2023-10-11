using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.UserModels;

/// <summary>
/// Reset password request schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserResetPasswordRequestModel"/> class.
/// </remarks>
/// <param name="email">
/// User's new email address.
/// </param>
[method: JsonConstructor]
public sealed class UserResetPasswordRequestModel(string email)
{
    #region Properties
    /// <summary>
    /// User's email.
    /// </summary>
    [Required, EmailAddress, DataType(DataType.EmailAddress)]
    public string Email { get; set; } = email;
    #endregion
}