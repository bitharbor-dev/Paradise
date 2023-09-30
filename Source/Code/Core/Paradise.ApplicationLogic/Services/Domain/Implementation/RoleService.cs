using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Paradise.ApplicationLogic.DataConverters.Domain;
using Paradise.ApplicationLogic.Extensions;
using Paradise.ApplicationLogic.Identity;
using Paradise.Domain.Roles;
using Paradise.Domain.Users;
using Paradise.Models;
using Paradise.Models.Domain.RoleModels;
using static Paradise.Models.ErrorCode;
using static System.Net.HttpStatusCode;

namespace Paradise.ApplicationLogic.Services.Domain.Implementation;

/// <summary>
/// Provides roles management functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RoleService"/> class.
/// </remarks>
/// <param name="userManager">
/// User manager.
/// </param>
/// <param name="roleManager">
/// Role manager.
/// </param>
public sealed class RoleService(UserManager userManager,
                                RoleManager<Role> roleManager)
    : IRoleService
{
    #region Public methods
    /// <inheritdoc/>
    public async Task<Result<IEnumerable<RoleModel>>> GetAllAsync(bool? isDefault = null, CancellationToken cancellationToken = default)
    {
        var rolesQuery = roleManager.Roles;

        if (isDefault.HasValue)
            rolesQuery = rolesQuery.Where(role => role.IsDefault == isDefault.Value);

        var roles = await rolesQuery.ToListAsync(cancellationToken);

        return new(roles.Select(role => role.ToModel()), OK);
    }

    /// <inheritdoc/>
    public async Task<Result<RoleModel>> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await FindRoleByIdAsync(roleId, cancellationToken);

        role.ThrowIfNull(NotFound, RoleIdNotFound, roleId);

        return new(role.ToModel(), OK);
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<RoleModel>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await FindUserByIdAsync(userId, cancellationToken);

        user.ThrowIfNull(NotFound, UserIdNotFound, userId);

        var userRoles = await GetUserRolesInternalAsync(user, cancellationToken);

        return new(userRoles.Select(role => role.ToModel()), OK);
    }

    /// <inheritdoc/>
    public async Task<Result<RoleModel>> CreateAsync(RoleCreationModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.Name.ThrowIfEmptyOrWhiteSpace(UnprocessableEntity, InvalidRoleName, model.Name);

        var role = await FindRoleByNameAsync(model.Name, cancellationToken);

        role.ThrowIfNotNull(UnprocessableEntity, DuplicateRoleName, model.Name);

        role = model.ToEntity();

        var creationResult = await roleManager.CreateAsync(role);

        creationResult.ThrowIfUnsuccessfulIdentityResult();

        return new(role.ToModel(), Created);
    }

    /// <inheritdoc/>
    public async Task<Result<RoleModel>> UpdateAsync(Guid roleId, RoleUpdateModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        var role = await FindRoleByIdAsync(roleId, cancellationToken);

        role.ThrowIfNull(NotFound, RoleIdNotFound, roleId);

        role.IsDefault = model.IsDefault;

        var updateResult = await roleManager.UpdateAsync(role);

        updateResult.ThrowIfUnsuccessfulIdentityResult();

        return new(role.ToModel(), OK);
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<RoleModel>>> DeleteAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await FindRoleByIdAsync(roleId, cancellationToken);

        role.ThrowIfNull(NotFound, RoleIdNotFound, roleId);

        var deletionResult = await roleManager.DeleteAsync(role);

        deletionResult.ThrowIfUnsuccessfulIdentityResult();

        var roles = await roleManager.Roles.ToListAsync(cancellationToken);

        return new(roles.Select(role => role.ToModel()), OK);
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
        var role = await FindRoleByIdAsync(roleId, cancellationToken);
        var user = await FindUserByIdAsync(userId, cancellationToken);

        role.ThrowIfNull(NotFound, RoleIdNotFound, roleId);
        user.ThrowIfNull(NotFound, UserIdNotFound, userId);

        var identityResult = await (assign
            ? userManager.AddToRoleAsync(user, role.Name)
            : userManager.RemoveFromRoleAsync(user, role.Name));

        identityResult.ThrowIfUnsuccessfulIdentityResult();

        var userRoles = await GetUserRolesInternalAsync(user, cancellationToken);

        return new(userRoles.Select(role => role.ToModel()), OK);
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
        var roleNames = await userManager.GetRolesAsync(user);
        var userRoles = new List<Role>();

        foreach (var name in roleNames)
        {
            var normalizedName = roleManager.NormalizeKey(name);

            var role = await roleManager.Roles.SingleAsync(r => r.NormalizedName == normalizedName, cancellationToken);
            userRoles.Add(role);
        }

        return userRoles;
    }
    #endregion
}