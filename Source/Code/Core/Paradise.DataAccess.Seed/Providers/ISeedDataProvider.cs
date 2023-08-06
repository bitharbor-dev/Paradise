using Paradise.DataAccess.Seed.Models.Application.MessageTemplates;
using Paradise.DataAccess.Seed.Models.Domain.Roles;
using Paradise.DataAccess.Seed.Models.Domain.Users;

namespace Paradise.DataAccess.Seed.Providers;

/// <summary>
/// An abstraction to provide the database seed data.
/// </summary>
public interface ISeedDataProvider
{
    #region Methods
    /// <summary>
    /// Gets a users list to seed the database.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="SeedUserModel"/>
    /// to seed the database.
    /// </returns>
    IEnumerable<SeedUserModel> GetSeedUsers();

    /// <summary>
    /// Gets a roles list to seed the database.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="SeedRoleModel"/>
    /// to seed the database.
    /// </returns>
    IEnumerable<SeedRoleModel> GetSeedRoles();

    /// <summary>
    /// Gets an email templates list to seed the database.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="SeedEmailTemplateModel"/>
    /// to seed the database.
    /// </returns>
    IEnumerable<SeedEmailTemplateModel> GetSeedEmailTemplates();
    #endregion
}