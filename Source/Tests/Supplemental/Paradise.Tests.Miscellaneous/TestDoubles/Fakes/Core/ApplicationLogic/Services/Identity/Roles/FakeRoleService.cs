using Paradise.ApplicationLogic.DataConverters.Domain.Identity.Roles;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.ApplicationLogic.Services.Identity.Roles;
using Paradise.Domain.Identity.Roles;
using Paradise.Domain.Identity.Users;
using Paradise.Models;
using Paradise.Models.Domain.Identity.Roles;
using static Paradise.Models.ErrorCode;
using static Paradise.Models.OperationStatus;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.ApplicationLogic.Services.Identity.Roles;

/// <summary>
/// Fake <see cref="IRoleService"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeRoleService"/> class.
/// </remarks>
/// <param name="roleManager">
/// Role manager.
/// </param>
/// <param name="userManager">
/// User manager.
/// </param>
public sealed class FakeRoleService(IRoleManager<Role> roleManager,
                                    IUserManager<User> userManager) : IRoleService
{
    #region Public methods
    /// <inheritdoc/>
    public Task<Result<IEnumerable<RoleModel>>> GetAllAsync(bool? isDefault = null, CancellationToken cancellationToken = default)
    {
        var rolesQuery = roleManager.Roles;

        if (isDefault.HasValue)
            rolesQuery = rolesQuery.Where(role => role.IsDefault == isDefault.Value);

        var roles = rolesQuery.Select(role => role.ToModel());

        return Task.FromResult(new Result<IEnumerable<RoleModel>>(roles, Success));
    }

    /// <inheritdoc/>
    public Task<Result<RoleModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = roleManager.Roles.FirstOrDefault(role => role.Id == id);

        var result = role is null
            ? new Result<RoleModel>(Missing, RoleIdNotFound, id)
            : new(role.ToModel(), Success);

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<RoleModel>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId)
            .ConfigureAwait(false);

        if (user is null)
            return new(Missing, UserIdNotFound, userId);

        var roleNames = await userManager.GetRolesAsync(user)
            .ConfigureAwait(false);

        var roles = roleManager
            .Roles
            .Where(role => roleNames.Any(name => role.Name.Equals(name, StringComparison.Ordinal)))
            .Select(role => role.ToModel());

        return new(roles, Success);
    }

    /// <inheritdoc/>
    public async Task<Result<RoleModel>> CreateAsync(RoleCreationModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        var roleExists = roleManager
            .Roles
            .Any(role => role.Name.Equals(model.Name, StringComparison.Ordinal));

        if (roleExists)
            return new(InvalidInput, DuplicateRoleName, model.Name);

        var role = new Role(model.Name, model.IsDefault);

        await roleManager.CreateAsync(role)
            .ConfigureAwait(false);

        return new(role.ToModel(), Created);
    }

    /// <inheritdoc/>
    public async Task<Result<RoleModel>> UpdateAsync(Guid id, RoleUpdateModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        var role = roleManager
            .Roles
            .FirstOrDefault(role => role.Id == id);

        if (role is null)
            return new(Missing, RoleIdNotFound, id);

        role.IsDefault = model.IsDefault;

        await roleManager.UpdateAsync(role)
            .ConfigureAwait(false);

        return new(role.ToModel(), Success);
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<RoleModel>>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = roleManager
            .Roles
            .FirstOrDefault(role => role.Id == id);

        if (role is null)
            return new(Missing, RoleIdNotFound, id);

        await roleManager.DeleteAsync(role)
            .ConfigureAwait(false);

        var roles = roleManager
            .Roles
            .Select(role => role.ToModel());

        return new(roles, Success);
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<RoleModel>>> AssignAsync(Guid roleId, Guid userId, CancellationToken cancellationToken = default)
    {
        var role = roleManager
            .Roles
            .FirstOrDefault(role => role.Id == roleId);

        if (role is null)
            return new(Missing, RoleIdNotFound, roleId);

        var user = await userManager.FindByIdAsync(userId)
            .ConfigureAwait(false);

        if (user is null)
            return new(Missing, UserIdNotFound, userId);

        await userManager.AddToRoleAsync(user, role.Name)
            .ConfigureAwait(false);

        var roleNames = await userManager.GetRolesAsync(user)
            .ConfigureAwait(false);

        var roles = roleManager
            .Roles
            .Where(role => roleNames.Any(name => role.Name.Equals(name, StringComparison.Ordinal)))
            .Select(role => role.ToModel());

        return new(roles, Success);
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<RoleModel>>> UnassignAsync(Guid roleId, Guid userId, CancellationToken cancellationToken = default)
    {
        var role = roleManager
            .Roles
            .FirstOrDefault(role => role.Id == roleId);

        if (role is null)
            return new(Missing, RoleIdNotFound, roleId);

        var user = await userManager.FindByIdAsync(userId)
            .ConfigureAwait(false);

        if (user is null)
            return new(Missing, UserIdNotFound, userId);

        await userManager.RemoveFromRoleAsync(user, role.Name)
            .ConfigureAwait(false);

        var roleNames = await userManager.GetRolesAsync(user)
            .ConfigureAwait(false);

        var roles = roleManager
            .Roles
            .Where(role => roleNames.Any(name => role.Name.Equals(name, StringComparison.Ordinal)))
            .Select(role => role.ToModel());

        return new(roles, Success);
    }
    #endregion
}