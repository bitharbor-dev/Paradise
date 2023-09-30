using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.DataConverters.Domain;
using Paradise.ApplicationLogic.Exceptions;
using Paradise.ApplicationLogic.Extensions;
using Paradise.ApplicationLogic.Identity;
using Paradise.ApplicationLogic.InternalModels;
using Paradise.ApplicationLogic.Services.Application;
using Paradise.Common.Extensions;
using Paradise.Common.Web;
using Paradise.DataAccess.Repositories.Domain;
using Paradise.Domain.Users;
using Paradise.Localization.DataValidation;
using Paradise.Models;
using Paradise.Models.Application.CommunicationModels;
using Paradise.Models.Domain.UserModels;
using Paradise.Options.Models;
using Paradise.Options.Models.Communication;
using System.Globalization;
using System.Security.Claims;
using System.Web;
using static Paradise.ApplicationLogic.Exceptions.ResultException;
using static Paradise.Common.Web.ParameterNames;
using static Paradise.Models.ErrorCode;
using static System.Net.HttpStatusCode;

namespace Paradise.ApplicationLogic.Services.Domain.Implementation;

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
/// <param name="jwtBearerOptions">
/// The accessor used to access the <see cref="JwtBearerOptions"/>.
/// </param>
/// <param name="emailTemplateOptions">
/// The accessor used to access the <see cref="EmailTemplateOptions"/>.
/// </param>
/// <param name="identityOptions">
/// The accessor used to access the <see cref="IdentityOptions"/>.
/// </param>
/// <param name="userManager">
/// User manager.
/// </param>
/// <param name="userRefreshTokensRepository">
/// User refresh tokens repository.
/// </param>
/// <param name="roleService">
/// Role service.
/// </param>
/// <param name="communicationService">
/// Communication service.
/// </param>
/// <param name="jsonWebTokenService">
/// JWT service.
/// </param>
/// <param name="dataProtectionService">
/// Data protection service.
/// </param>
public sealed class UserService(ILogger<UserService> logger,
                                IOptions<ApplicationOptions> applicationOptions,
                                IOptions<JwtBearerOptions> jwtBearerOptions,
                                IOptions<EmailTemplateOptions> emailTemplateOptions,
                                IOptions<IdentityOptions> identityOptions,
                                UserManager userManager,
                                IUserRefreshTokensRepository userRefreshTokensRepository,
                                IRoleService roleService,
                                ICommunicationService communicationService,
                                IJsonWebTokenService jsonWebTokenService,
                                IDataProtectionService dataProtectionService)
    : IUserService
{
    #region Fields
    private readonly ApplicationOptions _applicationOptions = applicationOptions.Value;
    private readonly JwtBearerOptions _jwtBearerOptions = jwtBearerOptions.Value;
    private readonly EmailTemplateOptions _emailTemplateOptions = emailTemplateOptions.Value;
    private readonly IdentityOptions _identityOptions = identityOptions.Value;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async Task<Result<IEnumerable<UserModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await userManager.Users.ToListAsync(cancellationToken);

        return new(users.Select(user => user.ToModel()), OK);
    }

    /// <inheritdoc/>
    public async Task<Result<UserModel>> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await GetUserByIdAsync(userId, cancellationToken);

        return new(user.ToModel(), OK);
    }

    /// <inheritdoc/>
    public async Task<Result<UserModel>> RegisterAsync(UserRegistrationModel model, CancellationToken cancellationToken = default)
    {
        await ValidateRegistrationModelAsync(model);

        var user = model.ToEntity();

        var creationResult = await userManager.CreateAsync(user, model.Password!);

        creationResult.ThrowIfUnsuccessfulIdentityResult();

        try
        {
            await SendEmailAddressConfirmationEmailAsync(user, cancellationToken);
        }
        catch
        {
            var deletionResult = await userManager.DeleteAsync(user);

            if (!deletionResult.Succeeded)
                logger.LogUnsuccessfulUserDeletionAfterFailedInvitation(user.Email, deletionResult);

            throw;
        }

        return new(user.ToModel(), Created);
    }

    /// <inheritdoc/>
    public async Task<Result<UserModel>> ConfirmEmailAsync(string identityToken, CancellationToken cancellationToken = default)
    {
        dataProtectionService
            .TryUnprotectJson<IdentityToken>(identityToken, out var identityTokenModel)
            .ThrowIfFalse(BadRequest, InvalidToken);

        identityTokenModel.IsOutdated().ThrowIfTrue(UnprocessableEntity, OutdatedToken);

        var email = identityTokenModel.Email;
        var emailConfirmationToken = identityTokenModel.InnerToken;

        var user = await userManager.FindByEmailAsync(email);
        user.ThrowIfNull(NotFound, UserEmailNotFound, email);

        user.EmailConfirmed.ThrowIfTrue(UnprocessableEntity, UserEmailAlreadyConfirmed, user.Email);

        var emailConfirmationResult = await userManager.ConfirmEmailAsync(user, emailConfirmationToken);

        emailConfirmationResult.ThrowIfUnsuccessfulIdentityResult();

        await AssignDefaultUserRolesAsync(user, cancellationToken);

        var updateResult = await userManager.UpdateAsync(user);

        updateResult.ThrowIfUnsuccessfulIdentityResult();

        return new(user.ToModel(), OK);
    }

    /// <inheritdoc/>
    public async Task<Result<UserAuthorizationTokenModel>> LoginAsync(UserLoginModel model, CancellationToken cancellationToken = default)
    {
        model.Password.ThrowIfEmptyOrWhiteSpace(BadRequest, PasswordMissing);

        var user = await FindUserByLoginModelAsync(model);

        await ValidateUserLockoutAsync(user);

        await ValidateUserPasswordAsync(user, model.Password);

        await ResetUserLockoutStateAsync(user);

        user.EmailConfirmed.ThrowIfFalse(Forbidden, UserEmailNotConfirmed, user.Email);

        if (user.TwoFactorEnabled)
        {
            var tokenModel = GenerateTwoFactorToken(user, out var verificationCode);

            var emailResult = await SendTwoFactorAuthenticationEmailAsync(user, verificationCode, cancellationToken);
            if (!emailResult.IsSuccess)
            {
                logger.LogResultErrors(emailResult);
                logger.LogResultCriticalErrors(emailResult);

                return new(null, InternalServerError);
            }

            // It is important to set the status code different
            // from the status code on the default login process
            // to be able to determine whether the two-factor authentication
            // is being used during the current login.
            // The 'HttpStatusCode.Accepted' looks preferable here.
            return new(tokenModel, Accepted);
        }
        else
        {
            // 'refreshTokenId: null' means that we are generating
            // access token which would be bound to a newly created refresh token.
            var accessTokenResult = await GenerateAccessTokenAsync(user, refreshTokenId: null, cancellationToken);

            return accessTokenResult;
        }
    }

    /// <inheritdoc/>
    public async Task<Result<UserAuthorizationTokenModel>> ConfirmLoginAsync(UserTwoFactorAuthenticationModel model, CancellationToken cancellationToken = default)
    {
        dataProtectionService
            .TryUnprotectJson<IdentityToken>(model.IdentityToken, out var identityTokenModel)
            .ThrowIfFalse(BadRequest, InvalidToken);

        identityTokenModel.IsOutdated().ThrowIfTrue(UnprocessableEntity, OutdatedToken);

        var verificationCode = identityTokenModel.InnerToken;
        var twoFactorCode = model.TwoFactorCode;
        var email = identityTokenModel.Email;

        twoFactorCode.ThrowIfNullOrWhiteSpace(BadRequest, InvalidModel);

        var codeIsCorrect = verificationCode.Trim() == twoFactorCode.Trim();

        codeIsCorrect.ThrowIfFalse(Unauthorized, UnauthorizedUser);

        var user = await userManager.FindByEmailAsync(email);
        user.ThrowIfNull(NotFound, UserEmailNotFound, email);

        // 'refreshTokenId: null' means that we are generating
        // access token which would be bound to a newly created refresh token.
        var accessTokenResult = await GenerateAccessTokenAsync(user, refreshTokenId: null, cancellationToken);

        return accessTokenResult;
    }

    /// <inheritdoc/>
    public async Task<Result<UserAuthorizationTokenModel>> RenewTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        jsonWebTokenService
            .TryParseToken(accessToken, out var securityToken, out var principal, false)
            .ThrowIfFalse(BadRequest, InvalidToken);

        Guid.TryParse(securityToken.Id, out var refreshTokenId)
            .ThrowIfFalse(BadRequest, InvalidToken);

        var userId = principal.GetGuidClaim(_identityOptions.ClaimsIdentity.UserIdClaimType);

        var user = await GetUserByIdAsync(userId, cancellationToken);
        var accessTokenResult = await GenerateAccessTokenAsync(user, refreshTokenId, cancellationToken);

        return accessTokenResult;
    }

    /// <inheritdoc/>
    public async Task<Result> LogoutAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        jsonWebTokenService
            .TryParseToken(accessToken, out var securityToken, out _)
            .ThrowIfFalse(BadRequest, InvalidToken);

        Guid.TryParse(securityToken.Id, out var refreshTokenId)
            .ThrowIfFalse(BadRequest, InvalidToken);

        userRefreshTokensRepository.RemoveById(refreshTokenId);

        await userRefreshTokensRepository.CommitAsync(cancellationToken);

        return OK;
    }

    /// <inheritdoc/>
    public async Task<Result> LogoutEverywhereAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        jsonWebTokenService
            .TryParseToken(accessToken, out _, out var principal)
            .ThrowIfFalse(BadRequest, InvalidToken);

        var userId = principal.GetGuidClaim(_identityOptions.ClaimsIdentity.UserIdClaimType);

        userRefreshTokensRepository.RemoveWhere(refreshToken => refreshToken.OwnerId == userId);

        await userRefreshTokensRepository.CommitAsync(cancellationToken);

        return OK;
    }

    /// <inheritdoc/>
    public async Task<Result> CreatePasswordResetRequestAsync(UserResetPasswordRequestModel model, CancellationToken cancellationToken = default)
    {
        model.Email.ThrowIfEmptyOrWhiteSpace(BadRequest, InvalidEmail, model.Email);

        model.Email.IsValidEmailAddress().ThrowIfFalse(BadRequest, InvalidEmail, model.Email);

        var user = await userManager.FindByEmailAsync(model.Email);

        if (user is not null)
        {
            var emailResult = await SendPasswordResetEmailAsync(user, cancellationToken);
            if (!emailResult.IsSuccess)
            {
                logger.LogResultErrors(emailResult);
                logger.LogResultCriticalErrors(emailResult);

                return InternalServerError;
            }
        }

        return OK;
    }

    /// <inheritdoc/>
    public async Task<Result> ResetPasswordAsync(UserResetPasswordModel model, CancellationToken cancellationToken = default)
    {
        dataProtectionService
            .TryUnprotectJson<IdentityToken>(model.IdentityToken, out var identityTokenModel)
            .ThrowIfFalse(BadRequest, InvalidToken);

        identityTokenModel.IsOutdated().ThrowIfTrue(UnprocessableEntity, OutdatedToken);

        model.Password.ThrowIfEmptyOrWhiteSpace(BadRequest, InvalidModel);

        var exception = new ResultException();

        await ValidatePasswordAsync(model.Password, model.PasswordConfirmation, exception);

        if (exception.HaveErrors)
            throw exception;

        var email = identityTokenModel.Email;
        var passwordResetToken = identityTokenModel.InnerToken;

        var user = await userManager.FindByEmailAsync(email);
        user.ThrowIfNull(NotFound, UserEmailNotFound, email);

        var passwordResetResult = await userManager.ResetPasswordAsync(user, passwordResetToken, model.Password);

        passwordResetResult.ThrowIfUnsuccessfulIdentityResult();

        try
        {
            await SendPasswordResetCompletedEmailAsync(user, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogUnhandledException(ex);
        }
        catch (ResultException ex)
        {
            logger.LogResultException(ex);
        }

        return OK;
    }

    /// <inheritdoc/>
    public async Task<Result> CreateEmailResetRequestAsync(Guid userId, UserResetEmailRequestModel model, CancellationToken cancellationToken = default)
    {
        model.Email.ThrowIfEmptyOrWhiteSpace(BadRequest, InvalidEmail, model.Email);

        model.Email.IsValidEmailAddress().ThrowIfFalse(BadRequest, InvalidEmail, model.Email);

        var emailAddressIsInUse = await CheckIfEmailAddressIsInUseAsync(model.Email);

        emailAddressIsInUse.ThrowIfTrue(BadRequest, DuplicateEmail, model.Email);

        var user = await GetUserByIdAsync(userId, cancellationToken);

        try
        {
            await SendEmailAddressResetNotificationEmailAsync(user, model.Email, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogUnhandledException(ex);
        }
        catch (ResultException ex)
        {
            logger.LogResultException(ex);
        }

        var emailResult = await SendEmailAddressResetEmailAsync(user, model.Email, cancellationToken);
        if (!emailResult.IsSuccess)
        {
            logger.LogResultErrors(emailResult);
            logger.LogResultCriticalErrors(emailResult);

            return InternalServerError;
        }

        return OK;
    }

    /// <inheritdoc/>
    public async Task<Result> ResetEmailAsync(string identityToken, CancellationToken cancellationToken = default)
    {
        dataProtectionService
            .TryUnprotectJson<IdentityToken>(identityToken, out var identityTokenModel)
            .ThrowIfFalse(BadRequest, InvalidToken);

        identityTokenModel.IsOutdated().ThrowIfTrue(UnprocessableEntity, OutdatedToken);

        var email = identityTokenModel.Email;
        var newEmail = identityTokenModel.Value;
        var changeEmailToken = identityTokenModel.InnerToken;

        newEmail.ThrowIfNullOrWhiteSpace(BadRequest, InvalidToken, newEmail);

        var emailAddressIsInUse = await CheckIfEmailAddressIsInUseAsync(newEmail);

        emailAddressIsInUse.ThrowIfTrue(BadRequest, DuplicateEmail, newEmail);

        var user = await userManager.FindByEmailAsync(email);
        user.ThrowIfNull(NotFound, UserEmailNotFound, email);

        var changeEmailResult = await userManager.ChangeEmailAsync(user, newEmail, changeEmailToken);

        changeEmailResult.ThrowIfUnsuccessfulIdentityResult();

        try
        {
            await SendEmailAddressResetCompletedEmailAsync(user.UserName, email, newEmail, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogUnhandledException(ex);
        }
        catch (ResultException ex)
        {
            logger.LogResultException(ex);
        }

        return OK;
    }

    /// <inheritdoc/>
    public async Task<Result<UserModel>> UpdateAsync(Guid userId, UserUpdateModel model, CancellationToken cancellationToken = default)
    {
        var user = await GetUserByIdAsync(userId, cancellationToken);

        if (model.IsPendingDeletion.HasValue)
        {
            user.DeletionRequestSubmitted = model.IsPendingDeletion.Value
                ? DateTime.UtcNow
                : null;
        }

        if (model.TwoFactorEnabled.HasValue)
            user.TwoFactorEnabled = model.TwoFactorEnabled.Value;

        if (model.UserName is not null)
        {
            var userNameIsValid = model.UserName.IsValidUserName(_identityOptions);

            userNameIsValid.ThrowIfFalse(UnprocessableEntity, InvalidUserName, model.UserName);

            var userNameInUse = await CheckIfUserNameIsInUseAsync(model.UserName);

            userNameInUse.ThrowIfTrue(UnprocessableEntity, DuplicateUserName, model.UserName);

            user.UserName = model.UserName;
        }

        var updateResult = await userManager.UpdateAsync(user);

        updateResult.ThrowIfUnsuccessfulIdentityResult();

        return new(user.ToModel(), OK);
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await GetUserByIdAsync(userId, cancellationToken);
        var requestLifetime = _applicationOptions.Tokens.UserDeletionRequestLifetime;

        user.DeletionRequestSubmitted.HasValue.ThrowIfFalse(BadRequest, UserNotPendingDeletion, user.Email);

        if (user.IsDeletionRequestOutdated(requestLifetime))
        {
            user.CancelDeletionRequest();

            var updateResult = await userManager.UpdateAsync(user);

            updateResult.ThrowIfUnsuccessfulIdentityResult();

            Throw(BadRequest, UserDeletionRequestExpired, requestLifetime);
        }

        var deletionResult = await userManager.DeleteAsync(user);

        deletionResult.ThrowIfUnsuccessfulIdentityResult();

        return OK;
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Generates the access token for the given <paramref name="user"/>.
    /// </summary>
    /// <param name="user">
    /// The <see cref="User"/> for whom to generate an access token.
    /// </param>
    /// <param name="refreshTokenId">
    /// The Id of the refresh token to be used to
    /// bound with the newly generated JWT.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="UserAuthorizationTokenModel"/> instance containing the JWT.
    /// </returns>
    private async Task<Result<UserAuthorizationTokenModel>> GenerateAccessTokenAsync(User user, Guid? refreshTokenId = null,
                                                                                     CancellationToken cancellationToken = default)
    {
        if (refreshTokenId.HasValue)
        {
            var refreshToken = await userRefreshTokensRepository.GetByIdAsync(refreshTokenId.Value, cancellationToken);

            refreshToken.ThrowIfNull(Unauthorized, OutdatedToken);

            refreshToken
                .IsOutdated(_applicationOptions.Authentication.RefreshTokenLifetime)
                .ThrowIfTrue(Unauthorized, OutdatedToken);
        }
        else
        {
            var refreshToken = new UserRefreshToken(user.Id);

            userRefreshTokensRepository.Add(refreshToken);

            await userRefreshTokensRepository.CommitAsync(cancellationToken);

            refreshTokenId = refreshToken.Id;
        }

        // The minimum claims for the authentication process to be working properly
        // is the user Id claim. In order to pass the authorization process as well -
        // role claims are required.
        var userClaims = await userManager.GetClaimsAsync(user);
        var rolesAsClaims = await GetUserRolesAsClaimsAsync(user);

        var tokenClaims = userClaims.Concat(rolesAsClaims);

        var accessToken = jsonWebTokenService.GenerateToken(tokenClaims, refreshTokenId.Value, out var expiryDate);

        return new(new(user.Email, expiryDate, accessToken), OK);
    }

    /// <summary>
    /// Gets the list of claims which contains
    /// the given <paramref name="user"/> roles data.
    /// </summary>
    /// <param name="user">
    /// The <see cref="User"/> whose roles to be listed as claims.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="Claim"/>, containing
    /// the given <paramref name="user"/> roles data.
    /// </returns>
    private async Task<IEnumerable<Claim>> GetUserRolesAsClaimsAsync(User user)
    {
        var roleClaimType = _jwtBearerOptions.TokenValidationParameters.RoleClaimType;

        var roleNames = await userManager.GetRolesAsync(user);
        var roleClaims = roleNames.Select(name => new Claim(roleClaimType, name));

        return roleClaims;
    }

    /// <summary>
    /// Generates the access token for the given <paramref name="user"/>.
    /// </summary>
    /// <param name="user">
    /// The <see cref="User"/> for whom to generate an access token.
    /// </param>
    /// <param name="verificationCode">
    /// Verification code to pass the current token validation.
    /// </param>
    /// <returns>
    /// A <see cref="UserAuthorizationTokenModel"/> instance containing the JWT.
    /// </returns>
    private UserAuthorizationTokenModel GenerateTwoFactorToken(User user, out string verificationCode)
    {
        verificationCode = dataProtectionService.GenerateRandomDigitCode(
            _applicationOptions.Authentication.TwoFactorVerificationCodeLength);

        var expiryDate = DateTime.UtcNow.Add(_applicationOptions.Authentication.TwoFactorTokenLifetime);
        var identityToken = dataProtectionService.ProtectAsJson(
            new IdentityToken(user.Email, verificationCode, expiryDate: expiryDate));

        return new(user.Email, expiryDate, identityToken);
    }

    /// <summary>
    /// Assigns the default application roles
    /// to the given <paramref name="user"/>.
    /// </summary>
    /// <param name="user">
    /// The <see cref="User"/> to whom roles to be assigned.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    private async Task AssignDefaultUserRolesAsync(User user, CancellationToken cancellationToken = default)
    {
        var defaultRolesResult = await roleService.GetAllAsync(true, cancellationToken);

        if (defaultRolesResult.Value is not null)
        {
            foreach (var role in defaultRolesResult.Value)
                await roleService.AssignAsync(role.Id, user.Id, cancellationToken);
        }
    }

    /// <summary>
    /// Resets the user's lockout state.
    /// </summary>
    /// <param name="user">
    /// The <see cref="User"/> which lockout state to be reset.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    private async Task ResetUserLockoutStateAsync(User user)
    {
        if (user.AccessFailedCount is not 0 || user.LockoutEnd is not null)
        {
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;

            var updateResult = await userManager.UpdateAsync(user);
            updateResult.ThrowIfUnsuccessfulIdentityResult();
        }
    }

    /// <summary>
    /// Creates a link with an identity token.
    /// </summary>
    /// <param name="baseUrl">
    /// Base <see cref="Uri"/>.
    /// </param>
    /// <param name="path">
    /// API action path.
    /// </param>
    /// <param name="user">
    /// Token owner.
    /// </param>
    /// <param name="innerToken">
    /// Token.
    /// </param>
    /// <param name="expiryDate">
    /// Token expiry date.
    /// </param>
    /// <param name="value">
    /// Target property's new value.
    /// </param>
    /// <returns>
    /// <see cref="Uri"/>, which leads to the specified API action
    /// and contains an identity token.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="baseUrl"/> is <see langword="null"/>
    /// </exception>
    private Uri CreateIdentityTokenLink(Uri? baseUrl, string path, User user, string innerToken,
                                        DateTime? expiryDate = null, string? value = null)
    {
        ArgumentNullException.ThrowIfNull(baseUrl);

        var identityToken = dataProtectionService.ProtectAsJson(
            new IdentityToken(user.Email, innerToken, value, expiryDate));

        // Since the scope of the current method is very concrete,
        // we can assume that the 'path' parameter has the '{identityToken}' placeholder string inside.
        // Here we are replacing it with the actual identity token.
        var placeholder = $"{{{IdentityTokenParameter}}}";
        var replacement = HttpUtility.UrlEncode(identityToken);

        path = path.Replace(placeholder, replacement, StringComparison.OrdinalIgnoreCase);

        return new($"{baseUrl}{path}");
    }

    /// <summary>
    /// Creates a "required at least one" error message.
    /// </summary>
    /// <param name="propertyNames">
    /// Properties to be included in the message.
    /// </param>
    /// <returns>
    /// A <see cref="string"/> value containing the error message.
    /// </returns>
    private static string CreateRequiredAtLeastOneErrorMessage(params string[] propertyNames)
    {
        const string Separator = ", ";

        var message = ValidationMessages.RequiredAtLeastOne;

        var properties = string.Join(Separator, propertyNames);

        return string.Format(CultureInfo.CurrentCulture, message, properties);
    }

    #region Lookup methods
    /// <summary>
    /// Gets the <see cref="User"/> with the data from the given <paramref name="model"/>.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserLoginModel"/>
    /// which data to be used to find a <see cref="User"/>.
    /// </param>
    /// <returns>
    /// The <see cref="User"/> with the data from the given <paramref name="model"/>.
    /// </returns>
    private async Task<User> FindUserByLoginModelAsync(UserLoginModel model)
    {
        User? user = null;

        if (model.Email.IsNotNullOrWhiteSpace())
        {
            model.Email.IsValidEmailAddress().ThrowIfFalse(BadRequest, InvalidEmail, model.Email);

            user = await userManager.FindByEmailAsync(model.Email);
        }
        else if (model.UserName.IsNotNullOrWhiteSpace())
        {
            model.UserName.IsValidUserName(_identityOptions).ThrowIfFalse(BadRequest, InvalidUserName, model.UserName);

            user = await userManager.FindByNameAsync(model.UserName);
        }
        else if (model.Phone.IsNotNullOrWhiteSpace())
        {
            model.Phone.IsValidPhoneNumber().ThrowIfFalse(BadRequest, InvalidPhoneNumber, model.Phone);

            user = await userManager.FindByPhoneNumberAsync(model.Phone);
        }
        else
        {
            var propertyNames = new[]
            {
                nameof(UserLoginModel.UserName),
                nameof(UserLoginModel.Email),
                nameof(UserLoginModel.Phone)
            };

            var error = CreateRequiredAtLeastOneErrorMessage(propertyNames);

            Throw(BadRequest, InvalidModel, error);
        }

        user.ThrowIfNull(Unauthorized, UserNotFoundOrPasswordMismatch);

        return user;
    }

    /// <summary>
    /// Gets the <see cref="User"/> with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    /// The Id of the <see cref="User"/> to be found.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="User"/> with the given <paramref name="id"/>.
    /// </returns>
    private async Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await userManager
            .Users
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

        return user ?? throw new ResultException(NotFound, UserIdNotFound, id);
    }
    #endregion

    #region Validation methods
    /// <summary>
    /// Validates the given <paramref name="model"/> in order to
    /// determine whether the registration can proceed.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserRegistrationModel"/>
    /// which data to be used to register a new user.
    /// </param>
    private async Task ValidateRegistrationModelAsync(UserRegistrationModel model)
    {
        var exception = new ResultException();

        await ValidateEmailAddressAsync(model.Email, exception);

        await ValidatePasswordAsync(model.Password, model.PasswordConfirmation, exception);

        if (model.Phone is not null)
            await ValidatePhoneNumberAsync(model.Phone, exception);

        await ValidateUserNameAsync(model.UserName, exception);

        if (exception.HaveErrors)
            throw exception;
    }

    /// <summary>
    /// Checks if the given <paramref name="email"/> meets the requirements.
    /// </summary>
    /// <param name="email">
    /// The email address to be checked.
    /// </param>
    /// <param name="exception">
    /// The <see cref="ResultException"/> into which the errors to be pushed.
    /// </param>
    private async Task ValidateEmailAddressAsync(string email, ResultException exception)
    {
        if (!email.IsValidEmailAddress())
        {
            exception.AddError(BadRequest, InvalidEmail, email);
        }
        else
        {
            var emailTaken = await CheckIfEmailAddressIsInUseAsync(email);

            if (emailTaken)
                exception.AddError(UnprocessableEntity, DuplicateEmail, email);
        }
    }

    /// <summary>
    /// Checks if the given <paramref name="password"/> meets the requirements.
    /// </summary>
    /// <param name="password">
    /// The password to be checked.
    /// </param>
    /// <param name="exception">
    /// The <see cref="ResultException"/> into which the errors to be pushed.
    /// </param>
    /// <param name="passwordConfirmation">
    /// Password confirmation value.
    /// </param>
    private Task ValidatePasswordAsync(string password, string passwordConfirmation, ResultException exception)
    {
        if (password != passwordConfirmation)
            exception.AddError(BadRequest, PasswordNotMatchConfirmation);

        return ValidatePasswordAsync(password, exception);
    }

    /// <summary>
    /// Checks if the given <paramref name="password"/> meets the requirements.
    /// </summary>
    /// <param name="password">
    /// The password to be checked.
    /// </param>
    /// <param name="exception">
    /// The <see cref="ResultException"/> into which the errors to be pushed.
    /// </param>
    private async Task ValidatePasswordAsync(string password, ResultException exception)
    {
        foreach (var validator in userManager.PasswordValidators)
        {
            var validationResult = await validator.ValidateAsync(userManager, null!, password);
            if (!validationResult.Succeeded)
                exception.AddError(validationResult, UnprocessableEntity);
        }
    }

    /// <summary>
    /// Checks if the given <paramref name="phone"/> meets the requirements.
    /// </summary>
    /// <param name="phone">
    /// The phone number to be checked.
    /// </param>
    /// <param name="exception">
    /// The <see cref="ResultException"/> into which the errors to be pushed.
    /// </param>
    private async Task ValidatePhoneNumberAsync(string phone, ResultException exception)
    {
        if (!phone.IsValidPhoneNumber())
        {
            exception.AddError(BadRequest, InvalidPhoneNumber, phone);
        }
        else
        {
            var phoneTaken = await CheckIfPhoneNumberIsInUseAsync(phone);

            if (phoneTaken)
                exception.AddError(UnprocessableEntity, DuplicatePhoneNumber, phone);
        }
    }

    /// <summary>
    /// Checks if the given <paramref name="userName"/> meets the requirements.
    /// </summary>
    /// <param name="userName">
    /// The user-name to be checked.
    /// </param>
    /// <param name="exception">
    /// The <see cref="ResultException"/> into which the errors to be pushed.
    /// </param>
    private async Task ValidateUserNameAsync(string userName, ResultException exception)
    {
        if (!userName.IsValidUserName(_identityOptions))
        {
            exception.AddError(BadRequest, InvalidUserName, userName);
        }
        else
        {
            var userNameTaken = await CheckIfUserNameIsInUseAsync(userName);

            if (userNameTaken)
                exception.AddError(UnprocessableEntity, DuplicateUserName, userName);
        }
    }

    /// <summary>
    /// Validates the <paramref name="user"/>'s lockout state.
    /// </summary>
    /// <remarks>
    /// If the <paramref name="user"/> is locked out - exception is thrown.
    /// </remarks>
    /// <param name="user">
    /// The <see cref="User"/> which lockout state to be validated.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    /// <exception cref="ResultException">
    /// Thrown if the <paramref name="user"/> is locked out.
    /// </exception>
    private async Task ValidateUserLockoutAsync(User user)
    {
        var userLockedOut = await userManager.IsLockedOutAsync(user);

        (user.LockoutEnabled && userLockedOut).ThrowIfTrue(Forbidden, UserLockedOut);
    }

    /// <summary>
    /// Validates the <paramref name="user"/>'s password.
    /// </summary>
    /// <remarks>
    /// If the <paramref name="password"/> does not match
    /// the <paramref name="user"/>'s one - exception is thrown.
    /// </remarks>
    /// <param name="user">
    /// The <see cref="User"/> which password to be validated.
    /// </param>
    /// <param name="password">
    /// The password to be compared to the <paramref name="user"/>'s one.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    /// <exception cref="ResultException">
    /// Thrown if the <paramref name="password"/> is incorrect.
    /// </exception>
    private async Task ValidateUserPasswordAsync(User user, string password)
    {
        var passwordIsCorrect = await userManager.CheckPasswordAsync(user, password);

        if (!passwordIsCorrect)
        {
            if (user.LockoutEnabled)
            {
                var incrementResult = await userManager.AccessFailedAsync(user);
                incrementResult.ThrowIfUnsuccessfulIdentityResult();
            }

            Throw(Unauthorized, UserNotFoundOrPasswordMismatch);
        }
    }

    /// <summary>
    /// Checks if the given <paramref name="email"/> address is already in use.
    /// </summary>
    /// <param name="email">
    /// The email address to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="email"/> address is already in use,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    private async Task<bool> CheckIfEmailAddressIsInUseAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);

        return user is not null;
    }

    /// <summary>
    /// Checks if the given <paramref name="phone"/> number is already in use.
    /// </summary>
    /// <param name="phone">
    /// The phone number to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="phone"/> number is already in use,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    private async Task<bool> CheckIfPhoneNumberIsInUseAsync(string phone)
    {
        var user = await userManager.FindByPhoneNumberAsync(phone);

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
        var user = await userManager.FindByNameAsync(userName);

        return user is not null;
    }
    #endregion

    #region Notification methods
    /// <summary>
    /// Sends an email with a link to confirm the email address.
    /// </summary>
    /// <param name="user">
    /// The <see cref="User"/> whose email address to be confirmed.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where the
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailModel"/>
    /// containing information about the message sent.
    /// </returns>
    private async Task<Result<EmailModel>> SendEmailAddressConfirmationEmailAsync(User user, CancellationToken cancellationToken = default)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var url = _applicationOptions.ApiUrl;
        var route = UserRoutes.ConfirmEmail;
        var expiryDate = DateTime.UtcNow.Add(_applicationOptions.Tokens.EmailConfirmationTokenLifetime);

        var link = CreateIdentityTokenLink(url, route, user, token, expiryDate);

        var template = _emailTemplateOptions.EmailAddressConfirmationTemplateName;

        var culture = Thread.CurrentThread.CurrentUICulture;

        var request = new EmailSendRequestModel(
            basicData: new([user.Email]),
            templateName: template,
            culture: culture,
            bodyArgs: [link]);

        var emailResult = await communicationService.SendEmailAsync(request, cancellationToken);

        return emailResult;
    }

    /// <summary>
    /// Sends the two-factor authentication message
    /// to the given <paramref name="user"/>.
    /// </summary>
    /// <param name="user">
    /// The <see cref="User"/> to be authenticated via two-factor method.
    /// </param>
    /// <param name="verificationCode">
    /// The two-factor authentication code to be sent.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where the
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailModel"/>
    /// containing information about the message sent.
    /// </returns>
    private Task<Result<EmailModel>> SendTwoFactorAuthenticationEmailAsync(User user, string verificationCode, CancellationToken cancellationToken = default)
    {
        var template = _emailTemplateOptions.TwoFactorVerificationTemplateName;

        var culture = Thread.CurrentThread.CurrentUICulture;

        var request = new EmailSendRequestModel(
            basicData: new([user.Email]),
            templateName: template,
            culture: culture,
            bodyArgs: [verificationCode]);

        return communicationService.SendEmailAsync(request, cancellationToken);
    }

    /// <summary>
    /// Sends an email with a link to reset the password.
    /// </summary>
    /// <param name="user">
    /// The <see cref="User"/> whose password to reset.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where the
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailModel"/>
    /// containing information about the message sent.
    /// </returns>
    private async Task<Result<EmailModel>> SendPasswordResetEmailAsync(User user, CancellationToken cancellationToken = default)
    {
        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var tokenLifetime = DateTime.UtcNow.Add(_applicationOptions.Tokens.ResetPasswordTokenLifetime);

        var identityToken = dataProtectionService.ProtectAsJson(
            new IdentityToken(user.Email, token, expiryDate: tokenLifetime));

        var template = _emailTemplateOptions.PasswordResetTemplateName;

        var culture = Thread.CurrentThread.CurrentUICulture;

        var request = new EmailSendRequestModel(
            basicData: new([user.Email]),
            templateName: template,
            culture: culture,
            bodyArgs: [identityToken]);

        var emailResult = await communicationService.SendEmailAsync(request, cancellationToken);

        return emailResult;
    }

    /// <summary>
    /// Sends an email notification about the completed password reset.
    /// </summary>
    /// <param name="user">
    /// The recipient <see cref="User"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where the
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailModel"/>
    /// containing information about the message sent.
    /// </returns>
    private Task<Result<EmailModel>> SendPasswordResetCompletedEmailAsync(User user, CancellationToken cancellationToken = default)
    {
        user.Email.ThrowIfNullOrWhiteSpace(BadRequest, InvalidEmail, user.Email);

        var template = _emailTemplateOptions.PasswordResetCompletedTemplateName;
        if (template.IsNullOrWhiteSpace())
            throw new InvalidOperationException(nameof(EmailTemplateOptions.PasswordResetCompletedTemplateName));

        var culture = Thread.CurrentThread.CurrentUICulture;

        var request = new EmailSendRequestModel(
            basicData: new([user.Email]),
            templateName: template,
            culture: culture);

        return communicationService.SendEmailAsync(request, cancellationToken);
    }

    /// <summary>
    /// Sends an email notification that the email address is going to be changed.
    /// </summary>
    /// <param name="user">
    /// The <see cref="User"/> whose email address to be changed.
    /// </param>
    /// <param name="newEmail">
    /// New email address.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where the
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailModel"/>
    /// containing information about the message sent.
    /// </returns>
    private Task<Result<EmailModel>> SendEmailAddressResetNotificationEmailAsync(User user, string newEmail, CancellationToken cancellationToken = default)
    {
        var template = _emailTemplateOptions.EmailAddressResetNotificationTemplateName;

        var culture = Thread.CurrentThread.CurrentUICulture;

        var request = new EmailSendRequestModel(
            basicData: new([user.Email]),
            templateName: template,
            culture: culture,
            bodyArgs: [user.UserName, newEmail]);

        return communicationService.SendEmailAsync(request, cancellationToken);
    }

    /// <summary>
    /// Sends an email with a link to reset the email address.
    /// </summary>
    /// <param name="user">
    /// The <see cref="User"/> whose email address to reset.
    /// </param>
    /// <param name="newEmail">
    /// New email address.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where the
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailModel"/>
    /// containing information about the message sent.
    /// </returns>
    private async Task<Result<EmailModel>> SendEmailAddressResetEmailAsync(User user, string newEmail, CancellationToken cancellationToken = default)
    {
        var token = await userManager.GenerateChangeEmailTokenAsync(user, newEmail);

        var url = _applicationOptions.ApiUrl;
        var route = UserRoutes.ResetEmail;
        var expiryDate = DateTime.UtcNow.Add(_applicationOptions.Tokens.ResetEmailAddressTokenLifetime);

        var link = CreateIdentityTokenLink(url, route, user, token, expiryDate, newEmail);

        var template = _emailTemplateOptions.EmailAddressResetTemplateName;
        if (template.IsNullOrWhiteSpace())
            throw new InvalidOperationException(nameof(EmailTemplateOptions.EmailAddressResetTemplateName));

        var culture = Thread.CurrentThread.CurrentUICulture;

        var request = new EmailSendRequestModel(
            basicData: new([newEmail]),
            templateName: template,
            culture: culture,
            bodyArgs: [user.UserName, link]);

        var emailResult = await communicationService.SendEmailAsync(request, cancellationToken);

        return emailResult;
    }

    /// <summary>
    /// Sends an email notification that the email address reset has been completed.
    /// </summary>
    /// <param name="userName">
    /// Recipient's user-name.
    /// </param>
    /// <param name="oldEmail">
    /// Old email address.
    /// </param>
    /// <param name="newEmail">
    /// New email address.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where the
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailModel"/>
    /// containing information about the message sent.
    /// </returns>
    private Task<Result<EmailModel>> SendEmailAddressResetCompletedEmailAsync(string? userName, string oldEmail, string newEmail, CancellationToken cancellationToken = default)
    {
        var template = _emailTemplateOptions.EmailAddressResetCompletedTemplateName;

        var culture = Thread.CurrentThread.CurrentUICulture;

        var request = new EmailSendRequestModel(
            basicData: new([oldEmail, newEmail]),
            templateName: template,
            culture: culture,
            bodyArgs: [userName, newEmail]);

        return communicationService.SendEmailAsync(request, cancellationToken);
    }
    #endregion

    #endregion
}