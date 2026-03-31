using Paradise.DataAccess.Seed.Models.Domain.Identity.Roles;
using Paradise.DataAccess.Seed.Models.Domain.Identity.Users;
using System.Text.Json.Serialization;

namespace Paradise.DataAccess.Seed.Models.Domain;

/// <summary>
/// Domain data seeding schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DomainDataSeedModel"/> class.
/// </remarks>
/// <param name="roles">
/// Seed roles.
/// </param>
/// <param name="users">
/// Seed users.
/// </param>
[method: JsonConstructor]
public sealed class DomainDataSeedModel(IEnumerable<SeedRoleModel> roles, IEnumerable<SeedUserModel> users)
{
    #region Properties
    /// <summary>
    /// Seed roles.
    /// </summary>
    public IEnumerable<SeedRoleModel> Roles { get; } = roles;

    /// <summary>
    /// Seed users.
    /// </summary>
    public IEnumerable<SeedUserModel> Users { get; } = users;
    #endregion
}