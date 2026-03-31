using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Paradise.ApplicationLogic.DataConverters.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.ApplicationLogic.DataConverters.Domain.Identity.Roles;
using Paradise.ApplicationLogic.DataConverters.Domain.Identity.Users;
using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Infrastructure.Extensions;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.ApplicationLogic.Infrastructure.Services;
using Paradise.Common.Extensions;
using Paradise.DataAccess;
using Paradise.DataAccess.Repositories.Attributes;
using Paradise.DataAccess.Seed.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess.Seed.Models.Domain.Identity.Roles;
using Paradise.DataAccess.Seed.Models.Domain.Identity.Users;
using Paradise.Domain.Identity.Roles;
using Paradise.Domain.Identity.Users;
using Paradise.Localization.ExceptionHandling;
using Paradise.Models;
using System.Globalization;
using System.Linq.Expressions;
using static Paradise.Models.OperationStatus;

namespace Paradise.ApplicationLogic.Infrastructure.Seed.Implementation;

/// <summary>
/// Provides database seeding functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DatabaseSeeder"/> class.
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
/// <param name="domainDataSource">
/// Domain data source.
/// </param>
/// <param name="infrastructureDataSource">
/// Infrastructure data source.
/// </param>
/// <param name="emailTemplateService">
/// Email template service.
/// </param>
internal sealed class DatabaseSeeder(ILogger<DatabaseSeeder> logger,
                                     IRoleManager<Role> roleManager,
                                     IUserManager<User> userManager,
                                     [DomainContextKey] IDataSource domainDataSource,
                                     [InfrastructureContextKey] IDataSource infrastructureDataSource,
                                     IEmailTemplateService emailTemplateService) : IDatabaseSeeder
{
    #region Public methods
    /// <inheritdoc/>
    public async Task EnsureStorageAvailableAsync(CancellationToken cancellationToken = default)
    {
        await infrastructureDataSource.PreparePersistenceStorageAsync(cancellationToken)
            .ConfigureAwait(false);

        await domainDataSource.PreparePersistenceStorageAsync(cancellationToken)
            .ConfigureAwait(false);
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
                var roleExists = await RoleExistsAsync(model, cancellationToken)
                    .ConfigureAwait(false);

                if (roleExists)
                    continue;

                await CreateRoleAsync(model)
                    .ConfigureAwait(false);

                addedItemsNumber++;
            }
            catch (InvalidOperationException exception)
            {
                logger.LogDatabaseSeedFailure(exception);
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
                var userExists = await UserExistsAsync(model, cancellationToken)
                    .ConfigureAwait(false);

                if (userExists)
                    continue;

                await CreateUserAsync(model)
                    .ConfigureAwait(false);

                addedItemsNumber++;
            }
            catch (InvalidOperationException exception)
            {
                logger.LogDatabaseSeedFailure(exception);
            }
        }

        return addedItemsNumber;
    }

    /// <inheritdoc/>
    public async Task<ushort> SeedEmailTemplatesAsync(IEnumerable<SeedEmailTemplateModel> seedEmailTemplates,
                                                      CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(seedEmailTemplates);

        ushort addedItemsNumber = 0;

        foreach (var model in seedEmailTemplates)
        {
            try
            {
                var emailTemplateExists = await UpdateEmailTemplateAsync(model, cancellationToken)
                    .ConfigureAwait(false);

                if (!emailTemplateExists)
                {
                    await CreateEmailTemplateAsync(model, cancellationToken)
                        .ConfigureAwait(false);

                    addedItemsNumber++;
                }
            }
            catch (InvalidOperationException exception)
            {
                logger.LogDatabaseSeedFailure(exception);
            }
        }

        return addedItemsNumber;
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
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    private async Task CreateRoleAsync(SeedRoleModel model)
    {
        var role = model.ToEntity();

        var creationResult = await roleManager.CreateAsync(role)
            .ConfigureAwait(false);

        if (creationResult.Succeeded)
        {
            logger.LogAddedSeedItem<Role>(role.Name);
        }
        else
        {
            var message = GetErrorString(creationResult);

            throw new InvalidOperationException(message);
        }
    }

    /// <summary>
    /// Creates a user in the database.
    /// </summary>
    /// <param name="model">
    /// The <see cref="SeedUserModel"/> to be converted
    /// into a user being created.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    private async Task CreateUserAsync(SeedUserModel model)
    {
        var user = model.ToEntity();

        if (model.Password.IsNullOrWhiteSpace())
        {
            var message = ExceptionMessages.GetMessageInvalidSeedData();

            throw new InvalidOperationException(message, new ArgumentException(nameof(model.Password)));
        }

        var creationResult = await userManager.CreateAsync(user, model.Password)
            .ConfigureAwait(false);

        if (!creationResult.Succeeded)
        {
            var message = GetErrorString(creationResult);

            throw new InvalidOperationException(message);
        }

        var rolesToAdd = MergeUserRolesWithDefault(model);

        var rolesResult = await userManager.AddToRolesAsync(user, rolesToAdd)
            .ConfigureAwait(false);

        if (rolesResult.Succeeded)
        {
            logger.LogAddedSeedItem<User>(user.UserName);
        }
        else
        {
            await DeleteUserAsync(user)
                .ConfigureAwait(false);

            var message = GetErrorString(rolesResult);

            throw new InvalidOperationException(message);
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
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    private async Task CreateEmailTemplateAsync(SeedEmailTemplateModel model, CancellationToken cancellationToken = default)
    {
        var result = await emailTemplateService.CreateAsync(model.ToCreationModel(), cancellationToken)
            .ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            var message = GetErrorString(result);

            throw new InvalidOperationException(message);
        }

        logger.LogAddedSeedItem<EmailTemplate>(model.TemplateName);
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

        var lookupResult = await emailTemplateService.GetByNameAndCultureAsync(model.TemplateName, culture, cancellationToken)
            .ConfigureAwait(false);

        var template = lookupResult.Value;

        if (template is null)
            return false;

        var result = await emailTemplateService.UpdateAsync(template.Id, model.ToUpdateModel(), cancellationToken)
            .ConfigureAwait(false);

        if (result.IsSuccess)
        {
            // Status "Success" means that changes
            // were applied to the template.
            // If no changes were applied -
            // status equals to "Received".
            if (result.Status is Success)
                logger.LogUpdatedSeedItem<EmailTemplate>(model.TemplateName);

            return true;
        }
        else
        {
            var message = GetErrorString(result);

            throw new InvalidOperationException(message);
        }
    }

    /// <summary>
    /// Deletes the given <paramref name="user"/>
    /// from the database.
    /// </summary>
    /// <param name="user">
    /// The <see cref="User"/> to be deleted.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    private async Task DeleteUserAsync(User user)
    {
        var deleteResult = await userManager.DeleteAsync(user)
            .ConfigureAwait(false);

        if (!deleteResult.Succeeded)
        {
            var message = GetErrorString(deleteResult);

            throw new InvalidOperationException(message);
        }
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
        var normalizedEmail = userManager.NormalizeEmail(model.EmailAddress);
        var normalizedUserName = userManager.NormalizeName(model.UserName);

        Expression<Func<User, bool>> predicate =
            user => user.NormalizedEmail == normalizedEmail || user.NormalizedUserName == normalizedUserName;

        return userManager.Users.AnyAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Builds a single error message string from
    /// the errors contained in the given <paramref name="identityResult"/>.
    /// </summary>
    /// <param name="identityResult">
    /// The <see cref="IdentityResult"/> instance
    /// containing the error information.
    /// </param>
    /// <returns>
    /// A string that concatenates all error descriptions
    /// from <paramref name="identityResult"/>,
    /// separated by <see cref="Environment.NewLine"/>.
    /// </returns>
    private static string GetErrorString(IdentityResult identityResult)
        => string.Join(Environment.NewLine, identityResult.Errors.Select(error => error.Description));

    /// <summary>
    /// Builds a single error message string from
    /// the errors contained in the given <paramref name="result"/>.
    /// </summary>
    /// <param name="result">
    /// The <see cref="ResultBase"/> instance
    /// containing the error information.
    /// </param>
    /// <returns>
    /// A string that concatenates all errors
    /// from <paramref name="result"/>,
    /// separated by <see cref="Environment.NewLine"/>.
    /// </returns>
    private static string GetErrorString(ResultBase result)
        => string.Join(Environment.NewLine, result.Errors);
    #endregion
}