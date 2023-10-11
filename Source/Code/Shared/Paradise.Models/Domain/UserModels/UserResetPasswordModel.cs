using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.UserModels;

/// <summary>
/// Reset password schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserResetPasswordModel"/> class.
/// </remarks>
/// <param name="identityToken">
/// The value to be used to determine the user
/// and validate password reset request.
/// </param>
/// <param name="password">
/// User's new password.
/// </param>
/// <param name="passwordConfirmation">
/// User's new password confirmation.
/// </param>
[method: JsonConstructor]
public sealed class UserResetPasswordModel(string identityToken, string password, string passwordConfirmation)
{
    #region Properties
    /// <summary>
    /// The value to be used to determine the user
    /// and validate password reset request.
    /// </summary>
    [Required]
    public string IdentityToken { get; set; } = identityToken;

    /// <summary>
    /// User's new password.
    /// </summary>
    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = password;

    /// <summary>
    /// User's new password confirmation.
    /// </summary>
    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    public string PasswordConfirmation { get; set; } = passwordConfirmation;
    #endregion
}