using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paradise.ApplicationLogic.Services.Identity.Roles;
using Paradise.Common.Web;
using Paradise.Models;
using Paradise.Models.Domain.Identity.Roles;
using Paradise.WebApi.Controllers.Base;
using Paradise.WebApi.Infrastructure.Extensions;
using Paradise.WebApi.Infrastructure.Filters.Metadata;
using System.ComponentModel.DataAnnotations;
using static Paradise.Common.RoleNames;
using static Paradise.Common.Web.ParameterNames;

namespace Paradise.WebApi.Controllers.Domain;

/// <summary>
/// Contains roles management actions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RolesController"/> class.
/// </remarks>
/// <param name="roleService">
/// Role service.
/// </param>
[Authorize(AuthenticationSchemes = AuthenticationSchemeNames.Default, Roles = Administrator)]
public sealed class RolesController(IRoleService roleService) : ApiControllerBase
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
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="RoleModel"/>
    /// containing information about the application roles.
    /// </returns>
    [HttpGet(RoleRoutes.GetAll)]
    [ResultResponse<IEnumerable<RoleModel>>(OperationStatus.Success)]
    [ResultResponse(OperationStatus.Unauthorized)]
    [ResultResponse(OperationStatus.Prohibited)]
    public async Task<IActionResult> GetAll([FromQuery(Name = IsDefaultParameter)] bool? isDefault = null)
        => (await roleService.GetAllAsync(isDefault, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Gets the role with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    /// The Id of the role to be found.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="RoleModel"/>
    /// containing information about the role found.
    /// </returns>
    [HttpGet(RoleRoutes.GetById)]
    [ResultResponse<RoleModel>(OperationStatus.Success)]
    [ResultResponse(OperationStatus.Unauthorized)]
    [ResultResponse(OperationStatus.Prohibited)]
    [ResultResponse(OperationStatus.Missing)]
    public async Task<IActionResult> GetById([FromRoute(Name = IdParameter)] Guid id)
        => (await roleService.GetByIdAsync(id, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Gets the list of application roles, which belongs
    /// to the user with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    /// The Id of the user whose roles to be found.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="RoleModel"/>
    /// containing information about the application roles, which belong
    /// to the user with the given <paramref name="id"/>.
    /// </returns>
    [HttpGet(RoleRoutes.GetUserRoles)]
    [ResultResponse<IEnumerable<RoleModel>>(OperationStatus.Success)]
    [ResultResponse(OperationStatus.Unauthorized)]
    [ResultResponse(OperationStatus.Prohibited)]
    [ResultResponse(OperationStatus.Missing)]
    public async Task<IActionResult> GetUserRoles([FromRoute(Name = UserIdParameter)] Guid id)
        => (await roleService.GetUserRolesAsync(id, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Creates a new application role.
    /// </summary>
    /// <param name="model">
    /// The <see cref="RoleCreationModel"/> to be used to
    /// create a new application role.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="RoleModel"/>
    /// containing information about the created role.
    /// </returns>
    [HttpPost(RoleRoutes.Create)]
    [ResultResponse<RoleModel>(OperationStatus.Created)]
    [ResultResponse(OperationStatus.Unauthorized)]
    [ResultResponse(OperationStatus.Prohibited)]
    [ResultResponse(OperationStatus.Blocked)]
    public async Task<IActionResult> Create([FromBody, Required] RoleCreationModel model)
        => (await roleService.CreateAsync(model, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

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
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="RoleModel"/>
    /// containing information about the updated role.
    /// </returns>
    [HttpPatch(RoleRoutes.Update)]
    [ResultResponse<RoleModel>(OperationStatus.Success)]
    [ResultResponse(OperationStatus.Unauthorized)]
    [ResultResponse(OperationStatus.Prohibited)]
    [ResultResponse(OperationStatus.Missing)]
    public async Task<IActionResult> Update([FromRoute(Name = IdParameter)] Guid id, [FromBody, Required] RoleUpdateModel model)
        => (await roleService.UpdateAsync(id, model, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Deletes an application role.
    /// </summary>
    /// <param name="id">
    /// The Id of the role to be deleted.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="RoleModel"/>
    /// containing information about the application roles.
    /// </returns>
    [HttpDelete(RoleRoutes.Delete)]
    [ResultResponse<IEnumerable<RoleModel>>(OperationStatus.Success)]
    [ResultResponse(OperationStatus.Unauthorized)]
    [ResultResponse(OperationStatus.Prohibited)]
    [ResultResponse(OperationStatus.Missing)]
    public async Task<IActionResult> Delete([FromRoute(Name = IdParameter)] Guid id)
        => (await roleService.DeleteAsync(id, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Assigns an application role to a user.
    /// </summary>
    /// <param name="roleId">
    /// The Id of the role to be assigned.
    /// </param>
    /// <param name="userId">
    /// The Id of the user to whom to assign the role.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="RoleModel"/>
    /// containing information about the application roles, which belong
    /// to the user with the given <paramref name="userId"/>.
    /// </returns>
    [HttpPatch(RoleRoutes.Assign)]
    [ResultResponse<IEnumerable<RoleModel>>(OperationStatus.Success)]
    [ResultResponse(OperationStatus.Unauthorized)]
    [ResultResponse(OperationStatus.Prohibited)]
    [ResultResponse(OperationStatus.Missing)]
    public async Task<IActionResult> Assign([FromRoute(Name = RoleIdParameter)] Guid roleId, [FromRoute(Name = UserIdParameter)] Guid userId)
        => (await roleService.AssignAsync(roleId, userId, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Unassigns an application role from a user.
    /// </summary>
    /// <param name="roleId">
    /// The Id of the role to be unassigned.
    /// </param>
    /// <param name="userId">
    /// The Id of the user from whom to unassign the role.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="RoleModel"/>
    /// containing information about the application roles, which belong
    /// to the user with the given <paramref name="userId"/>.
    /// </returns>
    [HttpDelete(RoleRoutes.Unassign)]
    [ResultResponse<IEnumerable<RoleModel>>(OperationStatus.Success)]
    [ResultResponse(OperationStatus.Unauthorized)]
    [ResultResponse(OperationStatus.Prohibited)]
    [ResultResponse(OperationStatus.Missing)]
    public async Task<IActionResult> Unassign([FromRoute(Name = RoleIdParameter)] Guid roleId, [FromRoute(Name = UserIdParameter)] Guid userId)
        => (await roleService.UnassignAsync(roleId, userId, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();
    #endregion
}