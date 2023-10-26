using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Paradise.ApplicationLogic.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Identity;
using Paradise.ApplicationLogic.Services.Application;
using Paradise.DataAccess.Repositories;
using Paradise.DataAccess.Repositories.Application.Implementation;
using Paradise.DataAccess.Repositories.Domain.Implementation;
using Paradise.Domain.Users;
using Paradise.Models.Domain.UserModels;
using Paradise.Options.Models;
using Paradise.Options.Models.Communication;
using System.Globalization;

namespace Paradise.ApplicationLogic.Tests.Services.Domain;

/// <summary>
/// Test class for the <see cref="UserService"/> methods.
/// </summary>
public sealed class UserServiceTests
{
    #region Constants
    /// <summary>
    /// Invalid test JWT.
    /// </summary>
    private const string InvalidAccessToken = "Invalid access token.";
    /// <summary>
    /// Invalid test identity token.
    /// </summary>
    private const string InvalidIdentityToken = "Invalid identity token.";
    /// <summary>
    /// Invalid test email address.
    /// </summary>
    private const string InvalidEmailAddress = "Invalid email address.";
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="UserServiceTests"/> class.
    /// </summary>
    /// <param name="output">
    /// Xunit output helper.
    /// </param>
    public UserServiceTests(ITestOutputHelper output)
    {
        var secret = "Dh1VOA2wGqGWrcyrxYDsvB9E3EPKUI7g";

        var logger = GetLogger<UserService>(output);
        var applicationOptions = GetApplicationOptions(secret);
        var jwtBearerOptions = GetJwtBearerOptions(secret);
        var emailTemplateOptions = GetEmailTemplateOptions();
        var identityOptions = GetIdentityOptions();
        var smtpOptions = GetSmtpOptions();

        var applicationDataSource = GetApplicationDataSource();
        var domainDataSource = GetDomainDataSource();

        var userManager = GetUserManager(output, domainDataSource, identityOptions);
        var roleManager = GetRoleManager(output, domainDataSource);

        var smtpClient = GetSmtpClient();

        var emailTemplatesRepository = new EmailTemplatesRepository(applicationDataSource);
        var userRefreshTokensRepository = new UserRefreshTokensRepository(domainDataSource);

        var communicationService = new CommunicationService(smtpOptions, smtpClient, emailTemplatesRepository);
        var jsonWebTokenService = new JsonWebTokenService(applicationOptions, jwtBearerOptions);
        var roleService = new RoleService(userManager, roleManager);
        var dataProtectionProvider = DataProtectionProvider.Create(nameof(Paradise));
        var dataProtectionService = new DataProtectionService(dataProtectionProvider);

        Options = applicationOptions.Value;

        TemplateOptions = emailTemplateOptions.Value;

        ApplicationDataSource = applicationDataSource;

        CommunicationService = communicationService;

        Manager = userManager;

        Service = new(logger,
                      applicationOptions,
                      emailTemplateOptions,
                      identityOptions,
                      userManager,
                      userRefreshTokensRepository,
                      roleService,
                      communicationService,
                      jsonWebTokenService,
                      dataProtectionService);

        CommunicationService.EmailMessageSent += (s, e) => SentEmailsCache.Add(e);
    }
    #endregion

    #region Properties
    /// <summary>
    /// Application options.
    /// </summary>
    public ApplicationOptions Options { get; }

    /// <summary>
    /// Email templates options.
    /// </summary>
    public EmailTemplateOptions TemplateOptions { get; }

    /// <summary>
    /// Application data source.
    /// </summary>
    public IApplicationDataSource ApplicationDataSource { get; }

    /// <summary>
    /// Communication service.
    /// </summary>
    public CommunicationService CommunicationService { get; }

    /// <summary>
    /// A <see cref="UserManager"/> instance used to
    /// arrange data and validate test results.
    /// </summary>
    public UserManager Manager { get; }

    /// <summary>
    /// A <see cref="UserService"/> instance to be tested.
    /// </summary>
    public UserService Service { get; }

    /// <summary>
    /// All sent emails will appear in this list during test methods execution.
    /// </summary>
    private List<EmailMessageSentEventArgs> SentEmailsCache { get; } = [];
    #endregion

    #region Public methods
    /// <summary>
    /// <see cref="UserService.ConfirmEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Marks the user's email address as confirmed,
    /// which allows further system usage.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task ConfirmEmailAsync()
    {
        // Arrange
        await CreateEmailAddressConfirmationTemplateAsync();

        await Service.RegisterAsync(new("test@email.com", "123Qwe!@", "123Qwe!@", "Test"));

        var confirmationUri = GetEmailBodyParameterOfType<Uri>();

        var identityToken = confirmationUri.Segments.Last();

        // Act
        var result = await Service.ConfirmEmailAsync(identityToken);

        // Assert
        result.AssertSuccess(OK);
    }

    /// <summary>
    /// <see cref="UserService.ConfirmEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the email address
    /// was already confirmed.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ConfirmEmailAsync_ThrowsOnAlreadyConfirmedEmail()
    {
        // Arrange
        await CreateEmailAddressConfirmationTemplateAsync();

        await Service.RegisterAsync(new("test@email.com", "123Qwe!@", "123Qwe!@", "Test"));

        var confirmationUri = GetEmailBodyParameterOfType<Uri>();

        var identityToken = confirmationUri.Segments.Last();

        await Service.ConfirmEmailAsync(identityToken);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ConfirmEmailAsync(identityToken));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(UnprocessableEntity, UserEmailAlreadyConfirmed);
    }

    /// <summary>
    /// <see cref="UserService.ConfirmEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// identity token is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ConfirmEmailAsync_ThrowsOnInvalidIdentityToken()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ConfirmEmailAsync(InvalidIdentityToken));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidToken);
    }

    /// <summary>
    /// <see cref="UserService.ConfirmEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="User"/>
    /// does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ConfirmEmailAsync_ThrowsOnNonExistingUser()
    {
        // Arrange
        const string Email = "test@email.com";

        await CreateEmailAddressConfirmationTemplateAsync();

        await Service.RegisterAsync(new(Email, "123Qwe!@", "123Qwe!@", "Test"));

        await DeleteUserAsync(Email);

        var confirmationUri = GetEmailBodyParameterOfType<Uri>();

        var identityToken = confirmationUri.Segments.Last();

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ConfirmEmailAsync(identityToken));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, UserEmailNotFound);
    }

    /// <summary>
    /// <see cref="UserService.ConfirmEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// identity token is outdated.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ConfirmEmailAsync_ThrowsOnOutdatedIdentityToken()
    {
        // Arrange
        await CreateEmailAddressConfirmationTemplateAsync();

        await Service.RegisterAsync(new("test@email.com", "123Qwe!@", "123Qwe!@", "Test"));

        var confirmationUri = GetEmailBodyParameterOfType<Uri>();

        var identityToken = confirmationUri.Segments.Last();

        // Act
        await Task.Delay(Options.Tokens.EmailConfirmationTokenLifetime + TimeSpan.FromSeconds(0.5));

        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ConfirmEmailAsync(identityToken));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(UnprocessableEntity, OutdatedToken);
    }

    /// <summary>
    /// <see cref="UserService.ConfirmLoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns an authorization token for the user after passing two-factor authentication.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreateTwoFactorVerificationTemplateAsync();

        await CreateUserAsync(email: Email, password: Password, twoFactorEnabled: true);

        var loginResult = await Service.LoginAsync(new(Password) { Email = Email });

        var twoFactorToken = loginResult.Value!.Token;
        var twoFactorCode = GetEmailBodyParameterOfType<string>();

        var userTwoFactorAuthenticationModel = new UserTwoFactorAuthenticationModel(twoFactorCode, twoFactorToken);

        // Act
        var result = await Service.ConfirmLoginAsync(userTwoFactorAuthenticationModel);

        // Assert
        result.AssertSuccess(OK);
    }

    /// <summary>
    /// <see cref="UserService.ConfirmLoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// identity token is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync_ThrowsOnInvalidIdentityToken()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreateTwoFactorVerificationTemplateAsync();

        await CreateUserAsync(email: Email, password: Password, twoFactorEnabled: true);

        var loginResult = await Service.LoginAsync(new(Password) { Email = Email });

        var twoFactorToken = loginResult.Value!.Token + InvalidIdentityToken;
        var twoFactorCode = GetEmailBodyParameterOfType<string>();

        var userTwoFactorAuthenticationModel = new UserTwoFactorAuthenticationModel(twoFactorToken, twoFactorCode);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ConfirmLoginAsync(userTwoFactorAuthenticationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidToken);
    }

    /// <summary>
    /// <see cref="UserService.ConfirmLoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="User"/>
    /// does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync_ThrowsOnNonExistingUser()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreateTwoFactorVerificationTemplateAsync();

        var user = await CreateUserAsync(email: Email, password: Password, twoFactorEnabled: true);

        var loginResult = await Service.LoginAsync(new(Password) { Email = Email });

        await DeleteUserAsync(user);

        var twoFactorToken = loginResult.Value!.Token;
        var twoFactorCode = GetEmailBodyParameterOfType<string>();

        var userTwoFactorAuthenticationModel = new UserTwoFactorAuthenticationModel(twoFactorCode, twoFactorToken);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ConfirmLoginAsync(userTwoFactorAuthenticationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, UserEmailNotFound);
    }

    /// <summary>
    /// <see cref="UserService.ConfirmLoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// identity token is outdated.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync_ThrowsOnOutdatedIdentityToken()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreateTwoFactorVerificationTemplateAsync();

        await CreateUserAsync(email: Email, password: Password, twoFactorEnabled: true);

        var loginResult = await Service.LoginAsync(new(Password) { Email = Email });

        var twoFactorToken = loginResult.Value!.Token;
        var twoFactorCode = GetEmailBodyParameterOfType<string>();

        var userTwoFactorAuthenticationModel = new UserTwoFactorAuthenticationModel(twoFactorCode, twoFactorToken);

        // Act
        await Task.Delay(Options.Authentication.TwoFactorTokenLifetime + TimeSpan.FromSeconds(0.5));

        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ConfirmLoginAsync(userTwoFactorAuthenticationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(UnprocessableEntity, OutdatedToken);
    }

    /// <summary>
    /// <see cref="UserService.ConfirmLoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// identity token contains an invalid verification code.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync_ThrowsOnVerificationCodeMismatch()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreateTwoFactorVerificationTemplateAsync();

        await CreateUserAsync(email: Email, password: Password, twoFactorEnabled: true);

        var loginResult = await Service.LoginAsync(new(Password) { Email = Email });

        var twoFactorToken = loginResult.Value!.Token;
        var twoFactorCode = GetEmailBodyParameterOfType<string>();

        var userTwoFactorAuthenticationModel = new UserTwoFactorAuthenticationModel(twoFactorCode + twoFactorCode, twoFactorToken);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ConfirmLoginAsync(userTwoFactorAuthenticationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(Unauthorized, UnauthorizedUser);
    }

    /// <summary>
    /// <see cref="UserService.ConfirmLoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// identity token does not contain a verification code.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ConfirmLoginAsync_ThrowsOnVerificationCodeMissing()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreateTwoFactorVerificationTemplateAsync();

        await CreateUserAsync(email: Email, password: Password, twoFactorEnabled: true);

        var loginResult = await Service.LoginAsync(new(Password) { Email = Email });

        var twoFactorToken = loginResult.Value!.Token;

        var userTwoFactorAuthenticationModel = new UserTwoFactorAuthenticationModel("", twoFactorToken);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ConfirmLoginAsync(userTwoFactorAuthenticationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidModel);
    }

    /// <summary>
    /// <see cref="UserService.CreateEmailResetRequestAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Generates an email address reset token and provides it to the user.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task CreateEmailResetRequestAsync()
    {
        // Arrange
        const string Email = "test@email.com";
        const string NewEmail = "newTest@email.com";

        await CreateEmailAddressResetTemplateAsync();

        var user = await CreateUserAsync(email: Email);

        var userResetEmailRequestModel = new UserResetEmailRequestModel(NewEmail, NewEmail);

        // Act
        var result = await Service.CreateEmailResetRequestAsync(user.Id, userResetEmailRequestModel);

        // Assert
        result.AssertSuccess(OK);
    }

    /// <summary>
    /// <see cref="UserService.CreateEmailResetRequestAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// email address is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task CreateEmailResetRequestAsync_ThrowsOnInvalidEmail()
    {
        // Arrange
        const string Email = "test@email.com";

        await CreateEmailAddressResetTemplateAsync();

        var user = await CreateUserAsync(email: Email);

        var userResetEmailRequestModel = new UserResetEmailRequestModel(InvalidEmailAddress, InvalidEmailAddress);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.CreateEmailResetRequestAsync(user.Id, userResetEmailRequestModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidEmail);
    }

    /// <summary>
    /// <see cref="UserService.CreateEmailResetRequestAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since there was no
    /// email address provided.
    /// </para>
    /// </summary>
    [Fact]
    public async Task CreateEmailResetRequestAsync_ThrowsOnMissingEmail()
    {
        // Arrange
        const string Email = "test@email.com";

        await CreateEmailAddressResetTemplateAsync();

        var user = await CreateUserAsync(email: Email);

        var userResetEmailRequestModel = new UserResetEmailRequestModel("", "");

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.CreateEmailResetRequestAsync(user.Id, userResetEmailRequestModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidEmail);
    }

    /// <summary>
    /// <see cref="UserService.CreateEmailResetRequestAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="EmailTemplate"/> to be used to
    /// send an email message does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task CreateEmailResetRequestAsync_ThrowsOnNonExistingEmailTemplate()
    {
        // Arrange
        const string Email = "test@email.com";
        const string NewEmail = "newTest@email.com";

        var user = await CreateUserAsync(email: Email);

        var userResetEmailRequestModel = new UserResetEmailRequestModel(NewEmail, NewEmail);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.CreateEmailResetRequestAsync(user.Id, userResetEmailRequestModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, MessageTemplateNotFound);
    }

    /// <summary>
    /// <see cref="UserService.CreateEmailResetRequestAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="User"/>
    /// does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task CreateEmailResetRequestAsync_ThrowsOnNonExistingUser()
    {
        // Arrange
        var userId = Guid.Empty;
        const string NewEmail = "newTest@email.com";

        await CreateEmailAddressResetTemplateAsync();

        var userResetEmailRequestModel = new UserResetEmailRequestModel(NewEmail, NewEmail);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.CreateEmailResetRequestAsync(userId, userResetEmailRequestModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, UserIdNotFound);
    }

    /// <summary>
    /// <see cref="UserService.CreatePasswordResetRequestAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Generates a password reset token and provides it to the user.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task CreatePasswordResetRequestAsync()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreatePasswordResetTemplateAsync();

        await CreateUserAsync(email: Email, password: Password);

        // Act
        var result = await Service.CreatePasswordResetRequestAsync(new(Email));

        // Assert
        result.AssertSuccess(OK);
    }

    /// <summary>
    /// <see cref="UserService.CreatePasswordResetRequestAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// No exception is thrown if there is no such user
    /// with the specified email address.
    /// Such behavior prevents user enumeration vulnerability.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task CreatePasswordResetRequestAsync_DoesNotThrowOnNonExistingUser()
    {
        // Arrange
        const string Email = "test@email.com";

        await CreatePasswordResetTemplateAsync();

        // Act
        var result = await Service.CreatePasswordResetRequestAsync(new(Email));

        // Assert
        result.AssertSuccess(OK);
    }

    /// <summary>
    /// <see cref="UserService.CreatePasswordResetRequestAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// email address is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task CreatePasswordResetRequestAsync_ThrowsOnInvalidEmail()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreatePasswordResetTemplateAsync();

        await CreateUserAsync(email: Email, password: Password);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.CreatePasswordResetRequestAsync(new(InvalidEmailAddress)));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidEmail);
    }

    /// <summary>
    /// <see cref="UserService.CreatePasswordResetRequestAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since there was no
    /// email address provided.
    /// </para>
    /// </summary>
    [Fact]
    public async Task CreatePasswordResetRequestAsync_ThrowsOnMissingEmail()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreatePasswordResetTemplateAsync();

        await CreateUserAsync(email: Email, password: Password);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.CreatePasswordResetRequestAsync(new("")));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidEmail);
    }

    /// <summary>
    /// <see cref="UserService.CreatePasswordResetRequestAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="EmailTemplate"/> to be used to
    /// send an email message does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task CreatePasswordResetRequestAsync_ThrowsOnNonExistingEmailTemplate()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreateUserAsync(email: Email, password: Password);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.CreatePasswordResetRequestAsync(new(Email)));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, MessageTemplateNotFound);
    }

    /// <summary>
    /// <see cref="UserService.DeleteAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Completely removes all data of a user
    /// with the specified Id.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task DeleteAsync()
    {
        // Arrange
        var user = await CreateUserAsync(deletionRequestSubmitted: DateTime.UtcNow);

        // Act
        var result = await Service.DeleteAsync(user.Id);

        // Assert
        result.AssertSuccess(OK);
        Assert.DoesNotContain(user, Manager.Users);
    }

    /// <summary>
    /// <see cref="UserService.DeleteAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="User"/>
    /// does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ThrowsOnNonExistingUser()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.DeleteAsync(Guid.Empty));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, UserIdNotFound);
    }

    /// <summary>
    /// <see cref="UserService.DeleteAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="User"/>
    /// is not pending deletion.
    /// </para>
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ThrowsOnNonPendingUser()
    {
        // Arrange
        var user = await CreateUserAsync(deletionRequestSubmitted: null);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.DeleteAsync(user.Id));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, UserNotPendingDeletion);
    }

    /// <summary>
    /// <see cref="UserService.DeleteAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the deletion
    /// request is expired.
    /// </para>
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ThrowsOnOutdatedDeletionRequest()
    {
        // Arrange
        var user = await CreateUserAsync(deletionRequestSubmitted: DateTime.UtcNow);

        // Act
        await Task.Delay(Options.Tokens.UserDeletionRequestLifetime + TimeSpan.FromSeconds(0.5));

        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.DeleteAsync(user.Id));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, UserDeletionRequestExpired);
    }

    /// <summary>
    /// <see cref="UserService.GetAllAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns a complete list of users.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task GetAllAsync()
    {
        // Arrange
        var user = await CreateUserAsync();

        // Act
        var result = await Service.GetAllAsync();

        // Assert
        result.AssertSuccess(OK);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        Assert.Equal(user.Id, result.Value.Single().Id);
    }

    /// <summary>
    /// <see cref="UserService.GetByIdAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the user found.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task GetByIdAsync()
    {
        // Arrange
        var user = await CreateUserAsync();

        // Act
        var result = await Service.GetByIdAsync(user.Id);

        // Assert
        result.AssertSuccess(OK);
        Assert.NotNull(result.Value);
        Assert.Equal(user.Id, result.Value.Id);
    }

    /// <summary>
    /// <see cref="UserService.GetByIdAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="User"/>
    /// does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ThrowsOnNonExistingUser()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.GetByIdAsync(Guid.NewGuid()));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, UserIdNotFound);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// User is logged in, after lockout period is exceeded.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_AfterLockout()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        var user = await CreateUserAsync(email: Email, password: Password);

        var loginModel = new UserLoginModel(Password + Password) { Email = Email };

        for (var i = 0; i < Manager.Options.Lockout.MaxFailedAccessAttempts; i++)
            await Assert.ThrowsAsync<ResultException>(() => Service.LoginAsync(loginModel));

        await Task.Delay(Manager.Options.Lockout.DefaultLockoutTimeSpan + TimeSpan.FromSeconds(0.5));

        loginModel.Password = Password;

        // Act
        var result = await Service.LoginAsync(loginModel);

        // Assert
        result.AssertSuccess(OK);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the user is
    /// locked out after a number of unsuccessful login attempts.
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_IncorrectPassword_LocksOutUser()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        var user = await CreateUserAsync(email: Email, password: Password);

        var loginModel = new UserLoginModel(Password + Password) { Email = Email };

        for (var i = 0; i < Manager.Options.Lockout.MaxFailedAccessAttempts; i++)
            await Assert.ThrowsAsync<ResultException>(() => Service.LoginAsync(loginModel));

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.LoginAsync(loginModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(Forbidden, UserLockedOut);
        Assert.NotNull(user.LockoutEnd);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since there was no
    /// email address, user-name or phone number provided.
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_ThrowsOnEmptyCredentials()
    {
        // Arrange
        const string Password = "123Qwe!@";

        var loginModel = new UserLoginModel(Password);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.LoginAsync(loginModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidModel);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since there was no
    /// password provided.
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_ThrowsOnEmptyPassword()
    {
        // Arrange
        const string Password = "";

        var loginModel = new UserLoginModel(Password);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.LoginAsync(loginModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, PasswordMissing);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// password is not correct.
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_ThrowsOnIncorrectPassword()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";
        const string IncorrectPassword = "Incorrect password";

        await CreateUserAsync(email: Email, password: Password);

        var loginModel = new UserLoginModel(IncorrectPassword) { Email = Email };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.LoginAsync(loginModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(Unauthorized, UserNotFoundOrPasswordMismatch);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// email address is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_ThrowsOnInvalidEmail()
    {
        // Arrange
        const string Password = "123Qwe!@";

        var loginModel = new UserLoginModel(Password) { Email = InvalidEmailAddress };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.LoginAsync(loginModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidEmail);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// phone number is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_ThrowsOnInvalidPhoneNumber()
    {
        // Arrange
        const string Phone = "Invalid phone";
        const string Password = "123Qwe!@";

        var loginModel = new UserLoginModel(Password) { Phone = Phone };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.LoginAsync(loginModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidPhoneNumber);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// user-name is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_ThrowsOnInvalidUserName()
    {
        // Arrange
        const string UserName = "%";
        const string Password = "123Qwe!@";

        var loginModel = new UserLoginModel(Password) { UserName = UserName };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.LoginAsync(loginModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidUserName);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since there is no
    /// such <see cref="User"/> with the email address provided.
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_ThrowsOnNonExistingEmail()
    {
        // Arrange
        const string Email = "non.existing@email.com";
        const string Password = "123Qwe!@";

        var loginModel = new UserLoginModel(Password) { Email = Email };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.LoginAsync(loginModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(Unauthorized, UserNotFoundOrPasswordMismatch);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since there is no
    /// such <see cref="User"/> with the phone number provided.
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_ThrowsOnNonExistingPhoneNumber()
    {
        // Arrange
        const string Phone = "+000000000000";
        const string Password = "123Qwe!@";

        var loginModel = new UserLoginModel(Password) { Phone = Phone };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.LoginAsync(loginModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(Unauthorized, UserNotFoundOrPasswordMismatch);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since there is no
    /// such <see cref="User"/> with the user-name provided.
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_ThrowsOnNonExistingUserName()
    {
        // Arrange
        const string UserName = "Test";
        const string Password = "123Qwe!@";

        var loginModel = new UserLoginModel(Password) { UserName = UserName };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.LoginAsync(loginModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(Unauthorized, UserNotFoundOrPasswordMismatch);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="User"/>
    /// did not confirm the email address.
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_ThrowsOnNotConfirmedEmail()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreateUserAsync(email: Email, password: Password, emailConfirmed: false);

        var loginModel = new UserLoginModel(Password) { Email = Email };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.LoginAsync(loginModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(Forbidden, UserEmailNotConfirmed);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since there was no
    /// password provided.
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_ThrowsOnPasswordMissing()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreateUserAsync(email: Email, password: Password);

        var loginModel = new UserLoginModel("") { Email = Email };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.LoginAsync(loginModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, PasswordMissing);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the user is
    /// locked out.
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_ThrowsOnUserLockedOut()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreateUserAsync(email: Email, password: Password, lockoutEnd: DateTime.UtcNow.AddDays(1));

        var loginModel = new UserLoginModel(Password) { Email = Email };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.LoginAsync(loginModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(Forbidden, UserLockedOut);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns an authorization token for the user with the specified email address.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithEmail()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreateUserAsync(email: Email, password: Password);

        var loginModel = new UserLoginModel(Password) { Email = Email };

        // Act
        var result = await Service.LoginAsync(loginModel);

        // Assert
        result.AssertSuccess(OK);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns an authorization token for the user with the specified phone number.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithPhoneNumber()
    {
        // Arrange
        const string Phone = "+000000000000";
        const string Password = "123Qwe!@";

        await CreateUserAsync(phoneNumber: Phone, password: Password);

        var loginModel = new UserLoginModel(Password) { Phone = Phone };

        // Act
        var result = await Service.LoginAsync(loginModel);

        // Assert
        result.AssertSuccess(OK);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns a two-factor authentication token for the user.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithTwoFactorMethod()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreateTwoFactorVerificationTemplateAsync();

        await CreateUserAsync(email: Email, password: Password, twoFactorEnabled: true);

        var loginModel = new UserLoginModel(Password) { Email = Email };

        // Act
        var result = await Service.LoginAsync(loginModel);

        // Assert
        result.AssertSuccess(Accepted);
        Assert.Contains(SentEmailsCache, args => args.TemplateName == TemplateOptions.TwoFactorVerificationTemplateName);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="EmailTemplate"/> to be used to
    /// send an email message does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithTwoFactorMethod_ThrowsOnNonExistingEmailTemplate()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreateUserAsync(email: Email, password: Password, twoFactorEnabled: true);

        var loginModel = new UserLoginModel(Password) { Email = Email };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.LoginAsync(loginModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, MessageTemplateNotFound);
    }

    /// <summary>
    /// <see cref="UserService.LoginAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns an authorization token for the user with the specified user-name.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithUserName()
    {
        // Arrange
        const string UserName = "Test";
        const string Password = "123Qwe!@";

        await CreateUserAsync(userName: UserName, password: Password);

        var loginModel = new UserLoginModel(Password) { UserName = UserName };

        // Act
        var result = await Service.LoginAsync(loginModel);

        // Assert
        result.AssertSuccess(OK);
    }

    /// <summary>
    /// <see cref="UserService.LogoutAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Invalidates the previously generated access token.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task LogoutAsync()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreateUserAsync(email: Email, password: Password);

        var loginResult = await Service.LoginAsync(new(Password) { Email = Email });

        var accessToken = loginResult.Value!.Token!;

        // Act
        var result = await Service.LogoutAsync(accessToken);

        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.RenewTokenAsync(accessToken));

        var renewResult = exception.GetResult();

        // Assert
        result.AssertSuccess(OK);
        renewResult.AssertFail(Unauthorized, OutdatedToken);
    }

    /// <summary>
    /// <see cref="UserService.LogoutAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// access token is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task LogoutAsync_ThrowsOnInvalidAccessToken()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.LogoutAsync(InvalidAccessToken));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidToken);
    }

    /// <summary>
    /// <see cref="UserService.LogoutEverywhereAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Invalidates all user's access tokens.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task LogoutEverywhereAsync()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreateUserAsync(email: Email, password: Password);

        var loginResult1 = await Service.LoginAsync(new(Password) { Email = Email });

        var accessToken1 = loginResult1.Value!.Token!;

        var loginResult2 = await Service.LoginAsync(new(Password) { Email = Email });

        var accessToken2 = loginResult2.Value!.Token!;

        // Act
        var result = await Service.LogoutEverywhereAsync(accessToken1);

        var exception1 = await Assert.ThrowsAsync<ResultException>(()
            => Service.RenewTokenAsync(accessToken1));

        var exception2 = await Assert.ThrowsAsync<ResultException>(()
            => Service.RenewTokenAsync(accessToken2));

        var renewResult1 = exception1.GetResult();
        var renewResult2 = exception2.GetResult();

        // Assert
        result.AssertSuccess(OK);
        renewResult1.AssertFail(Unauthorized, OutdatedToken);
        renewResult2.AssertFail(Unauthorized, OutdatedToken);
    }

    /// <summary>
    /// <see cref="UserService.LogoutEverywhereAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// access token is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task LogoutEverywhereAsync_ThrowsOnInvalidAccessToken()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.LogoutEverywhereAsync(InvalidAccessToken));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidToken);
    }

    /// <summary>
    /// <see cref="UserService.RegisterAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Creates a new user.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task RegisterAsync()
    {
        // Arrange
        await CreateEmailAddressConfirmationTemplateAsync();

        var registrationModel = new UserRegistrationModel("test@email.com", "123Qwe!@", "123Qwe!@", "Test")
        {
            Phone = "+000000000000"
        };

        // Act
        var result = await Service.RegisterAsync(registrationModel);

        // Assert
        result.AssertSuccess(Created);
    }

    /// <summary>
    /// <see cref="UserService.RegisterAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// email address is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task RegisterAsync_ThrowsOnInvalidEmail()
    {
        // Arrange
        await CreateEmailAddressConfirmationTemplateAsync();

        var registrationModel = new UserRegistrationModel(InvalidEmailAddress, "123Qwe!@", "123Qwe!@", "Test")
        {
            Phone = "+000000000000"
        };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.RegisterAsync(registrationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidEmail);
    }

    /// <summary>
    /// <see cref="UserService.RegisterAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// password is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task RegisterAsync_ThrowsOnInvalidPassword()
    {
        // Arrange
        await CreateEmailAddressConfirmationTemplateAsync();

        var registrationModel = new UserRegistrationModel("test@email.com", ".", ".", "Test")
        {
            Phone = "+000000000000"
        };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.RegisterAsync(registrationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(UnprocessableEntity,
                          PasswordRequiresDigit,
                          PasswordRequiresLower,
                          PasswordRequiresUpper,
                          PasswordTooShort,
                          PasswordRequiresUniqueChars);
    }

    /// <summary>
    /// <see cref="UserService.RegisterAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// phone number is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task RegisterAsync_ThrowsOnInvalidPhoneNumber()
    {
        // Arrange
        await CreateEmailAddressConfirmationTemplateAsync();

        var registrationModel = new UserRegistrationModel("test@email.com", "123Qwe!@", "123Qwe!@", "Test")
        {
            Phone = "Invalid phone number"
        };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.RegisterAsync(registrationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidPhoneNumber);
    }

    /// <summary>
    /// <see cref="UserService.RegisterAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// user-name is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task RegisterAsync_ThrowsOnInvalidUsername()
    {
        // Arrange
        await CreateEmailAddressConfirmationTemplateAsync();

        var registrationModel = new UserRegistrationModel("test@email.com", "123Qwe!@", "123Qwe!@", "Invalid user-name")
        {
            Phone = "+000000000000"
        };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.RegisterAsync(registrationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidUserName);
    }

    /// <summary>
    /// <see cref="UserService.RegisterAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="EmailTemplate"/> to be used to
    /// send an email message does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task RegisterAsync_ThrowsOnNonExistingEmailTemplate()
    {
        // Arrange
        var registrationModel = new UserRegistrationModel("test@email.com", "123Qwe!@", "123Qwe!@", "Test")
        {
            Phone = "+000000000000"
        };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.RegisterAsync(registrationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, MessageTemplateNotFound);
    }

    /// <summary>
    /// <see cref="UserService.RegisterAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the password provided
    /// does not match the password confirmation.
    /// </para>
    /// </summary>
    [Fact]
    public async Task RegisterAsync_ThrowsOnPasswordNotMatchConfirmation()
    {
        // Arrange
        const string Password = "123Qwe!@";

        await CreateEmailAddressConfirmationTemplateAsync();

        var registrationModel = new UserRegistrationModel("test@email.com", Password, Password + Password, "Test")
        {
            Phone = "+000000000000"
        };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.RegisterAsync(registrationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, PasswordNotMatchConfirmation);
    }

    /// <summary>
    /// <see cref="UserService.RegisterAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// email address is already in use.
    /// </para>
    /// </summary>
    [Fact]
    public async Task RegisterAsync_ThrowsOnTakenEmailAddress()
    {
        // Arrange
        const string Email = "test@email.com";

        await CreateEmailAddressConfirmationTemplateAsync();

        await CreateUserAsync(email: Email, userName: "Test2");

        var registrationModel = new UserRegistrationModel(Email, "123Qwe!@", "123Qwe!@", "Test")
        {
            Phone = "+000000000000"
        };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.RegisterAsync(registrationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(UnprocessableEntity, DuplicateEmail);
    }

    /// <summary>
    /// <see cref="UserService.RegisterAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// phone number is already in use.
    /// </para>
    /// </summary>
    [Fact]
    public async Task RegisterAsync_ThrowsOnTakenPhoneNumber()
    {
        // Arrange
        const string PhoneNumber = "+000000000000";

        await CreateEmailAddressConfirmationTemplateAsync();

        await CreateUserAsync(email: "test2@email.com", userName: "Test2", phoneNumber: PhoneNumber);

        var registrationModel = new UserRegistrationModel("test@email.com", "123Qwe!@", "123Qwe!@", "Test")
        {
            Phone = PhoneNumber
        };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.RegisterAsync(registrationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(UnprocessableEntity, DuplicatePhoneNumber);
    }

    /// <summary>
    /// <see cref="UserService.RegisterAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// user-name is already in use.
    /// </para>
    /// </summary>
    [Fact]
    public async Task RegisterAsync_ThrowsOnTakenUsername()
    {
        // Arrange
        const string UserName = "Test";

        await CreateEmailAddressConfirmationTemplateAsync();

        await CreateUserAsync(email: "test2@email.com", userName: UserName);

        var registrationModel = new UserRegistrationModel("test@email.com", "123Qwe!@", "123Qwe!@", UserName)
        {
            Phone = "+000000000000"
        };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.RegisterAsync(registrationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(UnprocessableEntity, DuplicateUserName);
    }

    /// <summary>
    /// <see cref="UserService.RegisterAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns a new access token based on the previous access token.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task RenewTokenAsync()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreateUserAsync(email: Email, password: Password);

        var loginResult = await Service.LoginAsync(new(Password) { Email = Email });

        var accessToken = loginResult.Value!.Token!;

        // Act
        var result = await Service.RenewTokenAsync(accessToken);

        // Assert
        result.AssertSuccess(OK);
    }

    /// <summary>
    /// <see cref="UserService.RenewTokenAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// access token is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task RenewTokenAsync_ThrowsOnInvalidAccessToken()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.RenewTokenAsync(InvalidAccessToken));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidToken);
    }

    /// <summary>
    /// <see cref="UserService.RenewTokenAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="User"/>
    /// does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task RenewTokenAsync_ThrowsOnNonExistingUser()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        var user = await CreateUserAsync(email: Email, password: Password);

        var loginResult = await Service.LoginAsync(new(Password) { Email = Email });

        var accessToken = loginResult.Value!.Token!;

        await DeleteUserAsync(user);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.RenewTokenAsync(accessToken));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, UserIdNotFound);
    }

    /// <summary>
    /// <see cref="UserService.RenewTokenAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// access token is outdated.
    /// </para>
    /// </summary>
    [Fact]
    public async Task RenewTokenAsync_ThrowsOnOutdatedRefreshToken()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreateUserAsync(email: Email, password: Password);

        var loginResult = await Service.LoginAsync(new(Password) { Email = Email });

        var accessToken = loginResult.Value!.Token!;

        // Act
        await Task.Delay(Options.Authentication.RefreshTokenLifetime + TimeSpan.FromSeconds(0.5));

        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.RenewTokenAsync(accessToken));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(Unauthorized, OutdatedToken);
    }

    /// <summary>
    /// <see cref="UserService.ResetEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Resets the user's email address.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task ResetEmailAsync()
    {
        // Arrange
        const string Email = "test@email.com";
        const string NewEmail = "newTest@email.com";

        await CreateEmailAddressResetTemplateAsync();

        var user = await CreateUserAsync(email: Email);

        var userResetEmailRequestModel = new UserResetEmailRequestModel(NewEmail, NewEmail);

        await Service.CreateEmailResetRequestAsync(user.Id, userResetEmailRequestModel);

        var identityTokenLink = GetEmailBodyParameterOfType<Uri>(1);

        var identityToken = identityTokenLink.Segments.Last();

        // Act
        var result = await Service.ResetEmailAsync(identityToken);

        // Assert
        result.AssertSuccess(OK);
        Assert.Equal(NewEmail, user.Email);
    }

    /// <summary>
    /// <see cref="UserService.ResetEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// identity token is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ResetEmailAsync_ThrowsOnInvalidIdentityToken()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ResetEmailAsync(InvalidIdentityToken));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidToken);
    }

    /// <summary>
    /// <see cref="UserService.ResetEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="User"/>
    /// does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ResetEmailAsync_ThrowsOnNonExistingUser()
    {
        // Arrange
        const string Email = "test@email.com";
        const string NewEmail = "newTest@email.com";

        await CreateEmailAddressResetTemplateAsync();

        var user = await CreateUserAsync(email: Email);

        var userResetEmailRequestModel = new UserResetEmailRequestModel(NewEmail, NewEmail);

        await Service.CreateEmailResetRequestAsync(user.Id, userResetEmailRequestModel);

        var identityTokenLink = GetEmailBodyParameterOfType<Uri>(1);

        var identityToken = identityTokenLink.Segments.Last();

        await DeleteUserAsync(user);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ResetEmailAsync(identityToken));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, UserEmailNotFound);
    }

    /// <summary>
    /// <see cref="UserService.ResetEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// identity token is outdated.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ResetEmailAsync_ThrowsOnOutdatedIdentityToken()
    {
        // Arrange
        const string Email = "test@email.com";
        const string NewEmail = "newTest@email.com";

        await CreateEmailAddressResetTemplateAsync();

        var user = await CreateUserAsync(email: Email);

        var userResetEmailRequestModel = new UserResetEmailRequestModel(NewEmail, NewEmail);

        await Service.CreateEmailResetRequestAsync(user.Id, userResetEmailRequestModel);

        var identityTokenLink = GetEmailBodyParameterOfType<Uri>(1);

        var identityToken = identityTokenLink.Segments.Last();

        await Task.Delay(Options.Tokens.ResetEmailAddressTokenLifetime + TimeSpan.FromSeconds(0.5));

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ResetEmailAsync(identityToken));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(UnprocessableEntity, OutdatedToken);
    }

    /// <summary>
    /// <see cref="UserService.ResetEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// email address is already in use.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ResetEmailAsync_ThrowsOnTakenEmailAddress()
    {
        // Arrange
        const string FirstUserName = "Test0";
        const string SecondUserName = "Test1";

        const string Email = "test@email.com";
        const string NewEmail = "newTest@email.com";

        await CreateEmailAddressResetTemplateAsync();

        var user = await CreateUserAsync(email: Email, userName: FirstUserName);

        var userResetEmailRequestModel = new UserResetEmailRequestModel(NewEmail, NewEmail);

        await Service.CreateEmailResetRequestAsync(user.Id, userResetEmailRequestModel);

        var identityTokenLink = GetEmailBodyParameterOfType<Uri>(1);

        var identityToken = identityTokenLink.Segments.Last();

        await CreateUserAsync(email: NewEmail, userName: SecondUserName);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ResetEmailAsync(identityToken));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, DuplicateEmail);
    }

    /// <summary>
    /// <see cref="UserService.ResetPasswordAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Resets the user's password.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task ResetPasswordAsync()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        const string NewPassword = Password + Password;

        await CreatePasswordResetTemplateAsync();

        await CreateUserAsync(email: Email, password: Password);

        await Service.CreatePasswordResetRequestAsync(new(Email));

        var identityToken = GetEmailBodyParameterOfType<string>();

        var resetPasswordModel = new UserResetPasswordModel(identityToken, NewPassword, NewPassword);

        // Act
        var result = await Service.ResetPasswordAsync(resetPasswordModel);

        // Assert
        result.AssertSuccess(OK);
    }

    /// <summary>
    /// <see cref="UserService.ResetPasswordAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// identity token is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ResetPasswordAsync_ThrowsOnInvalidIdentityToken()
    {
        // Arrange
        const string Password = "123Qwe!@";

        await CreatePasswordResetTemplateAsync();

        var resetPasswordModel = new UserResetPasswordModel(InvalidIdentityToken, Password, Password);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ResetPasswordAsync(resetPasswordModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidToken);
    }

    /// <summary>
    /// <see cref="UserService.ResetPasswordAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// password is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ResetPasswordAsync_ThrowsOnInvalidPassword()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = ".";

        await CreatePasswordResetTemplateAsync();

        await CreateUserAsync(email: Email, password: Password);

        await Service.CreatePasswordResetRequestAsync(new(Email));

        var identityToken = GetEmailBodyParameterOfType<string>();

        var resetPasswordModel = new UserResetPasswordModel(identityToken, Password, Password);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ResetPasswordAsync(resetPasswordModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(UnprocessableEntity,
                          PasswordRequiresDigit,
                          PasswordRequiresLower,
                          PasswordRequiresUpper,
                          PasswordTooShort,
                          PasswordRequiresUniqueChars);
    }

    /// <summary>
    /// <see cref="UserService.ResetPasswordAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="User"/>
    /// does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ResetPasswordAsync_ThrowsOnNonExistingUser()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreatePasswordResetTemplateAsync();

        var user = await CreateUserAsync(email: Email, password: Password);

        await Service.CreatePasswordResetRequestAsync(new(Email));

        await DeleteUserAsync(user);

        var identityToken = GetEmailBodyParameterOfType<string>();

        var resetPasswordModel = new UserResetPasswordModel(identityToken, Password, Password);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ResetPasswordAsync(resetPasswordModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, UserEmailNotFound);
    }

    /// <summary>
    /// <see cref="UserService.ResetPasswordAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// identity token is outdated.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ResetPasswordAsync_ThrowsOnOutdatedIdentityToken()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreatePasswordResetTemplateAsync();

        var user = await CreateUserAsync(email: Email, password: Password);

        await Service.CreatePasswordResetRequestAsync(new(Email));

        await Task.Delay(Options.Tokens.ResetPasswordTokenLifetime + TimeSpan.FromSeconds(0.5));

        var identityToken = GetEmailBodyParameterOfType<string>();

        var resetPasswordModel = new UserResetPasswordModel(identityToken, Password, Password);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ResetPasswordAsync(resetPasswordModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(UnprocessableEntity, OutdatedToken);
    }

    /// <summary>
    /// <see cref="UserService.ResetPasswordAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since there was no
    /// password provided.
    /// </para>
    /// </summary>
    [Fact]
    public async Task ResetPasswordAsync_ThrowsOnPasswordMissing()
    {
        // Arrange
        const string Email = "test@email.com";
        const string Password = "123Qwe!@";

        await CreatePasswordResetTemplateAsync();

        await CreateUserAsync(email: Email, password: Password);

        await Service.CreatePasswordResetRequestAsync(new(Email));

        var identityToken = GetEmailBodyParameterOfType<string>();

        var resetPasswordModel = new UserResetPasswordModel(identityToken, "", "");

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.ResetPasswordAsync(resetPasswordModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidModel);
    }

    /// <summary>
    /// <see cref="UserService.UpdateAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Updates the user with the new values.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        const bool TwoFactorEnabled = true;

        var user = await CreateUserAsync(twoFactorEnabled: !TwoFactorEnabled, deletionRequestSubmitted: DateTime.UtcNow);

        var userUpdateModel = new UserUpdateModel
        {
            IsPendingDeletion = false,
            TwoFactorEnabled = TwoFactorEnabled,
            UserName = "TestUserUpdated"
        };

        // Act
        var result = await Service.UpdateAsync(user.Id, userUpdateModel);

        // Assert
        result.AssertSuccess(OK);
        Assert.True(user.IsPendingDeletion == userUpdateModel.IsPendingDeletion);
        Assert.True(user.UserName == userUpdateModel.UserName);
        Assert.True(user.TwoFactorEnabled == userUpdateModel.TwoFactorEnabled);
    }

    /// <summary>
    /// <see cref="UserService.UpdateAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// user-name is not valid.
    /// </para>
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ThrowsOnInvalidUsername()
    {
        // Arrange
        const bool TwoFactorEnabled = true;

        var user = await CreateUserAsync(twoFactorEnabled: !TwoFactorEnabled, deletionRequestSubmitted: DateTime.UtcNow);

        var userUpdateModel = new UserUpdateModel
        {
            IsPendingDeletion = false,
            TwoFactorEnabled = TwoFactorEnabled,
            UserName = string.Empty
        };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.UpdateAsync(user.Id, userUpdateModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(UnprocessableEntity, InvalidUserName);
    }

    /// <summary>
    /// <see cref="UserService.UpdateAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="User"/>
    /// does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ThrowsOnNonExistingUser()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.UpdateAsync(Guid.Empty, new()));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, UserIdNotFound);
    }

    /// <summary>
    /// <see cref="UserService.UpdateAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the input
    /// user-name is already in use.
    /// </para>
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ThrowsOnTakenUsername()
    {
        // Arrange
        const string FirstUserName = "TestUser1";
        const string SecondUserName = "TestUser2";
        const string FirstEmail = "testUser1@email.com";
        const string SecondEmail = "testUser2@email.com";

        var firstUser = await CreateUserAsync(userName: FirstUserName, email: FirstEmail);

        var secondUser = await CreateUserAsync(userName: SecondUserName, email: SecondEmail);

        var userUpdateModel = new UserUpdateModel
        {
            UserName = firstUser.UserName
        };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.UpdateAsync(secondUser.Id, userUpdateModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(UnprocessableEntity, DuplicateUserName);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates a test <see cref="User"/>.
    /// </summary>
    /// <param name="email">
    /// Email address.
    /// </param>
    /// <param name="userName">
    /// User-name.
    /// </param>
    /// <param name="password">
    /// Password.
    /// </param>
    /// <param name="emailConfirmed">
    /// Indicates whether the user's email address is confirmed.
    /// </param>
    /// <param name="twoFactorEnabled">
    /// Indicates whether two-factor authentication
    /// is enabled for this user.
    /// </param>
    /// <param name="phoneNumber">
    /// User's phone number.
    /// </param>
    /// <param name="deletionRequestSubmitted">
    /// Deletion request submission date.
    /// </param>
    /// <param name="lockoutEnd">
    /// Lockout end date.
    /// </param>
    /// <returns>
    /// A newly created <see cref="User"/> instance.
    /// </returns>
    private async Task<User> CreateUserAsync(string email = "test@email.com", string userName = "Test", string password = "123Qwe!@",
                                             bool emailConfirmed = true, bool twoFactorEnabled = false, string? phoneNumber = null,
                                             DateTime? deletionRequestSubmitted = null, DateTime? lockoutEnd = null)
    {
        var user = new User(email, userName)
        {
            DeletionRequestSubmitted = deletionRequestSubmitted,
            LockoutEnd = lockoutEnd,
            EmailConfirmed = emailConfirmed,
            PhoneNumber = phoneNumber,
            TwoFactorEnabled = twoFactorEnabled
        };

        await Manager.CreateAsync(user);

        if (!string.IsNullOrWhiteSpace(password))
            await Manager.AddPasswordAsync(user, password);

        return user;
    }

    /// <summary>
    /// Deletes the user.
    /// </summary>
    /// <param name="user">
    /// The <see cref="User"/> to be deleted.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation,
    /// containing the <see cref="IdentityResult"/> of the operation.
    /// </returns>
    private Task<IdentityResult> DeleteUserAsync(User user)
        => Manager.DeleteAsync(user);

    /// <summary>
    /// Deletes the user by email address.
    /// </summary>
    /// <param name="email">
    /// The email address of the <see cref="User"/> to be deleted.
    /// </param>
    private async Task<IdentityResult> DeleteUserAsync(string email)
    {
        var user = await Manager.FindByEmailAsync(email);

        return user is not null
            ? await Manager.DeleteAsync(user)
            : IdentityResult.Failed(new IdentityErrorDescriber().InvalidEmail(email));
    }

    /// <summary>
    /// Creates a test email template in the database.
    /// </summary>
    /// <param name="name">
    /// Template name.
    /// </param>
    /// <param name="placeholdersNumber">
    /// The number of placeholders to be replaced with values during the message formatting.
    /// </param>
    /// <param name="subjectPlaceholdersNumber">
    /// The number of subject placeholders to be replaced with values during the message formatting.
    /// </param>
    /// <param name="culture">
    /// Template culture.
    /// </param>
    private async Task CreateEmailTemplateAsync(string name,
                                                ushort placeholdersNumber,
                                                ushort subjectPlaceholdersNumber,
                                                CultureInfo? culture = null)
    {
        const string Placeholder = "{p}";

        var subject = string.Join(string.Empty, Enumerable.Range(0, subjectPlaceholdersNumber).Select(n => $"{Placeholder}{n}"));
        var text = string.Join(string.Empty, Enumerable.Range(0, placeholdersNumber).Select(n => $"{Placeholder}{n}"));

        if (string.IsNullOrWhiteSpace(subject))
            subject = "TestSubject";

        if (string.IsNullOrWhiteSpace(text))
            text = "TestText";

        var emailTemplate = new EmailTemplate(name, text, subject)
        {
            Culture = culture,
            IsBodyHtml = false,
            PlaceholderName = Placeholder,
            PlaceholdersNumber = placeholdersNumber,
            SubjectPlaceholderName = Placeholder,
            SubjectPlaceholdersNumber = subjectPlaceholdersNumber
        };

        ApplicationDataSource.Add(emailTemplate);
        await ApplicationDataSource.SaveChangesAsync();
    }

    /// <summary>
    /// Creates an email address confirmation template.
    /// </summary>
    private Task CreateEmailAddressConfirmationTemplateAsync()
        => CreateEmailTemplateAsync(TemplateOptions.EmailAddressConfirmationTemplateName, 1, 0);

    /// <summary>
    /// Creates a two-factor verification template.
    /// </summary>
    private Task CreateTwoFactorVerificationTemplateAsync()
        => CreateEmailTemplateAsync(TemplateOptions.TwoFactorVerificationTemplateName, 1, 0);

    /// <summary>
    /// Creates a password reset template.
    /// </summary>
    private Task CreatePasswordResetTemplateAsync()
        => CreateEmailTemplateAsync(TemplateOptions.PasswordResetTemplateName, 1, 0);

    /// <summary>
    /// Creates an email address reset template.
    /// </summary>
    private Task CreateEmailAddressResetTemplateAsync()
        => CreateEmailTemplateAsync(TemplateOptions.EmailAddressResetTemplateName, 2, 0);

    /// <summary>
    /// Gets the argument of type <typeparamref name="T"/>
    /// with the given <paramref name="index"/>
    /// from first email body arguments stored in <see cref="SentEmailsCache"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Argument type.
    /// </typeparam>
    /// <param name="index">
    /// Argument index.
    /// </param>
    /// <returns>
    /// Argument of type <typeparamref name="T"/>
    /// with the given <paramref name="index"/>
    /// from first email body arguments stored in <see cref="SentEmailsCache"/>
    /// or <see langword="null"/>.
    /// </returns>
    private T GetEmailBodyParameterOfType<T>(int index = 0)
    {
        var firstEmail = SentEmailsCache.First();

        return firstEmail.BodyArgs is not null
            && firstEmail.BodyArgs.ElementAt(index) is T castedParameter
            ? castedParameter
            : throw new NullReferenceException();
    }
    #endregion
}