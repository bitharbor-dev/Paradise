using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.UserModels;

/// <summary>
/// Represents a user authorization token.
/// </summary>
public sealed class UserAuthorizationTokenModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="UserAuthorizationTokenModel"/> class.
    /// </summary>
    /// <param name="email">
    /// User's email address.
    /// </param>
    /// <param name="expiryDate">
    /// Token expiry date.
    /// </param>
    /// <param name="token">
    /// Authorization token.
    /// </param>
    [JsonConstructor]
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructors not working with constructor attributes.")]
    public UserAuthorizationTokenModel(string email, DateTime expiryDate, string token)
    {
        Email = email;
        ExpiryDate = expiryDate;
        Token = token;
    }
    #endregion

    #region Properties
    /// <summary>
    /// User's email address.
    /// </summary>
    [Required, EmailAddress, DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    /// <summary>
    /// Token expiry date.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime ExpiryDate { get; set; }

    /// <summary>
    /// Authorization token.
    /// </summary>
    [Required, DataType(DataType.Text)]
    public string Token { get; set; }
    #endregion
}