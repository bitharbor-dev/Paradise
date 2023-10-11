using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.UserModels;

/// <summary>
/// Represents a user authorization token.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserAuthorizationTokenModel"/> class.
/// </remarks>
/// <param name="email">
/// User's email address.
/// </param>
/// <param name="expiryDate">
/// Token expiry date.
/// </param>
/// <param name="token">
/// Authorization token.
/// </param>
[method: JsonConstructor]
public sealed class UserAuthorizationTokenModel(string email, DateTime expiryDate, string token)
{
    #region Properties
    /// <summary>
    /// User's email address.
    /// </summary>
    [Required, EmailAddress, DataType(DataType.EmailAddress)]
    public string Email { get; set; } = email;

    /// <summary>
    /// Token expiry date.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime ExpiryDate { get; set; } = expiryDate;

    /// <summary>
    /// Authorization token.
    /// </summary>
    [Required, DataType(DataType.Text)]
    public string Token { get; set; } = token;
    #endregion
}