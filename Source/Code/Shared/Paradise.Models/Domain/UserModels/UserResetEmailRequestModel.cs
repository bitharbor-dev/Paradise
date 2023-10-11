using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.UserModels;

/// <summary>
/// Reset email request schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserResetEmailRequestModel"/> class.
/// </remarks>
/// <param name="email">
/// User's new email address.
/// </param>
/// <param name="emailConfirmation">
/// User's new email address confirmation.
/// </param>
[method: JsonConstructor]
public sealed class UserResetEmailRequestModel(string email, string emailConfirmation)
{
    #region Properties
    /// <summary>
    /// User's new email address.
    /// </summary>
    [Required, EmailAddress, DataType(DataType.EmailAddress)]
    public string Email { get; set; } = email;

    /// <summary>
    /// User's new email address confirmation.
    /// </summary>
    [Required, Compare(nameof(Email))]
    [EmailAddress, DataType(DataType.EmailAddress)]
    public string EmailConfirmation { get; set; } = emailConfirmation;
    #endregion
}