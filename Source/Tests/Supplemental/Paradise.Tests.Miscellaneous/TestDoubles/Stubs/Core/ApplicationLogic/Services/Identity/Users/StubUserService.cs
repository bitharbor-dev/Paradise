using Paradise.ApplicationLogic.Services.Identity.Users;
using Paradise.Models;
using Paradise.Models.Domain.Identity.Users;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Core.ApplicationLogic.Services.Identity.Users;

/// <summary>
/// Stub <see cref="IUserService"/> implementation.
/// </summary>
public sealed class StubUserService : IUserService
{
    #region Properties
    /// <summary>
    /// <see cref="GetAllAsync"/> result.
    /// </summary>
    public Func<Task<Result<IEnumerable<UserModel>>>>? GetAllAsyncResult { get; set; }

    /// <summary>
    /// <see cref="GetByIdAsync"/> result.
    /// </summary>
    public Func<Task<Result<UserModel>>>? GetByIdAsyncResult { get; set; }

    /// <summary>
    /// <see cref="GetByEmailAddressAsync"/> result.
    /// </summary>
    public Func<Task<Result<UserModel>>>? GetByEmailAddressAsyncResult { get; set; }

    /// <summary>
    /// <see cref="GetByPhoneNumberAsync"/> result.
    /// </summary>
    public Func<Task<Result<UserModel>>>? GetByPhoneNumberAsyncResult { get; set; }

    /// <summary>
    /// <see cref="GetByUserNameAsync"/> result.
    /// </summary>
    public Func<Task<Result<UserModel>>>? GetByUserNameAsyncResult { get; set; }

    /// <summary>
    /// <see cref="GetUserClaimsAsync"/> result.
    /// </summary>
    public Func<Task<Result<IEnumerable<UserClaimModel>>>>? GetUserClaimsAsyncResult { get; set; }

    /// <summary>
    /// <see cref="RegisterAsync"/> result.
    /// </summary>
    public Func<Task<Result<UserModel>>>? RegisterAsyncResult { get; set; }

    /// <summary>
    /// <see cref="ConfirmEmailAddressAsync"/> result.
    /// </summary>
    public Func<Task<Result<UserModel>>>? ConfirmEmailAsyncResult { get; set; }

    /// <summary>
    /// <see cref="CheckPasswordAsync"/> result.
    /// </summary>
    public Func<Task<Result>>? CheckPasswordAsyncResult { get; set; }

    /// <summary>
    /// <see cref="CreatePasswordResetRequestAsync"/> result.
    /// </summary>
    public Func<Task<Result>>? CreatePasswordResetRequestAsyncResult { get; set; }

    /// <summary>
    /// <see cref="ResetPasswordAsync"/> result.
    /// </summary>
    public Func<Task<Result>>? ResetPasswordAsyncResult { get; set; }

    /// <summary>
    /// <see cref="CreateEmailAddressResetRequestAsync"/> result.
    /// </summary>
    public Func<Task<Result>>? CreateEmailAddressResetRequestAsyncResult { get; set; }

    /// <summary>
    /// <see cref="ResetEmailAddressAsync"/> result.
    /// </summary>
    public Func<Task<Result>>? ResetEmailAddressAsyncResult { get; set; }

    /// <summary>
    /// <see cref="UpdateAsync"/> result.
    /// </summary>
    public Func<Task<Result<UserModel>>>? UpdateAsyncResult { get; set; }

    /// <summary>
    /// <see cref="DeleteAsync"/> result.
    /// </summary>
    public Func<Task<Result>>? DeleteAsyncResult { get; set; }

    /// <summary>
    /// <see cref="DeleteUnconfirmedUsersAsync"/> result.
    /// </summary>
    public Func<Task<Result<int>>>? DeleteUnconfirmedUsersAsyncResult { get; set; }

    /// <summary>
    /// <see cref="CancelExpiredDeletionRequestsAsync"/> result.
    /// </summary>
    public Func<Task<Result<int>>>? CancelExpiredDeletionRequestsAsyncResult { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task<Result<IEnumerable<UserModel>>> GetAllAsync(CancellationToken cancellationToken = default)
        => GetAllAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<UserModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => GetByIdAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<UserModel>> GetByEmailAddressAsync(string emailAddress, CancellationToken cancellationToken = default)
        => GetByEmailAddressAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<UserModel>> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        => GetByPhoneNumberAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<UserModel>> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        => GetByUserNameAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<IEnumerable<UserClaimModel>>> GetUserClaimsAsync(Guid id, CancellationToken cancellationToken = default)
        => GetUserClaimsAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<UserModel>> RegisterAsync(UserRegistrationModel model, CancellationToken cancellationToken = default)
        => RegisterAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<UserModel>> ConfirmEmailAddressAsync(string identityToken, CancellationToken cancellationToken = default)
        => ConfirmEmailAsyncResult!();

    /// <inheritdoc/>
    public Task<Result> CheckPasswordAsync(Guid id, string password, CancellationToken cancellationToken = default)
        => CheckPasswordAsyncResult!();

    /// <inheritdoc/>
    public Task<Result> CreatePasswordResetRequestAsync(UserResetPasswordRequestModel model,
                                                        CancellationToken cancellationToken = default)
        => CreatePasswordResetRequestAsyncResult!();

    /// <inheritdoc/>
    public Task<Result> ResetPasswordAsync(UserResetPasswordModel model, CancellationToken cancellationToken = default)
        => ResetPasswordAsyncResult!();

    /// <inheritdoc/>
    public Task<Result> CreateEmailAddressResetRequestAsync(Guid id, UserResetEmailAddressRequestModel model,
                                                            CancellationToken cancellationToken = default)
        => CreateEmailAddressResetRequestAsyncResult!();

    /// <inheritdoc/>
    public Task<Result> ResetEmailAddressAsync(string identityToken, CancellationToken cancellationToken = default)
        => ResetEmailAddressAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<UserModel>> UpdateAsync(Guid id, UserUpdateModel model, CancellationToken cancellationToken = default)
        => UpdateAsyncResult!();

    /// <inheritdoc/>
    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => DeleteAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<int>> DeleteUnconfirmedUsersAsync(CancellationToken cancellationToken = default)
        => DeleteUnconfirmedUsersAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<int>> CancelExpiredDeletionRequestsAsync(CancellationToken cancellationToken = default)
        => CancelExpiredDeletionRequestsAsyncResult!();
    #endregion
}