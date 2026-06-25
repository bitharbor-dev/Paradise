using Microsoft.AspNetCore.Mvc;
using Paradise.ApplicationLogic.Infrastructure.Services.Identity;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.Primitives.Web;
using Paradise.WebApi.Extensions;
using Paradise.WebApi.Infrastructure.Extensions;

namespace Paradise.WebApi.Endpoints.Handlers.Infrastructure.Domain.Identity;

/// <summary>
/// Contains users management actions.
/// </summary>
internal static class UserHandlers
{
    #region Public methods
    /// <summary>
    /// Gets the list of application users.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="UserModel"/>
    /// containing information about the application users.
    /// </returns>
    public static Task<IResult> GetAllAsync([FromServices] IUserService service,
                                            CancellationToken cancellationToken)
    {
        var result = service.GetAllAsync(cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Gets the user with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="id">
    /// The Id of the user to be found.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="UserModel"/> containing information about the user found.
    /// </returns>
    public static Task<IResult> GetByIdAsync([FromServices] IUserService service,
                                             [FromRoute(Name = ParameterNames.IdParameter)] Guid id,
                                             CancellationToken cancellationToken)
    {
        var result = service.GetByIdAsync(id, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="model">
    /// The <see cref="UserRegistrationModel"/> to be used to
    /// register a new user.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="UserModel"/> containing information about the created user.
    /// </returns>
    public static Task<IResult> RegisterAsync([FromServices] IUserService service,
                                              [FromBody] UserRegistrationModel model,
                                              CancellationToken cancellationToken)
    {
        var result = service.RegisterAsync(model, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Confirms the user's email address.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="identityToken">
    /// An encrypted string value to be used to
    /// confirm the user's email address.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="UserModel"/> containing information about the updated user.
    /// </returns>
    public static Task<IResult> ConfirmEmailAddressAsync([FromServices] IUserService service,
                                                         [FromRoute(Name = ParameterNames.IdentityTokenParameter)] string identityToken,
                                                         CancellationToken cancellationToken)
    {
        var result = service.ConfirmEmailAddressAsync(identityToken, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Creates a password reset request.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="model">
    /// The <see cref="UserResetPasswordRequestModel"/> to be used to
    /// create a password reset request.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> instance containing errors data if any occurs.
    /// </returns>
    public static Task<IResult> CreatePasswordResetRequestAsync([FromServices] IUserService service,
                                                                [FromBody] UserResetPasswordRequestModel model,
                                                                CancellationToken cancellationToken)
    {
        var result = service.CreatePasswordResetRequestAsync(model, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Resets the user's password.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="model">
    /// The <see cref="UserResetPasswordModel"/> to be used to
    /// reset the user's password.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> instance containing errors data if any occurs.
    /// </returns>
    public static Task<IResult> ResetPasswordAsync([FromServices] IUserService service,
                                                   [FromBody] UserResetPasswordModel model,
                                                   CancellationToken cancellationToken)
    {
        var result = service.ResetPasswordAsync(model, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Creates an email address reset request.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="model">
    /// The <see cref="UserResetEmailAddressRequestModel"/> to be used to
    /// create an email address reset request.
    /// </param>
    /// <param name="httpContext">
    /// HTTP context.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> instance containing errors data if any occurs.
    /// </returns>
    public static Task<IResult> CreateEmailAddressResetRequestAsync([FromServices] IUserService service,
                                                                    [FromBody] UserResetEmailAddressRequestModel model,
                                                                    HttpContext httpContext,
                                                                    CancellationToken cancellationToken)
    {
        var id = httpContext.GetUserId();

        var result = service.CreateEmailAddressResetRequestAsync(id, model, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Resets the user's email address.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="identityToken">
    /// An encrypted string value to be used to
    /// reset the user's email address.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> instance containing errors data if any occurs.
    /// </returns>
    public static Task<IResult> ResetEmailAddressAsync([FromServices] IUserService service,
                                                       [FromRoute(Name = ParameterNames.IdentityTokenParameter)] string identityToken,
                                                       CancellationToken cancellationToken)
    {
        var result = service.ResetEmailAddressAsync(identityToken, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Updates the user.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="model">
    /// The <see cref="UserUpdateModel"/> to be used to
    /// update the user.
    /// </param>
    /// <param name="httpContext">
    /// HTTP context.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="UserModel"/> containing information about the updated user.
    /// </returns>
    public static Task<IResult> UpdateAsync([FromServices] IUserService service,
                                            [FromBody] UserUpdateModel model,
                                            HttpContext httpContext,
                                            CancellationToken cancellationToken)
    {
        var id = httpContext.GetUserId();

        var result = service.UpdateAsync(id, model, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Deletes the user.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="httpContext">
    /// HTTP context.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> instance containing errors data if any occurs.
    /// </returns>
    public static Task<IResult> DeleteAsync([FromServices] IUserService service,
                                            HttpContext httpContext,
                                            CancellationToken cancellationToken)
    {
        var id = httpContext.GetUserId();

        var result = service.DeleteAsync(id, cancellationToken);

        return result.AsHttpResultAsync();
    }
    #endregion
}