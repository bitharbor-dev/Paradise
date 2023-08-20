using Paradise.Models;
using Paradise.Models.Domain.RoleModels;

namespace Paradise.ApplicationLogic.Services.Domain;

/// <summary>
/// Provides roles management functionalities.
/// </summary>
public interface IRoleService
{
    #region Methods
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
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="RoleModel"/>
    /// containing information about the application roles.
    /// </returns>
    Task<Result<IEnumerable<RoleModel>>> GetAllAsync(bool? isDefault = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the role with the given <paramref name="roleId"/>.
    /// </summary>
    /// <param name="roleId">
    /// The Id of the role to be found.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="RoleModel"/>
    /// containing information about the role found.
    /// </returns>
    Task<Result<RoleModel>> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the list of application roles, which belongs
    /// to the user with the given <paramref name="userId"/>.
    /// </summary>
    /// <param name="userId">
    /// The Id of the user whose roles to be found.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="RoleModel"/>
    /// containing information about the application roles, which belong
    /// to the user with the given <paramref name="userId"/>.
    /// </returns>
    Task<Result<IEnumerable<RoleModel>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new application role.
    /// </summary>
    /// <param name="model">
    /// The <see cref="RoleCreationModel"/> to be used to
    /// create a new application role.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="RoleModel"/>
    /// containing information about the created role.
    /// </returns>
    Task<Result<RoleModel>> CreateAsync(RoleCreationModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an application role.
    /// </summary>
    /// <param name="roleId">
    /// The Id of the role to be updated.
    /// </param>
    /// <param name="model">
    /// The <see cref="RoleUpdateModel"/> to be used to
    /// update an application role.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="RoleModel"/>
    /// containing information about the updated role.
    /// </returns>
    Task<Result<RoleModel>> UpdateAsync(Guid roleId, RoleUpdateModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an application role.
    /// </summary>
    /// <param name="roleId">
    /// The Id of the role to be deleted.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="RoleModel"/>
    /// containing information about the application roles.
    /// </returns>
    Task<Result<IEnumerable<RoleModel>>> DeleteAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns an application role to a user.
    /// </summary>
    /// <param name="roleId">
    /// The Id of the role to be assigned.
    /// </param>
    /// <param name="userId">
    /// The Id of the user to whom to assign the role.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="RoleModel"/>
    /// containing information about the application roles, which belong
    /// to the user with the given <paramref name="userId"/>.
    /// </returns>
    Task<Result<IEnumerable<RoleModel>>> AssignAsync(Guid roleId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unassigns an application role from a user.
    /// </summary>
    /// <param name="roleId">
    /// The Id of the role to be unassigned.
    /// </param>
    /// <param name="userId">
    /// The Id of the user from whom to unassign the role.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="RoleModel"/>
    /// containing information about the application roles, which belong
    /// to the user with the given <paramref name="userId"/>.
    /// </returns>
    Task<Result<IEnumerable<RoleModel>>> UnassignAsync(Guid roleId, Guid userId, CancellationToken cancellationToken = default);
    #endregion
}