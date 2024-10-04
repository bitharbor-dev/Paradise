using Paradise.Models;
using Paradise.Models.Domain.UserModels;

namespace Paradise.ApplicationLogic.Services.Domain.Users;

/// <summary>
/// Provides users management functionalities.
/// </summary>
public interface IUserService
{
    #region Methods
    /// <summary>
    /// Gets the list of application users.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="UserModel"/>
    /// containing information about the application users.
    /// </returns>
    Task<Result<IEnumerable<UserModel>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user with the given <paramref name="userId"/>.
    /// </summary>
    /// <param name="userId">
    /// The Id of the user to be found.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserModel"/>
    /// containing information about the user found.
    /// </returns>
    Task<Result<UserModel>> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserRegistrationModel"/> to be used to
    /// register a new user.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserModel"/>
    /// containing information about the created user.
    /// </returns>
    Task<Result<UserModel>> RegisterAsync(UserRegistrationModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms the user's email address.
    /// </summary>
    /// <param name="identityToken">
    /// An encrypted string value to be used to
    /// confirm the user's email address.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserModel"/>
    /// containing information about the updated user.
    /// </returns>
    Task<Result<UserModel>> ConfirmEmailAsync(string identityToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a new user authorization token or
    /// two-factor authentication token in case it is enabled for the user.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserLoginModel"/> to be used to
    /// validate login data and generate an access token.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserAuthorizationTokenModel"/>
    /// containing information about the user authorization token or
    /// two-factor authentication token in case it is enabled for the user.
    /// </returns>
    Task<Result<UserAuthorizationTokenModel>> LoginAsync(UserLoginModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a new user authorization token
    /// for the user with two-factor authentication enabled.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserTwoFactorAuthenticationModel"/> to be used to
    /// validate the login data and generate an access token.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserAuthorizationTokenModel"/>
    /// containing information about the user authorization token.
    /// </returns>
    Task<Result<UserAuthorizationTokenModel>> ConfirmLoginAsync(UserTwoFactorAuthenticationModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a new user authorization token
    /// using the given <paramref name="accessToken"/>.
    /// </summary>
    /// <param name="accessToken">
    /// User authorization token.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserAuthorizationTokenModel"/>
    /// containing information about the user authorization token.
    /// </returns>
    Task<Result<UserAuthorizationTokenModel>> RenewTokenAsync(string accessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates the refresh token associated with
    /// the given <paramref name="accessToken"/>
    /// to make it unusable during the authentication process.
    /// </summary>
    /// <param name="accessToken">
    /// Authorization token to be invalidated.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    Task<Result> LogoutAsync(string accessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates all user's refresh tokens
    /// to make them all unusable during the authentication process.
    /// </summary>
    /// <param name="accessToken">
    /// Authorization token to be invalidated.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    Task<Result> LogoutEverywhereAsync(string accessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a password reset request.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserResetPasswordRequestModel"/> to be used to
    /// create a password reset request.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    Task<Result> CreatePasswordResetRequestAsync(UserResetPasswordRequestModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the user's password.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserResetPasswordModel"/> to be used to
    /// reset the user's password.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    Task<Result> ResetPasswordAsync(UserResetPasswordModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an email address reset request.
    /// </summary>
    /// <param name="userId">
    /// The Id of the user whose email to be reset.
    /// </param>
    /// <param name="model">
    /// The <see cref="UserResetEmailRequestModel"/> to be used to
    /// create an email address reset request.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    Task<Result> CreateEmailResetRequestAsync(Guid userId, UserResetEmailRequestModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the user's email address.
    /// </summary>
    /// <param name="identityToken">
    /// An encrypted string value to be used to
    /// reset the user's email address.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    Task<Result> ResetEmailAsync(string identityToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the user.
    /// </summary>
    /// <param name="userId">
    /// The Id of the user to be updated.
    /// </param>
    /// <param name="model">
    /// The <see cref="UserUpdateModel"/> to be used to
    /// update the user.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserModel"/>
    /// containing information about the updated user.
    /// </returns>
    Task<Result<UserModel>> UpdateAsync(Guid userId, UserUpdateModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the user.
    /// </summary>
    /// <param name="userId">
    /// The Id of the user to be deleted.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    Task<Result> DeleteAsync(Guid userId, CancellationToken cancellationToken = default);
    #endregion
}