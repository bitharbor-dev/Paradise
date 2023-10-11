using System.Text.Json.Serialization;

namespace Paradise.DataAccess.Seed.Models.Domain.Users;

/// <summary>
/// User seed data schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SeedUserModel"/> class.
/// </remarks>
/// <param name="userName">
/// User's user-name.
/// </param>
/// <param name="email">
/// User's email address.
/// </param>
/// <param name="password">
/// User's password.
/// </param>
[method: JsonConstructor]
public sealed class SeedUserModel(string userName, string email, string password)
{
    #region Properties
    /// <summary>
    /// User's user-name.
    /// </summary>
    public string UserName { get; set; } = userName;

    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; } = email;

    /// <summary>
    /// User's password.
    /// </summary>
    public string Password { get; set; } = password;

    /// <summary>
    /// Indicates whether the user's email address has been confirmed.
    /// </summary>
    public bool IsEmailConfirmed { get; set; }

    /// <summary>
    /// The list of the user's roles.
    /// </summary>
    public IEnumerable<string>? Roles { get; set; }
    #endregion
}