using Paradise.ApplicationLogic.Infrastructure.Services.Identity;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.WebApi.Infrastructure.Extensions;

namespace Paradise.WebApi.Endpoints.Handlers.Infrastructure.Domain.Identity;

/// <summary>
/// Contains roles management actions.
/// </summary>
internal static class RoleHandlers
{
    #region Public methods
    /// <summary>
    /// Gets the list of application roles.
    /// </summary>
    /// <param name="isDefault">
    /// Indicates whether the default, not default or all
    /// roles should be retrieved.
    /// <list type="bullet">
    /// <item>
    /// <see langword="null"/> - all roles.
    /// </item>
    /// <item>
    /// <see langword="true"/> - default roles.
    /// </item>
    /// <item>
    /// <see langword="false"/> - not default roles.
    /// </item>
    /// </list>
    /// </param>
    /// <param name="service">
    /// Role service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="RoleModel"/>
    /// containing information about the application roles.
    /// </returns>
    public static Task<IResult> GetAllAsync(IRoleService service, CancellationToken cancellationToken, bool? isDefault = null)
        => service.GetAllAsync(isDefault, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Gets the role with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    /// The Id of the role to be found.
    /// </param>
    /// <param name="service">
    /// Role service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="RoleModel"/> containing information about the role found.
    /// </returns>
    public static Task<IResult> GetByIdAsync(Guid id, IRoleService service, CancellationToken cancellationToken)
        => service.GetByIdAsync(id, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Gets the list of application roles, which belongs
    /// to the user with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    /// The Id of the user whose roles to be found.
    /// </param>
    /// <param name="service">
    /// Role service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="RoleModel"/>
    /// containing information about the application roles, which belong
    /// to the user with the given <paramref name="id"/>.
    /// </returns>
    public static Task<IResult> GetUserRolesAsync(Guid id, IRoleService service, CancellationToken cancellationToken)
        => service.GetUserRolesAsync(id, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Creates a new application role.
    /// </summary>
    /// <param name="model">
    /// The <see cref="RoleCreationModel"/> to be used to
    /// create a new application role.
    /// </param>
    /// <param name="service">
    /// Role service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="RoleModel"/> containing information about the created role.
    /// </returns>
    public static Task<IResult> CreateAsync(RoleCreationModel model, IRoleService service, CancellationToken cancellationToken)
        => service.CreateAsync(model, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Updates an application role.
    /// </summary>
    /// <param name="id">
    /// The Id of the role to be updated.
    /// </param>
    /// <param name="model">
    /// The <see cref="RoleUpdateModel"/> to be used to
    /// update an application role.
    /// </param>
    /// <param name="service">
    /// Role service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="RoleModel"/> containing information about the updated role.
    /// </returns>
    public static Task<IResult> UpdateAsync(Guid id, RoleUpdateModel model, IRoleService service, CancellationToken cancellationToken)
        => service.UpdateAsync(id, model, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Deletes an application role.
    /// </summary>
    /// <param name="id">
    /// The Id of the role to be deleted.
    /// </param>
    /// <param name="service">
    /// Role service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="RoleModel"/>
    /// containing information about the application roles.
    /// </returns>
    public static Task<IResult> DeleteAsync(Guid id, IRoleService service, CancellationToken cancellationToken)
        => service.DeleteAsync(id, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Assigns an application role to a user.
    /// </summary>
    /// <param name="roleId">
    /// The Id of the role to be assigned.
    /// </param>
    /// <param name="userId">
    /// The Id of the user to whom to assign the role.
    /// </param>
    /// <param name="service">
    /// Role service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="RoleModel"/>
    /// containing information about the application roles, which belong
    /// to the user with the given <paramref name="userId"/>.
    /// </returns>
    public static Task<IResult> AssignAsync(Guid roleId, Guid userId, IRoleService service, CancellationToken cancellationToken)
        => service.AssignAsync(roleId, userId, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Unassigns an application role from a user.
    /// </summary>
    /// <param name="roleId">
    /// The Id of the role to be unassigned.
    /// </param>
    /// <param name="userId">
    /// The Id of the user from whom to unassign the role.
    /// </param>
    /// <param name="service">
    /// Role service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="RoleModel"/>
    /// containing information about the application roles, which belong
    /// to the user with the given <paramref name="userId"/>.
    /// </returns>
    public static Task<IResult> UnassignAsync(Guid roleId, Guid userId, IRoleService service, CancellationToken cancellationToken)
        => service.UnassignAsync(roleId, userId, cancellationToken).AsHttpResultAsync();
    #endregion
}