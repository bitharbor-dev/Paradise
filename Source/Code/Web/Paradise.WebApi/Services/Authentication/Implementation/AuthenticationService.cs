using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Paradise.ApplicationLogic.Infrastructure.DataProtection;
using Paradise.ApplicationLogic.Services.Identity.Roles;
using Paradise.ApplicationLogic.Services.Identity.Users;
using Paradise.Common.Extensions;
using Paradise.Domain.Base.Events;
using Paradise.Domain.Events.Identity.Users;
using Paradise.Localization.DataValidation;
using Paradise.Models;
using Paradise.Models.Domain.Identity.Users;
using Paradise.Models.WebApi.Services.Authentication;
using Paradise.WebApi.Extensions;
using Paradise.WebApi.Infrastructure.Authentication.Caching;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer;
using Paradise.WebApi.Infrastructure.Options;
using System.Globalization;
using System.Security.Claims;
using static Paradise.Models.ErrorCode;
using static Paradise.Models.OperationStatus;

namespace Paradise.WebApi.Services.Authentication.Implementation;

/// <summary>
/// Provides authentication functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthenticationService"/> class.
/// </remarks>
/// <param name="authenticationOptions">
/// The accessor used to access the <see cref="AuthenticationOptions"/>.
/// </param>
/// <param name="identityOptions">
/// The accessor used to access the <see cref="IdentityOptions"/>.
/// </param>
/// <param name="timeProvider">
/// Time provider.
/// </param>
/// <param name="domainEventSink">
/// Domain event sink.
/// </param>
/// <param name="dataProtector">
/// Data protector.
/// </param>
/// <param name="jwtManager">
/// JSON web token manager.
/// </param>
/// <param name="refreshTokenCache">
/// Refresh token cache.
/// </param>
/// <param name="roleService">
/// Role service.
/// </param>
/// <param name="userService">
/// User service.
/// </param>
/// <param name="userRefreshTokenService">
/// User refresh token service.
/// </param>
internal sealed class AuthenticationService(IOptions<AuthenticationOptions> authenticationOptions,
                                            IOptions<IdentityOptions> identityOptions,
                                            TimeProvider timeProvider,
                                            IDomainEventSink domainEventSink,
                                            IDataProtector dataProtector,
                                            IJwtManager jwtManager,
                                            IRefreshTokenCache refreshTokenCache,
                                            IRoleService roleService,
                                            IUserService userService,
                                            IUserRefreshTokenService userRefreshTokenService) : IAuthenticationService
{
    #region Public methods
    /// <inheritdoc/>
    public async Task<Result<AccessTokenModel>> LoginAsync(LoginModel model, CancellationToken cancellationToken = default)
    {
        if (model is null)
            return new(InvalidInput, InvalidModel, nameof(model));

        if (model.Password.IsNullOrWhiteSpace())
            return new(InvalidInput, PasswordMissing);

        var lookupResult = await GetUserByLoginModelAsync(model, cancellationToken)
            .ConfigureAwait(false);

        if (lookupResult.Status is Missing)
            return new(Unauthorized, UserNotFoundOrPasswordMismatch);

        if (!lookupResult.IsSuccess || lookupResult.Value is null)
            return new(lookupResult.Status, lookupResult.Errors, null);

        var user = lookupResult.Value;

        var passwordValidationResult = await userService.CheckPasswordAsync(user.Id, model.Password, cancellationToken)
            .ConfigureAwait(false);

        if (!passwordValidationResult.IsSuccess)
            return new(Unauthorized, UserNotFoundOrPasswordMismatch);

        var isConfirmedEmailAddressRequired = identityOptions.Value.SignIn.RequireConfirmedEmail;
        var isEmailAddressNotConfirmed = !user.IsEmailAddressConfirmed;

        if (isConfirmedEmailAddressRequired && isEmailAddressNotConfirmed)
            return new(Prohibited, UserEmailAddressNotConfirmed, user.EmailAddress);

        if (user.IsTwoFactorEnabled)
        {
            var accessToken = GenerateTwoFactorToken(user, out var verificationCode);

            var domainEvent = new TwoFactorAuthenticationOccurringEvent(timeProvider.GetUtcNow(),
                                                                        user.EmailAddress,
                                                                        verificationCode,
                                                                        CultureInfo.CurrentCulture);

            await domainEventSink.PushAsync(domainEvent, cancellationToken)
                .ConfigureAwait(false);

            // It is important to set the status code different
            // from the status code on the default login process
            // to be able to determine whether the two-factor authentication
            // is being used during the current login.
            // The 'OperationStatus.Received' looks preferable here.
            return new(accessToken, Received);
        }
        else
        {
            // 'refreshTokenId: null' means that the new refresh token will be created
            // to be bound with the resulting access token.
            return await GenerateAccessTokenAsync(user, refreshTokenId: null, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task<Result<AccessTokenModel>> ConfirmLoginAsync(TwoFactorAuthenticationModel model, CancellationToken cancellationToken = default)
    {
        if (!dataProtector.TryUnprotect<IdentityToken>(model.IdentityToken, out var identityTokenModel))
            return new(InvalidInput, InvalidToken);

        if (identityTokenModel.IsExpired(timeProvider.GetUtcNow()))
            return new(Blocked, OutdatedToken);

        var verificationCode = identityTokenModel.InnerToken;
        var twoFactorCode = model.TwoFactorCode;
        var emailAddress = identityTokenModel.EmailAddress;

        if (twoFactorCode.IsNullOrWhiteSpace())
            return new(InvalidInput, InvalidToken);

        var codeIsCorrect = verificationCode
            .Trim()
            .Equals(twoFactorCode.Trim(), StringComparison.OrdinalIgnoreCase);

        if (!codeIsCorrect)
            return new(Unauthorized, UserUnauthorized);

        var lookupResult = await userService.GetByEmailAddressAsync(emailAddress, cancellationToken)
            .ConfigureAwait(false);

        if (lookupResult.Value is null)
            return new(Missing, UserEmailAddressNotFound, emailAddress);

        // 'refreshTokenId: null' means that the new refresh token will be created
        // to be bound with the resulting access token.
        return await GenerateAccessTokenAsync(lookupResult.Value, refreshTokenId: null, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<Result<AccessTokenModel>> RenewTokenAsync(string oldAccessToken, CancellationToken cancellationToken = default)
    {
        if (oldAccessToken.StartsWith(JwtBearerDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
            oldAccessToken = oldAccessToken[(JwtBearerDefaults.AuthenticationScheme.Length + 1)..];

        if (!jwtManager.TryParseToken(oldAccessToken, out var principal, false))
            return new(InvalidInput, InvalidToken);

        var refreshTokenId = principal.GetGuidClaim(JwtRegisteredClaimNames.Sid);

        if (refreshTokenId == Guid.Empty)
            return new(InvalidInput, InvalidToken);

        var idClaimType = identityOptions.Value.ClaimsIdentity.UserIdClaimType;

        var userId = principal.GetGuidClaim(idClaimType);

        var lookupResult = await userService.GetByIdAsync(userId, cancellationToken)
            .ConfigureAwait(false);

        return lookupResult.Value is null
            ? new(Missing, UserIdNotFound, userId)
            : await GenerateAccessTokenAsync(lookupResult.Value, refreshTokenId, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<Result> LogoutAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        if (accessToken.StartsWith(JwtBearerDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
            accessToken = accessToken[(JwtBearerDefaults.AuthenticationScheme.Length + 1)..];

        if (!jwtManager.TryParseToken(accessToken, out var principal, false))
            return new(InvalidInput, InvalidToken);

        var refreshTokenId = principal.GetGuidClaim(JwtRegisteredClaimNames.Sid);

        if (refreshTokenId == Guid.Empty)
            return new(InvalidInput, InvalidToken);

        var idClaimType = identityOptions.Value.ClaimsIdentity.UserIdClaimType;

        var userId = principal.GetGuidClaim(idClaimType);

        await userRefreshTokenService.DeactivateAsync(userId, refreshTokenId, cancellationToken)
            .ConfigureAwait(false);

        await refreshTokenCache.RevokeAsync(refreshTokenId, cancellationToken)
            .ConfigureAwait(false);

        return new(Success);
    }

    /// <inheritdoc/>
    public async Task<Result> TerminateSessionsAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        if (accessToken.StartsWith(JwtBearerDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
            accessToken = accessToken[(JwtBearerDefaults.AuthenticationScheme.Length + 1)..];

        if (!jwtManager.TryParseToken(accessToken, out var principal))
            return new(InvalidInput, InvalidToken);

        var idClaimType = identityOptions.Value.ClaimsIdentity.UserIdClaimType;

        var userId = principal.GetGuidClaim(idClaimType);

        var deactivatedTokensResult = await userRefreshTokenService.DeactivateAllAsync(userId, cancellationToken)
            .ConfigureAwait(false);

        if (deactivatedTokensResult.Value is not null)
        {
            var tasks = deactivatedTokensResult
                .Value
                .Select(token => refreshTokenCache.RevokeAsync(token.Id, cancellationToken));

            await Task.WhenAll(tasks)
                .ConfigureAwait(false);
        }

        return new(Success);
    }

    /// <inheritdoc/>
    public async Task<Result> CheckSessionAsync(ClaimsPrincipal? principal, CancellationToken cancellationToken = default)
    {
        if (principal is null)
            return new(Unauthorized, InvalidToken);

        var refreshTokenId = principal.GetGuidClaim(JwtRegisteredClaimNames.Sid);

        return refreshTokenId == Guid.Empty
            ? new(Unauthorized, InvalidToken)
            : await ValidateRefreshTokenInternalAsync(refreshTokenId, cancellationToken).ConfigureAwait(false);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Retrieves a user based on the login data provided in the <paramref name="model"/>.
    /// </summary>
    /// <remarks>
    /// The lookup is performed in the following order:
    /// <list type="number">
    /// <item>
    /// <description>
    /// <see cref="LoginModel.EmailAddress"/>.
    /// If present and valid, the user is retrieved by email address.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <see cref="LoginModel.PhoneNumber"/>.
    /// If present and valid, the user is retrieved by phone number.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <see cref="LoginModel.UserName"/>.
    /// If present and valid, the user is retrieved by user-name.
    /// </description>
    /// </item>
    /// </list>
    /// If the provided login data is invalid or none of the fields are supplied,
    /// the method returns a failed <see cref="Result{T}"/> containing validation errors.
    /// </remarks>
    /// <param name="model">
    /// The login information containing an email address, phone number, or user-name.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="Result{T}"/> with the resolved
    /// <see cref="UserModel"/> on success, or validation errors on failure.
    /// </returns>
    private Task<Result<UserModel>> GetUserByLoginModelAsync(LoginModel model, CancellationToken cancellationToken)
    {
        var emailAddress = model.EmailAddress;
        var phoneNumber = model.PhoneNumber;
        var userName = model.UserName;
        var allowedUserNameCharacters = identityOptions.Value.User.AllowedUserNameCharacters;

        var validationResult = new Result<UserModel>(null, InvalidInput);

        if (emailAddress.IsNotNullOrWhiteSpace())
        {
            if (emailAddress.IsValidEmailAddress())
                return userService.GetByEmailAddressAsync(emailAddress, cancellationToken);

            validationResult.AddError(InvalidEmailAddress, emailAddress);
        }
        else if (phoneNumber.IsNotNullOrWhiteSpace())
        {
            if (phoneNumber.IsValidPhoneNumber())
                return userService.GetByPhoneNumberAsync(phoneNumber, cancellationToken);

            validationResult.AddError(InvalidPhoneNumber, phoneNumber);
        }
        else if (userName.IsNotNullOrWhiteSpace())
        {
            if (userName.IsValidUserName(allowedUserNameCharacters))
                return userService.GetByUserNameAsync(userName, cancellationToken);

            validationResult.AddError(InvalidUserName, userName);
        }
        else
        {
            var error = ValidationMessages.GetMessageRequiredAtLeastOne(
                nameof(LoginModel.EmailAddress),
                nameof(LoginModel.PhoneNumber),
                nameof(LoginModel.UserName));

            validationResult.AddError(InvalidModel, error);
        }

        return Task.FromResult(validationResult);
    }

    /// <summary>
    /// Generates the access token for the given <paramref name="user"/>.
    /// </summary>
    /// <param name="user">
    /// The user for whom to generate an access token.
    /// </param>
    /// <param name="refreshTokenId">
    /// The Id of the refresh token to
    /// bind with the newly generated JWT.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="AccessTokenModel"/>
    /// containing information about the access token.
    /// </returns>
    private async Task<Result<AccessTokenModel>> GenerateAccessTokenAsync(UserModel user, Guid? refreshTokenId = null,
                                                                          CancellationToken cancellationToken = default)
    {
        if (refreshTokenId.HasValue)
        {
            var validationResult = await ValidateRefreshTokenInternalAsync(refreshTokenId.Value, cancellationToken)
                .ConfigureAwait(false);

            if (!validationResult.IsSuccess)
                return new(validationResult.Status, validationResult.Errors, null);
        }
        else
        {
            refreshTokenId = await CreateRefreshTokenAsync(user.Id, cancellationToken)
                .ConfigureAwait(false);
        }

        var tokenClaims = await GetTokenClaimsAsync(user.Id, cancellationToken)
            .ConfigureAwait(false);

        var accessToken = jwtManager.IssueToken(tokenClaims, refreshTokenId.Value, out var expiryDate);

        await refreshTokenCache.SetAsync(refreshTokenId.Value, new(expiryDate.Ticks, false), cancellationToken)
            .ConfigureAwait(false);

        return new(new(user.EmailAddress, expiryDate, accessToken), Success);
    }

    /// <summary>
    /// Validates the refresh token with the given <paramref name="tokenId"/>.
    /// </summary>
    /// <param name="tokenId">
    /// The Id of the refresh token to be validated.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    /// <remarks>
    /// This method checks if the refresh token data is present in the cache,
    /// and if so - validates the cached entry, otherwise - it validates
    /// refresh token on the database level.
    /// </remarks>
    private async Task<Result> ValidateRefreshTokenInternalAsync(Guid tokenId, CancellationToken cancellationToken)
    {
        var currentTime = timeProvider.GetUtcNow();

        var cachedEntry = await refreshTokenCache.GetAsync(tokenId, cancellationToken)
            .ConfigureAwait(false);

        if (cachedEntry.HasValue)
        {
            if (!cachedEntry.Value.IsActive(currentTime))
                return new(Unauthorized, OutdatedToken);
        }
        else
        {
            var lookupResult = await userRefreshTokenService.GetByIdAsync(tokenId, cancellationToken)
                .ConfigureAwait(false);

            var refreshToken = lookupResult.Value;

            var isInvalidToken = refreshToken is null
                || refreshToken.IsExpired(currentTime)
                || !refreshToken.IsActive;

            if (isInvalidToken)
                return new(Unauthorized, OutdatedToken);
        }

        return new(Success);
    }

    /// <summary>
    /// Creates a new refresh token for the specified
    /// user and returns its identifier.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user
    /// for whom the refresh token is created.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the identifier of the newly created refresh token.
    /// </returns>
    private async Task<Guid> CreateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        var lifetime = authenticationOptions.Value.RefreshTokenLifetime;

        var creationResult = await userRefreshTokenService.CreateAsync(userId, lifetime, cancellationToken)
            .ConfigureAwait(false);

        if (creationResult.Value is null)
        {
            var message = string.Join(Environment.NewLine, creationResult.Errors);

            throw new InvalidOperationException(message);
        }

        return creationResult.Value.Id;
    }

    /// <summary>
    /// Builds the complete set of claims required for
    /// issuing an authentication token for the specified user.
    /// </summary>
    /// <remarks>
    /// At minimum, the returned claims include
    /// the user identifier claim required for authentication.
    /// <para>
    /// Role claims are also included to allow
    /// the authorization pipeline to function correctly.
    /// </para>
    /// </remarks>
    /// <param name="userId">
    /// The unique identifier of the user
    /// whose claims are being retrieved.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the collection of claims to be embedded into the token.
    /// </returns>
    private async Task<IEnumerable<Claim>> GetTokenClaimsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userClaimsResult = await userService.GetUserClaimsAsync(userId, cancellationToken)
            .ConfigureAwait(false);

        if (userClaimsResult.Value is null)
        {
            var message = string.Join(Environment.NewLine, userClaimsResult.Errors);

            throw new InvalidOperationException(message);
        }

        var rolesAsClaims = await GetUserRolesAsClaimsAsync(userId, cancellationToken)
            .ConfigureAwait(false);

        return userClaimsResult
            .Value
            .Concat(rolesAsClaims)
            .Select(claim => new Claim(claim.Type, claim.Value));
    }

    /// <summary>
    /// Generates the two-factor token for the given <paramref name="user"/>.
    /// </summary>
    /// <param name="user">
    /// The user for whom to generate a two-factor token.
    /// </param>
    /// <param name="verificationCode">
    /// Verification code used to retrieve an access token
    /// from the output two-factor token.
    /// </param>
    /// <returns>
    /// A <see cref="AccessTokenModel"/> instance containing the two-factor token.
    /// </returns>
    private AccessTokenModel GenerateTwoFactorToken(UserModel user, out string verificationCode)
    {
        var length = authenticationOptions.Value.TwoFactorVerificationCodeLength;
        verificationCode = dataProtector.GenerateRandomDigitCode(length);

        var lifetime = authenticationOptions.Value.TwoFactorTokenLifetime;
        var expiryDate = timeProvider.GetUtcNow().Add(lifetime);

        var identityToken = new IdentityToken(user.EmailAddress, verificationCode, expiryDate: expiryDate);
        var twoFactorToken = dataProtector.Protect(identityToken);

        return new(user.EmailAddress, expiryDate, twoFactorToken);
    }

    /// <summary>
    /// Gets the list of claims which contains
    /// roles data of the user with the given <paramref name="userId"/>.
    /// </summary>
    /// <param name="userId">
    /// The user whose roles to be listed as claims.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="UserClaimModel"/>, containing
    /// roles data of the user with the given <paramref name="userId"/>.
    /// </returns>
    private async Task<IEnumerable<UserClaimModel>> GetUserRolesAsClaimsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var roleClaimType = identityOptions.Value.ClaimsIdentity.RoleClaimType;

        var lookupResult = await roleService.GetUserRolesAsync(userId, cancellationToken)
            .ConfigureAwait(false);

        if (lookupResult.Value is null)
        {
            var message = string.Join(Environment.NewLine, lookupResult.Errors);

            throw new InvalidOperationException(message);
        }

        return lookupResult.Value.Select(role => new UserClaimModel(roleClaimType, role.Name));
    }
    #endregion
}