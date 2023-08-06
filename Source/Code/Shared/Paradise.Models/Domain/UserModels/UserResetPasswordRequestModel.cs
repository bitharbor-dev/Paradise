using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.UserModels;

/// <summary>
/// Reset password request schema.
/// </summary>
public sealed class UserResetPasswordRequestModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="UserResetPasswordRequestModel"/> class.
    /// </summary>
    /// <param name="email">
    /// User's new email address.
    /// </param>
    [JsonConstructor]
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructors not working with constructor attributes.")]
    public UserResetPasswordRequestModel(string email)
        => Email = email;
    #endregion

    #region Properties
    /// <summary>
    /// User's email.
    /// </summary>
    [Required, EmailAddress, DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    #endregion
}