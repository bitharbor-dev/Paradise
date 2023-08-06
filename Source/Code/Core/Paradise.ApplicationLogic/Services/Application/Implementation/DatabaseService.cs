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
    #region Fields
    private readonly ILogger<DatabaseService> _logger = logger;
    private readonly RoleManager<Role> _roleManager = roleManager;
    private readonly UserManager _userManager = userManager;
    private readonly IApplicationDataSource _applicationDataSource = applicationDataSource;
    private readonly IDomainDataSource _domainDataSource = domainDataSource;
    private readonly IUserRefreshTokensRepository _userRefreshTokensRepository = userRefreshTokensRepository;
    private readonly IEmailTemplatesRepository _emailTemplatesRepository = emailTemplatesRepository;
    private readonly IEmailTemplateService _emailTemplateService = emailTemplateService;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async Task EnsureDatabasesCreatedAsync(CancellationToken cancellationToken = default)
    {
        await _applicationDataSource.PreparePersistenceStorageAsync(cancellationToken);
        await _domainDataSource.PreparePersistenceStorageAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ushort> SeedRolesAsync(IEnumerable<SeedRoleModel> seedRoles, CancellationToken cancellationToken = default)
    {
        ushort addedItemsNumber = 0;

        foreach (var model in seedRoles)
        {
            try
            {
                if (await RoleExistsAsync(model, cancellationToken))
                    continue;

                await CreateRoleAsync(model);
                addedItemsNumber++;
            }
            catch (Exception e)
            {
                _logger.LogDatabaseSeedFailure(e);
            }
        }

        return addedItemsNumber;
    }

    /// <inheritdoc/>
    public async Task<ushort> SeedUsersAsync(IEnumerable<SeedUserModel> seedUsers, CancellationToken cancellationToken = default)
    {
        ushort addedItemsNumber = 0;

        foreach (var model in seedUsers)
        {
            try
            {
                if (await UserExistsAsync(model, cancellationToken))
                    continue;

                await CreateUserAsync(model);
                addedItemsNumber++;
            }
            catch (Exception e)
            {
                _logger.LogDatabaseSeedFailure(e);
            }
        }

        return addedItemsNumber;
    }

    /// <inheritdoc/>
    public async Task<ushort> SeedEmailTemplatesAsync(IEnumerable<SeedEmailTemplateModel> seedEmailTemplates, CancellationToken cancellationToken = default)
    {
        ushort addedItemsNumber = 0;

        foreach (var model in seedEmailTemplates)
        {
            try
            {
                if (!await UpdateEmailTemplateAsync(model, cancellationToken))
                {
                    await CreateEmailTemplateAsync(model, cancellationToken);
                    addedItemsNumber++;
                }
            }
            catch (Exception e)
            {
                _logger.LogDatabaseSeedFailure(e);
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
                _logger.LogDatabaseException(e);
            }
        }

        _logger.LogUnconfirmedUsersNumber(deletedItemsNumber);

        return deletedItemsNumber;
    }

    /// <inheritdoc/>
    public async Task<int> ResetUsersPendingDeletionAsync(TimeSpan requestLifetime, CancellationToken cancellationToken = default)
    {
        ushort updatedItemsNumber = 0;

        static void ResetAction(User user)
            => user.CancelDeletionRequest();

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
                _logger.LogDatabaseException(e);
            }
        }

        _logger.LogPendingDeletionUsersNumber(updatedItemsNumber);

        return updatedItemsNumber;
    }

    /// <inheritdoc/>
    public async Task<int> DeleteOutdatedTokensAsync(TimeSpan refreshTokenLifetime, CancellationToken cancellationToken = default)
    {
        var removedTokensNumber = 0;

        try
        {
            var outdatedTokens = await GetOutdatedTokensAsync(refreshTokenLifetime, cancellationToken);

            _userRefreshTokensRepository.RemoveRange(outdatedTokens);

            removedTokensNumber = await _userRefreshTokensRepository.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogDatabaseException(e);
        }

        _logger.LogOutdatedTokensNumber(removedTokensNumber);

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

        var creationResult = await _roleManager.CreateAsync(role);

        if (creationResult.Succeeded)
            _logger.LogAddedSeedItem<Role>(role.Name);
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

        var creationResult = await _userManager.CreateAsync(user, model.Password);
        if (!creationResult.Succeeded)
            throw new IdentityException(creationResult);

        var rolesToAdd = MergeUserRolesWithDefault(model);
        var rolesResult = await _userManager.AddToRolesAsync(user, rolesToAdd);

        if (rolesResult.Succeeded)
        {
            _logger.LogAddedSeedItem<User>(user.UserName);
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
        var result = await _emailTemplateService.CreateAsync(model.ToCreationModel(), cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogAddedSeedItem<EmailTemplate>(model.TemplateName);
        }
        else
        {
            _logger.LogDatabaseEntrySeedFailure(model.TemplateName, result);
        }
    }

    private async Task<bool> UpdateEmailTemplateAsync(SeedEmailTemplateModel model, CancellationToken cancellationToken = default)
    {
        var culture = model.CultureId.HasValue ? CultureInfo.GetCultureInfo(model.CultureId.Value) : null;

        var template = await _emailTemplatesRepository.GetByNameAndCultureAsync(model.TemplateName, culture, cancellationToken);

        if (template is null)
            return false;

        var result = await _emailTemplateService.UpdateAsync(template.Id, model.ToUpdateModel(), cancellationToken);

        if (result.IsSuccess)
        {
            // Status code "OK" means that changes
            // were applied to the template.
            // If no changes were applied -
            // status code equals "Accepted".
            if (result.StatusCode is OK)
                _logger.LogUpdatedSeedItem<EmailTemplate>(model.TemplateName);

            return true;
        }
        else
        {
            _logger.LogDatabaseEntrySeedFailure(model.TemplateName, result);

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
        var updateResult = await _userManager.UpdateAsync(user);

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
        var deleteResult = await _userManager.DeleteAsync(user);

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
        var defaultRoles = _roleManager.Roles
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
        Expression<Func<Role, bool>> predicate =
            role => role.Name == model.Name;

        return _roleManager.Roles.AnyAsync(predicate, cancellationToken);
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
        Expression<Func<User, bool>> predicate =
            user => user.Email == model.Email || user.UserName == model.UserName;

        return _userManager.Users.AnyAsync(predicate, cancellationToken);
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

        var unconfirmedUsers = await _userManager.Users
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

        var pendingUsers = await _userManager.Users
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
        // TODO: Make filtering on database side.
        bool Predicate(UserRefreshToken userRefreshToken)
            => userRefreshToken.IsOutdated(refreshTokenLifetime);

        var refreshTokens = await _userRefreshTokensRepository.GetAllAsync(cancellationToken);

        return refreshTokens.Where(Predicate);
    }
    #endregion
}