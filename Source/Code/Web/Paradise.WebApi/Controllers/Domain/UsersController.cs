using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Extensions;
using Paradise.ApplicationLogic.Services.Domain.Users;
using Paradise.Common.Web;
using Paradise.Models;
using Paradise.Models.Domain.UserModels;
using Paradise.WebApi.Controllers.Base;
using Paradise.WebApi.Filters.Annotation;
using System.ComponentModel.DataAnnotations;
using System.Net;
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
/// <param name="identityOptions">
/// The accessor used to access the <see cref="IdentityOptions"/>.
/// </param>
public sealed class UsersController(IUserService userService, IOptions<IdentityOptions> identityOptions) : ApiControllerBase
{
    #region Fields
    private readonly string _idClaimType = identityOptions.Value.ClaimsIdentity.UserIdClaimType;
    #endregion

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
    [HttpGet(UserRoutes.GetAll)]
    [ResultResponse<IEnumerable<UserModel>>(HttpStatusCode.OK)]
    [ResultResponse(HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> GetAll()
        => await userService.GetAllAsync().ConfigureAwait(false);

    /// <summary>
    /// Gets the user with the given <paramref name="userId"/>.
    /// </summary>
    /// <param name="userId">
    /// The Id of the user to be found.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserModel"/>
    /// containing information about the user found.
    /// </returns>
    [HttpGet(UserRoutes.GetById)]
    [ResultResponse<UserModel>(HttpStatusCode.OK)]
    [ResultResponse(HttpStatusCode.Unauthorized)]
    [ResultResponse(HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetById([FromRoute(Name = UserIdParameter)] Guid userId)
        => await userService.GetByIdAsync(userId).ConfigureAwait(false);

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
    [ResultResponse<UserModel>(HttpStatusCode.Created)]
    [ResultResponse(HttpStatusCode.NotFound)]
    [ResultResponse(HttpStatusCode.UnprocessableEntity)]
    public async Task<IActionResult> Register([FromBody, Required] UserRegistrationModel model)
        => await userService.RegisterAsync(model).ConfigureAwait(false);

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
    [HttpGet(UserRoutes.ConfirmEmail), AllowAnonymous]
    [ResultResponse<UserModel>(HttpStatusCode.OK)]
    [ResultResponse(HttpStatusCode.BadRequest)]
    [ResultResponse(HttpStatusCode.NotFound)]
    public async Task<IActionResult> ConfirmEmail([FromRoute(Name = IdentityTokenParameter)] string identityToken)
        => await userService.ConfirmEmailAsync(identityToken).ConfigureAwait(false);

    /// <summary>
    /// Generates a new user authorization token or
    /// two-factor authentication token in case it is enabled for the user.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserLoginModel"/> to be used to
    /// validate login data and generate an access token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserAuthorizationTokenModel"/>
    /// containing information about the user authorization token or
    /// two-factor authentication token in case it is enabled for the user.
    /// </returns>
    [HttpPost(UserRoutes.Login), AllowAnonymous]
    [ResultResponse<UserAuthorizationTokenModel>(HttpStatusCode.OK)]
    [ResultResponse<UserAuthorizationTokenModel>(HttpStatusCode.Accepted)]
    [ResultResponse(HttpStatusCode.BadRequest)]
    [ResultResponse(HttpStatusCode.NotFound)]
    [ResultResponse(HttpStatusCode.Forbidden)]
    public async Task<IActionResult> Login([FromBody, Required] UserLoginModel model)
        => await userService.LoginAsync(model).ConfigureAwait(false);

    /// <summary>
    /// Generates a new user authorization token
    /// for the user with two-factor authentication enabled.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserTwoFactorAuthenticationModel"/> to be used to
    /// validate the login data and generate an access token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserAuthorizationTokenModel"/>
    /// containing information about the user authorization token.
    /// </returns>
    [HttpPut(UserRoutes.ConfirmLogin), AllowAnonymous]
    [ResultResponse<UserAuthorizationTokenModel>(HttpStatusCode.OK)]
    [ResultResponse(HttpStatusCode.BadRequest)]
    [ResultResponse(HttpStatusCode.Unauthorized)]
    [ResultResponse(HttpStatusCode.NotFound)]
    [ResultResponse(HttpStatusCode.UnprocessableEntity)]
    public async Task<IActionResult> ConfirmLogin([FromBody, Required] UserTwoFactorAuthenticationModel model)
        => await userService.ConfirmLoginAsync(model).ConfigureAwait(false);

    /// <summary>
    /// Generates a new user authorization token
    /// using the given <paramref name="accessToken"/>.
    /// </summary>
    /// <param name="accessToken">
    /// User authorization token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserAuthorizationTokenModel"/>
    /// containing information about the user authorization token.
    /// </returns>
    [HttpGet(UserRoutes.RenewToken)]
    [ResultResponse<UserAuthorizationTokenModel>(HttpStatusCode.OK)]
    [ResultResponse(HttpStatusCode.BadRequest)]
    [ResultResponse(HttpStatusCode.Unauthorized)]
    [ResultResponse(HttpStatusCode.NotFound)]
    public async Task<IActionResult> RenewToken([FromHeader(Name = AuthorizationHeaderName)] string accessToken)
        => await userService.RenewTokenAsync(accessToken).ConfigureAwait(false);

    /// <summary>
    /// Invalidates the given <paramref name="accessToken"/>
    /// to make it unusable during the authentication process.
    /// </summary>
    /// <param name="accessToken">
    /// Authorization token to be invalidated.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    [HttpDelete(UserRoutes.Logout)]
    [ResultResponse(HttpStatusCode.OK)]
    [ResultResponse(HttpStatusCode.BadRequest)]
    [ResultResponse(HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> Logout([FromHeader(Name = AuthorizationHeaderName), Required] string accessToken)
        => await userService.LogoutAsync(accessToken).ConfigureAwait(false);

    /// <summary>
    /// Invalidates all user's refresh tokens
    /// to make them all unusable during the authentication process.
    /// </summary>
    /// <param name="accessToken">
    /// Authorization token to be invalidated.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    [HttpDelete(UserRoutes.LogoutEverywhere)]
    [ResultResponse(HttpStatusCode.OK)]
    [ResultResponse(HttpStatusCode.BadRequest)]
    [ResultResponse(HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> LogoutEverywhere([FromHeader(Name = AuthorizationHeaderName), Required] string accessToken)
        => await userService.LogoutEverywhereAsync(accessToken).ConfigureAwait(false);

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
    [ResultResponse(HttpStatusCode.OK)]
    [ResultResponse(HttpStatusCode.BadRequest)]
    [ResultResponse(HttpStatusCode.NotFound)]
    public async Task<IActionResult> CreatePasswordResetRequest([FromBody, Required] UserResetPasswordRequestModel model)
        => await userService.CreatePasswordResetRequestAsync(model).ConfigureAwait(false);

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
    [ResultResponse(HttpStatusCode.OK)]
    [ResultResponse(HttpStatusCode.BadRequest)]
    [ResultResponse(HttpStatusCode.NotFound)]
    [ResultResponse(HttpStatusCode.UnprocessableEntity)]
    public async Task<IActionResult> ResetPassword([FromBody, Required] UserResetPasswordModel model)
        => await userService.ResetPasswordAsync(model).ConfigureAwait(false);

    /// <summary>
    /// Creates an email address reset request.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserResetEmailRequestModel"/> to be used to
    /// create an email address reset request.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    [HttpPost(UserRoutes.CreateEmailResetRequest)]
    [ResultResponse(HttpStatusCode.OK)]
    [ResultResponse(HttpStatusCode.BadRequest)]
    [ResultResponse(HttpStatusCode.Unauthorized)]
    [ResultResponse(HttpStatusCode.NotFound)]
    public async Task<IActionResult> CreateEmailResetRequest([FromBody, Required] UserResetEmailRequestModel model)
        => await userService.CreateEmailResetRequestAsync(User.GetGuidClaim(_idClaimType), model).ConfigureAwait(false);

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
    [HttpGet(UserRoutes.ResetEmail), AllowAnonymous]
    [ResultResponse(HttpStatusCode.OK)]
    [ResultResponse(HttpStatusCode.BadRequest)]
    [ResultResponse(HttpStatusCode.Unauthorized)]
    [ResultResponse(HttpStatusCode.NotFound)]
    [ResultResponse(HttpStatusCode.UnprocessableEntity)]
    public async Task<IActionResult> ResetEmail([FromRoute(Name = IdentityTokenParameter)] string identityToken)
        => await userService.ResetEmailAsync(identityToken).ConfigureAwait(false);

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
    [HttpPatch(UserRoutes.Update)]
    [ResultResponse<UserModel>(HttpStatusCode.OK)]
    [ResultResponse(HttpStatusCode.Unauthorized)]
    [ResultResponse(HttpStatusCode.NotFound)]
    [ResultResponse(HttpStatusCode.UnprocessableEntity)]
    public async Task<IActionResult> Update([FromBody, Required] UserUpdateModel model)
        => await userService.UpdateAsync(User.GetGuidClaim(_idClaimType), model).ConfigureAwait(false);

    /// <summary>
    /// Deletes the user.
    /// </summary>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    [HttpDelete(UserRoutes.Delete)]
    [ResultResponse(HttpStatusCode.OK)]
    [ResultResponse(HttpStatusCode.BadRequest)]
    [ResultResponse(HttpStatusCode.Unauthorized)]
    [ResultResponse(HttpStatusCode.NotFound)]
    public async Task<IActionResult> Delete()
        => await userService.DeleteAsync(User.GetGuidClaim(_idClaimType)).ConfigureAwait(false);
    #endregion
}