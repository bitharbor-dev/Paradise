using Microsoft.EntityFrameworkCore;
using Paradise.ApplicationLogic.DataConverters.Domain.Identity.Roles;
using Paradise.ApplicationLogic.Infrastructure.Extensions;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.Common.Extensions;
using Paradise.Domain.Identity.Roles;
using Paradise.Domain.Identity.Users;
using Paradise.Models;
using Paradise.Models.Domain.Identity.Roles;
using static Paradise.Models.ErrorCode;
using static Paradise.Models.OperationStatus;

namespace Paradise.ApplicationLogic.Services.Identity.Roles.Implementation;

/// <summary>
/// Provides roles management functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RoleService"/> class.
/// </remarks>
/// <param name="roleManager">
/// Role manager.
/// </param>
/// <param name="userManager">
/// User manager.
/// </param>
internal sealed class RoleService(IRoleManager<Role> roleManager, IUserManager<User> userManager) : IRoleService
{
    #region Public methods
    /// <inheritdoc/>
    public async Task<Result<IEnumerable<RoleModel>>> GetAllAsync(bool? isDefault = null, CancellationToken cancellationToken = default)
    {
        var rolesQuery = roleManager.Roles;

        if (isDefault.HasValue)
            rolesQuery = rolesQuery.Where(role => role.IsDefault == isDefault.Value);

        var roles = await rolesQuery
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new(roles.Select(role => role.ToModel()), Success);
    }

    /// <inheritdoc/>
    public async Task<Result<RoleModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await FindRoleByIdAsync(id, cancellationToken)
            .ConfigureAwait(false);

        return role is null
            ? new(Missing, RoleIdNotFound, id)
            : new(role.ToModel(), Success);
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<RoleModel>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await FindUserByIdAsync(userId, cancellationToken)
            .ConfigureAwait(false);

        if (user is null)
            return new(Missing, UserIdNotFound, userId);

        var userRoles = await GetUserRolesInternalAsync(user, cancellationToken)
            .ConfigureAwait(false);

        return new(userRoles.Select(role => role.ToModel()), Success);
    }

    /// <inheritdoc/>
    public async Task<Result<RoleModel>> CreateAsync(RoleCreationModel model, CancellationToken cancellationToken = default)
    {
        if (model is null)
            return new(InvalidInput, InvalidModel, nameof(model));

        if (model.Name.IsNullOrWhiteSpace())
            return new(InvalidInput, InvalidRoleName, model.Name);

        var role = await FindRoleByNameAsync(model.Name, cancellationToken)
            .ConfigureAwait(false);

        if (role is not null)
            return new(Blocked, DuplicateRoleName, model.Name);

        role = model.ToEntity();

        var creationResult = await roleManager.CreateAsync(role)
            .ConfigureAwait(false);

        return creationResult.Succeeded
            ? new(role.ToModel(), Created)
            : creationResult.GetResult<RoleModel>();
    }

    /// <inheritdoc/>
    public async Task<Result<RoleModel>> UpdateAsync(Guid id, RoleUpdateModel model, CancellationToken cancellationToken = default)
    {
        if (model is null)
            return new(InvalidInput, InvalidModel, nameof(model));

        var role = await FindRoleByIdAsync(id, cancellationToken)
            .ConfigureAwait(false);

        if (role is null)
            return new(Missing, RoleIdNotFound, id);

        role.IsDefault = model.IsDefault;

        var updateResult = await roleManager
            .UpdateAsync(role)
            .ConfigureAwait(false);

        return updateResult.Succeeded
            ? new(role.ToModel(), Success)
            : updateResult.GetResult<RoleModel>();
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<RoleModel>>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await FindRoleByIdAsync(id, cancellationToken)
            .ConfigureAwait(false);

        if (role is null)
            return new(Missing, RoleIdNotFound, id);

        var deletionResult = await roleManager
            .DeleteAsync(role)
            .ConfigureAwait(false);

        if (!deletionResult.Succeeded)
            return deletionResult.GetResult<IEnumerable<RoleModel>>();

        var roles = await roleManager
            .Roles
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new(roles.Select(role => role.ToModel()), Success);
    }

    /// <inheritdoc/>
    public Task<Result<IEnumerable<RoleModel>>> AssignAsync(Guid roleId, Guid userId, CancellationToken cancellationToken = default)
        => AssignOrUnassignUserRole(roleId, userId, true, cancellationToken);

    /// <inheritdoc/>
    public Task<Result<IEnumerable<RoleModel>>> UnassignAsync(Guid roleId, Guid userId, CancellationToken cancellationToken = default)
        => AssignOrUnassignUserRole(roleId, userId, false, cancellationToken);
    #endregion

    #region Private methods
    /// <summary>
    /// Assigns/unassigns a role to/from the user.
    /// </summary>
    /// <param name="roleId">
    /// The Id of the role to be assigned.
    /// </param>
    /// <param name="userId">
    /// The Id of the user to whom to assign the role.
    /// </param>
    /// <param name="assign">
    /// <see langword="true"/> if role should be assigned, <see langword="false"/> if unassigned.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where the
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="RoleModel"/>
    /// containing information about the application roles, which belong
    /// to the user with the specified <paramref name="userId"/>.
    /// </returns>
    private async Task<Result<IEnumerable<RoleModel>>> AssignOrUnassignUserRole(Guid roleId, Guid userId, bool assign, CancellationToken cancellationToken = default)
    {
        var role = await FindRoleByIdAsync(roleId, cancellationToken)
            .ConfigureAwait(false);

        if (role is null)
            return new(Missing, RoleIdNotFound, roleId);

        var user = await FindUserByIdAsync(userId, cancellationToken)
            .ConfigureAwait(false);

        if (user is null)
            return new(Missing, UserIdNotFound, userId);

        var identityResult = assign
            ? await userManager.AddToRoleAsync(user, role.Name)
                .ConfigureAwait(false)
            : await userManager.RemoveFromRoleAsync(user, role.Name)
                .ConfigureAwait(false);

        if (!identityResult.Succeeded)
            return identityResult.GetResult<IEnumerable<RoleModel>>();

        var userRoles = await GetUserRolesInternalAsync(user, cancellationToken)
            .ConfigureAwait(false);

        return new(userRoles.Select(role => role.ToModel()), Success);
    }

    /// <summary>
    /// Gets the <see cref="Role"/> with the given <paramref name="name"/>.
    /// </summary>
    /// <param name="name">
    /// The Name of the <see cref="Role"/> to be found.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="Role"/> with the given <paramref name="name"/>.
    /// </returns>
    private Task<Role?> FindRoleByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalizedName = roleManager.NormalizeKey(name);

        return roleManager
            .Roles
            .SingleOrDefaultAsync(role => role.NormalizedName == normalizedName, cancellationToken);
    }

    /// <summary>
    /// Gets the <see cref="Role"/> with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    /// The Id of the <see cref="Role"/> to be found.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="Role"/> with the given <paramref name="id"/>.
    /// </returns>
    private Task<Role?> FindRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => roleManager.Roles.SingleOrDefaultAsync(role => role.Id == id, cancellationToken);

    /// <summary>
    /// Gets the <see cref="User"/> with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    /// The Id of the <see cref="User"/> to be found.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="User"/> with the given <paramref name="id"/>.
    /// </returns>
    private Task<User?> FindUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => userManager.Users.SingleOrDefaultAsync(user => user.Id == id, cancellationToken);

    /// <summary>
    /// Gets the list of the given <paramref name="user"/> roles.
    /// </summary>
    /// <param name="user">
    /// The <see cref="User"/> whose roles to be found.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="Role"/>,
    /// which belongs to the given <paramref name="user"/>.
    /// </returns>
    private async Task<IEnumerable<Role>> GetUserRolesInternalAsync(User user, CancellationToken cancellationToken = default)
    {
        var roleNames = await userManager
            .GetRolesAsync(user)
            .ConfigureAwait(false);

        var userRoles = new List<Role>();

        foreach (var name in roleNames)
        {
            var normalizedName = roleManager.NormalizeKey(name);

            var role = await roleManager
                .Roles
                .SingleAsync(r => r.NormalizedName == normalizedName, cancellationToken)
                .ConfigureAwait(false);

            userRoles.Add(role);
        }

        return userRoles;
    }
    #endregion
}