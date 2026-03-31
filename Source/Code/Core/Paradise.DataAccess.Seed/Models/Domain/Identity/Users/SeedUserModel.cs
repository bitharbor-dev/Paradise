using System.Text.Json.Serialization;

namespace Paradise.DataAccess.Seed.Models.Domain.Identity.Users;

/// <summary>
/// User seed data schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SeedUserModel"/> class.
/// </remarks>
/// <param name="emailAddress">
/// User's email address.
/// </param>
/// <param name="userName">
/// User's user-name.
/// </param>
/// <param name="password">
/// User's password.
/// </param>
/// <param name="isEmailConfirmed">
/// Indicates whether the user's email address has been confirmed.
/// </param>
/// <param name="roles">
/// The list of the user's roles.
/// </param>
[method: JsonConstructor]
public sealed class SeedUserModel(string emailAddress, string userName, string password, bool isEmailConfirmed, IEnumerable<string>? roles)
{
    #region Properties
    /// <summary>
    /// User's user-name.
    /// </summary>
    public string UserName { get; } = userName;

    /// <summary>
    /// User's email address.
    /// </summary>
    public string EmailAddress { get; } = emailAddress;

    /// <summary>
    /// User's password.
    /// </summary>
    public string Password { get; } = password;

    /// <summary>
    /// Indicates whether the user's email address has been confirmed.
    /// </summary>
    public bool IsEmailConfirmed { get; } = isEmailConfirmed;

    /// <summary>
    /// The list of the user's roles.
    /// </summary>
    public IEnumerable<string>? Roles { get; } = roles;
    #endregion
}