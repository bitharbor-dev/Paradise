using Paradise.DataAccess.Seed.Models.Domain.Roles;
using Paradise.DataAccess.Seed.Models.Domain.Users;

namespace Paradise.DataAccess.Seed.Models.Domain;

/// <summary>
/// Domain data seeding schema.
/// </summary>
public sealed class DomainDataSeedModel
{
    #region Properties
    /// <summary>
    /// Seed roles.
    /// </summary>
    public IEnumerable<SeedRoleModel>? Roles { get; set; }

    /// <summary>
    /// Seed users.
    /// </summary>
    public IEnumerable<SeedUserModel>? Users { get; set; }
    #endregion
}