using System.ComponentModel.DataAnnotations;

namespace Paradise.Models.Domain.Identity.Users;

/// <summary>
/// Update user schema.
/// </summary>
public sealed class UserUpdateModel
{
    #region Properties
    /// <summary>
    /// User's user-name.
    /// </summary>
    [DataType(DataType.Text)]
    public string? UserName { get; set; }

    /// <summary>
    /// Indicates whether two-factor authentication
    /// is enabled for this user.
    /// </summary>
    public bool? TwoFactorEnabled { get; set; }

    /// <summary>
    /// Indicates whether the user is pending deletion.
    /// </summary>
    public bool? IsPendingDeletion { get; set; }
    #endregion
}