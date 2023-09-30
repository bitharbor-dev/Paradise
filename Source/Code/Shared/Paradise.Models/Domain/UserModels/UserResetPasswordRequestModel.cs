using Paradise.Common;
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
    [SuppressMessage(SuppressionOfIDE0290.Category, SuppressionOfIDE0290.CheckId, Justification = SuppressionOfIDE0290.Justification)]
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