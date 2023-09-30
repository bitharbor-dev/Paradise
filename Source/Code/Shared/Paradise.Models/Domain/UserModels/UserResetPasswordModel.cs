using Paradise.Common;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.UserModels;

/// <summary>
/// Reset password schema.
/// </summary>
public sealed class UserResetPasswordModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="UserResetPasswordModel"/> class.
    /// </summary>
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
    [JsonConstructor]
    [SuppressMessage(SuppressionOfIDE0290.Category, SuppressionOfIDE0290.CheckId, Justification = SuppressionOfIDE0290.Justification)]
    public UserResetPasswordModel(string identityToken, string password, string passwordConfirmation)
    {
        IdentityToken = identityToken;
        Password = password;
        PasswordConfirmation = passwordConfirmation;
    }
    #endregion

    #region Properties
    /// <summary>
    /// The value to be used to determine the user
    /// and validate password reset request.
    /// </summary>
    [Required]
    public string IdentityToken { get; set; }

    /// <summary>
    /// User's new password.
    /// </summary>
    [Required, DataType(DataType.Password)]
    public string Password { get; set; }

    /// <summary>
    /// User's new password confirmation.
    /// </summary>
    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    public string PasswordConfirmation { get; set; }
    #endregion
}