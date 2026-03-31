using Paradise.DataAccess.Seed.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess.Seed.Models.Domain.Identity.Roles;
using Paradise.DataAccess.Seed.Models.Domain.Identity.Users;

namespace Paradise.ApplicationLogic.Infrastructure.Seed;

/// <summary>
/// Provides database seeding functionalities.
/// </summary>
public interface IDatabaseSeeder
{
    #region Methods
    /// <summary>
    /// Ensures that application and domain databases are available.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task EnsureStorageAvailableAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Seeds the database with the given <paramref name="seedRoles"/>.
    /// </summary>
    /// <param name="seedRoles">
    /// Roles to be added to the database.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// Added entries number.
    /// </returns>
    Task<ushort> SeedRolesAsync(IEnumerable<SeedRoleModel> seedRoles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Seeds the database with the given <paramref name="seedUsers"/>.
    /// </summary>
    /// <param name="seedUsers">
    /// Users to be added to the database.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// Added entries number.
    /// </returns>
    Task<ushort> SeedUsersAsync(IEnumerable<SeedUserModel> seedUsers, CancellationToken cancellationToken = default);

    /// <summary>
    /// Seeds the database with the given <paramref name="seedEmailTemplates"/>.
    /// </summary>
    /// <param name="seedEmailTemplates">
    /// Email templates to be added to the database.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// Added entries number.
    /// </returns>
    Task<ushort> SeedEmailTemplatesAsync(IEnumerable<SeedEmailTemplateModel> seedEmailTemplates,
                                         CancellationToken cancellationToken = default);
    #endregion
}