using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.Identity.Users;

/// <summary>
/// Reset password request schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserResetPasswordRequestModel"/> class.
/// </remarks>
/// <param name="emailAddress">
/// User's email address.
/// </param>
[method: JsonConstructor]
public sealed class UserResetPasswordRequestModel(string emailAddress)
{
    #region Properties
    /// <summary>
    /// User's email address.
    /// </summary>
    [EmailAddress, DataType(DataType.EmailAddress)]
    public string EmailAddress { get; } = emailAddress;
    #endregion
}