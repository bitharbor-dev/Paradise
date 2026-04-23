using Paradise.ApplicationLogic.Services.Identity.Roles;
using Paradise.ApplicationLogic.Services.Identity.Users;
using Paradise.Models.WebApi.Services.Authentication;
using Paradise.Tests.Miscellaneous;
using Paradise.WebApi.Services.Authentication.Implementation;
using System.Security.Claims;
using static Paradise.Models.ErrorCode;
using static Paradise.Models.OperationStatus;

namespace Paradise.WebApi.Tests.Unit.Services.Authentication.Implementation;

/// <summary>
/// <see cref="AuthenticationService"/> test class.
/// </summary>
public sealed partial class AuthenticationServiceTests
{
    #region Constants
    private const string DefaultEmailAddress = "test@email.com";
    private const string DefaultPhoneNumber = "+100000000000";
    private const string DefaultUserName = "UserName";

    private const string Password = "Password123!";

    private const string NotEmailAddress = "Not an email address";
    private const string NotPhoneNumber = "Not a phone number";
    private const string BadUserName = "!@#$%^&*() Bad user name";

    private const string AccessToken = "AccessToken";
    private const string PrefixedAccessToken = "Bearer AccessToken";
    #endregion

    #region Properties
    /// <summary>
    /// Provides member data for <see cref="LoginAsync_FailsOnMissingPassword"/> method.
    /// </summary>
    public static TheoryData<string?> LoginAsync_FailsOnMissingPassword_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };

    /// <summary>
    /// Provides member data for <see cref="LoginAsync_ReturnsTwoFactorToken"/> method.
    /// </summary>
    public static TheoryData<LoginModel> LoginAsync_ReturnsTwoFactorToken_MemberData { get; } = new()
    {
        { new LoginModel(Password) { EmailAddress = DefaultEmailAddress }   },
        { new LoginModel(Password) { PhoneNumber = DefaultPhoneNumber }     },
        { new LoginModel(Password) { UserName = DefaultUserName }           }
    };

    /// <summary>
    /// Provides member data for <see cref="ConfirmLoginAsync_FailsOnEmptyTwoFactorCode"/> method.
    /// </summary>
    public static TheoryData<string?> ConfirmLoginAsync_FailsOnEmptyTwoFactorCode_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };

    /// <summary>
    /// Provides member data for <see cref="RenewTokenAsync_WithPersistedRefreshToken"/> method.
    /// </summary>
    public static TheoryData<string> RenewTokenAsync_WithPersistedRefreshToken_MemberData { get; } = new()
    {
        { AccessToken           },
        { PrefixedAccessToken   }
    };

    /// <summary>
    /// Provides member data for <see cref="RenewTokenAsync_WithCachedRefreshToken"/> method.
    /// </summary>
    public static TheoryData<string> RenewTokenAsync_WithCachedRefreshToken_MemberData { get; } = new()
    {
        { AccessToken           },
        { PrefixedAccessToken   }
    };

    /// <summary>
    /// Provides member data for <see cref="LogoutAsync"/> method.
    /// </summary>
    public static TheoryData<string> LogoutAsync_MemberData { get; } = new()
    {
        { AccessToken           },
        { PrefixedAccessToken   }
    };

    /// <summary>
    /// Provides member data for <see cref="TerminateSessionsAsync"/> method.
    /// </summary>
    public static TheoryData<string> TerminateSessionsAsync_MemberData { get; } = new()
    {
        { AccessToken           },
        { PrefixedAccessToken   }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// return an access token when user successfully
    /// logins with their email address.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithEmailAddress()
    {
        // Arrange
        Test.SetupLoginSuccessWithEmailAddress(out var user, out var accessToken);

        var model = new LoginModel(Password)
        {
            EmailAddress = user.EmailAddress
        };

        // Act
        var result = await Test.Target.LoginAsync(model, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.Equal(user.EmailAddress, result.Value.EmailAddress);
        Assert.Equal(accessToken, result.Value.Token);
        Assert.Equal(Test.AccessTokenExpiryDate, result.Value.ExpiryDate);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// fail to return an access token if the input
    /// email address is invalid.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithEmailAddress_FailsOnInvalidEmailAddress()
    {
        // Arrange
        var model = new LoginModel(Password)
        {
            EmailAddress = NotEmailAddress
        };

        // Act
        var result = await Test.Target.LoginAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidEmailAddress, model.EmailAddress);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// fail to return an access token when no user
    /// with the specified email address exists.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithEmailAddress_FailsOnNonExistingUser()
    {
        // Arrange
        Test.SetupLoginFailureByMissingEmailAddress(out var emailAddress);

        var model = new LoginModel(Password)
        {
            EmailAddress = emailAddress
        };

        // Act
        var result = await Test.Target.LoginAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Unauthorized, result.Status);

        Assert.ContainsErrorCode(result, UserNotFoundOrPasswordMismatch);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// return an access token when user successfully
    /// logins with their phone number.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithPhoneNumber()
    {
        // Arrange
        Test.SetupLoginSuccessWithPhoneNumber(out var user, out var accessToken);

        var model = new LoginModel(Password)
        {
            PhoneNumber = user.PhoneNumber
        };

        // Act
        var result = await Test.Target.LoginAsync(model, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.Equal(user.EmailAddress, result.Value.EmailAddress);
        Assert.Equal(accessToken, result.Value.Token);
        Assert.Equal(Test.AccessTokenExpiryDate, result.Value.ExpiryDate);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// fail to return an access token if the input
    /// phone number is invalid.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithPhoneNumber_FailsOnInvalidPhoneNumber()
    {
        // Arrange
        var model = new LoginModel(Password)
        {
            PhoneNumber = NotPhoneNumber
        };

        // Act
        var result = await Test.Target.LoginAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidPhoneNumber, model.PhoneNumber);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// fail to return an access token when no user
    /// with the specified phone number exists.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithPhoneNumber_FailsOnNonExistingUser()
    {
        // Arrange
        Test.SetupLoginFailureByMissingPhoneNumber(out var phoneNumber);

        var model = new LoginModel(Password)
        {
            PhoneNumber = phoneNumber
        };

        // Act
        var result = await Test.Target.LoginAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Unauthorized, result.Status);

        Assert.ContainsErrorCode(result, UserNotFoundOrPasswordMismatch);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// return an access token when user successfully
    /// logins with their username.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithUserName()
    {
        // Arrange
        Test.SetupLoginSuccessWithUserName(out var user, out var accessToken);

        var model = new LoginModel(Password)
        {
            UserName = user.UserName
        };

        // Act
        var result = await Test.Target.LoginAsync(model, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.Equal(user.EmailAddress, result.Value.EmailAddress);
        Assert.Equal(accessToken, result.Value.Token);
        Assert.Equal(Test.AccessTokenExpiryDate, result.Value.ExpiryDate);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// fail to return an access token if the input
    /// username is invalid.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithUserName_FailsOnInvalidUserName()
    {
        // Arrange
        var model = new LoginModel(Password)
        {
            UserName = BadUserName
        };

        // Act
        var result = await Test.Target.LoginAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidUserName, model.UserName);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// fail to return an access token when no user
    /// with the specified user name exists.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithUserName_FailsOnNonExistingUser()
    {
        // Arrange
        Test.SetupLoginFailureByMissingUserName(out var userName);

        var model = new LoginModel(Password)
        {
            UserName = userName
        };

        // Act
        var result = await Test.Target.LoginAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Unauthorized, result.Status);

        Assert.ContainsErrorCode(result, UserNotFoundOrPasswordMismatch);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// fail to return an access token if the input
    /// <see cref="LoginModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task LoginAsync_FailsOnInvalidModel()
    {
        // Arrange
        var model = null as LoginModel;

        // Act
        var result = await Test.Target.LoginAsync(model!, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidModel);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// fail to return an access token if the input
    /// password is equal to <see langword="null"/>, <see cref="string.Empty"/> or whitespace-only.
    /// </summary>
    [Theory, MemberData(nameof(LoginAsync_FailsOnMissingPassword_MemberData))]
    public async Task LoginAsync_FailsOnMissingPassword(string? password)
    {
        // Arrange
        var model = new LoginModel(password!);

        // Act
        var result = await Test.Target.LoginAsync(model!, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, PasswordMissing);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// fail to return an access token when user's password
    /// does not match the provided password value.
    /// </summary>
    [Fact]
    public async Task LoginAsync_FailsOnPasswordMismatch()
    {
        // Arrange
        Test.SetupLoginFailureByPasswordMismatch(out var emailAddress, out var password);

        var model = new LoginModel(password)
        {
            EmailAddress = emailAddress
        };

        // Act
        var result = await Test.Target.LoginAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Unauthorized, result.Status);

        Assert.ContainsErrorCode(result, UserNotFoundOrPasswordMismatch);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// return a two-factor authentication token and status <see cref="Received"/> when
    /// the two-factor authentication is enabled for the user.
    /// </summary>
    [Theory, MemberData(nameof(LoginAsync_ReturnsTwoFactorToken_MemberData))]
    public async Task LoginAsync_ReturnsTwoFactorToken(LoginModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        // Arrange
        Test.SetupLoginSuccessWithTwoFactorAuthentication(model, out var twoFactorToken);

        // Act
        var result = await Test.Target.LoginAsync(model, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Received, result.Status);

        Assert.Equal(twoFactorToken, result.Value.Token);
        Assert.Equal(Test.TwoFactorTokenExpiryDate, result.Value.ExpiryDate);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// fail to return an access token when the user's email address
    /// is not confirmed and it is required by the application.
    /// </summary>
    [Fact]
    public async Task LoginAsync_FailsOnUnconfirmedEmailAddress()
    {
        // Arrange
        Test.SetupLoginFailureByUnconfirmedEmailAddress(out var emailAddress);

        var model = new LoginModel(Password)
        {
            EmailAddress = emailAddress
        };

        // Act
        var result = await Test.Target.LoginAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Prohibited, result.Status);

        Assert.ContainsErrorCode(result, UserEmailAddressNotConfirmed);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// fail to return an access token if the input
    /// <see cref="LoginModel"/> does not have its' email address,
    /// phone number or user name values set.
    /// </summary>
    [Fact]
    public async Task LoginAsync_FailsOnIncompleteLoginModel()
    {
        // Arrange
        var model = new LoginModel(Password);

        // Act
        var result = await Test.Target.LoginAsync(model, Token);

        // Assert
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidModel);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// fail to return an access token if the
    /// inner <see cref="IUserRefreshTokenService"/> instance reports error
    /// upon creating and saving user refresh token data to the persistence storage.
    /// </summary>
    [Fact]
    public async Task LoginAsync_FailsOnDependencyFailure_UponRefreshTokenCreation()
    {
        // Arrange
        Test.SetupLoginFailureByUserRefreshTokenCreation(out var emailAddress);

        var model = new LoginModel(Password)
        {
            EmailAddress = emailAddress
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Test.Target.LoginAsync(model, Token));
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// fail to return an access token if the
    /// inner <see cref="IUserService"/> instance reports error
    /// upon retrieving the list of user's claims.
    /// </summary>
    [Fact]
    public async Task LoginAsync_FailsOnDependencyFailure_UponClaimsFetching()
    {
        // Arrange
        Test.SetupLoginFailureByClaimsFetching(out var emailAddress);

        var model = new LoginModel(Password)
        {
            EmailAddress = emailAddress
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Test.Target.LoginAsync(model, Token));
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LoginAsync"/> method should
    /// fail to return an access token if the
    /// inner <see cref="IRoleService"/> instance reports error
    /// upon retrieving the list of user's roles.
    /// </summary>
    [Fact]
    public async Task LoginAsync_FailsOnDependencyFailure_UponRolesFetching()
    {
        // Arrange
        Test.SetupLoginFailureByRolesFetching(out var emailAddress);

        var model = new LoginModel(Password)
        {
            EmailAddress = emailAddress
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Test.Target.LoginAsync(model, Token));
    }

    /// <summary>
    /// The <see cref="AuthenticationService.ConfirmLoginAsync"/> method should
    /// return an access token when user successfully
    /// passes two-factor authentication.
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync()
    {
        // Arrange
        Test.SetupConfirmLoginSuccess(out var emailAddress,
                                      out var twoFactorCode,
                                      out var identityToken,
                                      out var accessToken);

        var model = new TwoFactorAuthenticationModel(twoFactorCode, identityToken);

        // Act
        var result = await Test.Target.ConfirmLoginAsync(model, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.Equal(emailAddress, result.Value.EmailAddress);
        Assert.Equal(accessToken, result.Value.Token);
        Assert.Equal(Test.AccessTokenExpiryDate, result.Value.ExpiryDate);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.ConfirmLoginAsync"/> method should
    /// fail to return an access token if the input
    /// identity token is invalid.
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync_FailsOnInvalidToken()
    {
        // Arrange
        Test.SetupConfirmLoginFailureByInvalidIdentityToken(out var twoFactorCode, out var identityToken);

        var model = new TwoFactorAuthenticationModel(twoFactorCode, identityToken);

        // Act
        var result = await Test.Target.ConfirmLoginAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidToken);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.ConfirmLoginAsync"/> method should
    /// fail to return an access token if the input
    /// identity token is expired.
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync_FailsOnExpiredToken()
    {
        // Arrange
        Test.SetupConfirmLoginFailureByExpiredIdentityToken(out var twoFactorCode, out var identityToken);

        var model = new TwoFactorAuthenticationModel(twoFactorCode, identityToken);

        // Act
        var result = await Test.Target.ConfirmLoginAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Blocked, result.Status);

        Assert.ContainsErrorCode(result, OutdatedToken);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.ConfirmLoginAsync"/> method should
    /// fail to return an access token if the input
    /// two-factor code is equal to <see langword="null"/>, <see cref="string.Empty"/> or whitespace-only.
    /// </summary>
    /// <param name="twoFactorCode">
    /// The code to be used to confirm the user's login.
    /// </param>
    [Theory, MemberData(nameof(ConfirmLoginAsync_FailsOnEmptyTwoFactorCode_MemberData))]
    public async Task ConfirmLoginAsync_FailsOnEmptyTwoFactorCode(string? twoFactorCode)
    {
        // Arrange
        Test.SetupConfirmLoginFailureByInvalidTwoFactorCode(twoFactorCode, out var identityToken);

        var model = new TwoFactorAuthenticationModel(twoFactorCode!, identityToken);

        // Act
        var result = await Test.Target.ConfirmLoginAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidToken);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.ConfirmLoginAsync"/> method should
    /// fail to return an access token if the input
    /// two-factor code is incorrect.
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync_FailsOnIncorrectTwoFactorCode()
    {
        // Arrange
        Test.SetupConfirmLoginFailureByIncorrectTwoFactorCode(out var twoFactorCode,
                                                              out var identityToken);

        var model = new TwoFactorAuthenticationModel(twoFactorCode, identityToken);

        // Act
        var result = await Test.Target.ConfirmLoginAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Unauthorized, result.Status);

        Assert.ContainsErrorCode(result, UserUnauthorized);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.ConfirmLoginAsync"/> method should
    /// fail to return an access token if the
    /// user confirming the login does not exist anymore.
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync_FailsOnNonExistingUser()
    {
        // Arrange
        Test.SetupConfirmLoginFailureByMissingUser(out var twoFactorCode,
                                                   out var identityToken);

        var model = new TwoFactorAuthenticationModel(twoFactorCode, identityToken);

        // Act
        var result = await Test.Target.ConfirmLoginAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, UserEmailAddressNotFound);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.ConfirmLoginAsync"/> method should
    /// fail to return an access token if the
    /// inner <see cref="IUserRefreshTokenService"/> instance reports error
    /// upon creating and saving user refresh token data to the persistence storage.
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync_FailsOnDependencyFailure_UponRefreshTokenCreation()
    {
        // Arrange
        Test.SetupConfirmLoginFailureByUserRefreshTokenCreation(out var twoFactorCode,
                                                                out var identityToken);

        var model = new TwoFactorAuthenticationModel(twoFactorCode, identityToken);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Test.Target.ConfirmLoginAsync(model, Token));
    }

    /// <summary>
    /// The <see cref="AuthenticationService.ConfirmLoginAsync"/> method should
    /// fail to return an access token if the
    /// inner <see cref="IUserService"/> instance reports error
    /// upon retrieving the list of user's claims.
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync_FailsOnDependencyFailure_UponClaimsFetching()
    {
        // Arrange
        Test.SetupConfirmLoginFailureByClaimsFetching(out var twoFactorCode,
                                                      out var identityToken);

        var model = new TwoFactorAuthenticationModel(twoFactorCode, identityToken);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Test.Target.ConfirmLoginAsync(model, Token));
    }

    /// <summary>
    /// The <see cref="AuthenticationService.ConfirmLoginAsync"/> method should
    /// fail to return an access token if the
    /// inner <see cref="IRoleService"/> instance reports error
    /// upon retrieving the list of user's roles.
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync_FailsOnDependencyFailure_UponRolesFetching()
    {
        // Arrange
        Test.SetupConfirmLoginFailureByRolesFetching(out var twoFactorCode,
                                                     out var identityToken);

        var model = new TwoFactorAuthenticationModel(twoFactorCode, identityToken);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Test.Target.ConfirmLoginAsync(model, Token));
    }

    /// <summary>
    /// The <see cref="AuthenticationService.RenewTokenAsync"/> method should
    /// return an access token when user successfully
    /// refreshes their session using the refresh token
    /// stored in database.
    /// </summary>
    /// <param name="oldAccessToken">
    /// The JWT to be renewed.
    /// </param>
    [Theory, MemberData(nameof(RenewTokenAsync_WithPersistedRefreshToken_MemberData))]
    public async Task RenewTokenAsync_WithPersistedRefreshToken(string oldAccessToken)
    {
        // Arrange
        Test.SetupRenewTokenSuccessWithPersistedRefreshToken(out var accessToken);

        // Act
        var result = await Test.Target.RenewTokenAsync(oldAccessToken, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.Equal(accessToken, result.Value.Token);
        Assert.Equal(Test.AccessTokenExpiryDate, result.Value.ExpiryDate);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.RenewTokenAsync"/> method should
    /// fail to return an access token if the
    /// refresh token stored in the database is expired.
    /// </summary>
    [Fact]
    public async Task RenewTokenAsync_FailsOnExpiredPersistedRefreshToken()
    {
        // Arrange
        Test.SetupRenewTokenFailureByExpiredPersistedRefreshToken(out var accessToken);

        // Act
        var result = await Test.Target.RenewTokenAsync(accessToken, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Unauthorized, result.Status);

        Assert.ContainsErrorCode(result, OutdatedToken);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.RenewTokenAsync"/> method should
    /// fail to return an access token if the
    /// refresh token stored in the database is inactive.
    /// </summary>
    [Fact]
    public async Task RenewTokenAsync_FailsOnInactivePersistedRefreshToken()
    {
        // Arrange
        Test.SetupRenewTokenFailureByInactivePersistedRefreshToken(out var accessToken);

        // Act
        var result = await Test.Target.RenewTokenAsync(accessToken, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Unauthorized, result.Status);

        Assert.ContainsErrorCode(result, OutdatedToken);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.RenewTokenAsync"/> method should
    /// return an access token when user successfully
    /// refreshes their session using the refresh token
    /// stored in cache.
    /// </summary>
    [Theory, MemberData(nameof(RenewTokenAsync_WithCachedRefreshToken_MemberData))]
    public async Task RenewTokenAsync_WithCachedRefreshToken(string oldAccessToken)
    {
        // Arrange
        Test.SetupRenewTokenSuccessWithCachedRefreshToken(out var accessToken);

        // Act
        var result = await Test.Target.RenewTokenAsync(oldAccessToken, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.Equal(accessToken, result.Value.Token);
        Assert.Equal(Test.AccessTokenExpiryDate, result.Value.ExpiryDate);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.RenewTokenAsync"/> method should
    /// fail to return an access token if the
    /// refresh token is revoked in the cache.
    /// </summary>
    [Fact]
    public async Task RenewTokenAsync_FailsOnRevokedCachedRefreshToken()
    {
        // Arrange
        Test.SetupRenewTokenFailureByRevokedCachedRefreshToken();

        // Act
        var result = await Test.Target.RenewTokenAsync(AccessToken, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Unauthorized, result.Status);

        Assert.ContainsErrorCode(result, OutdatedToken);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.RenewTokenAsync"/> method should
    /// fail to return an access token if
    /// no refresh token data is present in the database or cache.
    /// </summary>
    [Fact]
    public async Task RenewTokenAsync_FailsOnMissingRefreshToken()
    {
        // Arrange
        Test.SetupRenewTokenFailureByMissingRefreshToken();

        // Act
        var result = await Test.Target.RenewTokenAsync(AccessToken, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Unauthorized, result.Status);

        Assert.ContainsErrorCode(result, OutdatedToken);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.RenewTokenAsync"/> method should
    /// fail to return an access token if the input
    /// access token is invalid.
    /// </summary>
    [Fact]
    public async Task RenewTokenAsync_FailsOnInvalidToken()
    {
        // Arrange
        Test.SetupRenewTokenFailureByInvalidAccessToken(out var accessToken);

        // Act
        var result = await Test.Target.RenewTokenAsync(accessToken, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidToken);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.RenewTokenAsync"/> method should
    /// fail to return an access token if the input
    /// access token does not contain a refresh token identifier.
    /// </summary>
    [Fact]
    public async Task RenewTokenAsync_FailsOnMissingRefreshTokenId()
    {
        // Arrange
        Test.SetupRenewTokenFailureByMissingRefreshTokenId(out var accessToken);

        // Act
        var result = await Test.Target.RenewTokenAsync(accessToken, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidToken);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.RenewTokenAsync"/> method should
    /// fail to return an access token if the input
    /// access token is linked to a non-existing user.
    /// </summary>
    [Fact]
    public async Task RenewTokenAsync_FailsOnNonExistingUser()
    {
        // Arrange
        Test.SetupRenewTokenFailureByMissingUser(out var accessToken);

        // Act
        var result = await Test.Target.RenewTokenAsync(accessToken, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, UserIdNotFound);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LogoutAsync"/> method should
    /// invalidate the refresh token associated to the input access token.
    /// </summary>
    [Theory, MemberData(nameof(LogoutAsync_MemberData))]
    public async Task LogoutAsync(string accessToken)
    {
        // Arrange
        Test.SetupLogoutSuccess(out var context);

        // Act
        var result = await Test.Target.LogoutAsync(accessToken, Token);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(context.DatabaseCleared);
        Assert.True(context.CacheCleared);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LogoutAsync"/> method should
    /// fail to invalidate the refresh token if the input
    /// access token is invalid.
    /// </summary>
    [Fact]
    public async Task LogoutAsync_FailsOnInvalidToken()
    {
        // Arrange
        Test.SetupLogoutFailureByInvalidAccessToken(out var accessToken);

        // Act
        var result = await Test.Target.LogoutAsync(accessToken, Token);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidToken);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.LogoutAsync"/> method should
    /// fail to invalidate the refresh token if the input
    /// access token does not contain a refresh token Id claim.
    /// </summary>
    [Fact]
    public async Task LogoutAsync_FailsOnMissingRefreshTokenId()
    {
        // Arrange
        Test.SetupLogoutFailureByMissingRefreshTokenId(out var accessToken);

        // Act
        var result = await Test.Target.LogoutAsync(accessToken, Token);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidToken);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.TerminateSessionsAsync"/> method should
    /// invalidate all refresh tokens associated with the user
    /// who is linked to the input access token.
    /// </summary>
    [Theory, MemberData(nameof(TerminateSessionsAsync_MemberData))]
    public async Task TerminateSessionsAsync(string accessToken)
    {
        // Arrange
        Test.SetupTerminateSessionsSuccess(out var context);

        // Act
        var result = await Test.Target.TerminateSessionsAsync(accessToken, Token);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(context.DatabaseCleared);
        Assert.True(context.CacheCleared);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.TerminateSessionsAsync"/> method should
    /// fail to invalidate all refresh tokens if the input
    /// access token is invalid.
    /// </summary>
    [Fact]
    public async Task TerminateSessionsAsync_FailsOnInvalidToken()
    {
        // Arrange
        Test.SetupTerminateSessionsFailureByInvalidAccessToken(out var accessToken);

        // Act
        var result = await Test.Target.TerminateSessionsAsync(accessToken, Token);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidToken);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.CheckSessionAsync"/> method should
    /// return a successful persisted refresh token validation result.
    /// </summary>
    [Fact]
    public async Task CheckSessionAsync_WithPersistedRefreshToken()
    {
        // Arrange
        Test.SetupCheckSessionSuccessWithPersistedRefreshToken(out var principal);

        // Act
        var result = await Test.Target.CheckSessionAsync(principal, Token);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Success, result.Status);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.CheckSessionAsync"/> method should
    /// return an unsuccessful persisted refresh token validation result if the
    /// target refresh token is expired.
    /// </summary>
    [Fact]
    public async Task CheckSessionAsync_FailsOnExpiredPersistedRefreshToken()
    {
        // Arrange
        Test.SetupCheckSessionFailureByExpiredPersistedRefreshToken(out var principal);

        // Act
        var result = await Test.Target.CheckSessionAsync(principal, Token);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Unauthorized, result.Status);

        Assert.ContainsErrorCode(result, OutdatedToken);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.CheckSessionAsync"/> method should
    /// return an unsuccessful persisted refresh token validation result if the
    /// target refresh token is inactive.
    /// </summary>
    [Fact]
    public async Task CheckSessionAsync_FailsOnInactivePersistedRefreshToken()
    {
        // Arrange
        Test.SetupCheckSessionFailureByInactivePersistedRefreshToken(out var principal);

        // Act
        var result = await Test.Target.CheckSessionAsync(principal, Token);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Unauthorized, result.Status);

        Assert.ContainsErrorCode(result, OutdatedToken);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.CheckSessionAsync"/> method should
    /// return a successful cached refresh token validation result.
    /// </summary>
    [Fact]
    public async Task CheckSessionAsync_WithCachedRefreshToken()
    {
        // Arrange
        Test.SetupCheckSessionSuccessWithCachedRefreshToken(out var principal);

        // Act
        var result = await Test.Target.CheckSessionAsync(principal, Token);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Success, result.Status);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.CheckSessionAsync"/> method should
    /// return an unsuccessful cached refresh token validation result if the
    /// target refresh token is revoked.
    /// </summary>
    [Fact]
    public async Task CheckSessionAsync_FailsOnRevokedCachedRefreshToken()
    {
        // Arrange
        Test.SetupCheckSessionFailureByRevokedCachedRefreshToken(out var principal);

        // Act
        var result = await Test.Target.CheckSessionAsync(principal, Token);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Unauthorized, result.Status);

        Assert.ContainsErrorCode(result, OutdatedToken);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.CheckSessionAsync"/> method should
    /// return an unsuccessful cached refresh token validation result if
    /// no refresh token data is present in the database or cache.
    /// </summary>
    [Fact]
    public async Task CheckSessionAsync_FailsOnMissingRefreshToken()
    {
        // Arrange
        Test.SetupCheckSessionFailureByMissingRefreshToken(out var principal);

        // Act
        var result = await Test.Target.CheckSessionAsync(principal, Token);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Unauthorized, result.Status);

        Assert.ContainsErrorCode(result, OutdatedToken);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.CheckSessionAsync"/> method should
    /// return an unsuccessful refresh token validation result if the input
    /// <see cref="ClaimsPrincipal"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task CheckSessionAsync_FailsOnNullPrincipal()
    {
        // Arrange
        var principal = null as ClaimsPrincipal;

        // Act
        var result = await Test.Target.CheckSessionAsync(principal, Token);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Unauthorized, result.Status);

        Assert.ContainsErrorCode(result, InvalidToken);
    }

    /// <summary>
    /// The <see cref="AuthenticationService.CheckSessionAsync"/> method should
    /// return an unsuccessful refresh token validation result if the input
    /// <see cref="ClaimsPrincipal"/> does not contain a refresh token identifier.
    /// </summary>
    [Fact]
    public async Task CheckSessionAsync_FailsOnMissingRefreshTokenId()
    {
        // Arrange
        Test.SetupCheckSessionFailureByMissingRefreshTokenId(out var principal);

        // Act
        var result = await Test.Target.CheckSessionAsync(principal, Token);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Unauthorized, result.Status);

        Assert.ContainsErrorCode(result, InvalidToken);
    }
    #endregion
}