using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.Identity.Users;

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
    public string IdentityToken { get; } = identityToken;

    /// <summary>
    /// User's new password.
    /// </summary>
    [DataType(DataType.Password)]
    public string Password { get; } = password;

    /// <summary>
    /// User's new password confirmation.
    /// </summary>
    [DataType(DataType.Password), Compare(nameof(Password))]
    public string PasswordConfirmation { get; } = passwordConfirmation;
    #endregion
}