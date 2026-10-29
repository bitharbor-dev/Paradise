using Microsoft.AspNetCore.Authentication.JwtBearer;
using Paradise.ApplicationLogic.Infrastructure.Domain.Events.Identity;
using Paradise.Models;
using Paradise.Models.WebApi.Services.Authentication;
using Paradise.Tests.Extensibility;
using System.Net;
using System.Net.Http.Json;
using static Paradise.Primitives.Web.AuthenticationRoutes;

namespace Paradise.WebApi.Tests.Integration.Endpoints.Handlers.Infrastructure;

/// <summary>
/// Authentication handlers test class.
/// </summary>
public sealed class AuthenticationHandlersTests : EndpointHandlersTests
{
    #region Constants
    private const string DefaultEmailAddress = "test@email.com";
    private const string DefaultUserName = "UserName";
    private const string DefaultPhoneNumber = "+100000000000";
    private const string DefaultTwoFactorCode = "123456";

    private const string Password = "Password123!";

    private const string NotEmailAddress = "Not an email address";
    private const string NotPhoneNumber = "Not a phone number";
    private const string BadUserName = "!@#$%^&*() Bad user name";
    private const string InvalidToken = "InvalidToken";

    private const string BearerScheme = JwtBearerDefaults.AuthenticationScheme;
    #endregion

    #region Properties
    /// <summary>
    /// Provides member data for the <see cref="LoginAsync_FailsOnMissingPassword"/> method.
    /// </summary>
    public static TheoryData<string?> LoginAsync_FailsOnMissingPassword_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };

    /// <summary>
    /// Provides member data for the <see cref="ConfirmLoginAsync_FailsOnEmptyTwoFactorCode"/> method.
    /// </summary>
    public static TheoryData<string?> ConfirmLoginAsync_FailsOnEmptyTwoFactorCode_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The "login" endpoint should
    /// return an access token
    /// when the user exists and the password is correct.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithEmailAddress()
    {
        // Arrange
        var model = new LoginModel(Password)
        {
            EmailAddress = DefaultEmailAddress
        };

        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password);

        // Act
        using var response = await Client.PostAsJsonAsync(Prefix + Login, model, Token);

        // Assert
        var accessToken = await Assert.BodyAsync<AccessTokenModel>(response, HttpStatusCode.OK, Token);

        Assert.NotEmpty(accessToken.Token);
    }

    /// <summary>
    /// The "login" endpoint should
    /// return a bad request status
    /// when the email address is invalid.
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
        using var response = await Client.PostAsJsonAsync(Prefix + Login, model, Token);

        // Assert
        await Assert.ProblemAsync(response, HttpStatusCode.BadRequest, ErrorCode.InvalidModel, Token);
    }

    /// <summary>
    /// The "login" endpoint should
    /// return an unauthorized status
    /// when the user does not exist.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithEmailAddress_FailsOnNonExistingUser()
    {
        // Arrange
        var model = new LoginModel(Password)
        {
            EmailAddress = DefaultEmailAddress
        };

        // Act
        using var response = await Client.PostAsJsonAsync(Prefix + Login, model, Token);

        // Assert
        await Assert.ProblemAsync(response, HttpStatusCode.Unauthorized, ErrorCode.UserNotFoundOrPasswordMismatch, Token);
    }

    /// <summary>
    /// The "login" endpoint should
    /// return an access token
    /// when the user exists and the password is correct.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithPhoneNumber()
    {
        // Arrange
        var model = new LoginModel(Password)
        {
            PhoneNumber = DefaultPhoneNumber
        };

        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password, DefaultPhoneNumber);

        // Act
        using var response = await Client.PostAsJsonAsync(Prefix + Login, model, Token);

        // Assert
        var accessToken = await Assert.BodyAsync<AccessTokenModel>(response, HttpStatusCode.OK, Token);

        Assert.NotEmpty(accessToken.Token);
    }

    /// <summary>
    /// The "login" endpoint should
    /// return a bad request status
    /// when the phone number is invalid.
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
        using var response = await Client.PostAsJsonAsync(Prefix + Login, model, Token);

        // Assert
        await Assert.ProblemAsync(response, HttpStatusCode.BadRequest, ErrorCode.InvalidModel, Token);
    }

    /// <summary>
    /// The "login" endpoint should
    /// return an unauthorized status
    /// when the user does not exist.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithPhoneNumber_FailsOnNonExistingUser()
    {
        // Arrange
        var model = new LoginModel(Password)
        {
            PhoneNumber = DefaultPhoneNumber
        };

        // Act
        using var response = await Client.PostAsJsonAsync(Prefix + Login, model, Token);

        // Assert
        await Assert.ProblemAsync(response, HttpStatusCode.Unauthorized, ErrorCode.UserNotFoundOrPasswordMismatch, Token);
    }

    /// <summary>
    /// The "login" endpoint should
    /// return an access token
    /// when the user exists and the password is correct.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithUserName()
    {
        // Arrange
        var model = new LoginModel(Password)
        {
            UserName = DefaultUserName
        };

        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password);

        // Act
        using var response = await Client.PostAsJsonAsync(Prefix + Login, model, Token);

        // Assert
        var accessToken = await Assert.BodyAsync<AccessTokenModel>(response, HttpStatusCode.OK, Token);

        Assert.NotEmpty(accessToken.Token);
    }

    /// <summary>
    /// The "login" endpoint should
    /// return a bad request status
    /// when the user-name is invalid.
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
        using var response = await Client.PostAsJsonAsync(Prefix + Login, model, Token);

        // Assert
        await Assert.ProblemAsync(response, HttpStatusCode.BadRequest, ErrorCode.InvalidUserName, Token);
    }

    /// <summary>
    /// The "login" endpoint should
    /// return an unauthorized status
    /// when the user does not exist.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithUserName_FailsOnNonExistingUser()
    {
        // Arrange
        var model = new LoginModel(Password)
        {
            UserName = DefaultUserName
        };

        // Act
        using var response = await Client.PostAsJsonAsync(Prefix + Login, model, Token);

        // Assert
        await Assert.ProblemAsync(response, HttpStatusCode.Unauthorized, ErrorCode.UserNotFoundOrPasswordMismatch, Token);
    }

    /// <summary>
    /// The "login" endpoint should
    /// return a bad request status
    /// when the input model is invalid.
    /// </summary>
    [Fact]
    public async Task LoginAsync_FailsOnInvalidModel()
    {
        // Arrange
        var model = null as LoginModel;

        // Act
        using var response = await Client.PostAsJsonAsync(Prefix + Login, model, Token);

        // Assert
        await Assert.ProblemAsync(response, HttpStatusCode.BadRequest, ErrorCode.InvalidModel, Token);
    }

    /// <summary>
    /// The "login" endpoint should
    /// return a bad request status
    /// when the password is not specified.
    /// </summary>
    [Theory, MemberData(nameof(LoginAsync_FailsOnMissingPassword_MemberData))]
    public async Task LoginAsync_FailsOnMissingPassword(string? password)
    {
        // Arrange
        var model = new LoginModel(password!);

        // Act
        using var response = await Client.PostAsJsonAsync(Prefix + Login, model, Token);

        // Assert
        await Assert.ProblemAsync(response, HttpStatusCode.BadRequest, ErrorCode.InvalidModel, Token);
    }

    /// <summary>
    /// The "login" endpoint should
    /// return an unauthorized status
    /// when the user exists but the password is incorrect.
    /// </summary>
    [Fact]
    public async Task LoginAsync_FailsOnPasswordMismatch()
    {
        // Arrange
        var model = new LoginModel(Password)
        {
            UserName = DefaultUserName,
            EmailAddress = DefaultEmailAddress
        };

        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password + "Incorrect");

        // Act
        using var response = await Client.PostAsJsonAsync(Prefix + Login, model, Token);

        // Assert
        await Assert.ProblemAsync(response, HttpStatusCode.Unauthorized, ErrorCode.UserNotFoundOrPasswordMismatch, Token);
    }

    /// <summary>
    /// The "login" endpoint should
    /// return a 2FA token and accepted status
    /// when the user has 2FA enabled.
    /// </summary>
    [Fact]
    public async Task LoginAsync_ReturnsTwoFactorToken()
    {
        // Arrange
        var model = new LoginModel(Password)
        {
            EmailAddress = DefaultEmailAddress
        };

        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password, twoFactorEnabled: true);

        // Act
        using var response = await Client.PostAsJsonAsync(Prefix + Login, model, Token);

        // Assert
        var twoFactorToken = await Assert.BodyAsync<AccessTokenModel>(response, HttpStatusCode.Accepted, Token);

        Assert.NotEmpty(twoFactorToken.Token);
    }

    /// <summary>
    /// The "login" endpoint should
    /// return a forbidden status
    /// when the user did not confirm their email address.
    /// </summary>
    [Fact]
    public async Task LoginAsync_FailsOnUnconfirmedEmailAddress()
    {
        // Arrange
        var model = new LoginModel(Password)
        {
            EmailAddress = DefaultEmailAddress
        };

        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password, isEmailAddressConfirmed: false);

        // Act
        using var response = await Client.PostAsJsonAsync(Prefix + Login, model, Token);

        // Assert
        await Assert.ProblemAsync(response, HttpStatusCode.Forbidden, ErrorCode.UserEmailAddressNotConfirmed, Token);
    }

    /// <summary>
    /// The "login" endpoint should
    /// return a bad request status
    /// when none of the following properties
    /// had their values set: EmailAddress, UserName, PhoneNumber.
    /// </summary>
    [Fact]
    public async Task LoginAsync_FailsOnIncompleteLoginModel()
    {
        // Arrange
        var model = new LoginModel(Password);

        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password);

        // Act
        using var response = await Client.PostAsJsonAsync(Prefix + Login, model, Token);

        // Assert
        await Assert.ProblemAsync(response, HttpStatusCode.BadRequest, ErrorCode.InvalidModel, Token);
    }

    /// <summary>
    /// The "confirm login" endpoint should
    /// return an access token
    /// when the user successfully passes two-factor authentication.
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync()
    {
        // Arrange
        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password, twoFactorEnabled: true);

        var twoFactorToken = await Application.AuthenticateUserAsync(DefaultUserName, Password, Token);
        var twoFactorAuthenticationEvent = await Application.WaitForEventAsync<TwoFactorAuthenticationOccurringEvent>(Token);

        var model = new TwoFactorAuthenticationModel(twoFactorAuthenticationEvent.VerificationCode, twoFactorToken.Token);

        // Act
        using var response = await Client.PutAsJsonAsync(Prefix + ConfirmLogin, model, Token);

        // Assert
        var accessToken = await Assert.BodyAsync<AccessTokenModel>(response, HttpStatusCode.OK, Token);

        Assert.NotEmpty(accessToken.Token);
        Assert.Equal(DefaultEmailAddress, accessToken.EmailAddress);
    }

    /// <summary>
    /// The "confirm login" endpoint should
    /// return a bad request status
    /// when the identity token is invalid.
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync_FailsOnInvalidToken()
    {
        // Arrange
        var model = new TwoFactorAuthenticationModel(DefaultTwoFactorCode, InvalidToken);

        // Act
        using var response = await Client.PutAsJsonAsync(Prefix + ConfirmLogin, model, Token);

        // Assert
        await Assert.ProblemAsync(response, HttpStatusCode.BadRequest, ErrorCode.InvalidToken, Token);
    }

    /// <summary>
    /// The "confirm login" endpoint should
    /// return an unprocessable entity status
    /// when the identity token is expired.
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync_FailsOnExpiredToken()
    {
        // Arrange
        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password, twoFactorEnabled: true);

        var twoFactorToken = await Application.AuthenticateUserAsync(DefaultUserName, Password, Token);
        var twoFactorAuthenticationEvent = await Application.WaitForEventAsync<TwoFactorAuthenticationOccurringEvent>(Token);

        var model = new TwoFactorAuthenticationModel(twoFactorAuthenticationEvent.VerificationCode, twoFactorToken.Token);

        Application.UtcNow += Application.AuthenticationOptions.TwoFactorTokenLifetime + TimeSpan.FromSeconds(1);

        // Act
        using var response = await Client.PutAsJsonAsync(Prefix + ConfirmLogin, model, Token);

        // Assert
        await Assert.ProblemAsync(response, HttpStatusCode.UnprocessableEntity, ErrorCode.OutdatedToken, Token);
    }

    /// <summary>
    /// The "confirm login" endpoint should
    /// return a bad request status
    /// when the two-factor code is equal to <see langword="null"/>, <see cref="string.Empty"/> or whitespace-only.
    /// </summary>
    /// <param name="twoFactorCode">
    /// The code to be used to confirm the user's login.
    /// </param>
    [Theory, MemberData(nameof(ConfirmLoginAsync_FailsOnEmptyTwoFactorCode_MemberData))]
    public async Task ConfirmLoginAsync_FailsOnEmptyTwoFactorCode(string? twoFactorCode)
    {
        // Arrange
        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password, twoFactorEnabled: true);

        var twoFactorToken = await Application.AuthenticateUserAsync(DefaultUserName, Password, Token);

        var model = new TwoFactorAuthenticationModel(twoFactorCode!, twoFactorToken.Token);

        // Act
        using var response = await Client.PutAsJsonAsync(Prefix + ConfirmLogin, model, Token);

        // Assert
        await Assert.ProblemAsync(response, HttpStatusCode.BadRequest, ErrorCode.InvalidToken, Token);
    }

    /// <summary>
    /// The "confirm login" endpoint should
    /// return an unauthorized status
    /// when the two-factor code is incorrect.
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync_FailsOnIncorrectTwoFactorCode()
    {
        // Arrange
        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password, twoFactorEnabled: true);

        var twoFactorToken = await Application.AuthenticateUserAsync(DefaultUserName, Password, Token);
        var twoFactorAuthenticationEvent = await Application.WaitForEventAsync<TwoFactorAuthenticationOccurringEvent>(Token);

        var incorrectTwoFactorCode = GetIncorrectTwoFactorCode(twoFactorAuthenticationEvent.VerificationCode);

        var model = new TwoFactorAuthenticationModel(incorrectTwoFactorCode, twoFactorToken.Token);

        // Act
        using var response = await Client.PutAsJsonAsync(Prefix + ConfirmLogin, model, Token);

        // Assert
        await Assert.ProblemAsync(response, HttpStatusCode.Unauthorized, ErrorCode.UserUnauthorized, Token);
    }

    /// <summary>
    /// The "confirm login" endpoint should
    /// return a not found status
    /// when the user confirming the login does not exist anymore.
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync_FailsOnNonExistingUser()
    {
        // Arrange
        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password, twoFactorEnabled: true);

        var twoFactorToken = await Application.AuthenticateUserAsync(DefaultUserName, Password, Token);
        var twoFactorAuthenticationEvent = await Application.WaitForEventAsync<TwoFactorAuthenticationOccurringEvent>(Token);

        var model = new TwoFactorAuthenticationModel(twoFactorAuthenticationEvent.VerificationCode, twoFactorToken.Token);

        await Application.DeleteUserAsync(DefaultUserName);

        // Act
        using var response = await Client.PutAsJsonAsync(Prefix + ConfirmLogin, model, Token);

        // Assert
        await Assert.ProblemAsync(response, HttpStatusCode.NotFound, ErrorCode.UserEmailAddressNotFound, Token);
    }

    /// <summary>
    /// The "renew token" endpoint should
    /// return an access token
    /// when the user's session is active.
    /// </summary>
    [Fact]
    public async Task RenewTokenAsync()
    {
        // Arrange
        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password);

        var oldAccessToken = await Application.AuthenticateUserAsync(DefaultUserName, Password, Token);

        Client.DefaultRequestHeaders.Authorization = new(BearerScheme, oldAccessToken.Token);

        // Act
        using var response = await Client.GetAsync(Prefix + RenewToken, Token);

        // Assert
        var accessToken = await Assert.BodyAsync<AccessTokenModel>(response, HttpStatusCode.OK, Token);

        Assert.NotEmpty(accessToken.Token);
        Assert.Equal(DefaultEmailAddress, accessToken.EmailAddress);
    }

    /// <summary>
    /// The "renew token" endpoint should
    /// return an unauthorized status
    /// when the refresh token is expired.
    /// </summary>
    [Fact]
    public async Task RenewTokenAsync_FailsOnExpiredRefreshToken()
    {
        // Arrange
        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password);

        var accessToken = await Application.AuthenticateUserAsync(DefaultUserName, Password, Token);

        Client.DefaultRequestHeaders.Authorization = new(BearerScheme, accessToken.Token);

        Application.UtcNow += Application.AuthenticationOptions.RefreshTokenLifetime + TimeSpan.FromSeconds(1);

        // Act
        using var response = await Client.GetAsync(Prefix + RenewToken, Token);

        // Assert
        Assert.StatusCode(response, HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// The "renew token" endpoint should
    /// return an unauthorized status
    /// when the refresh token is revoked.
    /// </summary>
    [Fact]
    public async Task RenewTokenAsync_FailsOnRevokedRefreshToken()
    {
        // Arrange
        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password);

        var accessToken = await Application.AuthenticateUserAsync(DefaultUserName, Password, Token);

        Client.DefaultRequestHeaders.Authorization = new(BearerScheme, accessToken.Token);

        using var logoutResponse = await Client.DeleteAsync(Prefix + Logout, Token);

        logoutResponse.EnsureSuccessStatusCode();

        // Act
        using var response = await Client.GetAsync(Prefix + RenewToken, Token);

        // Assert
        Assert.StatusCode(response, HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// The "renew token" endpoint should
    /// return an unauthorized status
    /// when no refresh token data is present in the database or cache.
    /// </summary>
    [Fact]
    public async Task RenewTokenAsync_FailsOnMissingRefreshToken()
    {
        // Arrange
        var accessToken = await Application.IssueAccessTokenAsync(Guid.NewGuid());

        Client.DefaultRequestHeaders.Authorization = new(BearerScheme, accessToken);

        // Act
        using var response = await Client.GetAsync(Prefix + RenewToken, Token);

        // Assert
        Assert.StatusCode(response, HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// The "renew token" endpoint should
    /// return an unauthorized status
    /// when the access token is invalid.
    /// </summary>
    [Fact]
    public async Task RenewTokenAsync_FailsOnInvalidToken()
    {
        // Arrange
        Client.DefaultRequestHeaders.Authorization = new(BearerScheme, InvalidToken);

        // Act
        using var response = await Client.GetAsync(Prefix + RenewToken, Token);

        // Assert
        Assert.StatusCode(response, HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// The "renew token" endpoint should
    /// return a not found status
    /// when the access token is linked to a non-existing user.
    /// </summary>
    [Fact]
    public async Task RenewTokenAsync_FailsOnNonExistingUser()
    {
        // Arrange
        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password);

        var accessToken = await Application.AuthenticateUserAsync(DefaultUserName, Password, Token);

        Client.DefaultRequestHeaders.Authorization = new(BearerScheme, accessToken.Token);

        await Application.DeleteUserAsync(DefaultUserName);

        // Act
        using var response = await Client.GetAsync(Prefix + RenewToken, Token);

        // Assert
        await Assert.ProblemAsync(response, HttpStatusCode.NotFound, ErrorCode.UserIdNotFound, Token);
    }

    /// <summary>
    /// The "logout" endpoint should
    /// invalidate the refresh token associated
    /// with the input access token.
    /// </summary>
    [Fact]
    public async Task LogoutAsync()
    {
        // Arrange
        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password);

        var accessToken = await Application.AuthenticateUserAsync(DefaultUserName, Password, Token);

        Client.DefaultRequestHeaders.Authorization = new(BearerScheme, accessToken.Token);

        // Act
        using var response = await Client.DeleteAsync(Prefix + Logout, Token);

        // Assert
        Assert.StatusCode(response, HttpStatusCode.OK);

        using var renewResponse = await Client.GetAsync(Prefix + RenewToken, Token);

        Assert.StatusCode(renewResponse, HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// The "logout" endpoint should
    /// return an unauthorized status
    /// when the access token is invalid.
    /// </summary>
    [Fact]
    public async Task LogoutAsync_FailsOnInvalidToken()
    {
        // Arrange
        Client.DefaultRequestHeaders.Authorization = new(BearerScheme, InvalidToken);

        // Act
        using var response = await Client.DeleteAsync(Prefix + Logout, Token);

        // Assert
        Assert.StatusCode(response, HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// The "terminate sessions" endpoint should
    /// invalidate all refresh tokens associated with the user
    /// who is linked to the input access token.
    /// </summary>
    [Fact]
    public async Task TerminateSessionsAsync()
    {
        // Arrange
        await Application.AddUserAsync(DefaultUserName, DefaultEmailAddress, Password);

        var firstAccessToken = await Application.AuthenticateUserAsync(DefaultUserName, Password, Token);
        var secondAccessToken = await Application.AuthenticateUserAsync(DefaultUserName, Password, Token);

        Client.DefaultRequestHeaders.Authorization = new(BearerScheme, firstAccessToken.Token);

        // Act
        using var response = await Client.DeleteAsync(Prefix + TerminateSessions, Token);

        // Assert
        Assert.StatusCode(response, HttpStatusCode.OK);

        using var firstRenewResponse = await Client.GetAsync(Prefix + RenewToken, Token);

        Client.DefaultRequestHeaders.Authorization = new(BearerScheme, secondAccessToken.Token);

        using var secondRenewResponse = await Client.GetAsync(Prefix + RenewToken, Token);

        Assert.StatusCode(firstRenewResponse, HttpStatusCode.Unauthorized);
        Assert.StatusCode(secondRenewResponse, HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// The "terminate sessions" endpoint should
    /// return an unauthorized status
    /// when the access token is invalid.
    /// </summary>
    [Fact]
    public async Task TerminateSessionsAsync_FailsOnInvalidToken()
    {
        // Arrange
        Client.DefaultRequestHeaders.Authorization = new(BearerScheme, InvalidToken);

        // Act
        using var response = await Client.DeleteAsync(Prefix + TerminateSessions, Token);

        // Assert
        Assert.StatusCode(response, HttpStatusCode.Unauthorized);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Gets a two-factor code which is guaranteed to differ
    /// from the given <paramref name="verificationCode"/>.
    /// </summary>
    /// <param name="verificationCode">
    /// The issued two-factor verification code.
    /// </param>
    /// <returns>
    /// A two-factor code of the same length, where each digit
    /// of the <paramref name="verificationCode"/> is shifted by one.
    /// </returns>
    private static string GetIncorrectTwoFactorCode(string verificationCode)
        => string.Concat(verificationCode.Select(digit => (char)('0' + ((digit - '0' + 1) % 10))));
    #endregion
}