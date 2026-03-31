using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paradise.ApplicationLogic.Services.Identity.Users;
using Paradise.Common.Web;
using Paradise.Models;
using Paradise.Models.Domain.Identity.Users;
using Paradise.WebApi.Controllers.Base;
using Paradise.WebApi.Infrastructure.Extensions;
using Paradise.WebApi.Infrastructure.Filters.Metadata;
using System.ComponentModel.DataAnnotations;
using static Paradise.Common.Web.ParameterNames;

namespace Paradise.WebApi.Controllers.Domain;

/// <summary>
/// Contains users management actions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UsersController"/> class.
/// </remarks>
/// <param name="userService">
/// User service.
/// </param>
public sealed class UsersController(IUserService userService) : ApiControllerBase
{
    #region Public methods
    /// <summary>
    /// Gets the list of application users.
    /// </summary>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="UserModel"/>
    /// containing information about the application users.
    /// </returns>
    [Authorize(AuthenticationSchemes = AuthenticationSchemeNames.Default)]
    [HttpGet(UserRoutes.GetAll)]
    [ResultResponse<IEnumerable<UserModel>>(OperationStatus.Success)]
    [ResultResponse(OperationStatus.Unauthorized)]
    public async Task<IActionResult> GetAll()
        => (await userService.GetAllAsync(HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Gets the user with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    /// The Id of the user to be found.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserModel"/>
    /// containing information about the user found.
    /// </returns>
    [Authorize(AuthenticationSchemes = AuthenticationSchemeNames.Default)]
    [HttpGet(UserRoutes.GetById)]
    [ResultResponse<UserModel>(OperationStatus.Success)]
    [ResultResponse(OperationStatus.Unauthorized)]
    [ResultResponse(OperationStatus.Missing)]
    public async Task<IActionResult> GetById([FromRoute(Name = IdParameter)] Guid id)
        => (await userService.GetByIdAsync(id, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserRegistrationModel"/> to be used to
    /// register a new user.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserModel"/>
    /// containing information about the created user.
    /// </returns>
    [HttpPost(UserRoutes.Register), AllowAnonymous]
    [ResultResponse<UserModel>(OperationStatus.Created)]
    [ResultResponse(OperationStatus.Missing)]
    [ResultResponse(OperationStatus.Blocked)]
    public async Task<IActionResult> Register([FromBody, Required] UserRegistrationModel model)
        => (await userService.RegisterAsync(model, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Confirms the user's email address.
    /// </summary>
    /// <param name="identityToken">
    /// An encrypted string value to be used to
    /// confirm the user's email address.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserModel"/>
    /// containing information about the updated user.
    /// </returns>
    [HttpGet(UserRoutes.ConfirmEmailAddress), AllowAnonymous]
    [ResultResponse<UserModel>(OperationStatus.Success)]
    [ResultResponse(OperationStatus.InvalidInput)]
    [ResultResponse(OperationStatus.Missing)]
    public async Task<IActionResult> ConfirmEmailAddress([FromRoute(Name = IdentityTokenParameter)] string identityToken)
        => (await userService.ConfirmEmailAddressAsync(identityToken, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Creates a password reset request.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserResetPasswordRequestModel"/> to be used to
    /// create a password reset request.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    [HttpPost(UserRoutes.CreatePasswordResetRequest), AllowAnonymous]
    [ResultResponse(OperationStatus.Success)]
    [ResultResponse(OperationStatus.InvalidInput)]
    [ResultResponse(OperationStatus.Missing)]
    public async Task<IActionResult> CreatePasswordResetRequest([FromBody, Required] UserResetPasswordRequestModel model)
        => (await userService.CreatePasswordResetRequestAsync(model, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Resets the user's password.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserResetPasswordModel"/> to be used to
    /// reset the user's password.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    [HttpPatch(UserRoutes.ResetPassword), AllowAnonymous]
    [ResultResponse(OperationStatus.Success)]
    [ResultResponse(OperationStatus.InvalidInput)]
    [ResultResponse(OperationStatus.Missing)]
    [ResultResponse(OperationStatus.Blocked)]
    public async Task<IActionResult> ResetPassword([FromBody, Required] UserResetPasswordModel model)
        => (await userService.ResetPasswordAsync(model, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Creates an email address reset request.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserResetEmailAddressRequestModel"/> to be used to
    /// create an email address reset request.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    [Authorize(AuthenticationSchemes = AuthenticationSchemeNames.Default)]
    [HttpPost(UserRoutes.CreateEmailAddressResetRequest)]
    [ResultResponse(OperationStatus.Success)]
    [ResultResponse(OperationStatus.InvalidInput)]
    [ResultResponse(OperationStatus.Unauthorized)]
    [ResultResponse(OperationStatus.Missing)]
    [ResultResponse(OperationStatus.Blocked)]
    public async Task<IActionResult> CreateEmailResetRequest([FromBody, Required] UserResetEmailAddressRequestModel model)
        => (await userService.CreateEmailAddressResetRequestAsync(GetCurrentUserId(), model, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Resets the user's email address.
    /// </summary>
    /// <param name="identityToken">
    /// An encrypted string value to be used to
    /// reset the user's email address.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    [HttpGet(UserRoutes.ResetEmailAddress), AllowAnonymous]
    [ResultResponse(OperationStatus.Success)]
    [ResultResponse(OperationStatus.InvalidInput)]
    [ResultResponse(OperationStatus.Unauthorized)]
    [ResultResponse(OperationStatus.Missing)]
    [ResultResponse(OperationStatus.Blocked)]
    public async Task<IActionResult> ResetEmailAddress([FromRoute(Name = IdentityTokenParameter)] string identityToken)
        => (await userService.ResetEmailAddressAsync(identityToken, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Updates the user.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserUpdateModel"/> to be used to
    /// update the user.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserModel"/>
    /// containing information about the updated user.
    /// </returns>
    [Authorize(AuthenticationSchemes = AuthenticationSchemeNames.Default)]
    [HttpPatch(UserRoutes.Update)]
    [ResultResponse<UserModel>(OperationStatus.Success)]
    [ResultResponse(OperationStatus.Unauthorized)]
    [ResultResponse(OperationStatus.Missing)]
    [ResultResponse(OperationStatus.Blocked)]
    public async Task<IActionResult> Update([FromBody, Required] UserUpdateModel model)
        => (await userService.UpdateAsync(GetCurrentUserId(), model, HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();

    /// <summary>
    /// Deletes the user.
    /// </summary>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    [Authorize(AuthenticationSchemes = AuthenticationSchemeNames.Default)]
    [HttpDelete(UserRoutes.Delete)]
    [ResultResponse(OperationStatus.Success)]
    [ResultResponse(OperationStatus.InvalidInput)]
    [ResultResponse(OperationStatus.Unauthorized)]
    [ResultResponse(OperationStatus.Missing)]
    public async Task<IActionResult> Delete()
        => (await userService.DeleteAsync(GetCurrentUserId(), HttpContext.RequestAborted).ConfigureAwait(false)).AsActionResult();
    #endregion
}