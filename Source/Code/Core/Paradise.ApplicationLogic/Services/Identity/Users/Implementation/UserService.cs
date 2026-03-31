using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.DataConverters.Domain.Identity.Users;
using Paradise.ApplicationLogic.Infrastructure.DataProtection;
using Paradise.ApplicationLogic.Infrastructure.Extensions;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.ApplicationLogic.Options.Models;
using Paradise.Common.Extensions;
using Paradise.Domain.Base.Events;
using Paradise.Domain.Events.Identity.Users;
using Paradise.Domain.Identity.Users;
using Paradise.Models;
using Paradise.Models.Domain.Identity.Users;
using System.Globalization;
using static Paradise.Models.ErrorCode;
using static Paradise.Models.OperationStatus;

namespace Paradise.ApplicationLogic.Services.Identity.Users.Implementation;

/// <summary>
/// Provides users management functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserService"/> class.
/// </remarks>
/// <param name="logger">
/// Logger.
/// </param>
/// <param name="applicationOptions">
/// The accessor used to access the <see cref="ApplicationOptions"/>.
/// </param>
/// <param name="timeProvider">
/// Time provider.
/// </param>
/// <param name="userManager">
/// User manager.
/// </param>
/// <param name="domainEventSink">
/// Domain event sink.
/// </param>
/// <param name="dataProtector">
/// Data protector.
/// </param>
internal sealed class UserService(ILogger<UserService> logger,
                                  IOptions<ApplicationOptions> applicationOptions,
                                  TimeProvider timeProvider,
                                  IUserManager<User> userManager,
                                  IDomainEventSink domainEventSink,
                                  IDataProtector dataProtector) : IUserService
{
    #region Public methods
    /// <inheritdoc/>
    public async Task<Result<IEnumerable<UserModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await userManager
            .Users
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new(users.Select(user => user.ToModel()), Success);
    }

    /// <inheritdoc/>
    public async Task<Result<UserModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(id)
            .ConfigureAwait(false);

        return user is null
            ? new(Missing, UserIdNotFound, id)
            : new(user.ToModel(), Success);
    }

    /// <inheritdoc/>
    public async Task<Result<UserModel>> GetByEmailAddressAsync(string emailAddress, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(emailAddress)
            .ConfigureAwait(false);

        return user is null
            ? new(Missing, UserEmailAddressNotFound, emailAddress)
            : new(user.ToModel(), Success);
    }

    /// <inheritdoc/>
    public async Task<Result<UserModel>> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByPhoneNumberAsync(phoneNumber)
            .ConfigureAwait(false);

        return user is null
            ? new(Missing, UserPhoneNumberNotFound, phoneNumber)
            : new(user.ToModel(), Success);
    }

    /// <inheritdoc/>
    public async Task<Result<UserModel>> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByNameAsync(userName)
            .ConfigureAwait(false);

        return user is null
            ? new(Missing, UserUserNameNotFound, userName)
            : new(user.ToModel(), Success);
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<UserClaimModel>>> GetUserClaimsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(id)
            .ConfigureAwait(false);

        if (user is null)
            return new(Missing, UserIdNotFound, id);

        var claims = await userManager.GetClaimsAsync(user)
            .ConfigureAwait(false);

        return new(claims.Select(claim => claim.ToModel()), Success);
    }

    /// <inheritdoc/>
    public async Task<Result<UserModel>> RegisterAsync(UserRegistrationModel model, CancellationToken cancellationToken = default)
    {
        if (model is null)
            return new(InvalidInput, InvalidModel, nameof(model));

        var validationResult = await ValidateRegistrationModelAsync(model)
            .ConfigureAwait(false);

        if (!validationResult.IsSuccess)
            return new(validationResult.Status, validationResult.Errors, default);

        var user = model.ToEntity();

        var creationResult = await userManager.CreateAsync(user, model.Password)
            .ConfigureAwait(false);

        if (!creationResult.Succeeded)
            return creationResult.GetResult<UserModel>();

        var emailAddressConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user)
            .ConfigureAwait(false);

        var registrationEvent = new UserRegisteredEvent(timeProvider.GetUtcNow(),
                                                        user.Email,
                                                        emailAddressConfirmationToken,
                                                        CultureInfo.CurrentCulture);

        await domainEventSink.PushAsync(registrationEvent, cancellationToken)
            .ConfigureAwait(false);

        return new(user.ToModel(), Created);
    }

    /// <inheritdoc/>
    public async Task<Result<UserModel>> ConfirmEmailAddressAsync(string identityToken, CancellationToken cancellationToken = default)
    {
        if (!dataProtector.TryUnprotect<IdentityToken>(identityToken, out var identityTokenModel))
            return new(InvalidInput, InvalidToken);

        if (identityTokenModel.IsExpired(timeProvider.GetUtcNow()))
            return new(Blocked, OutdatedToken);

        var emailAddress = identityTokenModel.EmailAddress;
        var emailConfirmationToken = identityTokenModel.InnerToken;
        var expiryDate = identityTokenModel.ExpiryDate;

        if (!expiryDate.HasValue)
            return new(InvalidInput, InvalidToken);

        var user = await userManager.FindByEmailAsync(emailAddress)
            .ConfigureAwait(false);

        if (user is null)
            return new(Missing, UserEmailAddressNotFound, emailAddress);

        if (user.EmailConfirmed)
            return new(Blocked, UserEmailAddressAlreadyConfirmed, user.Email);

        var timeout = applicationOptions.Value.Timeout.EmailConfirmationTimeout;

        if (user.CanConfirmEmailAddress(timeout, timeProvider.GetUtcNow()))
            return new(Blocked, OutdatedToken);

        var emailConfirmationResult = await userManager.ConfirmEmailAsync(user, emailConfirmationToken)
            .ConfigureAwait(false);

        if (!emailConfirmationResult.Succeeded)
            return emailConfirmationResult.GetResult<UserModel>();

        var confirmationEvent = new EmailAddressConfirmedEvent(timeProvider.GetUtcNow(), user.Id);

        await domainEventSink.PushAsync(confirmationEvent, cancellationToken)
            .ConfigureAwait(false);

        return new(user.ToModel(), Success);
    }

    /// <inheritdoc/>
    public async Task<Result> CheckPasswordAsync(Guid id, string password, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(id)
            .ConfigureAwait(false);

        if (user is null)
            return new(Missing, UserIdNotFound, id);

        // Step 1: Check if the used is locked out.
        var userLockedOut = await userManager
            .IsLockedOutAsync(user)
            .ConfigureAwait(false);

        if (user.LockoutEnabled && userLockedOut)
            return new(Prohibited, UserLockedOut);

        // Step 2: Check the user's password and increment failed access attempts number
        // if the password is incorrect.
        var passwordIsCorrect = await userManager.CheckPasswordAsync(user, password)
            .ConfigureAwait(false);

        if (!passwordIsCorrect)
        {
            if (user.LockoutEnabled)
            {
                var incrementResult = await userManager.AccessFailedAsync(user)
                     .ConfigureAwait(false);

                if (!incrementResult.Succeeded)
                    logger.LogIdentityResult(incrementResult);
            }

            return new(Unauthorized, UserNotFoundOrPasswordMismatch);
        }

        // Step 3: Reaching this line means the user is not locked out
        // and the password is correct - reset failed access attempts number.
        if (user.AccessFailedCount is not 0 || user.LockoutEnd is not null)
        {
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;

            var updateResult = await userManager.UpdateAsync(user)
                .ConfigureAwait(false);

            if (!updateResult.Succeeded)
                logger.LogIdentityResult(updateResult);
        }

        return new(Success);
    }

    /// <inheritdoc/>
    public async Task<Result> CreatePasswordResetRequestAsync(
        UserResetPasswordRequestModel model, CancellationToken cancellationToken = default)
    {
        if (model is null)
            return new(InvalidInput, InvalidModel, nameof(model));

        if (model.EmailAddress.IsNullOrWhiteSpace())
            return new(InvalidInput, InvalidEmailAddress, model.EmailAddress);

        if (!model.EmailAddress.IsValidEmailAddress())
            return new(InvalidInput, InvalidEmailAddress, model.EmailAddress);

        var user = await userManager.FindByEmailAsync(model.EmailAddress)
            .ConfigureAwait(false);

        if (user is not null)
        {
            var changePasswordToken = await userManager.GeneratePasswordResetTokenAsync(user)
                .ConfigureAwait(false);

            var resetRequestedEvent = new PasswordResetRequestedEvent(timeProvider.GetUtcNow(),
                                                                      user.Email,
                                                                      changePasswordToken,
                                                                      CultureInfo.CurrentCulture);

            await domainEventSink.PushAsync(resetRequestedEvent, cancellationToken)
                .ConfigureAwait(false);
        }

        return new(Received);
    }

    /// <inheritdoc/>
    public async Task<Result> ResetPasswordAsync(UserResetPasswordModel model, CancellationToken cancellationToken = default)
    {
        if (model is null)
            return new(InvalidInput, InvalidModel, nameof(model));

        if (model.Password.IsNullOrWhiteSpace())
            return new(InvalidInput, PasswordMissing);

        if (!dataProtector.TryUnprotect<IdentityToken>(model.IdentityToken, out var identityTokenModel))
            return new(InvalidInput, InvalidToken);

        if (identityTokenModel.IsExpired(timeProvider.GetUtcNow()))
            return new(Blocked, OutdatedToken);

        var passwordValidationResult = await ValidatePasswordAsync(model.Password, model.PasswordConfirmation)
            .ConfigureAwait(false);

        if (!passwordValidationResult.IsSuccess)
            return passwordValidationResult;

        var emailAddress = identityTokenModel.EmailAddress;
        var passwordResetToken = identityTokenModel.InnerToken;

        var user = await userManager.FindByEmailAsync(emailAddress)
            .ConfigureAwait(false);

        if (user is null)
            return new(Missing, UserEmailAddressNotFound, emailAddress);

        var passwordResetResult = await userManager.ResetPasswordAsync(user, passwordResetToken, model.Password)
            .ConfigureAwait(false);

        if (!passwordResetResult.Succeeded)
            return passwordResetResult.GetResult();

        var resetCompletedEvent = new PasswordResetCompletedEvent(timeProvider.GetUtcNow(),
                                                                  user.Email,
                                                                  CultureInfo.CurrentCulture);

        await domainEventSink.PushAsync(resetCompletedEvent, cancellationToken)
            .ConfigureAwait(false);

        return new(Success);
    }

    /// <inheritdoc/>
    public async Task<Result> CreateEmailAddressResetRequestAsync(
        Guid id, UserResetEmailAddressRequestModel model, CancellationToken cancellationToken = default)
    {
        if (model is null)
            return new(InvalidInput, InvalidModel, nameof(model));

        if (model.EmailAddress.IsNullOrWhiteSpace())
            return new(InvalidInput, InvalidEmailAddress, model.EmailAddress);

        if (!model.EmailAddress.IsValidEmailAddress())
            return new(InvalidInput, InvalidEmailAddress, model.EmailAddress);

        if (!model.EmailAddress.Equals(model.EmailAddressConfirmation, StringComparison.OrdinalIgnoreCase))
            return new(InvalidInput, EmailAddressNotMatchConfirmation, model.EmailAddress, model.EmailAddressConfirmation);

        var emailAddressIsInUse = await CheckIfEmailAddressIsInUseAsync(model.EmailAddress)
            .ConfigureAwait(false);

        if (emailAddressIsInUse)
            return new(Blocked, DuplicateEmailAddress, model.EmailAddress);

        var user = await userManager.FindByIdAsync(id)
            .ConfigureAwait(false);

        if (user is null)
            return new(Missing, UserIdNotFound, id);

        var chageEmailAddressToken = await userManager.GenerateChangeEmailTokenAsync(user, model.EmailAddress)
            .ConfigureAwait(false);

        var resetRequestedEvent = new EmailAddressResetRequestedEvent(timeProvider.GetUtcNow(),
                                                                      user.UserName,
                                                                      chageEmailAddressToken,
                                                                      user.Email,
                                                                      model.EmailAddress,
                                                                      CultureInfo.CurrentCulture);

        await domainEventSink.PushAsync(resetRequestedEvent, cancellationToken)
            .ConfigureAwait(false);

        return new(Success);
    }

    /// <inheritdoc/>
    public async Task<Result> ResetEmailAddressAsync(string identityToken, CancellationToken cancellationToken = default)
    {
        if (!dataProtector.TryUnprotect<IdentityToken>(identityToken, out var identityTokenModel))
            return new(InvalidInput, InvalidToken);

        if (identityTokenModel.IsExpired(timeProvider.GetUtcNow()))
            return new(Blocked, OutdatedToken);

        var oldEmailAddress = identityTokenModel.EmailAddress;
        var newEmailAddress = identityTokenModel.NewValue;
        var changeEmailAddressToken = identityTokenModel.InnerToken;

        if (newEmailAddress.IsNullOrWhiteSpace() || !newEmailAddress.IsValidEmailAddress())
            return new(InvalidInput, InvalidToken);

        var emailAddressIsInUse = await CheckIfEmailAddressIsInUseAsync(newEmailAddress)
            .ConfigureAwait(false);

        if (emailAddressIsInUse)
            return new(Blocked, DuplicateEmailAddress, newEmailAddress);

        var user = await userManager.FindByEmailAsync(oldEmailAddress)
            .ConfigureAwait(false);

        if (user is null)
            return new(Missing, UserEmailAddressNotFound, oldEmailAddress);

        var changeEmailResult = await userManager.ChangeEmailAsync(user, newEmailAddress, changeEmailAddressToken)
            .ConfigureAwait(false);

        if (!changeEmailResult.Succeeded)
            return changeEmailResult.GetResult();

        var resetCompletedEvent = new EmailAddressResetCompletedEvent(timeProvider.GetUtcNow(),
                                                                      user.UserName,
                                                                      user.Email,
                                                                      oldEmailAddress,
                                                                      CultureInfo.CurrentCulture);

        await domainEventSink.PushAsync(resetCompletedEvent, cancellationToken)
            .ConfigureAwait(false);

        return new(Success);
    }

    /// <inheritdoc/>
    public async Task<Result<UserModel>> UpdateAsync(Guid id, UserUpdateModel model, CancellationToken cancellationToken = default)
    {
        if (model is null)
            return new(InvalidInput, InvalidModel, nameof(model));

        var user = await userManager.FindByIdAsync(id)
            .ConfigureAwait(false);

        if (user is null)
            return new(Missing, UserIdNotFound, id);

        if (model.IsPendingDeletion.HasValue)
        {
            user.DeletionRequestSubmitted = model.IsPendingDeletion.Value
                ? timeProvider.GetUtcNow()
                : null;
        }

        if (model.TwoFactorEnabled.HasValue)
            user.TwoFactorEnabled = model.TwoFactorEnabled.Value;

        if (model.UserName is not null)
        {
            var allowedCharacters = userManager.Options.User.AllowedUserNameCharacters;

            if (!model.UserName.IsValidUserName(allowedCharacters))
                return new(InvalidInput, InvalidUserName, model.UserName);

            var userNameIsInUse = await CheckIfUserNameIsInUseAsync(model.UserName)
                .ConfigureAwait(false);

            if (userNameIsInUse)
                return new(Blocked, DuplicateUserName, model.UserName);

            user.UserName = model.UserName;
        }

        var upadteResult = await userManager.UpdateAsync(user)
            .ConfigureAwait(false);

        return upadteResult.Succeeded
            ? new(user.ToModel(), Success)
            : upadteResult.GetResult<UserModel>();
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(id)
            .ConfigureAwait(false);

        if (user is null)
            return new(Missing, UserIdNotFound, id);

        if (!user.IsPendingDeletion)
            return new(InvalidInput, UserNotPendingDeletion, user.Email);

        var timeout = applicationOptions.Value.Timeout.UserDeletionRequestTimeout;

        if (!user.CanBeDeleted(timeout, timeProvider.GetUtcNow()))
        {
            user.CancelDeletionRequest();

            var updateResult = await userManager.UpdateAsync(user)
                .ConfigureAwait(false);

            if (!updateResult.Succeeded)
                logger.LogIdentityResult(updateResult);

            return new(InvalidInput, UserDeletionRequestExpired, timeout);
        }

        var deletionResult = await userManager.DeleteAsync(user)
            .ConfigureAwait(false);

        if (!deletionResult.Succeeded)
        {
            logger.LogIdentityResult(deletionResult);

            return deletionResult.GetResult();
        }

        return new(Success);
    }

    /// <inheritdoc/>
    public async Task<Result<int>> DeleteUnconfirmedUsersAsync(CancellationToken cancellationToken = default)
    {
        var currentTime = timeProvider.GetUtcNow();

        var timeout = applicationOptions.Value.Timeout.EmailConfirmationTimeout;

        var users = await userManager
            .Users
            .Where(user => user.Created.Add(timeout) < currentTime && !user.EmailConfirmed)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var deletedUsers = 0;

        foreach (var user in users)
        {
            await userManager.DeleteAsync(user)
                .ConfigureAwait(false);

            deletedUsers++;
        }

        return new(deletedUsers, Success);
    }

    /// <inheritdoc/>
    public async Task<Result<int>> CancelExpiredDeletionRequestsAsync(CancellationToken cancellationToken = default)
    {
        var currentTime = timeProvider.GetUtcNow();

        var timeout = applicationOptions.Value.Timeout.UserDeletionRequestTimeout;

        var users = await userManager
            .Users
            .Where(user => user.DeletionRequestSubmitted.HasValue && user.DeletionRequestSubmitted.Value.Add(timeout) < currentTime)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var updatedUsersNumber = 0;

        foreach (var user in users)
        {
            user.CancelDeletionRequest();

            await userManager.UpdateAsync(user)
                .ConfigureAwait(false);

            updatedUsersNumber++;
        }

        return new(updatedUsersNumber, Success);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Validates the given <paramref name="model"/> in order to
    /// determine whether the registration can proceed.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserRegistrationModel"/>
    /// which data to be used to register a new user.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    private async Task<Result> ValidateRegistrationModelAsync(UserRegistrationModel model)
    {
        var result = new Result(Success);

        var emailAddressValidationResult = await ValidateEmailAddressAsync(model.EmailAddress)
            .ConfigureAwait(false);

        result.AddErrors(emailAddressValidationResult.Errors);

        var passwordValidationResult = await ValidatePasswordAsync(model.Password, model.PasswordConfirmation)
            .ConfigureAwait(false);

        result.AddErrors(passwordValidationResult.Errors);

        if (model.PhoneNumber is not null)
        {
            var phoneNumberValidationResult = await ValidatePhoneNumberAsync(model.PhoneNumber)
                .ConfigureAwait(false);

            result.AddErrors(phoneNumberValidationResult.Errors);
        }

        var userNameValidationResult = await ValidateUserNameAsync(model.UserName)
            .ConfigureAwait(false);

        result.AddErrors(userNameValidationResult.Errors);

        if (!result.IsSuccess)
            result.Status = InvalidInput;

        return result;
    }

    /// <summary>
    /// Checks if the given <paramref name="emailAddress"/> meets the requirements.
    /// </summary>
    /// <param name="emailAddress">
    /// The email address to be checked.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    private async Task<Result> ValidateEmailAddressAsync(string emailAddress)
    {
        if (!emailAddress.IsValidEmailAddress())
            return new(InvalidInput, InvalidEmailAddress, emailAddress);

        var emailTaken = await CheckIfEmailAddressIsInUseAsync(emailAddress)
            .ConfigureAwait(false);

        return emailTaken
            ? new(Blocked, DuplicateEmailAddress, emailAddress)
            : new(Success);
    }

    /// <summary>
    /// Checks if the given <paramref name="password"/> meets the requirements.
    /// </summary>
    /// <param name="password">
    /// The password to be checked.
    /// </param>
    /// <param name="passwordConfirmation">
    /// Password confirmation value.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    private async Task<Result> ValidatePasswordAsync(string password, string passwordConfirmation)
    {
        if (!password.Equals(passwordConfirmation, StringComparison.Ordinal))
            return new(InvalidInput, PasswordNotMatchConfirmation);

        var validationResult = await userManager.ValidatePasswordAsync(null!, password)
            .ConfigureAwait(false);

        return validationResult.Succeeded
            ? new(Success)
            : validationResult.GetResult(InvalidInput);
    }

    /// <summary>
    /// Checks if the given <paramref name="phoneNumber"/> meets the requirements.
    /// </summary>
    /// <param name="phoneNumber">
    /// The phone number to be checked.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    private async Task<Result> ValidatePhoneNumberAsync(string phoneNumber)
    {
        if (!phoneNumber.IsValidPhoneNumber())
            return new(InvalidInput, InvalidPhoneNumber, phoneNumber);

        var phoneNumberTaken = await CheckIfPhoneNumberIsInUseAsync(phoneNumber)
            .ConfigureAwait(false);

        return phoneNumberTaken
            ? new(Blocked, DuplicatePhoneNumber, phoneNumber)
            : new(Success);
    }

    /// <summary>
    /// Checks if the given <paramref name="userName"/> meets the requirements.
    /// </summary>
    /// <param name="userName">
    /// The user-name to be checked.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    private async Task<Result> ValidateUserNameAsync(string userName)
    {
        var allowedCharacters = userManager.Options.User.AllowedUserNameCharacters;

        if (!userName.IsValidUserName(allowedCharacters))
            return new(InvalidInput, InvalidUserName, userName);

        var userNameTaken = await CheckIfUserNameIsInUseAsync(userName)
            .ConfigureAwait(false);

        return userNameTaken
            ? new(Blocked, DuplicateUserName, userName)
            : new(Success);
    }

    /// <summary>
    /// Checks if the given <paramref name="emailAddress"/> address is already in use.
    /// </summary>
    /// <param name="emailAddress">
    /// The email address to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="emailAddress"/> address is already in use,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    private async Task<bool> CheckIfEmailAddressIsInUseAsync(string emailAddress)
    {
        // "userManager.Users.AnyAsync()" is not used here
        // because "FindByEmailAsync" method takes into account
        // that user's email address might be stored as protected data.
        var user = await userManager.FindByEmailAsync(emailAddress)
            .ConfigureAwait(false);

        return user is not null;
    }

    /// <summary>
    /// Checks if the given <paramref name="phoneNumber"/> number is already in use.
    /// </summary>
    /// <param name="phoneNumber">
    /// The phone number to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="phoneNumber"/> number is already in use,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    private async Task<bool> CheckIfPhoneNumberIsInUseAsync(string phoneNumber)
    {
        // "userManager.Users.AnyAsync()" is not used here
        // because "FindByPhoneNumberAsync" method takes into account
        // that user's phone number might be stored as protected data.
        var user = await userManager.FindByPhoneNumberAsync(phoneNumber)
            .ConfigureAwait(false);

        return user is not null;
    }

    /// <summary>
    /// Checks if the given <paramref name="userName"/> is already in use.
    /// </summary>
    /// <param name="userName">
    /// The user-name to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="userName"/> is already in use,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    private async Task<bool> CheckIfUserNameIsInUseAsync(string userName)
    {
        // "userManager.Users.AnyAsync()" is not used here
        // because "FindByNameAsync" method takes into account
        // that user's name might be stored as protected data.
        var user = await userManager.FindByNameAsync(userName)
            .ConfigureAwait(false);

        return user is not null;
    }
    #endregion
}