using Paradise.Common;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.UserModels;

/// <summary>
/// Reset email request schema.
/// </summary>
public sealed class UserResetEmailRequestModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="UserResetEmailRequestModel"/> class.
    /// </summary>
    /// <param name="email">
    /// User's new email address.
    /// </param>
    /// <param name="emailConfirmation">
    /// User's new email address confirmation.
    /// </param>
    [JsonConstructor]
    [SuppressMessage(SuppressionOfIDE0290.Category, SuppressionOfIDE0290.CheckId, Justification = SuppressionOfIDE0290.Justification)]
    public UserResetEmailRequestModel(string email, string emailConfirmation)
    {
        Email = email;
        EmailConfirmation = emailConfirmation;
    }
    #endregion

    #region Properties
    /// <summary>
    /// User's new email address.
    /// </summary>
    [Required, EmailAddress, DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    /// <summary>
    /// User's new email address confirmation.
    /// </summary>
    [Required, Compare(nameof(Email))]
    [EmailAddress, DataType(DataType.EmailAddress)]
    public string EmailConfirmation { get; set; }
    #endregion
}