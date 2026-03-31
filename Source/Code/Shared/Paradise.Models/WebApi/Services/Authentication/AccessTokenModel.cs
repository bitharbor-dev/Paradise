using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.WebApi.Services.Authentication;

/// <summary>
/// Represents an access token.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AccessTokenModel"/> class.
/// </remarks>
/// <param name="emailAddress">
/// User's email address.
/// </param>
/// <param name="expiryDate">
/// Token expiry date.
/// </param>
/// <param name="token">
/// Access token.
/// </param>
[method: JsonConstructor]
public sealed class AccessTokenModel(string emailAddress, DateTimeOffset expiryDate, string token)
{
    #region Properties
    /// <summary>
    /// User's email address.
    /// </summary>
    [EmailAddress, DataType(DataType.EmailAddress)]
    public string EmailAddress { get; } = emailAddress;

    /// <summary>
    /// Token expiry date.
    /// </summary>
    [DataType(DataType.DateTime)]
    public DateTimeOffset ExpiryDate { get; } = expiryDate;

    /// <summary>
    /// Access token.
    /// </summary>
    [DataType(DataType.Text)]
    public string Token { get; } = token;
    #endregion
}