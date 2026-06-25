using Microsoft.AspNetCore.Mvc;
using Paradise.ApplicationLogic.Infrastructure.Services.Identity;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.Primitives.Web;
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
    /// The service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="RoleModel"/>
    /// containing information about the application roles.
    /// </returns>
    public static Task<IResult> GetAllAsync([FromServices] IRoleService service,
                                            [FromQuery(Name = ParameterNames.IsDefaultParameter)] bool? isDefault,
                                            CancellationToken cancellationToken)
    {
        var result = service.GetAllAsync(isDefault, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Gets the role with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="id">
    /// The Id of the role to be found.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="RoleModel"/> containing information about the role found.
    /// </returns>
    public static Task<IResult> GetByIdAsync([FromServices] IRoleService service,
                                             [FromRoute(Name = ParameterNames.IdParameter)] Guid id,
                                             CancellationToken cancellationToken)
    {
        var result = service.GetByIdAsync(id, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Gets the list of application roles, which belongs
    /// to the user with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="id">
    /// The Id of the user whose roles to be found.
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
    public static Task<IResult> GetUserRolesAsync([FromServices] IRoleService service,
                                                  [FromRoute(Name = ParameterNames.UserIdParameter)] Guid id,
                                                  CancellationToken cancellationToken)
    {
        var result = service.GetUserRolesAsync(id, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Creates a new application role.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="model">
    /// The <see cref="RoleCreationModel"/> to be used to
    /// create a new application role.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="RoleModel"/> containing information about the created role.
    /// </returns>
    public static Task<IResult> CreateAsync([FromServices] IRoleService service,
                                            [FromBody] RoleCreationModel model,
                                            CancellationToken cancellationToken)
    {
        var result = service.CreateAsync(model, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Updates an application role.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="model">
    /// The <see cref="RoleUpdateModel"/> to be used to
    /// update an application role.
    /// </param>
    /// <param name="id">
    /// The Id of the role to be updated.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="RoleModel"/> containing information about the updated role.
    /// </returns>
    public static Task<IResult> UpdateAsync([FromServices] IRoleService service,
                                            [FromBody] RoleUpdateModel model,
                                            [FromRoute(Name = ParameterNames.IdParameter)] Guid id,
                                            CancellationToken cancellationToken)
    {
        var result = service.UpdateAsync(id, model, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Deletes an application role.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="id">
    /// The Id of the role to be deleted.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="RoleModel"/>
    /// containing information about the application roles.
    /// </returns>
    public static Task<IResult> DeleteAsync([FromServices] IRoleService service,
                                            [FromRoute(Name = ParameterNames.IdParameter)] Guid id,
                                            CancellationToken cancellationToken)
    {
        var result = service.DeleteAsync(id, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Assigns an application role to a user.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
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
    /// An <see cref="IEnumerable{T}"/> of <see cref="RoleModel"/>
    /// containing information about the application roles, which belong
    /// to the user with the given <paramref name="userId"/>.
    /// </returns>
    public static Task<IResult> AssignAsync([FromServices] IRoleService service,
                                            [FromRoute(Name = ParameterNames.RoleIdParameter)] Guid roleId,
                                            [FromRoute(Name = ParameterNames.UserIdParameter)] Guid userId,
                                            CancellationToken cancellationToken)
    {
        var result = service.AssignAsync(roleId, userId, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Unassigns an application role from a user.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
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
    /// An <see cref="IEnumerable{T}"/> of <see cref="RoleModel"/>
    /// containing information about the application roles, which belong
    /// to the user with the given <paramref name="userId"/>.
    /// </returns>
    public static Task<IResult> UnassignAsync([FromServices] IRoleService service,
                                              [FromRoute(Name = ParameterNames.RoleIdParameter)] Guid roleId,
                                              [FromRoute(Name = ParameterNames.UserIdParameter)] Guid userId,
                                              CancellationToken cancellationToken)
    {
        var result = service.UnassignAsync(roleId, userId, cancellationToken);

        return result.AsHttpResultAsync();
    }
    #endregion
}