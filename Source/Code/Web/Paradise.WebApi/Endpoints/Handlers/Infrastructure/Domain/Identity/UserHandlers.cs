using Paradise.ApplicationLogic.Infrastructure.Services.Identity;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.Identity;
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
    /// User service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="UserModel"/>
    /// containing information about the application users.
    /// </returns>
    public static Task<IResult> GetAllAsync(IUserService service, CancellationToken cancellationToken)
        => service.GetAllAsync(cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Gets the user with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    /// The Id of the user to be found.
    /// </param>
    /// <param name="service">
    /// User service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="UserModel"/> containing information about the user found.
    /// </returns>
    public static Task<IResult> GetByIdAsync(Guid id, IUserService service, CancellationToken cancellationToken)
        => service.GetByIdAsync(id, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserRegistrationModel"/> to be used to
    /// register a new user.
    /// </param>
    /// <param name="service">
    /// User service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="UserModel"/> containing information about the created user.
    /// </returns>
    public static Task<IResult> RegisterAsync(UserRegistrationModel model, IUserService service, CancellationToken cancellationToken)
        => service.RegisterAsync(model, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Confirms the user's email address.
    /// </summary>
    /// <param name="identityToken">
    /// An encrypted string value to be used to
    /// confirm the user's email address.
    /// </param>
    /// <param name="service">
    /// User service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="UserModel"/> containing information about the updated user.
    /// </returns>
    public static Task<IResult> ConfirmEmailAddressAsync(string identityToken, IUserService service, CancellationToken cancellationToken)
        => service.ConfirmEmailAddressAsync(identityToken, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Creates a password reset request.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserResetPasswordRequestModel"/> to be used to
    /// create a password reset request.
    /// </param>
    /// <param name="service">
    /// User service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> instance containing errors data if any occurs.
    /// </returns>
    public static Task<IResult> CreatePasswordResetRequestAsync(UserResetPasswordRequestModel model, IUserService service, CancellationToken cancellationToken)
        => service.CreatePasswordResetRequestAsync(model, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Resets the user's password.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserResetPasswordModel"/> to be used to
    /// reset the user's password.
    /// </param>
    /// <param name="service">
    /// User service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> instance containing errors data if any occurs.
    /// </returns>
    public static Task<IResult> ResetPasswordAsync(UserResetPasswordModel model, IUserService service, CancellationToken cancellationToken)
        => service.ResetPasswordAsync(model, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Creates an email address reset request.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserResetEmailAddressRequestModel"/> to be used to
    /// create an email address reset request.
    /// </param>
    /// <param name="httpContext">
    /// HTTP context.
    /// </param>
    /// <param name="service">
    /// User service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> instance containing errors data if any occurs.
    /// </returns>
    public static Task<IResult> CreateEmailAddressResetRequestAsync(UserResetEmailAddressRequestModel model, HttpContext httpContext, IUserService service, CancellationToken cancellationToken)
    {
        var id = httpContext.GetUserId();

        return service.CreateEmailAddressResetRequestAsync(id, model, cancellationToken).AsHttpResultAsync();
    }

    /// <summary>
    /// Resets the user's email address.
    /// </summary>
    /// <param name="identityToken">
    /// An encrypted string value to be used to
    /// reset the user's email address.
    /// </param>
    /// <param name="service">
    /// User service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> instance containing errors data if any occurs.
    /// </returns>
    public static Task<IResult> ResetEmailAddressAsync(string identityToken, IUserService service, CancellationToken cancellationToken)
        => service.ResetEmailAddressAsync(identityToken, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Updates the user.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserUpdateModel"/> to be used to
    /// update the user.
    /// </param>
    /// <param name="httpContext">
    /// HTTP context.
    /// </param>
    /// <param name="service">
    /// User service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="UserModel"/> containing information about the updated user.
    /// </returns>
    public static Task<IResult> UpdateAsync(UserUpdateModel model, HttpContext httpContext, IUserService service, CancellationToken cancellationToken)
    {
        var id = httpContext.GetUserId();

        return service.UpdateAsync(id, model, cancellationToken).AsHttpResultAsync();
    }

    /// <summary>
    /// Deletes the user.
    /// </summary>
    /// <param name="httpContext">
    /// HTTP context.
    /// </param>
    /// <param name="service">
    /// User service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> instance containing errors data if any occurs.
    /// </returns>
    public static Task<IResult> DeleteAsync(HttpContext httpContext, IUserService service, CancellationToken cancellationToken)
    {
        var id = httpContext.GetUserId();

        return service.DeleteAsync(id, cancellationToken).AsHttpResultAsync();
    }
    #endregion
}