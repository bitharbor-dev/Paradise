using Paradise.DataAccess.Seed.Models.Domain.Roles;
using Paradise.Domain.Roles;
using Paradise.Models.Domain.RoleModels;

namespace Paradise.ApplicationLogic.DataConverters.Domain;

/// <summary>
/// Contains extension methods for "Role" objects conversion operations.
/// </summary>
internal static class RoleDataConverter
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
        => new(seedRole.Name, seedRole.IsDefault);

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
        => new(creationModel.Name, creationModel.IsDefault);

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
        => new(role.Name)
        {
            Created = role.Created,
            Id = role.Id,
            IsDefault = role.IsDefault,
            Modified = role.Modified
        };
    #endregion
}