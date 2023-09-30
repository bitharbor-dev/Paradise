using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Paradise.ApplicationLogic.DataConverters.Application;
using Paradise.ApplicationLogic.DataConverters.Domain;
using Paradise.ApplicationLogic.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Extensions;
using Paradise.ApplicationLogic.Identity;
using Paradise.Common.Extensions;
using Paradise.DataAccess.Repositories;
using Paradise.DataAccess.Repositories.Application;
using Paradise.DataAccess.Repositories.Domain;
using Paradise.DataAccess.Seed.Models.Application.MessageTemplates;
using Paradise.DataAccess.Seed.Models.Domain.Roles;
using Paradise.DataAccess.Seed.Models.Domain.Users;
using Paradise.Domain.Roles;
using Paradise.Domain.Users;
using Paradise.Localization.ExceptionsHandling;
using System.Globalization;
using System.Linq.Expressions;
using static System.Net.HttpStatusCode;

namespace Paradise.ApplicationLogic.Services.Application.Implementation;

/// <summary>
/// Provides database seed and maintenance functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DatabaseService"/> class.
/// </remarks>
/// <param name="logger">
/// Logger.
/// </param>
/// <param name="roleManager">
/// Role manager.
/// </param>
/// <param name="userManager">
/// User manager.
/// </param>
/// <param name="applicationDataSource">
/// Application data source.
/// </param>
/// <param name="domainDataSource">
/// Domain data source.
/// </param>
/// <param name="userRefreshTokensRepository">
/// User refresh tokens repository.
/// </param>
/// <param name="emailTemplatesRepository">
/// Email templates repository.
/// </param>
/// <param name="emailTemplateService">
/// Email template service.
/// </param>
public sealed class DatabaseService(ILogger<DatabaseService> logger,
                                    RoleManager<Role> roleManager,
                                    UserManager userManager,
                                    IApplicationDataSource applicationDataSource,
                                    IDomainDataSource domainDataSource,
                                    IUserRefreshTokensRepository userRefreshTokensRepository,
                                    IEmailTemplatesRepository emailTemplatesRepository,
                                    IEmailTemplateService emailTemplateService)
    : IDatabaseService
{
    #region Public methods
    /// <inheritdoc/>
    public async Task EnsureDatabasesCreatedAsync(CancellationToken cancellationToken = default)
    {
        await applicationDataSource.PreparePersistenceStorageAsync(cancellationToken);
        await domainDataSource.PreparePersistenceStorageAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ushort> SeedRolesAsync(IEnumerable<SeedRoleModel> seedRoles, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(seedRoles);

        ushort addedItemsNumber = 0;

        foreach (var model in seedRoles)
        {
            try
            {
                var roleExists = await RoleExistsAsync(model, cancellationToken);

                if (roleExists)
                    continue;

                await CreateRoleAsync(model);
                addedItemsNumber++;
            }
            catch (Exception e)
            {
                logger.LogDatabaseSeedFailure(e);
            }
        }

        return addedItemsNumber;
    }

    /// <inheritdoc/>
    public async Task<ushort> SeedUsersAsync(IEnumerable<SeedUserModel> seedUsers, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(seedUsers);

        ushort addedItemsNumber = 0;

        foreach (var model in seedUsers)
        {
            try
            {
                var userExists = await UserExistsAsync(model, cancellationToken);

                if (userExists)
                    continue;

                await CreateUserAsync(model);
                addedItemsNumber++;
            }
            catch (Exception e)
            {
                logger.LogDatabaseSeedFailure(e);
            }
        }

        return addedItemsNumber;
    }

    /// <inheritdoc/>
    public async Task<ushort> SeedEmailTemplatesAsync(IEnumerable<SeedEmailTemplateModel> seedEmailTemplates, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(seedEmailTemplates);

        ushort addedItemsNumber = 0;

        foreach (var model in seedEmailTemplates)
        {
            try
            {
                var emailTemplateExists = await UpdateEmailTemplateAsync(model, cancellationToken);

                if (!emailTemplateExists)
                {
                    await CreateEmailTemplateAsync(model, cancellationToken);
                    addedItemsNumber++;
                }
            }
            catch (Exception e)
            {
                logger.LogDatabaseSeedFailure(e);
            }
        }

        return addedItemsNumber;
    }

    /// <inheritdoc/>
    public async Task<int> DeleteUnconfirmedUsersAsync(TimeSpan confirmationPeriod, CancellationToken cancellationToken = default)
    {
        var deletedItemsNumber = 0;

        var unconfirmedUsers = await GetUnconfirmedUsersAsync(confirmationPeriod, cancellationToken);

        foreach (var user in unconfirmedUsers)
        {
            try
            {
                await DeleteUserAsync(user);
                deletedItemsNumber++;
            }
            catch (Exception e)
            {
                logger.LogDatabaseException(e);
            }
        }

        logger.LogUnconfirmedUsersNumber(deletedItemsNumber);

        return deletedItemsNumber;
    }

    /// <inheritdoc/>
    public async Task<int> ResetUsersPendingDeletionAsync(TimeSpan requestLifetime, CancellationToken cancellationToken = default)
    {
        static void ResetAction(User user)
            => user.CancelDeletionRequest();

        ushort updatedItemsNumber = 0;

        var pendingDeletionUsers = await GetPendingDeletionUsersAsync(requestLifetime, cancellationToken);

        foreach (var user in pendingDeletionUsers)
        {
            try
            {
                await UpdateUserAsync(user, ResetAction);
                updatedItemsNumber++;
            }
            catch (Exception e)
            {
                logger.LogDatabaseException(e);
            }
        }

        logger.LogPendingDeletionUsersNumber(updatedItemsNumber);

        return updatedItemsNumber;
    }

    /// <inheritdoc/>
    public async Task<int> DeleteOutdatedTokensAsync(TimeSpan refreshTokenLifetime, CancellationToken cancellationToken = default)
    {
        var removedTokensNumber = 0;

        try
        {
            var outdatedTokens = await GetOutdatedTokensAsync(refreshTokenLifetime, cancellationToken);

            userRefreshTokensRepository.RemoveRange(outdatedTokens);

            removedTokensNumber = await userRefreshTokensRepository.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogDatabaseException(e);
        }

        logger.LogOutdatedTokensNumber(removedTokensNumber);

        return removedTokensNumber;
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates a role in the database.
    /// </summary>
    /// <param name="model">
    /// The <see cref="SeedRoleModel"/> to be converted
    /// into a role being created.
    /// </param>
    private async Task CreateRoleAsync(SeedRoleModel model)
    {
        var role = model.ToEntity();

        var creationResult = await roleManager.CreateAsync(role);

        if (creationResult.Succeeded)
            logger.LogAddedSeedItem<Role>(role.Name);
        else
            throw new IdentityException(creationResult);
    }

    /// <summary>
    /// Creates a user in the database.
    /// </summary>
    /// <param name="model">
    /// The <see cref="SeedUserModel"/> to be converted
    /// into a user being created.
    /// </param>
    private async Task CreateUserAsync(SeedUserModel model)
    {
        var user = model.ToEntity();

        if (model.Password.IsNullOrWhiteSpace())
            throw new InvalidOperationException(ExceptionMessages.IvalidSeedData, new ArgumentException(nameof(model.Password)));

        var creationResult = await userManager.CreateAsync(user, model.Password);
        if (!creationResult.Succeeded)
            throw new IdentityException(creationResult);

        var rolesToAdd = MergeUserRolesWithDefault(model);
        var rolesResult = await userManager.AddToRolesAsync(user, rolesToAdd);

        if (rolesResult.Succeeded)
        {
            logger.LogAddedSeedItem<User>(user.UserName);
        }
        else
        {
            await DeleteUserAsync(user);
            throw new IdentityException(rolesResult);
        }
    }

    /// <summary>
    /// Creates an email template in the database.
    /// </summary>
    /// <param name="model">
    /// The <see cref="SeedEmailTemplateModel"/> to be converted
    /// into an email template being created.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    private async Task CreateEmailTemplateAsync(SeedEmailTemplateModel model, CancellationToken cancellationToken = default)
    {
        var result = await emailTemplateService.CreateAsync(model.ToCreationModel(), cancellationToken);

        if (result.IsSuccess)
        {
            logger.LogAddedSeedItem<EmailTemplate>(model.TemplateName);
        }
        else
        {
            logger.LogDatabaseEntrySeedFailure(model.TemplateName, result);
        }
    }

    /// <summary>
    /// Updates an <see cref="EmailTemplate"/> with the data from the given
    /// <paramref name="model"/>.
    /// </summary>
    /// <param name="model">
    /// The <see cref="SeedEmailTemplateModel"/> which data to be used to
    /// update the email template.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if an <see cref="EmailTemplate"/> was found and updated,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    private async Task<bool> UpdateEmailTemplateAsync(SeedEmailTemplateModel model, CancellationToken cancellationToken = default)
    {
        var culture = model.CultureId.HasValue ? CultureInfo.GetCultureInfo(model.CultureId.Value) : null;

        var template = await emailTemplatesRepository.GetByNameAndCultureAsync(model.TemplateName, culture, cancellationToken);

        if (template is null)
            return false;

        var result = await emailTemplateService.UpdateAsync(template.Id, model.ToUpdateModel(), cancellationToken);

        if (result.IsSuccess)
        {
            // Status code "OK" means that changes
            // were applied to the template.
            // If no changes were applied -
            // status code equals "Accepted".
            if (result.StatusCode is OK)
                logger.LogUpdatedSeedItem<EmailTemplate>(model.TemplateName);

            return true;
        }
        else
        {
            logger.LogDatabaseEntrySeedFailure(model.TemplateName, result);

            return false;
        }
    }

    /// <summary>
    /// Updates the given <paramref name="user"/> using
    /// the specified <paramref name="updateAction"/>
    /// and saves the changes into the database.
    /// </summary>
    /// <param name="user">
    /// The <see cref="User"/> to be updated.
    /// </param>
    /// <param name="updateAction">
    /// The <see cref="Action{T}"/> to be performed
    /// on the given <paramref name="user"/>.
    /// </param>
    private async Task UpdateUserAsync(User user, Action<User> updateAction)
    {
        updateAction(user);
        var updateResult = await userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
            throw new IdentityException(updateResult);
    }

    /// <summary>
    /// Deletes the given <paramref name="user"/>
    /// from the database.
    /// </summary>
    /// <param name="user">
    /// The <see cref="User"/> to be deleted.
    /// </param>
    private async Task DeleteUserAsync(User user)
    {
        var deleteResult = await userManager.DeleteAsync(user);

        if (!deleteResult.Succeeded)
            throw new IdentityException(deleteResult);
    }

    /// <summary>
    /// Merges the given <paramref name="model"/> roles with
    /// the application default roles into a single sequence.
    /// </summary>
    /// <param name="model">
    /// The <see cref="SeedUserModel"/> which roles to be merged.
    /// </param>
    /// <returns>
    /// The sequence of the given <paramref name="model"/> roles
    /// and the application default roles.
    /// </returns>
    private IEnumerable<string> MergeUserRolesWithDefault(SeedUserModel model)
    {
        var defaultRoles = roleManager.Roles
            .Where(role => role.IsDefault)
            .Select(role => role.Name);

        return model.Roles is null
            ? defaultRoles
            : model.Roles.Concat(defaultRoles).Distinct();
    }

    /// <summary>
    /// Checks if the database contains the given role.
    /// </summary>
    /// <param name="model">
    /// The role to be checked in the database.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if such role exists,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    private Task<bool> RoleExistsAsync(SeedRoleModel model, CancellationToken cancellationToken = default)
    {
        var normalizedName = roleManager.NormalizeKey(model.Name);

        Expression<Func<Role, bool>> predicate =
            role => role.NormalizedName == normalizedName;

        return roleManager.Roles.AnyAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Checks if the database contains the given user.
    /// </summary>
    /// <param name="model">
    /// The user to be checked in the database.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if such user exists,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    private Task<bool> UserExistsAsync(SeedUserModel model, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = userManager.NormalizeEmail(model.Email);
        var normalizedUserName = userManager.NormalizeName(model.UserName);

        Expression<Func<User, bool>> predicate =
            user => user.NormalizedEmail == normalizedEmail || user.NormalizedUserName == normalizedUserName;

        return userManager.Users.AnyAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Gets the list of users with an unconfirmed email address
    /// which registration date has exceeded
    /// the given <paramref name="confirmationPeriod"/>.
    /// </summary>
    /// <param name="confirmationPeriod">
    /// Email confirmation period.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The list of users with an unconfirmed email address
    /// which creation date has exceeded
    /// the given <paramref name="confirmationPeriod"/>.
    /// </returns>
    private async Task<IEnumerable<User>> GetUnconfirmedUsersAsync(TimeSpan confirmationPeriod, CancellationToken cancellationToken = default)
    {
        bool Predicate(User user)
            => user.IsEmailConfirmationPeriodExceeded(confirmationPeriod);

        var unconfirmedUsers = await userManager.Users
            .Where(user => !user.EmailConfirmed).ToListAsync(cancellationToken);

        return unconfirmedUsers.Where(Predicate);
    }

    /// <summary>
    /// Gets the list of users who are pending deletion
    /// and whose deletion request has exceeded
    /// the given <paramref name="requestLifetime"/>.
    /// </summary>
    /// <param name="requestLifetime">
    /// User deletion request lifetime.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The list of users who are pending deletion
    /// and whose deletion request has exceeded
    /// the given <paramref name="requestLifetime"/>.
    /// </returns>
    private async Task<IEnumerable<User>> GetPendingDeletionUsersAsync(TimeSpan requestLifetime, CancellationToken cancellationToken = default)
    {
        bool Predicate(User user)
            => user.IsDeletionRequestOutdated(requestLifetime);

        var pendingUsers = await userManager.Users
            .Where(user => user.DeletionRequestSubmitted.HasValue).ToListAsync(cancellationToken);

        return pendingUsers.Where(Predicate);
    }

    /// <summary>
    /// Gets the list of outdated users' refresh tokens.
    /// </summary>
    /// <param name="refreshTokenLifetime">
    /// User refresh token lifetime.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The list of outdated users' refresh tokens.
    /// </returns>
    private async Task<IEnumerable<UserRefreshToken>> GetOutdatedTokensAsync(TimeSpan refreshTokenLifetime, CancellationToken cancellationToken = default)
    {
        bool Predicate(UserRefreshToken userRefreshToken)
            => userRefreshToken.IsOutdated(refreshTokenLifetime);

        var refreshTokens = await userRefreshTokensRepository.GetAllAsync(cancellationToken);

        return refreshTokens.Where(Predicate);
    }
    #endregion
}