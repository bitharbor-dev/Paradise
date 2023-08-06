using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Paradise.DataAccess.Seed.Models.Domain.Users;

/// <summary>
/// User seed data schema.
/// </summary>
public sealed class SeedUserModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="SeedUserModel"/> class.
    /// </summary>
    /// <param name="userName">
    /// User's user-name.
    /// </param>
    /// <param name="email">
    /// User's email address.
    /// </param>
    /// <param name="password">
    /// User's password.
    /// </param>
    [JsonConstructor]
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructors not working with constructor attributes.")]
    public SeedUserModel(string userName, string email, string password)
    {
        UserName = userName;
        Email = email;
        Password = password;
    }
    #endregion

    #region Properties
    /// <summary>
    /// User's user-name.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// User's password.
    /// </summary>
    public string Password { get; set; }

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