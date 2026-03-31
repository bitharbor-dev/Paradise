using Paradise.Models;
using Paradise.Models.Domain.Identity.Users;

namespace Paradise.ApplicationLogic.Services.Identity.Users;

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
    /// Gets the user with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
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
    Task<Result<UserModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user with the given <paramref name="emailAddress"/>.
    /// </summary>
    /// <param name="emailAddress">
    /// The email address of the user to be found.
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
    Task<Result<UserModel>> GetByEmailAddressAsync(string emailAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user with the given <paramref name="phoneNumber"/>.
    /// </summary>
    /// <param name="phoneNumber">
    /// The phone number of the user to be found.
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
    Task<Result<UserModel>> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user with the given <paramref name="userName"/>.
    /// </summary>
    /// <param name="userName">
    /// The user-name of the user to be found.
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
    Task<Result<UserModel>> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the claims list which belong to the user with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    /// The Id of the user whose claims to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="UserClaimModel"/>
    /// containing information about the claims, which belong
    /// to the user with the given <paramref name="id"/>.
    /// </returns>
    Task<Result<IEnumerable<UserClaimModel>>> GetUserClaimsAsync(Guid id, CancellationToken cancellationToken = default);

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
    Task<Result<UserModel>> ConfirmEmailAddressAsync(string identityToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the given <paramref name="password"/>
    /// is valid for the user with the given <paramref name="id"/>.
    /// </summary>
    /// <remarks>
    /// This action also modifies the user's lockout state
    /// depending on successful on unsuccessful password checks,
    /// if lockout is enabled for the user.
    /// </remarks>
    /// <param name="id">
    /// The Id of the user whose password to be checked.
    /// </param>
    /// <param name="password">
    /// User's password.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    Task<Result> CheckPasswordAsync(Guid id, string password, CancellationToken cancellationToken = default);

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
    /// <param name="id">
    /// The Id of the user whose email to be reset.
    /// </param>
    /// <param name="model">
    /// The <see cref="UserResetEmailAddressRequestModel"/> to be used to
    /// create an email address reset request.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    Task<Result> CreateEmailAddressResetRequestAsync(Guid id, UserResetEmailAddressRequestModel model, CancellationToken cancellationToken = default);

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
    Task<Result> ResetEmailAddressAsync(string identityToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the user.
    /// </summary>
    /// <param name="id">
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
    Task<Result<UserModel>> UpdateAsync(Guid id, UserUpdateModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the user.
    /// </summary>
    /// <param name="id">
    /// The Id of the user to be deleted.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the users whose email addresses were not confirmed by the time of
    /// the email address confirmation period expiration.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="int"/>
    /// representing the number of deleted users.
    /// </returns>
    Task<Result<int>> DeleteUnconfirmedUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels the users' deletion requests for those users who were not deleted by the time of
    /// the deletion period expiration.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="int"/>
    /// representing the number of updated users.
    /// </returns>
    Task<Result<int>> CancelExpiredDeletionRequestsAsync(CancellationToken cancellationToken = default);
    #endregion
}