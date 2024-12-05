using Paradise.DataAccess.Seed.Models.Application.MessageTemplates;
using Paradise.DataAccess.Seed.Models.Domain.Roles;
using Paradise.DataAccess.Seed.Models.Domain.Users;
using Paradise.Domain.Base;
using Paradise.Domain.Users;

namespace Paradise.ApplicationLogic.Services.Application;

/// <summary>
/// Provides database seed and maintenance functionalities.
/// </summary>
public interface IDatabaseService
{
    #region Methods
    /// <summary>
    /// Ensures that application and domain databases were created.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    Task EnsureDatabasesCreatedAsync(CancellationToken cancellationToken = default);

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
    Task<ushort> SeedEmailTemplatesAsync(IEnumerable<SeedEmailTemplateModel> seedEmailTemplates, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes users who have exceeded the email confirmation period.
    /// <para>
    /// <code>
    /// <see cref="IDatabaseRecord.Created"/> + <paramref name="confirmationPeriod"/> &lt; <see cref="DateTime.UtcNow"/>
    /// </code>
    /// </para>
    /// </summary>
    /// <param name="confirmationPeriod">
    /// Email confirmation period.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// Deleted entries number.
    /// </returns>
    Task<int> DeleteUnconfirmedUsersAsync(TimeSpan confirmationPeriod, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets <see cref="User.IsPendingDeletion"/> state
    /// in case of deletion request expiry.
    /// </summary>
    /// <param name="requestLifetime">
    /// User deletion request lifetime.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// Updated entries number.
    /// </returns>
    Task<int> ResetUsersPendingDeletionAsync(TimeSpan requestLifetime, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes outdated <see cref="UserRefreshToken"/> entries
    /// from the database.
    /// </summary>
    /// <param name="refreshTokenLifetime">
    /// Refresh token lifetime.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// Deleted entries number.
    /// </returns>
    Task<int> DeleteOutdatedTokensAsync(TimeSpan refreshTokenLifetime, CancellationToken cancellationToken = default);
    #endregion
}