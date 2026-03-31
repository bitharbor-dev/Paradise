using Paradise.DataAccess.Seed.Models.Domain.Identity.Roles;
using Paradise.Domain.Identity.Roles;
using Paradise.Models.Domain.Identity.Roles;

namespace Paradise.ApplicationLogic.DataConverters.Domain.Identity.Roles;

/// <summary>
/// Contains extension methods for <see cref="Role"/> conversion operations.
/// </summary>
public static class RoleDataConverter
{
    #region Public methods
    /// <summary>
    /// Converts the <see cref="SeedRoleModel"/> into the <see cref="Role"/>.
    /// </summary>
    /// <param name="seedRole">
    /// The input <see cref="SeedRoleModel"/> to be converted.
    /// </param>
    /// <returns>
    /// A new <see cref="Role"/> instance
    /// converted from the input <paramref name="seedRole"/>.
    /// </returns>
    public static Role ToEntity(this SeedRoleModel seedRole)
    {
        ArgumentNullException.ThrowIfNull(seedRole);

        var name = seedRole.Name;
        var isDefault = seedRole.IsDefault;

        return new(name, isDefault);
    }

    /// <summary>
    /// Converts the <see cref="RoleCreationModel"/> into the <see cref="Role"/>.
    /// </summary>
    /// <param name="creationModel">
    /// The input <see cref="RoleCreationModel"/> to be converted.
    /// </param>
    /// <returns>
    /// A new <see cref="Role"/> instance
    /// converted from the input <paramref name="creationModel"/>.
    /// </returns>
    public static Role ToEntity(this RoleCreationModel creationModel)
    {
        ArgumentNullException.ThrowIfNull(creationModel);

        var name = creationModel.Name;
        var isDefault = creationModel.IsDefault;

        return new(name, isDefault);
    }

    /// <summary>
    /// Converts the <see cref="Role"/> into the <see cref="RoleModel"/>.
    /// </summary>
    /// <param name="role">
    /// The input <see cref="Role"/> to be converted.
    /// </param>
    /// <returns>
    /// A new <see cref="RoleModel"/> instance
    /// converted from the input <paramref name="role"/>.
    /// </returns>
    public static RoleModel ToModel(this Role role)
    {
        ArgumentNullException.ThrowIfNull(role);

        var id = role.Id;
        var created = role.Created;
        var modified = role.Modified;
        var name = role.Name;
        var isDefault = role.IsDefault;

        return new(id, created, modified, name, isDefault);
    }
    #endregion
}