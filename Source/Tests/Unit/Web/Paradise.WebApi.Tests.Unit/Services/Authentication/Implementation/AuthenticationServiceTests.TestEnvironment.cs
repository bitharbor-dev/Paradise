using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Time.Testing;
using Microsoft.IdentityModel.JsonWebTokens;
using Paradise.ApplicationLogic.Infrastructure.DataProtection;
using Paradise.ApplicationLogic.Services.Identity.Roles;
using Paradise.ApplicationLogic.Services.Identity.Users;
using Paradise.Domain.Base.Events;
using Paradise.Models;
using Paradise.Models.Domain.Identity.Roles;
using Paradise.Models.Domain.Identity.Users;
using Paradise.Models.WebApi.Services.Authentication;
using Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.Domain.Events;
using Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Core.ApplicationLogic.Infrastructure.DataProtection;
using Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Core.ApplicationLogic.Services.Identity.Roles;
using Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Core.ApplicationLogic.Services.Identity.Users;
using Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Web.WebApi.Infrastructure.Authentication.Caching;
using Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Web.WebApi.Infrastructure.Authentication.JwtBearer;
using Paradise.WebApi.Infrastructure.Authentication.Caching;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer;
using Paradise.WebApi.Infrastructure.Options;
using Paradise.WebApi.Services.Authentication.Implementation;
using System.Security.Claims;
using static Paradise.Models.ErrorCode;
using static Paradise.Models.OperationStatus;
using OptionsBuilder = Microsoft.Extensions.Options.Options;

namespace Paradise.WebApi.Tests.Unit.Services.Authentication.Implementation;

public sealed partial class AuthenticationServiceTests : IDisposable
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();

    /// <summary>
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </summary>
    public CancellationToken Token { get; } = TestContext.Current.CancellationToken;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Dispose()
        => Test.Dispose();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="AuthenticationServiceTests"/> class.
    /// </summary>
    private sealed class TestEnvironment : IDisposable
    {
        #region Constants
        private const string DefaultAccessToken = "AccessToken";
        private const string DefaultRoleName = "User";
        private const string DefaultUserName = "UserName";
        private const string DefaultEmailAddress = "test@email.com";
        private const string DefaultPhoneNumber = "+100000000000";
        private const string DefaultPassword = "Password123!";
        private const string DefaultTwoFactorToken = "TwoFactorToken";
        private const string DefaultRandomDigitCode = "123456";
        private const string DefaultIdentityToken = "IdentityToken";
        #endregion

        #region Fields
        private static readonly Guid _userId = Guid.Parse("69c645b0-862c-832c-9ed5-02c4c76f398a");
        private static readonly Guid _roleId = Guid.Parse("69c645b0-862c-832c-9ed5-02c4c76f398a");
        private static readonly Guid _refreshTokenId = Guid.Parse("69c645b0-862c-832c-9ed5-02c4c76f398a");

        private readonly IList<IDomainEvent> _domainEvents = [];

        private readonly AuthenticationOptions _authenticationOptions = new();
        private readonly IdentityOptions _identityOptions = new();

        private readonly FakeTimeProvider _timeProvider = new();
        private readonly SpyDomainEventSink _domainEventSink = new();
        private readonly StubDataProtector _dataProtector = new();
        private readonly StubJwtManager _jwtManager = new();
        private readonly StubRefreshTokenCache _refreshTokenCache = new();
        private readonly StubRoleService _roleService = new();
        private readonly StubUserService _userService = new();
        private readonly StubUserRefreshTokenService _userRefreshTokenService = new();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _domainEventSink.Pushed += OnPushed;

            Target = new(OptionsBuilder.Create(_authenticationOptions),
                         OptionsBuilder.Create(_identityOptions),
                         _timeProvider,
                         _domainEventSink,
                         _dataProtector,
                         _jwtManager,
                         _refreshTokenCache,
                         _roleService,
                         _userService,
                         _userRefreshTokenService);
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public AuthenticationService Target { get; }

        /// <summary>
        /// Gets or sets the current UTC time.
        /// </summary>
        public DateTimeOffset UtcNow
        {
            get => _timeProvider.GetUtcNow();
            set => _timeProvider.SetUtcNow(value);
        }

        /// <summary>
        /// Calculates the expected access token expiration date
        /// based on the current <see cref="UtcNow"/> time
        /// and configured access token lifetime.
        /// </summary>
        public DateTimeOffset AccessTokenExpiryDate
            => UtcNow.Add(_authenticationOptions.AccessTokenLifetime);

        /// <summary>
        /// Calculates the expected refresh token expiration date
        /// based on the current <see cref="UtcNow"/> time
        /// and configured refresh token lifetime.
        /// </summary>
        public DateTimeOffset RefreshTokenExpiryDate
            => UtcNow.Add(_authenticationOptions.RefreshTokenLifetime);

        /// <summary>
        /// Calculates the expected two-factor authentication token expiration date
        /// based on the current <see cref="UtcNow"/> time
        /// and configured two-factor token lifetime.
        /// </summary>
        public DateTimeOffset TwoFactorTokenExpiryDate
            => UtcNow.Add(_authenticationOptions.TwoFactorTokenLifetime);
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public void Dispose()
            => _domainEventSink.Pushed -= OnPushed;

        /// <summary>
        /// Configures the test environment to simulate
        /// successful email address and password login flow.
        /// </summary>
        /// <param name="user">
        /// The expected authenticated user returned by the user service.
        /// </param>
        /// <param name="accessToken">
        /// The issued JWT access token.
        /// </param>
        public void SetupLoginSuccessWithEmailAddress(out UserModel user, out string accessToken)
        {
            user = GetUser();
            accessToken = DefaultAccessToken;

            var refreshToken = GetUserRefreshToken(user.Id);
            var claims = GetUserClaims(user.Id);
            var roles = GetUserRoles();

            SetUserServiceGetByEmailAddressAsync(new(user, Success));
            SetUserSerivceCheckPasswordAsync(new(Success));
            SetUserRefreshTokenServiceCreateAsync(new(refreshToken, Success));
            SetUserServiceGetUserClaimsAsync(new(claims, Success));
            SetRoleServiceGetUserRolesAsync(new(roles, Success));
            SetJwtManagerIssueToken(accessToken, AccessTokenExpiryDate);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// a login attempt where the user is not found by email address.
        /// </summary>
        /// <param name="emailAddress">
        /// The email address used for the login attempt.
        /// </param>
        public void SetupLoginFailureByMissingEmailAddress(out string emailAddress)
        {
            emailAddress = DefaultEmailAddress;

            SetUserServiceGetByEmailAddressAsync(new(null, Missing));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// successful phone number and password login flow.
        /// </summary>
        /// <param name="user">
        /// The expected authenticated user returned by the user service.
        /// </param>
        /// <param name="accessToken">
        /// The issued JWT access token.
        /// </param>
        public void SetupLoginSuccessWithPhoneNumber(out UserModel user, out string accessToken)
        {
            user = GetUser(phoneNumber: DefaultPhoneNumber);
            accessToken = DefaultAccessToken;

            var refreshToken = GetUserRefreshToken(user.Id);
            var claims = GetUserClaims(user.Id);
            var roles = GetUserRoles();

            SetUserServiceGetByPhoneNumberAsync(new(user, Success));
            SetUserSerivceCheckPasswordAsync(new(Success));
            SetUserRefreshTokenServiceCreateAsync(new(refreshToken, Success));
            SetUserServiceGetUserClaimsAsync(new(claims, Success));
            SetRoleServiceGetUserRolesAsync(new(roles, Success));
            SetJwtManagerIssueToken(accessToken, AccessTokenExpiryDate);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// a login attempt where the user is not found by phone number.
        /// </summary>
        /// <param name="phoneNumber">
        /// The phone number used for the login attempt.
        /// </param>
        public void SetupLoginFailureByMissingPhoneNumber(out string phoneNumber)
        {
            phoneNumber = DefaultPhoneNumber;

            SetUserServiceGetByPhoneNumberAsync(new(null, Missing));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// successful user name and password login flow.
        /// </summary>
        /// <param name="user">
        /// The expected authenticated user returned by the user service.
        /// </param>
        /// <param name="accessToken">
        /// The issued JWT access token.
        /// </param>
        public void SetupLoginSuccessWithUserName(out UserModel user, out string accessToken)
        {
            user = GetUser();
            accessToken = DefaultAccessToken;

            var refreshToken = GetUserRefreshToken(user.Id);
            var claims = GetUserClaims(user.Id);
            var roles = GetUserRoles();

            SetUserServiceGetByUserNameAsync(new(user, Success));
            SetUserSerivceCheckPasswordAsync(new(Success));
            SetUserRefreshTokenServiceCreateAsync(new(refreshToken, Success));
            SetUserServiceGetUserClaimsAsync(new(claims, Success));
            SetRoleServiceGetUserRolesAsync(new(roles, Success));
            SetJwtManagerIssueToken(accessToken, AccessTokenExpiryDate);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// a login attempt where the user is not found by user name.
        /// </summary>
        /// <param name="userName">
        /// The user name used for the login attempt.
        /// </param>
        public void SetupLoginFailureByMissingUserName(out string userName)
        {
            userName = DefaultUserName;

            SetUserServiceGetByUserNameAsync(new(null, Missing));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// a login attempt where the provided password
        /// does not match the user's password.
        /// </summary>
        /// <param name="emailAddress">
        /// The email address used for the login attempt.
        /// </param>
        /// <param name="password">
        /// The password provided for the login attempt.
        /// </param>
        public void SetupLoginFailureByPasswordMismatch(out string emailAddress, out string password)
        {
            emailAddress = DefaultEmailAddress;
            password = DefaultPassword;

            var user = GetUser(emailAddress: emailAddress);

            SetUserServiceGetByEmailAddressAsync(new(user, Success));
            SetUserSerivceCheckPasswordAsync(new(Unauthorized, UserNotFoundOrPasswordMismatch));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// successful two-factor authentication login flow.
        /// </summary>
        /// <param name="model">
        /// The login model used for authentication.
        /// </param>
        /// <param name="twoFactorToken">
        /// The generated two-factor authentication token.
        /// </param>
        public void SetupLoginSuccessWithTwoFactorAuthentication(LoginModel model, out string twoFactorToken)
        {
            twoFactorToken = DefaultTwoFactorToken;
            model.UserName ??= DefaultUserName;
            model.EmailAddress ??= DefaultEmailAddress;

            var user = GetUser(userName: model.UserName,
                               emailAddress: model.EmailAddress,
                               isTwoFactorEnabled: true);

            SetUserServiceGetByEmailAddressAsync(new(user, Success));
            SetUserSerivceCheckPasswordAsync(new(Success));
            SetDataProtectorGenerateRandomDigitCode(DefaultRandomDigitCode);
            SetDataProtectorProtect(twoFactorToken);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// a login attempt with an unconfirmed email address.
        /// </summary>
        /// <param name="emailAddress">
        /// The email address used for the login attempt.
        /// </param>
        public void SetupLoginFailureByUnconfirmedEmailAddress(out string emailAddress)
        {
            emailAddress = DefaultEmailAddress;

            _identityOptions.SignIn.RequireConfirmedEmail = true;

            var user = GetUser(emailAddress: emailAddress, isEmailAddressConfirmed: false);

            SetUserServiceGetByEmailAddressAsync(new(user, Success));
            SetUserSerivceCheckPasswordAsync(new(Success));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// a login attempt where refresh token creation fails.
        /// </summary>
        /// <param name="emailAddress">
        /// The email address used for the login attempt.
        /// </param>
        public void SetupLoginFailureByUserRefreshTokenCreation(out string emailAddress)
        {
            emailAddress = DefaultEmailAddress;

            var user = GetUser(emailAddress: emailAddress);

            SetUserServiceGetByEmailAddressAsync(new(user, Success));
            SetUserSerivceCheckPasswordAsync(new(Success));
            SetUserRefreshTokenServiceCreateAsync(new(null, Failure));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// a login attempt where user claim retrieval fails.
        /// </summary>
        /// <param name="emailAddress">
        /// The email address used for the login attempt.
        /// </param>
        public void SetupLoginFailureByClaimsFetching(out string emailAddress)
        {
            emailAddress = DefaultEmailAddress;

            var user = GetUser(emailAddress: emailAddress);
            var refreshToken = GetUserRefreshToken(user.Id);

            SetUserServiceGetByEmailAddressAsync(new(user, Success));
            SetUserSerivceCheckPasswordAsync(new(Success));
            SetUserRefreshTokenServiceCreateAsync(new(refreshToken, Success));
            SetUserServiceGetUserClaimsAsync(new(null, Failure));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// a login attempt where role retrieval fails.
        /// </summary>
        /// <param name="emailAddress">
        /// The email address used for the login attempt.
        /// </param>
        public void SetupLoginFailureByRolesFetching(out string emailAddress)
        {
            emailAddress = DefaultEmailAddress;

            var user = GetUser(emailAddress: emailAddress);
            var refreshToken = GetUserRefreshToken(user.Id);
            var claims = GetUserClaims(user.Id);

            SetUserServiceGetByEmailAddressAsync(new(user, Success));
            SetUserSerivceCheckPasswordAsync(new(Success));
            SetUserRefreshTokenServiceCreateAsync(new(refreshToken, Success));
            SetUserServiceGetUserClaimsAsync(new(claims, Success));
            SetRoleServiceGetUserRolesAsync(new(null, Failure));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// successful two-factor authentication flow.
        /// </summary>
        /// <param name="emailAddress">
        /// The email address used for the login attempt.
        /// </param>
        /// <param name="twoFactorCode">
        /// The code to be used to confirm the user's login.
        /// </param>
        /// <param name="identityToken">
        /// The value to be used to determine the user
        /// and validate a login request.
        /// </param>
        /// <param name="accessToken">
        /// The issued JWT access token.
        /// </param>
        public void SetupConfirmLoginSuccess(out string emailAddress,
                                             out string twoFactorCode,
                                             out string identityToken,
                                             out string accessToken)
        {
            emailAddress = DefaultEmailAddress;
            twoFactorCode = DefaultRandomDigitCode;
            identityToken = DefaultIdentityToken;
            accessToken = DefaultAccessToken;

            var identityTokenModel = GetIdentityToken(emailAddress, twoFactorCode,
                                                      expiryDate: TwoFactorTokenExpiryDate);

            var user = GetUser(emailAddress: emailAddress);
            var refreshToken = GetUserRefreshToken(user.Id);
            var claims = GetUserClaims(user.Id);
            var roles = GetUserRoles();

            SetDataProtectorTryUnprotect(true, identityTokenModel);
            SetUserServiceGetByEmailAddressAsync(new(user, Success));
            SetUserRefreshTokenServiceCreateAsync(new(refreshToken, Success));
            SetUserServiceGetUserClaimsAsync(new(claims, Success));
            SetRoleServiceGetUserRolesAsync(new(roles, Success));
            SetJwtManagerIssueToken(accessToken, AccessTokenExpiryDate);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// two-factor authentication flow where
        /// identity token could not be parsed.
        /// </summary>
        /// <param name="twoFactorCode">
        /// The code to be used to confirm the user's login.
        /// </param>
        /// <param name="identityToken">
        /// The value to be used to determine the user
        /// and validate a login request.
        /// </param>
        public void SetupConfirmLoginFailureByInvalidIdentityToken(out string twoFactorCode, out string identityToken)
        {
            twoFactorCode = DefaultRandomDigitCode;
            identityToken = DefaultIdentityToken;

            SetDataProtectorTryUnprotect(false, null);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// two-factor authentication flow where
        /// identity token is expired.
        /// </summary>
        /// <param name="twoFactorCode">
        /// The code to be used to confirm the user's login.
        /// </param>
        /// <param name="identityToken">
        /// The value to be used to determine the user
        /// and validate a login request.
        /// </param>
        public void SetupConfirmLoginFailureByExpiredIdentityToken(out string twoFactorCode, out string identityToken)
        {
            twoFactorCode = DefaultRandomDigitCode;
            identityToken = DefaultIdentityToken;

            var expiryDate = UtcNow.Subtract(TimeSpan.FromDays(1));
            var identityTokenModel = GetIdentityToken(DefaultEmailAddress, twoFactorCode,
                                                      expiryDate: expiryDate);

            SetDataProtectorTryUnprotect(true, identityTokenModel);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// two-factor authentication flow where
        /// two-factor code is equal to <see langword="null"/>, <see cref="string.Empty"/> or whitespace-only.
        /// </summary>
        /// <param name="twoFactorCode">
        /// The code to be used to confirm the user's login.
        /// </param>
        /// <param name="identityToken">
        /// The value to be used to determine the user
        /// and validate a login request.
        /// </param>
        public void SetupConfirmLoginFailureByInvalidTwoFactorCode(string? twoFactorCode, out string identityToken)
        {
            identityToken = DefaultIdentityToken;

            var identityTokenModel = GetIdentityToken(DefaultEmailAddress, twoFactorCode!,
                                                      expiryDate: TwoFactorTokenExpiryDate);

            SetDataProtectorTryUnprotect(true, identityTokenModel);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// two-factor authentication flow where
        /// two-factor code is incorrect.
        /// </summary>
        /// <param name="twoFactorCode">
        /// The code to be used to confirm the user's login.
        /// </param>
        /// <param name="identityToken">
        /// The value to be used to determine the user
        /// and validate a login request.
        /// </param>
        public void SetupConfirmLoginFailureByIncorrectTwoFactorCode(out string twoFactorCode, out string identityToken)
        {
            twoFactorCode = DefaultRandomDigitCode;
            identityToken = DefaultIdentityToken;

            var identityTokenModel = GetIdentityToken(DefaultEmailAddress, twoFactorCode + twoFactorCode,
                                                      expiryDate: TwoFactorTokenExpiryDate);

            SetDataProtectorTryUnprotect(true, identityTokenModel);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// two-factor authentication flow where
        /// the user is not found by email address.
        /// </summary>
        /// <param name="twoFactorCode">
        /// The code to be used to confirm the user's login.
        /// </param>
        /// <param name="identityToken">
        /// The value to be used to determine the user
        /// and validate a login request.
        /// </param>
        public void SetupConfirmLoginFailureByMissingUser(out string twoFactorCode, out string identityToken)
        {
            twoFactorCode = DefaultRandomDigitCode;
            identityToken = DefaultIdentityToken;

            var identityTokenModel = GetIdentityToken(DefaultEmailAddress, twoFactorCode,
                                                      expiryDate: TwoFactorTokenExpiryDate);

            SetDataProtectorTryUnprotect(true, identityTokenModel);
            SetUserServiceGetByEmailAddressAsync(new(null, Missing));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// two-factor authentication flow where
        /// refresh token creation fails.
        /// </summary>
        /// <param name="twoFactorCode">
        /// The code to be used to confirm the user's login.
        /// </param>
        /// <param name="identityToken">
        /// The value to be used to determine the user
        /// and validate a login request.
        /// </param>
        public void SetupConfirmLoginFailureByUserRefreshTokenCreation(out string twoFactorCode, out string identityToken)
        {
            twoFactorCode = DefaultRandomDigitCode;
            identityToken = DefaultIdentityToken;

            var identityTokenModel = GetIdentityToken(DefaultEmailAddress, twoFactorCode,
                                                      expiryDate: TwoFactorTokenExpiryDate);

            var user = GetUser(emailAddress: identityTokenModel.EmailAddress);

            SetDataProtectorTryUnprotect(true, identityTokenModel);
            SetUserServiceGetByEmailAddressAsync(new(user, Success));
            SetUserRefreshTokenServiceCreateAsync(new(null, Failure));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// two-factor authentication flow where
        /// user claim retrieval fails.
        /// </summary>
        /// <param name="twoFactorCode">
        /// The code to be used to confirm the user's login.
        /// </param>
        /// <param name="identityToken">
        /// The value to be used to determine the user
        /// and validate a login request.
        /// </param>
        public void SetupConfirmLoginFailureByClaimsFetching(out string twoFactorCode, out string identityToken)
        {
            twoFactorCode = DefaultRandomDigitCode;
            identityToken = DefaultIdentityToken;

            var identityTokenModel = GetIdentityToken(DefaultEmailAddress, twoFactorCode,
                                                      expiryDate: TwoFactorTokenExpiryDate);

            var user = GetUser(emailAddress: identityTokenModel.EmailAddress);
            var refreshToken = GetUserRefreshToken(user.Id);

            SetDataProtectorTryUnprotect(true, identityTokenModel);
            SetUserServiceGetByEmailAddressAsync(new(user, Success));
            SetUserRefreshTokenServiceCreateAsync(new(refreshToken, Success));
            SetUserServiceGetUserClaimsAsync(new(null, Failure));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// two-factor authentication flow where
        /// role retrieval fails.
        /// </summary>
        /// <param name="twoFactorCode">
        /// The code to be used to confirm the user's login.
        /// </param>
        /// <param name="identityToken">
        /// The value to be used to determine the user
        /// and validate a login request.
        /// </param>
        public void SetupConfirmLoginFailureByRolesFetching(out string twoFactorCode, out string identityToken)
        {
            twoFactorCode = DefaultRandomDigitCode;
            identityToken = DefaultIdentityToken;

            var identityTokenModel = GetIdentityToken(DefaultEmailAddress, twoFactorCode,
                                                      expiryDate: TwoFactorTokenExpiryDate);

            var user = GetUser(emailAddress: identityTokenModel.EmailAddress);
            var refreshToken = GetUserRefreshToken(user.Id);
            var claims = GetUserClaims(user.Id);

            SetDataProtectorTryUnprotect(true, identityTokenModel);
            SetUserServiceGetByEmailAddressAsync(new(user, Success));
            SetUserRefreshTokenServiceCreateAsync(new(refreshToken, Success));
            SetUserServiceGetUserClaimsAsync(new(claims, Success));
            SetRoleServiceGetUserRolesAsync(new(null, Failure));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// successful access token renewal using the
        /// data stored in database.
        /// </summary>
        /// <param name="accessToken">
        /// The issued JWT access token.
        /// </param>
        public void SetupRenewTokenSuccessWithPersistedRefreshToken(out string accessToken)
        {
            accessToken = DefaultAccessToken;

            var user = GetUser();
            var refreshToken = GetUserRefreshToken(user.Id);
            var principal = GetPrincipal(refreshTokenId: refreshToken.Id);
            var claims = GetUserClaims(user.Id);
            var roles = GetUserRoles();

            SetJwtManagerTryParseToken(true, principal);
            SetUserServiceGetByIdAsync(new(user, Success));
            SetRefreshTokenCacheGetAsync(null);
            SetUserRefreshTokenServiceGetByIdAsync(new(refreshToken, Success));
            SetUserServiceGetUserClaimsAsync(new(claims, Success));
            SetRoleServiceGetUserRolesAsync(new(roles, Success));
            SetJwtManagerIssueToken(accessToken, AccessTokenExpiryDate);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// access token renewal flow where
        /// persisted refresh token is expired.
        /// </summary>
        /// <param name="accessToken">
        /// The issued JWT access token.
        /// </param>
        public void SetupRenewTokenFailureByExpiredPersistedRefreshToken(out string accessToken)
        {
            accessToken = DefaultAccessToken;

            var expiryDate = RefreshTokenExpiryDate.Subtract(TimeSpan.FromDays(1));

            var user = GetUser();
            var refreshToken = GetUserRefreshToken(user.Id, expiryDateUtc: expiryDate);
            var principal = GetPrincipal(refreshTokenId: refreshToken.Id);

            SetJwtManagerTryParseToken(true, principal);
            SetUserServiceGetByIdAsync(new(user, Success));
            SetRefreshTokenCacheGetAsync(null);
            SetUserRefreshTokenServiceGetByIdAsync(new(refreshToken, Success));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// access token renewal flow where
        /// persisted refresh token is inactive.
        /// </summary>
        /// <param name="accessToken">
        /// The issued JWT access token.
        /// </param>
        public void SetupRenewTokenFailureByInactivePersistedRefreshToken(out string accessToken)
        {
            accessToken = DefaultAccessToken;

            var user = GetUser();
            var refreshToken = GetUserRefreshToken(user.Id, isActive: false);
            var principal = GetPrincipal(refreshTokenId: refreshToken.Id);

            SetJwtManagerTryParseToken(true, principal);
            SetUserServiceGetByIdAsync(new(user, Success));
            SetRefreshTokenCacheGetAsync(null);
            SetUserRefreshTokenServiceGetByIdAsync(new(refreshToken, Success));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// a successful access token renewal using the
        /// data stored in cache.
        /// </summary>
        /// <param name="accessToken">
        /// The issued JWT access token.
        /// </param>
        public void SetupRenewTokenSuccessWithCachedRefreshToken(out string accessToken)
        {
            accessToken = DefaultAccessToken;

            var user = GetUser();
            var principal = GetPrincipal(refreshTokenId: _refreshTokenId);
            var cacheEntry = new RefreshTokenCacheEntry(long.MaxValue, false);
            var claims = GetUserClaims(user.Id);
            var roles = GetUserRoles();

            SetJwtManagerTryParseToken(true, principal);
            SetUserServiceGetByIdAsync(new(user, Success));
            SetRefreshTokenCacheGetAsync(cacheEntry);
            SetUserServiceGetUserClaimsAsync(new(claims, Success));
            SetRoleServiceGetUserRolesAsync(new(roles, Success));
            SetJwtManagerIssueToken(accessToken, AccessTokenExpiryDate);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// successful access token renewal using the
        /// data stored in cache.
        /// </summary>
        public void SetupRenewTokenFailureByRevokedCachedRefreshToken()
        {
            var user = GetUser();
            var principal = GetPrincipal(refreshTokenId: _refreshTokenId);
            var cacheEntry = new RefreshTokenCacheEntry(long.MaxValue, true);

            SetJwtManagerTryParseToken(true, principal);
            SetUserServiceGetByIdAsync(new(user, Success));
            SetRefreshTokenCacheGetAsync(cacheEntry);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// access token renewal flow where
        /// no refresh token data is present in the database or cache.
        /// </summary>
        public void SetupRenewTokenFailureByMissingRefreshToken()
        {
            var user = GetUser();
            var refreshToken = GetUserRefreshToken(user.Id);
            var principal = GetPrincipal(refreshTokenId: refreshToken.Id);

            SetJwtManagerTryParseToken(true, principal);
            SetUserServiceGetByIdAsync(new(user, Success));
            SetRefreshTokenCacheGetAsync(null);
            SetUserRefreshTokenServiceGetByIdAsync(new(null, Missing));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// access token renewal flow where
        /// input access token is invalid.
        /// </summary>
        public void SetupRenewTokenFailureByInvalidAccessToken(out string accessToken)
        {
            accessToken = DefaultAccessToken;

            SetJwtManagerTryParseToken(false, null);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// access token renewal flow where
        /// input access token does not contain a refresh token identifier.
        /// </summary>
        /// <param name="accessToken">
        /// The issued JWT access token.
        /// </param>
        public void SetupRenewTokenFailureByMissingRefreshTokenId(out string accessToken)
        {
            accessToken = DefaultAccessToken;

            var principal = GetPrincipal(refreshTokenId: null);

            SetJwtManagerTryParseToken(true, principal);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// access token renewal flow where
        /// input access token is linked to a non-existing user.
        /// </summary>
        /// <param name="accessToken">
        /// The issued JWT access token.
        /// </param>
        public void SetupRenewTokenFailureByMissingUser(out string accessToken)
        {
            accessToken = DefaultAccessToken;

            var principal = GetPrincipal(_userId, _refreshTokenId);

            SetJwtManagerTryParseToken(true, principal);
            SetUserServiceGetByIdAsync(new(null, Missing));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// refresh token invalidation and cache cleanup,
        /// effectively - logging out the user.
        /// </summary>
        /// <param name="context">
        /// The <see cref="LogoutContext"/> containing the shared logout activity state.
        /// </param>
        public void SetupLogoutSuccess(out LogoutContext context)
        {
            context = new();

            var refreshToken = GetUserRefreshToken(_userId, false);
            var principal = GetPrincipal(_userId, refreshToken.Id);

            SetJwtManagerTryParseToken(true, principal);
            SetUserRefreshTokenServiceDeactivateAsync(context, new(refreshToken, Success));
            SetRefreshTokenCacheRevokeAsync(context);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// logout flow where
        /// input access token is invalid.
        /// </summary>
        public void SetupLogoutFailureByInvalidAccessToken(out string accessToken)
        {
            accessToken = DefaultAccessToken;

            SetJwtManagerTryParseToken(false, null);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// logout flow where
        /// input access token does not contain a refresh token identifier.
        /// </summary>
        /// <param name="accessToken">
        /// The issued JWT access token.
        /// </param>
        public void SetupLogoutFailureByMissingRefreshTokenId(out string accessToken)
        {
            accessToken = DefaultAccessToken;

            var principal = GetPrincipal(refreshTokenId: null);

            SetJwtManagerTryParseToken(true, principal);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// all refresh tokens invalidation and cache cleanup,
        /// effectively - logging out the user on all sessions.
        /// </summary>
        /// <param name="context">
        /// The <see cref="LogoutContext"/> containing the shared logout activity state.
        /// </param>
        public void SetupTerminateSessionsSuccess(out LogoutContext context)
        {
            context = new();

            var refreshToken = GetUserRefreshToken(_userId, false);
            var principal = GetPrincipal(_userId, refreshToken.Id);

            SetJwtManagerTryParseToken(true, principal);
            SetUserRefreshTokenServiceDeactivateAllAsync(context, new([refreshToken], Success));
            SetRefreshTokenCacheRevokeAsync(context);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// sessions termination flow where
        /// input access token is invalid.
        /// </summary>
        public void SetupTerminateSessionsFailureByInvalidAccessToken(out string accessToken)
        {
            accessToken = DefaultAccessToken;

            SetJwtManagerTryParseToken(false, null);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// successful persisted refresh token validation.
        /// </summary>
        /// <param name="principal">
        /// A <see cref="ClaimsPrincipal"/> representation
        /// of the issued JWT access token.
        /// </param>
        public void SetupCheckSessionSuccessWithPersistedRefreshToken(out ClaimsPrincipal principal)
        {
            var refreshToken = GetUserRefreshToken(_userId);
            principal = GetPrincipal(refreshTokenId: refreshToken.Id);

            SetRefreshTokenCacheGetAsync(null);
            SetUserRefreshTokenServiceGetByIdAsync(new(refreshToken, Success));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// persisted refresh token validation flow where
        /// refresh token is expired.
        /// </summary>
        /// <param name="principal">
        /// A <see cref="ClaimsPrincipal"/> representation
        /// of the issued JWT access token.
        /// </param>
        public void SetupCheckSessionFailureByExpiredPersistedRefreshToken(out ClaimsPrincipal principal)
        {
            var expiryDate = RefreshTokenExpiryDate.Subtract(TimeSpan.FromDays(1));

            var refreshToken = GetUserRefreshToken(_userId, expiryDateUtc: expiryDate);
            principal = GetPrincipal(refreshTokenId: refreshToken.Id);

            SetRefreshTokenCacheGetAsync(null);
            SetUserRefreshTokenServiceGetByIdAsync(new(refreshToken, Success));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// persisted refresh token validation flow where
        /// refresh token is inactive.
        /// </summary>
        /// <param name="principal">
        /// A <see cref="ClaimsPrincipal"/> representation
        /// of the issued JWT access token.
        /// </param>
        public void SetupCheckSessionFailureByInactivePersistedRefreshToken(out ClaimsPrincipal principal)
        {
            var refreshToken = GetUserRefreshToken(_userId, isActive: false);
            principal = GetPrincipal(refreshTokenId: refreshToken.Id);

            SetRefreshTokenCacheGetAsync(null);
            SetUserRefreshTokenServiceGetByIdAsync(new(refreshToken, Success));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// successful cached refresh token validation.
        /// </summary>
        /// <param name="principal">
        /// A <see cref="ClaimsPrincipal"/> representation
        /// of the issued JWT access token.
        /// </param>
        public void SetupCheckSessionSuccessWithCachedRefreshToken(out ClaimsPrincipal principal)
        {
            principal = GetPrincipal(refreshTokenId: _refreshTokenId);
            var cacheEntry = new RefreshTokenCacheEntry(long.MaxValue, false);

            SetRefreshTokenCacheGetAsync(cacheEntry);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// cached refresh token validation flow where
        /// refresh token is revoked.
        /// </summary>
        /// <param name="principal">
        /// A <see cref="ClaimsPrincipal"/> representation
        /// of the issued JWT access token.
        /// </param>
        public void SetupCheckSessionFailureByRevokedCachedRefreshToken(out ClaimsPrincipal principal)
        {
            principal = GetPrincipal(refreshTokenId: _refreshTokenId);
            var cacheEntry = new RefreshTokenCacheEntry(long.MaxValue, true);

            SetRefreshTokenCacheGetAsync(cacheEntry);
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// refresh token validation flow where
        /// no refresh token data is present in the database or cache.
        /// </summary>
        /// <param name="principal">
        /// A <see cref="ClaimsPrincipal"/> representation
        /// of the issued JWT access token.
        /// </param>
        public void SetupCheckSessionFailureByMissingRefreshToken(out ClaimsPrincipal principal)
        {
            principal = GetPrincipal(refreshTokenId: _refreshTokenId);

            SetRefreshTokenCacheGetAsync(null);
            SetUserRefreshTokenServiceGetByIdAsync(new(null, Missing));
        }

        /// <summary>
        /// Configures the test environment to simulate
        /// refresh token validation flow where
        /// input principal does not contain a refresh token identifier.
        /// </summary>
        /// <param name="principal">
        /// A <see cref="ClaimsPrincipal"/> representation
        /// of the issued JWT access token.
        /// </param>
        public void SetupCheckSessionFailureByMissingRefreshTokenId(out ClaimsPrincipal principal)
            => principal = GetPrincipal(refreshTokenId: null);
        #endregion

        #region Private methods
        /// <summary>
        /// Gets the <see cref="UserModel"/> with configured values.
        /// </summary>
        /// <param name="id">
        /// User Id.
        /// </param>
        /// <param name="userName">
        /// User's user-name.
        /// </param>
        /// <param name="emailAddress">
        /// User's email address.
        /// </param>
        /// <param name="isEmailAddressConfirmed">
        /// Indicates whether the user's email address has been confirmed.
        /// </param>
        /// <param name="phoneNumber">
        /// User's phone number.
        /// </param>
        /// <param name="isPhoneNumberConfirmed">
        /// Indicates whether the user's phone number has been confirmed.
        /// </param>
        /// <param name="isPendingDeletion">
        /// Indicates whether the user is pending deletion.
        /// </param>
        /// <param name="isTwoFactorEnabled">
        /// Indicates whether two-factor authentication is enabled for the user.
        /// </param>
        /// <param name="registrationDate">
        /// User registration date.
        /// </param>
        /// <returns>
        /// A configured <see cref="UserModel"/> instance.
        /// </returns>
        private UserModel GetUser(Guid? id = null,
                                  string userName = DefaultUserName,
                                  string emailAddress = DefaultEmailAddress,
                                  bool isEmailAddressConfirmed = true,
                                  string? phoneNumber = null,
                                  bool isPhoneNumberConfirmed = false,
                                  bool isPendingDeletion = false,
                                  bool isTwoFactorEnabled = false,
                                  DateTimeOffset? registrationDate = null)
        {
            id ??= _userId;
            registrationDate ??= UtcNow;

            return new(id.Value, userName, emailAddress, isEmailAddressConfirmed,
                       phoneNumber, isPhoneNumberConfirmed, isPendingDeletion,
                       isTwoFactorEnabled, registrationDate.Value);
        }

        /// <summary>
        /// Gets the <see cref="UserRefreshTokenModel"/> with configured values.
        /// </summary>
        /// <param name="ownerId">
        /// Refresh token owner's Id.
        /// </param>
        /// <param name="isActive">
        /// Indicates whether the refresh token is active (was not revoked)
        /// and can be used during authentication processes.
        /// </param>
        /// <param name="expiryDateUtc">
        /// Refresh token expiry date.
        /// </param>
        /// <returns>
        /// A configured <see cref="UserRefreshTokenModel"/> instance.
        /// </returns>
        private UserRefreshTokenModel GetUserRefreshToken(Guid ownerId, bool isActive = true, DateTimeOffset? expiryDateUtc = null)
        {
            expiryDateUtc ??= RefreshTokenExpiryDate;
            return new(_refreshTokenId, ownerId, UtcNow, isActive, expiryDateUtc.Value);
        }

        /// <summary>
        /// Gets the list of user claims,
        /// containing a single claim of type "Id"
        /// with its value set to the given <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">
        /// User Id.
        /// </param>
        /// <returns>
        /// The list of user claims.
        /// </returns>
        private IEnumerable<UserClaimModel> GetUserClaims(Guid userId)
            => [new(_identityOptions.ClaimsIdentity.UserIdClaimType, userId.ToString())];

        /// <summary>
        /// Gets the list of user roles,
        /// containing a single role
        /// with its name set to <see cref="DefaultRoleName"/>.
        /// </summary>
        /// <returns>
        /// The list of user roles.
        /// </returns>
        private IEnumerable<RoleModel> GetUserRoles()
            => [new(_roleId, UtcNow, UtcNow, DefaultRoleName, true)];

        /// <summary>
        /// Gets the <see cref="IdentityToken"/> with configured values.
        /// </summary>
        /// <param name="emailAddress">
        /// Email address.
        /// </param>
        /// <param name="innerToken">
        /// Inner token.
        /// </param>
        /// <param name="newValue">
        /// The new value associated with the token, if any.
        /// </param>
        /// <param name="expiryDate">
        /// Token expiry date.
        /// <para>
        /// <see langword="null"/> value means that token never expires.
        /// </para>
        /// </param>
        /// <returns>
        /// A configured <see cref="IdentityToken"/> instance.
        /// </returns>
        private static IdentityToken GetIdentityToken(string emailAddress, string innerToken,
                                                      string? newValue = null, DateTimeOffset? expiryDate = null)
            => new(emailAddress, innerToken, newValue, expiryDate);

        /// <summary>
        /// Gets the <see cref="ClaimsPrincipal"/> with configured claims.
        /// </summary>
        /// <param name="userId">
        /// User Id.
        /// </param>
        /// <param name="refreshTokenId">
        /// Refresh token Id.
        /// </param>
        /// <returns>
        /// A configured <see cref="ClaimsPrincipal"/> instance.
        /// </returns>
        private ClaimsPrincipal GetPrincipal(Guid? userId = null, Guid? refreshTokenId = null)
        {
            var claims = new List<Claim>();

            if (userId.HasValue)
                claims.Add(new(_identityOptions.ClaimsIdentity.UserIdClaimType, userId.Value.ToString()));

            if (refreshTokenId.HasValue)
                claims.Add(new(JwtRegisteredClaimNames.Sid, refreshTokenId.Value.ToString()));

            return new(new ClaimsIdentity(claims));
        }

        /// <summary>
        /// Intercepts the internal <see cref="IUserService.GetByEmailAddressAsync"/>
        /// method call and makes it return the <paramref name="result"/>.
        /// </summary>
        /// <param name="result">
        /// <see cref="IUserService.GetByEmailAddressAsync"/> result.
        /// </param>
        private void SetUserServiceGetByEmailAddressAsync(Result<UserModel> result)
            => _userService.GetByEmailAddressAsyncResult = () => Task.FromResult(result);

        /// <summary>
        /// Intercepts the internal <see cref="IUserService.GetByPhoneNumberAsync"/>
        /// method call and makes it return the <paramref name="result"/>.
        /// </summary>
        /// <param name="result">
        /// <see cref="IUserService.GetByPhoneNumberAsync"/> result.
        /// </param>
        private void SetUserServiceGetByPhoneNumberAsync(Result<UserModel> result)
            => _userService.GetByPhoneNumberAsyncResult = () => Task.FromResult(result);

        /// <summary>
        /// Intercepts the internal <see cref="IUserService.GetByUserNameAsync"/>
        /// method call and makes it return the <paramref name="result"/>.
        /// </summary>
        /// <param name="result">
        /// <see cref="IUserService.GetByUserNameAsync"/> result.
        /// </param>
        private void SetUserServiceGetByUserNameAsync(Result<UserModel> result)
            => _userService.GetByUserNameAsyncResult = () => Task.FromResult(result);

        /// <summary>
        /// Intercepts the internal <see cref="IUserService.GetByIdAsync"/>
        /// method call and makes it return the <paramref name="result"/>.
        /// </summary>
        /// <param name="result">
        /// <see cref="IUserService.GetByIdAsync"/> result.
        /// </param>
        private void SetUserServiceGetByIdAsync(Result<UserModel> result)
            => _userService.GetByIdAsyncResult = () => Task.FromResult(result);

        /// <summary>
        /// Intercepts the internal <see cref="IUserService.CheckPasswordAsync"/>
        /// method call and makes it return the <paramref name="result"/>.
        /// </summary>
        /// <param name="result">
        /// <see cref="IUserService.CheckPasswordAsync"/> result.
        /// </param>
        private void SetUserSerivceCheckPasswordAsync(Result result)
            => _userService.CheckPasswordAsyncResult = () => Task.FromResult(result);

        /// <summary>
        /// Intercepts the internal <see cref="IUserService.GetUserClaimsAsync"/>
        /// method call and makes it return the <paramref name="result"/>.
        /// </summary>
        /// <param name="result">
        /// <see cref="IUserService.GetUserClaimsAsync"/> result.
        /// </param>
        private void SetUserServiceGetUserClaimsAsync(Result<IEnumerable<UserClaimModel>> result)
            => _userService.GetUserClaimsAsyncResult = () => Task.FromResult(result);

        /// <summary>
        /// Intercepts the internal <see cref="IUserRefreshTokenService.CreateAsync"/>
        /// method call and makes it return the <paramref name="result"/>.
        /// </summary>
        /// <param name="result">
        /// <see cref="IUserRefreshTokenService.CreateAsync"/> result.
        /// </param>
        private void SetUserRefreshTokenServiceCreateAsync(Result<UserRefreshTokenModel> result)
            => _userRefreshTokenService.CreateAsyncResult = () => Task.FromResult(result);

        /// <summary>
        /// Intercepts the internal <see cref="IUserRefreshTokenService.DeactivateAsync"/>
        /// method call and makes it set the <see cref="LogoutContext.DatabaseCleared"/>
        /// vale to <see langword="true"/> for proper logout activities tracking.
        /// </summary>
        /// <param name="context">
        /// <see cref="LogoutContext"/> instance used to share logout activities state.
        /// </param>
        /// <param name="result">
        /// <see cref="IUserRefreshTokenService.DeactivateAsync"/> result.
        /// </param>
        private void SetUserRefreshTokenServiceDeactivateAsync(LogoutContext context, Result<UserRefreshTokenModel> result)
        {
            _userRefreshTokenService.DeactivateAsyncResult = () =>
            {
                context.DatabaseCleared = true;
                return Task.FromResult(result);
            };
        }

        /// <summary>
        /// Intercepts the internal <see cref="IUserRefreshTokenService.DeactivateAllAsync"/>
        /// method call and makes it set the <see cref="LogoutContext.DatabaseCleared"/>
        /// vale to <see langword="true"/> for proper logout activities tracking.
        /// </summary>
        /// <param name="context">
        /// <see cref="LogoutContext"/> instance used to share logout activities state.
        /// </param>
        /// <param name="result">
        /// <see cref="IUserRefreshTokenService.DeactivateAllAsync"/> result.
        /// </param>
        private void SetUserRefreshTokenServiceDeactivateAllAsync(LogoutContext context, Result<IEnumerable<UserRefreshTokenModel>> result)
        {
            _userRefreshTokenService.DeactivateAllAsyncResult = () =>
            {
                context.DatabaseCleared = true;
                return Task.FromResult(result);
            };
        }

        /// <summary>
        /// Intercepts the internal <see cref="IUserRefreshTokenService.GetByIdAsync"/>
        /// method call and makes it return the <paramref name="result"/>.
        /// </summary>
        /// <param name="result">
        /// <see cref="IUserRefreshTokenService.GetByIdAsync"/> result.
        /// </param>
        private void SetUserRefreshTokenServiceGetByIdAsync(Result<UserRefreshTokenModel> result)
            => _userRefreshTokenService.GetByIdAsyncResult = () => Task.FromResult(result);

        /// <summary>
        /// Intercepts the internal <see cref="IRoleService.GetUserRolesAsync"/>
        /// method call and makes it return the <paramref name="result"/>.
        /// </summary>
        /// <param name="result">
        /// <see cref="IRoleService.GetUserRolesAsync"/> result.
        /// </param>
        private void SetRoleServiceGetUserRolesAsync(Result<IEnumerable<RoleModel>> result)
            => _roleService.GetUserRolesAsyncResult = () => Task.FromResult(result);

        /// <summary>
        /// Intercepts the internal <see cref="IJwtManager.IssueToken"/>
        /// method call and makes it return the <paramref name="token"/>
        /// and <paramref name="expiryDate"/>.
        /// </summary>
        /// <param name="token">
        /// <see cref="IJwtManager.IssueToken"/> result.
        /// </param>
        /// <param name="expiryDate">
        /// <see cref="IJwtManager.IssueToken"/> output parameter.
        /// </param>
        private void SetJwtManagerIssueToken(string token, DateTimeOffset expiryDate)
            => _jwtManager.IssueTokenResult = new(token, expiryDate);

        /// <summary>
        /// Intercepts the internal <see cref="IJwtManager.TryParseToken"/>
        /// method call and makes it return the <paramref name="result"/>
        /// and <paramref name="principal"/>.
        /// </summary>
        /// <param name="result">
        /// <see cref="IJwtManager.TryParseToken"/> result.
        /// </param>
        /// <param name="principal">
        /// <see cref="IJwtManager.TryParseToken"/> output parameter.
        /// </param>
        private void SetJwtManagerTryParseToken(bool result, ClaimsPrincipal? principal)
            => _jwtManager.TryParseTokenResult = new(result, principal);

        /// <summary>
        /// Intercepts the internal <see cref="IDataProtector.GenerateRandomDigitCode"/>
        /// method call and makes it return the <paramref name="result"/>.
        /// </summary>
        /// <param name="result">
        /// <see cref="IDataProtector.GenerateRandomDigitCode"/> result.
        /// </param>
        private void SetDataProtectorGenerateRandomDigitCode(string result)
            => _dataProtector.GenerateRandomDigitCodeResult = result;

        /// <summary>
        /// Intercepts the internal <see cref="IDataProtector.Protect"/>
        /// method call and makes it return the <paramref name="result"/>.
        /// </summary>
        /// <param name="result">
        /// <see cref="IDataProtector.Protect"/> result.
        /// </param>
        private void SetDataProtectorProtect(string result)
            => _dataProtector.ProtectResult = result;

        /// <summary>
        /// Intercepts the internal <see cref="IDataProtector.TryUnprotect"/>
        /// method call and makes it return the <paramref name="returnValue"/>
        /// and <paramref name="outputParameter"/>.
        /// </summary>
        /// <param name="returnValue">
        /// <see cref="IDataProtector.TryUnprotect"/> result.
        /// </param>
        /// <param name="outputParameter">
        /// <see cref="IDataProtector.TryUnprotect"/> output parameter.
        /// </param>
        private void SetDataProtectorTryUnprotect(bool returnValue, object? outputParameter)
            => _dataProtector.TryUnprotectResult = new(returnValue, outputParameter);

        /// <summary>
        /// Intercepts the internal <see cref="IRefreshTokenCache.GetAsync"/>
        /// method call and makes it return the <paramref name="cacheEntry"/>.
        /// </summary>
        /// <param name="cacheEntry">
        /// <see cref="IRefreshTokenCache.GetAsync"/> result.
        /// </param>
        private void SetRefreshTokenCacheGetAsync(RefreshTokenCacheEntry? cacheEntry)
            => _refreshTokenCache.GetAsyncResult = () => Task.FromResult(cacheEntry);

        /// <summary>
        /// Intercepts the internal <see cref="IRefreshTokenCache.RevokeAsync"/>
        /// method call and makes it set the <see cref="LogoutContext.CacheCleared"/>
        /// vale to <see langword="true"/> for proper logout activities tracking.
        /// </summary>
        /// <param name="context">
        /// <see cref="LogoutContext"/> instance used to share logout activities state.
        /// </param>
        private void SetRefreshTokenCacheRevokeAsync(LogoutContext context)
        {
            _refreshTokenCache.RevokeAsyncResult = () =>
            {
                context.CacheCleared = true;
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// <see cref="SpyDomainEventSink.Pushed"/> event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="DomainEventPushedEventArgs"/> instance containing the event data.
        /// </param>
        private void OnPushed(object? sender, DomainEventPushedEventArgs e)
            => _domainEvents.Add(e.DomainEvent);
        #endregion
    }

    /// <summary>
    /// A helper class which helps tracking logout activities.
    /// </summary>
    private sealed class LogoutContext
    {
        #region Properties
        /// <summary>
        /// Indicates whether the refresh token data
        /// was removed from the cache.
        /// </summary>
        public bool CacheCleared { get; set; }

        /// <summary>
        /// Indicates whether the refresh token data
        /// was removed from the database.
        /// </summary>
        public bool DatabaseCleared { get; set; }
        #endregion
    }
    #endregion
}