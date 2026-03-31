using Microsoft.AspNetCore.Identity;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.ApplicationLogic.Services.Identity.Users.Implementation;
using Paradise.Domain.Events.Identity.Users;
using Paradise.Models.Domain.Identity.Users;
using Paradise.Tests.Miscellaneous;
using System.Security.Claims;
using static Paradise.Models.ErrorCode;
using static Paradise.Models.OperationStatus;

namespace Paradise.ApplicationLogic.Tests.Unit.Services.Identity.Users;

/// <summary>
/// <see cref="UserService"/> test class.
/// </summary>
public sealed partial class UserServiceTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="RegisterAsync_FailsOnInvalidEmailAddress"/> method.
    /// </summary>
    public static TheoryData<string> RegisterAsync_FailsOnInvalidEmailAddress_MemberData { get; } = new()
    {
        { string.Empty              },
        { "Invalid email address"   }
    };

    /// <summary>
    /// Provides member data for <see cref="CheckPasswordAsync_FailsOnPasswordMismatch"/> method.
    /// </summary>
    public static TheoryData<bool> CheckPasswordAsync_FailsOnPasswordMismatch_MemberData { get; } = new()
    {
        { true  },
        { false }
    };

    /// <summary>
    /// Provides member data for <see cref="CreatePasswordResetRequestAsync_FailsOnInvalidEmailAddress"/> method.
    /// </summary>
    public static TheoryData<string?> CreatePasswordResetRequestAsync_FailsOnInvalidEmailAddress_MemberData { get; } = new()
    {
        { null as string            },
        { string.Empty              },
        { "Invalid email address"   }
    };

    /// <summary>
    /// Provides member data for <see cref="ResetPasswordAsync_FailsOnInvalidPassword"/> method.
    /// </summary>
    public static TheoryData<string?> ResetPasswordAsync_FailsOnInvalidPassword_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };

    /// <summary>
    /// Provides member data for <see cref="CreateEmailAddressResetRequestAsync_FailsOnInvalidEmailAddress"/> method.
    /// </summary>
    public static TheoryData<string?> CreateEmailAddressResetRequestAsync_FailsOnInvalidEmailAddress_MemberData { get; } = new()
    {
        { null as string            },
        { string.Empty              },
        { "Invalid email address"   }
    };

    /// <summary>
    /// Provides member data for <see cref="ResetEmailAddressAsync_FailsOnInvalidEmailAddress"/> method.
    /// </summary>
    public static TheoryData<string?> ResetEmailAddressAsync_FailsOnInvalidEmailAddress_MemberData { get; } = new()
    {
        { null as string            },
        { string.Empty              },
        { "Invalid email address"   }
    };

    /// <summary>
    /// Provides member data for <see cref="UpdateAsync"/> method.
    /// </summary>
    public static TheoryData<bool?> UpdateAsync_MemberData { get; } = new()
    {
        { null as bool? },
        { true          },
        { false         }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="UserService.GetAllAsync"/> method should
    /// return the list of users.
    /// </summary>
    [Fact]
    public async Task GetAllAsync()
    {
        // Arrange
        var user1 = await Test.AddUserAsync(email: "user1@test.com", userName: "user1");
        var user2 = await Test.AddUserAsync(email: "user2@test.com", userName: "user2");

        // Act
        var result = await Test.Target.GetAllAsync(Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.Equal(2, result.Value.Count());
        Assert.Contains(result.Value, user => user.Id == user1.Id);
        Assert.Contains(result.Value, user => user.Id == user2.Id);
    }

    /// <summary>
    /// The <see cref="UserService.GetByIdAsync"/> method should
    /// return a user with the specified Id.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync()
    {
        // Arrange
        var user = await Test.AddUserAsync();

        // Act
        var result = await Test.Target.GetByIdAsync(user.Id, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.Equal(user.Id, result.Value.Id);
    }

    /// <summary>
    /// The <see cref="UserService.GetByIdAsync"/> method should
    /// fail to retrieve a user
    /// when no user with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_FailsOnNonExistingUser()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var result = await Test.Target.GetByIdAsync(id, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, UserIdNotFound);
    }

    /// <summary>
    /// The <see cref="UserService.GetByEmailAddressAsync"/> method should
    /// return a user with the specified email address.
    /// </summary>
    [Fact]
    public async Task GetByEmailAddressAsync()
    {
        // Arrange
        var user = await Test.AddUserAsync();

        // Act
        var result = await Test.Target.GetByEmailAddressAsync(user.Email, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.Equal(user.Email, result.Value.EmailAddress);
    }

    /// <summary>
    /// The <see cref="UserService.GetByEmailAddressAsync"/> method should
    /// fail to retrieve a user
    /// when no user with the specified email address exists.
    /// </summary>
    [Fact]
    public async Task GetByEmailAddressAsync_FailsOnNonExistingUser()
    {
        // Arrange
        var email = "test@email.com";

        // Act
        var result = await Test.Target.GetByEmailAddressAsync(email, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, UserEmailAddressNotFound);
    }

    /// <summary>
    /// The <see cref="UserService.GetByPhoneNumberAsync"/> method should
    /// return a user with the specified phone number.
    /// </summary>
    [Fact]
    public async Task GetByPhoneNumberAsync()
    {
        // Arrange
        var user = await Test.AddUserAsync();

        // Act
        var result = await Test.Target.GetByPhoneNumberAsync(user.PhoneNumber!, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.Equal(user.PhoneNumber, result.Value.PhoneNumber);
    }

    /// <summary>
    /// The <see cref="UserService.GetByPhoneNumberAsync"/> method should
    /// fail to retrieve a user
    /// when no user with the specified phone number exists.
    /// </summary>
    [Fact]
    public async Task GetByPhoneNumberAsync_FailsOnNonExistingUser()
    {
        // Arrange
        var phoneNumber = "+1000000000";

        // Act
        var result = await Test.Target.GetByPhoneNumberAsync(phoneNumber, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, UserPhoneNumberNotFound);
    }

    /// <summary>
    /// The <see cref="UserService.GetByUserNameAsync"/> method should
    /// return a user with the specified user-name.
    /// </summary>
    [Fact]
    public async Task GetByUserNameAsync()
    {
        // Arrange
        var user = await Test.AddUserAsync();

        // Act
        var result = await Test.Target.GetByUserNameAsync(user.UserName, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.Equal(user.UserName, result.Value.UserName);
    }

    /// <summary>
    /// The <see cref="UserService.GetByUserNameAsync"/> method should
    /// fail to retrieve a user
    /// when no user with the specified user-name exists.
    /// </summary>
    [Fact]
    public async Task GetByUserNameAsync_FailsOnNonExistingUser()
    {
        // Arrange
        var userName = "user";

        // Act
        var result = await Test.Target.GetByUserNameAsync(userName, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, UserUserNameNotFound);
    }

    /// <summary>
    /// The <see cref="UserService.GetUserClaimsAsync"/> method should
    /// return the list of user's claims.
    /// </summary>
    [Fact]
    public async Task GetUserClaimsAsync()
    {
        // Arrange
        var user = await Test.AddUserAsync();
        var claim = new Claim("Type", "Value");

        Test.SetUserManagerGetClaimsAsyncResult(() => Task.FromResult<IList<Claim>>([claim]));

        // Act
        var result = await Test.Target.GetUserClaimsAsync(user.Id, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.Contains(result.Value, claimModel => claimModel.Type == claim.Type && claimModel.Value == claim.Value);
    }

    /// <summary>
    /// The <see cref="UserService.GetUserClaimsAsync"/> method should
    /// fail to retrieve the list of user's claims
    /// when no user with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task GetUserClaimsAsync_FailsOnNonExistingUser()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var result = await Test.Target.GetUserClaimsAsync(id, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, UserIdNotFound);
    }

    /// <summary>
    /// The <see cref="UserService.RegisterAsync"/> method should
    /// create a new user via default user-registration flow.
    /// </summary>
    [Fact]
    public async Task RegisterAsync()
    {
        // Arrange
        var model = new UserRegistrationModel(
            userName: "UserName",
            emailAddress: "test@email.com",
            phoneNumber: "+1000000000",
            password: "Password123!",
            passwordConfirmation: "Password123!");

        // Act
        var result = await Test.Target.RegisterAsync(model, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Created, result.Status);

        Assert.True(Test.IdExists(result.Value.Id));
        var domainEvent = Assert.Single(Test.DomainEvents);
        Assert.IsType<UserRegisteredEvent>(domainEvent);
    }

    /// <summary>
    /// The <see cref="UserService.RegisterAsync"/> method should
    /// fail to create a new user if the input
    /// <see cref="UserRegistrationModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task RegisterAsync_FailsOnNull()
    {
        // Arrange
        var model = null as UserRegistrationModel;

        // Act
        var result = await Test.Target.RegisterAsync(model!, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidModel);
    }

    /// <summary>
    /// The <see cref="UserService.RegisterAsync"/> method should
    /// fail to create a new user if the input
    /// email address is invalid.
    /// </summary>
    /// <param name="emailAddress">
    /// User's email address.
    /// </param>
    [Theory, MemberData(nameof(RegisterAsync_FailsOnInvalidEmailAddress_MemberData))]
    public async Task RegisterAsync_FailsOnInvalidEmailAddress(string emailAddress)
    {
        // Arrange
        var model = new UserRegistrationModel(
            userName: "UserName",
            emailAddress: emailAddress,
            phoneNumber: "+1000000000",
            password: "Password123!",
            passwordConfirmation: "Password123!");

        // Act
        var result = await Test.Target.RegisterAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.False(Test.EmailAddressExists(model.EmailAddress));
        Assert.ContainsErrorCode(result, InvalidEmailAddress);
    }

    /// <summary>
    /// The <see cref="UserService.RegisterAsync"/> method should
    /// fail to create a new user if the input
    /// email address is already in use.
    /// </summary>
    [Fact]
    public async Task RegisterAsync_FailsOnDuplicateEmailAddress()
    {
        // Arrange
        var emailAddress = "test@email.com";
        await Test.AddUserAsync(emailAddress);

        var model = new UserRegistrationModel(
            userName: "user2",
            emailAddress: emailAddress,
            phoneNumber: "+2000000000",
            password: "Password123!",
            passwordConfirmation: "Password123!");

        // Act
        var result = await Test.Target.RegisterAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.True(Test.EmailAddressExists(emailAddress));
        Assert.ContainsErrorCode(result, DuplicateEmailAddress);
    }

    /// <summary>
    /// The <see cref="UserService.RegisterAsync"/> method should
    /// fail to create a new user if the input
    /// password is invalid.
    /// </summary>
    [Fact]
    public async Task RegisterAsync_FailsOnInvalidPassword()
    {
        // Arrange
        var model = new UserRegistrationModel(
            userName: "UserName",
            emailAddress: "test@email.com",
            phoneNumber: "+1000000000",
            password: string.Empty,
            passwordConfirmation: string.Empty);

        // Act
        var result = await Test.Target.RegisterAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.False(Test.EmailAddressExists(model.EmailAddress));
        Assert.ContainsErrorCode(result, DefaultError);
    }

    /// <summary>
    /// The <see cref="UserService.RegisterAsync"/> method should
    /// fail to create a new user if the input
    /// password does not match it's confirmation value.
    /// </summary>
    [Fact]
    public async Task RegisterAsync_FailsOnInvalidPasswordConfirmation()
    {
        // Arrange
        var model = new UserRegistrationModel(
            userName: "UserName",
            emailAddress: "test@email.com",
            phoneNumber: "+1000000000",
            password: "Password123!",
            passwordConfirmation: "Password1234!");

        // Act
        var result = await Test.Target.RegisterAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.False(Test.EmailAddressExists(model.EmailAddress));
        Assert.ContainsErrorCode(result, PasswordNotMatchConfirmation);
    }

    /// <summary>
    /// The <see cref="UserService.RegisterAsync"/> method should
    /// fail to create a new user if the input
    /// phone number is invalid.
    /// </summary>
    [Fact]
    public async Task RegisterAsync_FailsOnInvalidPhoneNumber()
    {
        // Arrange
        var model = new UserRegistrationModel(
            userName: "UserName",
            emailAddress: "test@email.com",
            phoneNumber: "Invalid phone number",
            password: "Password123!",
            passwordConfirmation: "Password123!");

        // Act
        var result = await Test.Target.RegisterAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.False(Test.PhoneNumberExists(model.PhoneNumber));
        Assert.ContainsErrorCode(result, InvalidPhoneNumber);
    }

    /// <summary>
    /// The <see cref="UserService.RegisterAsync"/> method should
    /// fail to create a new user if the input
    /// phone number is in use.
    /// </summary>
    [Fact]
    public async Task RegisterAsync_FailsOnDuplicatePhoneNumber()
    {
        // Arrange
        var phoneNumber = "+1000000000";
        await Test.AddUserAsync(phoneNumber: phoneNumber);

        var model = new UserRegistrationModel(
            userName: "user2",
            emailAddress: "user2@email.com",
            phoneNumber: phoneNumber,
            password: "Password123!",
            passwordConfirmation: "Password123!");

        // Act
        var result = await Test.Target.RegisterAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.True(Test.PhoneNumberExists(phoneNumber));
        Assert.ContainsErrorCode(result, DuplicatePhoneNumber);
    }

    /// <summary>
    /// The <see cref="UserService.RegisterAsync"/> method should
    /// fail to create a new user if the input
    /// user-name is invalid.
    /// </summary>
    [Fact]
    public async Task RegisterAsync_FailsOnInvalidUserName()
    {
        // Arrange
        var model = new UserRegistrationModel(
            userName: "!@#$%^&*()",
            emailAddress: "user2@email.com",
            phoneNumber: "+1000000000",
            password: "Password123!",
            passwordConfirmation: "Password123!");

        Test.IdentityOptions.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz";

        // Act
        var result = await Test.Target.RegisterAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.False(Test.UserNameExists(model.UserName));
        Assert.ContainsErrorCode(result, InvalidUserName);
    }

    /// <summary>
    /// The <see cref="UserService.RegisterAsync"/> method should
    /// fail to create a new user if the input
    /// user-name is in use.
    /// </summary>
    [Fact]
    public async Task RegisterAsync_FailsOnDuplicateUserName()
    {
        // Arrange
        var userName = "UserName";
        await Test.AddUserAsync(userName: userName);

        var model = new UserRegistrationModel(
            userName: userName,
            emailAddress: "user2@email.com",
            phoneNumber: "+2000000000",
            password: "Password123!",
            passwordConfirmation: "Password123!");

        // Act
        var result = await Test.Target.RegisterAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.True(Test.UserNameExists(userName));
        Assert.ContainsErrorCode(result, DuplicateUserName);
    }

    /// <summary>
    /// The <see cref="UserService.RegisterAsync"/> method should
    /// fail to create a new user if the
    /// inner <see cref="IUserManager{TRole}"/> instance reports error
    /// upon saving user data to the persistence storage.
    /// </summary>
    [Fact]
    public async Task RegisterAsync_FailsOnDependencyFailure()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to create user"
        };

        Test.SetUserManagerCreateAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        var model = new UserRegistrationModel(
            userName: "UserName",
            emailAddress: "test@email.com",
            phoneNumber: "+1000000000",
            password: "Password123!",
            passwordConfirmation: "Password123!");

        // Act
        var result = await Test.Target.RegisterAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Failure, result.Status);

        Assert.False(Test.EmailAddressExists(model.EmailAddress));
        Assert.ContainsErrorCode(result, DefaultError, error.Description);
    }

    /// <summary>
    /// The <see cref="UserService.ConfirmEmailAddressAsync"/> method should
    /// confirm the user's email address.
    /// </summary>
    [Fact]
    public async Task ConfirmEmailAddressAsync()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.EmailConfirmationTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.EmailConfirmationTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.EmailConfirmationTimeout / 2);

        var user = await Test.AddUserAsync(emailConfirmed: false);
        var identityToken = await Test.GenerateEmailAddressConfirmationIdentityTokenAsync(user, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.ConfirmEmailAddressAsync(identityToken, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.True(Test.GetEmailConfirmed(user.Id));
        var domainEvent = Assert.Single(Test.DomainEvents);
        Assert.IsType<EmailAddressConfirmedEvent>(domainEvent);
    }

    /// <summary>
    /// The <see cref="UserService.ConfirmEmailAddressAsync"/> method should
    /// fail to confirm the user's email address if the input
    /// identity token is invalid.
    /// </summary>
    [Fact]
    public async Task ConfirmEmailAddressAsync_FailsOnInvalidIdentityToken()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.EmailConfirmationTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.EmailConfirmationTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.EmailConfirmationTimeout / 2);

        var user = await Test.AddUserAsync(emailConfirmed: false);
        var identityToken = await Test.GenerateInvalidEmailAddressConfirmationIdentityTokenAsync(user, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.ConfirmEmailAddressAsync(identityToken, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidToken);
    }

    /// <summary>
    /// The <see cref="UserService.ConfirmEmailAddressAsync"/> method should
    /// fail to confirm the user's email address if the input
    /// identity token is missing the expiry date value.
    /// </summary>
    [Fact]
    public async Task ConfirmEmailAddressAsync_FailsOnInvalidIdentityToken_WithoutExpiryDate()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.EmailConfirmationTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = null as DateTimeOffset?;
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.EmailConfirmationTimeout / 2);

        var user = await Test.AddUserAsync(emailConfirmed: false);
        var identityToken = await Test.GenerateEmailAddressConfirmationIdentityTokenAsync(user, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.ConfirmEmailAddressAsync(identityToken, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidToken);
    }

    /// <summary>
    /// The <see cref="UserService.ConfirmEmailAddressAsync"/> method should
    /// fail to confirm the user's email address if the input
    /// identity token is expired.
    /// </summary>
    [Fact]
    public async Task ConfirmEmailAddressAsync_FailsOnExpiredIdentityToken()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.EmailConfirmationTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.EmailConfirmationTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.EmailConfirmationTimeout * 2);

        var user = await Test.AddUserAsync(emailConfirmed: false);
        var identityToken = await Test.GenerateEmailAddressConfirmationIdentityTokenAsync(user, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.ConfirmEmailAddressAsync(identityToken, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Blocked, result.Status);

        Assert.ContainsErrorCode(result, OutdatedToken);
    }

    /// <summary>
    /// The <see cref="UserService.ConfirmEmailAddressAsync"/> method should
    /// fail to confirm the user's email address
    /// when no user with the specified email address exists.
    /// </summary>
    [Fact]
    public async Task ConfirmEmailAddressAsync_FailsOnNonExistingUser()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.EmailConfirmationTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.EmailConfirmationTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.EmailConfirmationTimeout / 2);

        var user = await Test.AddUserAsync(emailConfirmed: false);
        var identityToken = await Test.GenerateEmailAddressConfirmationIdentityTokenAsync(user, identityTokenExpiryDate);

        await Test.DeleteUserAsync(user);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.ConfirmEmailAddressAsync(identityToken, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, UserEmailAddressNotFound);
    }

    /// <summary>
    /// The <see cref="UserService.ConfirmEmailAddressAsync"/> method should
    /// fail to confirm the user's email address
    /// when the user's email address is already confirmed.
    /// </summary>
    [Fact]
    public async Task ConfirmEmailAddressAsync_FailsOnCompletedEmailAddressConfirmation()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.EmailConfirmationTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.EmailConfirmationTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.EmailConfirmationTimeout / 2);

        var user = await Test.AddUserAsync(emailConfirmed: true);
        var identityToken = await Test.GenerateEmailAddressConfirmationIdentityTokenAsync(user, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.ConfirmEmailAddressAsync(identityToken, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Blocked, result.Status);

        Assert.ContainsErrorCode(result, UserEmailAddressAlreadyConfirmed);
    }

    /// <summary>
    /// The <see cref="UserService.ConfirmEmailAddressAsync"/> method should
    /// fail to confirm the user's email address if the input
    /// email confirmation token is expired.
    /// </summary>
    [Fact]
    public async Task ConfirmEmailAddressAsync_FailsOnExpiredConfirmationToken()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.EmailConfirmationTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.EmailConfirmationTimeout * 3);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.EmailConfirmationTimeout * 2);

        var user = await Test.AddUserAsync(emailConfirmed: false);
        var identityToken = await Test.GenerateEmailAddressConfirmationIdentityTokenAsync(user, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.ConfirmEmailAddressAsync(identityToken, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Blocked, result.Status);

        Assert.ContainsErrorCode(result, OutdatedToken);
    }

    /// <summary>
    /// The <see cref="UserService.ConfirmEmailAddressAsync"/> method should
    /// fail to confirm the user's email address if the
    /// inner <see cref="IUserManager{TRole}"/> instance reports error
    /// upon confirming user email address.
    /// </summary>
    [Fact]
    public async Task ConfirmEmailAddressAsync_FailsOnDependencyFailure()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to confirm email address"
        };

        Test.SetUserManagerConfirmEmailAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        Test.ApplicationOptions.Timeout.EmailConfirmationTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.EmailConfirmationTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.EmailConfirmationTimeout / 2);

        var user = await Test.AddUserAsync(emailConfirmed: false);
        var identityToken = await Test.GenerateEmailAddressConfirmationIdentityTokenAsync(user, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.ConfirmEmailAddressAsync(identityToken, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Failure, result.Status);

        Assert.False(Test.GetEmailConfirmed(user.Id));
        Assert.ContainsErrorCode(result, DefaultError, error.Description);
    }

    /// <summary>
    /// The <see cref="UserService.CheckPasswordAsync"/> method should
    /// return successful password verification result.
    /// </summary>
    [Fact]
    public async Task CheckPasswordAsync()
    {
        // Arrange
        var password = "Password123!";
        var user = await Test.AddUserAsync(emailConfirmed: true,
                                           password: password);

        // Act
        var result = await Test.Target.CheckPasswordAsync(user.Id, password, Token);

        // Assert
        Assert.Equal(Success, result.Status);
    }

    /// <summary>
    /// The <see cref="UserService.CheckPasswordAsync"/> method should
    /// return successful password verification result
    /// and reset the user's failed access counter.
    /// </summary>
    [Fact]
    public async Task CheckPasswordAsync_ResetsUserLockoutStateOnSuccess()
    {
        // Arrange
        var password = "Password123!";
        var user = await Test.AddUserAsync(emailConfirmed: true,
                                           password: password,
                                           accessFailedCount: 2);

        // Act
        var result = await Test.Target.CheckPasswordAsync(user.Id, password, Token);

        // Assert
        Assert.Equal(Success, result.Status);
        Assert.Equal(0, Test.GetAccessFailedCount(user.Id));
    }

    /// <summary>
    /// The <see cref="UserService.CheckPasswordAsync"/> method should
    /// return successful password verification result
    /// but fail to reset the user's failed access counter if the
    /// inner <see cref="IUserManager{TRole}"/> instance reports error
    /// upon updating user data in the persistence storage.
    /// </summary>
    [Fact]
    public async Task CheckPasswordAsync_LogsResetUserLockoutStateFailure()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to update user"
        };

        Test.SetUserManagerUpdateAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        var password = "Password123!";
        var user = await Test.AddUserAsync(emailConfirmed: true,
                                           password: password,
                                           accessFailedCount: 2);

        // Act
        var result = await Test.Target.CheckPasswordAsync(user.Id, password, Token);

        // Assert
        Assert.Equal(Success, result.Status);

        Assert.Contains(Test.LoggedMessages, message => message.RawMessage.Contains(error.Description, StringComparison.Ordinal));
    }

    /// <summary>
    /// The <see cref="UserService.CheckPasswordAsync"/> method should
    /// fail to return password verification result
    /// when no user with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task CheckPasswordAsync_FailsOnNonExistingUser()
    {
        // Arrange
        var password = "Password123!";
        var id = Guid.Empty;

        // Act
        var result = await Test.Target.CheckPasswordAsync(id, password, Token);

        // Assert
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, UserIdNotFound);
    }

    /// <summary>
    /// The <see cref="UserService.CheckPasswordAsync"/> method should
    /// return unsuccessful password verification result
    /// when the user is locked out.
    /// </summary>
    [Fact]
    public async Task CheckPasswordAsync_FailsOnUserLockedOut()
    {
        // Arrange
        var password = "Password123!";
        var user = await Test.AddUserAsync(emailConfirmed: true,
                                           password: password,
                                           lockoutEnabled: true,
                                           lockoutEnd: Test.UtcNow.AddDays(1));

        // Act
        var result = await Test.Target.CheckPasswordAsync(user.Id, password, Token);

        // Assert
        Assert.Equal(Prohibited, result.Status);

        Assert.ContainsErrorCode(result, UserLockedOut);
    }

    /// <summary>
    /// The <see cref="UserService.CheckPasswordAsync"/> method should
    /// return unsuccessful password verification result
    /// when the input password does not match the user's password,
    /// and increment the user's access failures counter
    /// if lockout is enabled for the user.
    /// </summary>
    /// <param name="lockoutEnabled">
    /// A flag indicating if the user could be locked out.
    /// </param>
    [Theory, MemberData(nameof(CheckPasswordAsync_FailsOnPasswordMismatch_MemberData))]
    public async Task CheckPasswordAsync_FailsOnPasswordMismatch(bool lockoutEnabled)
    {
        // Arrange
        var expectedAccessFailedCount = lockoutEnabled
            ? 1
            : 0;

        var password = "Password123!";
        var user = await Test.AddUserAsync(emailConfirmed: true,
                                           password: password,
                                           lockoutEnabled: lockoutEnabled);

        // Act
        var result = await Test.Target.CheckPasswordAsync(user.Id, password + "Mismatch", Token);

        // Assert
        Assert.Equal(Unauthorized, result.Status);

        Assert.Equal(expectedAccessFailedCount, Test.GetAccessFailedCount(user.Id));
        Assert.ContainsErrorCode(result, UserNotFoundOrPasswordMismatch);
    }

    /// <summary>
    /// The <see cref="UserService.CheckPasswordAsync"/> method should
    /// return unsuccessful password verification result
    /// when the input password does not match the user's password,
    /// but fail to increment the user's access failures counter if the
    /// inner <see cref="IUserManager{TRole}"/> instance reports error
    /// upon incrementing user access failures counter.
    /// </summary>
    [Fact]
    public async Task CheckPasswordAsync_FailsOnPasswordMismatch_LogsAccessFailedCountIncrementFailure()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to increment access failure counter"
        };

        Test.SetUserManagerAccessFailedAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        var password = "Password123!";
        var user = await Test.AddUserAsync(emailConfirmed: true,
                                           password: password,
                                           lockoutEnabled: true);
        // Act
        var result = await Test.Target.CheckPasswordAsync(user.Id, password + "Mismatch", Token);

        // Assert
        Assert.Equal(Unauthorized, result.Status);

        Assert.Equal(0, Test.GetAccessFailedCount(user.Id));
        Assert.Contains(Test.LoggedMessages, message => message.RawMessage.Contains(error.Description, StringComparison.Ordinal));
        Assert.ContainsErrorCode(result, UserNotFoundOrPasswordMismatch);
    }

    /// <summary>
    /// The <see cref="UserService.CreatePasswordResetRequestAsync"/> method should
    /// create a password reset request and submit corresponding domain event.
    /// </summary>
    [Fact]
    public async Task CreatePasswordResetRequestAsync()
    {
        // Arrange
        var user = await Test.AddUserAsync();

        var model = new UserResetPasswordRequestModel(user.Email);

        // Act
        var result = await Test.Target.CreatePasswordResetRequestAsync(model, Token);

        // Assert
        Assert.Equal(Received, result.Status);

        var domainEvent = Assert.Single(Test.DomainEvents);
        Assert.IsType<PasswordResetRequestedEvent>(domainEvent);
    }

    /// <summary>
    /// The <see cref="UserService.CreatePasswordResetRequestAsync"/> method should
    /// fail to create a password reset request
    /// when no user with the specified email address exists,
    /// yet return the same operation status.
    /// </summary>
    [Fact]
    public async Task CreatePasswordResetRequestAsync_IgnoresMissingUser()
    {
        // Arrange
        var model = new UserResetPasswordRequestModel("test@email.com");

        // Act
        var result = await Test.Target.CreatePasswordResetRequestAsync(model, Token);

        // Assert
        Assert.Equal(Received, result.Status);

        Assert.Empty(Test.DomainEvents);
    }

    /// <summary>
    /// The <see cref="UserService.CreatePasswordResetRequestAsync"/> method should
    /// fail to create a password reset request if the input
    /// <see cref="UserResetPasswordRequestModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task CreatePasswordResetRequestAsync_FailsOnNull()
    {
        // Arrange
        var model = null as UserResetPasswordRequestModel;

        // Act
        var result = await Test.Target.CreatePasswordResetRequestAsync(model!, Token);

        // Assert
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidModel);
    }

    /// <summary>
    /// The <see cref="UserService.CreatePasswordResetRequestAsync"/> method should
    /// fail to create a password reset request if the input
    /// email address is invalid.
    /// </summary>
    /// <param name="emailAddress">
    /// User's email address.
    /// </param>
    [Theory, MemberData(nameof(CreatePasswordResetRequestAsync_FailsOnInvalidEmailAddress_MemberData))]
    public async Task CreatePasswordResetRequestAsync_FailsOnInvalidEmailAddress(string? emailAddress)
    {
        // Arrange
        var model = new UserResetPasswordRequestModel(emailAddress!);

        // Act
        var result = await Test.Target.CreatePasswordResetRequestAsync(model, Token);

        // Assert
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidEmailAddress);
    }

    /// <summary>
    /// The <see cref="UserService.ResetPasswordAsync"/> method should
    /// reset the user's password.
    /// </summary>
    [Fact]
    public async Task ResetPasswordAsync()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.ResetPasswordTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetPasswordTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetPasswordTimeout / 2);

        var password = "Password123!";
        var newPassword = "New" + password;

        var user = await Test.AddUserAsync(password: password);

        var identityToken = await Test.GeneratePasswordResetIdentityTokenAsync(user, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        var model = new UserResetPasswordModel(
            identityToken: identityToken,
            password: newPassword,
            passwordConfirmation: newPassword);

        // Act
        var result = await Test.Target.ResetPasswordAsync(model, Token);

        // Assert
        Assert.Equal(Success, result.Status);

        Assert.Equal(newPassword, Test.GetPassword(user.Id));
        var domainEvent = Assert.Single(Test.DomainEvents);
        Assert.IsType<PasswordResetCompletedEvent>(domainEvent);
    }

    /// <summary>
    /// The <see cref="UserService.ResetPasswordAsync"/> method should
    /// fail to reset the user's password if the input
    /// <see cref="UserResetPasswordModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task ResetPasswordAsync_FailsOnNull()
    {
        // Arrange
        var model = null as UserResetPasswordModel;

        // Act
        var result = await Test.Target.ResetPasswordAsync(model!, Token);

        // Assert
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidModel);
    }

    /// <summary>
    /// The <see cref="UserService.ResetPasswordAsync"/> method should
    /// fail to reset the user's password if the <paramref name="newPassword"/>
    /// is equal to <see langword="null"/>, <see cref="string.Empty"/> or whitespace-only.
    /// </summary>
    /// <param name="newPassword">
    /// User's new password.
    /// </param>
    [Theory, MemberData(nameof(ResetPasswordAsync_FailsOnInvalidPassword_MemberData))]
    public async Task ResetPasswordAsync_FailsOnInvalidPassword(string? newPassword)
    {
        // Arrange
        Test.ApplicationOptions.Timeout.ResetPasswordTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetPasswordTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetPasswordTimeout / 2);

        var password = "Password123!";

        var user = await Test.AddUserAsync(password: password);

        var identityToken = await Test.GeneratePasswordResetIdentityTokenAsync(user, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        var model = new UserResetPasswordModel(
            identityToken: identityToken,
            password: newPassword!,
            passwordConfirmation: newPassword!);

        // Act
        var result = await Test.Target.ResetPasswordAsync(model, Token);

        // Assert
        Assert.Equal(InvalidInput, result.Status);

        Assert.Equal(password, Test.GetPassword(user.Id));
        Assert.ContainsErrorCode(result, PasswordMissing);
    }

    /// <summary>
    /// The <see cref="UserService.ResetPasswordAsync"/> method should
    /// fail to reset the user's password if the input
    /// identity token is invalid.
    /// </summary>
    [Fact]
    public async Task ResetPasswordAsync_FailsOnInvalidIdentityToken()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.ResetPasswordTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetPasswordTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetPasswordTimeout / 2);

        var password = "Password123!";
        var newPassword = "New" + password;

        var user = await Test.AddUserAsync(password: password);

        var identityToken = await Test.GenerateInvalidPasswordResetIdentityTokenAsync(user, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        var model = new UserResetPasswordModel(
            identityToken: identityToken,
            password: newPassword,
            passwordConfirmation: newPassword);

        // Act
        var result = await Test.Target.ResetPasswordAsync(model, Token);

        // Assert
        Assert.Equal(InvalidInput, result.Status);

        Assert.Equal(password, Test.GetPassword(user.Id));
        Assert.ContainsErrorCode(result, InvalidToken);
    }

    /// <summary>
    /// The <see cref="UserService.ResetPasswordAsync"/> method should
    /// fail to reset the user's password if the input
    /// identity token is expired.
    /// </summary>
    [Fact]
    public async Task ResetPasswordAsync_FailsOnExpiredIdentityToken()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.ResetPasswordTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetPasswordTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetPasswordTimeout * 2);

        var password = "Password123!";
        var newPassword = "New" + password;

        var user = await Test.AddUserAsync(password: password);

        var identityToken = await Test.GeneratePasswordResetIdentityTokenAsync(user, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        var model = new UserResetPasswordModel(
            identityToken: identityToken,
            password: newPassword,
            passwordConfirmation: newPassword);

        // Act
        var result = await Test.Target.ResetPasswordAsync(model, Token);

        // Assert
        Assert.Equal(Blocked, result.Status);

        Assert.Equal(password, Test.GetPassword(user.Id));
        Assert.ContainsErrorCode(result, OutdatedToken);
    }

    /// <summary>
    /// The <see cref="UserService.ResetPasswordAsync"/> method should
    /// fail to reset the user's password if the input
    /// password does not match it's confirmation value.
    /// </summary>
    [Fact]
    public async Task ResetPasswordAsync_FailsOnInvalidPasswordConfirmation()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.ResetPasswordTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetPasswordTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetPasswordTimeout / 2);

        var password = "Password123!";
        var newPassword = "New" + password;

        var user = await Test.AddUserAsync(password: password);

        var identityToken = await Test.GeneratePasswordResetIdentityTokenAsync(user, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        var model = new UserResetPasswordModel(
            identityToken: identityToken,
            password: newPassword,
            passwordConfirmation: newPassword + "Mismatch");

        // Act
        var result = await Test.Target.ResetPasswordAsync(model, Token);

        // Assert
        Assert.Equal(InvalidInput, result.Status);

        Assert.Equal(password, Test.GetPassword(user.Id));
        Assert.ContainsErrorCode(result, PasswordNotMatchConfirmation);
    }

    /// <summary>
    /// The <see cref="UserService.ResetPasswordAsync"/> method should
    /// fail to reset the user's password
    /// when no user with the specified email address exists.
    /// </summary>
    [Fact]
    public async Task ResetPasswordAsync_FailsOnNonExistingUser()
    {
        // Arrange
        var password = "Password123!";
        var newPassword = "New" + password;

        var user = await Test.AddUserAsync(password: password);

        var identityToken = await Test.GeneratePasswordResetIdentityTokenAsync(user);

        await Test.DeleteUserAsync(user);

        var model = new UserResetPasswordModel(
            identityToken: identityToken,
            password: newPassword,
            passwordConfirmation: newPassword);

        // Act
        var result = await Test.Target.ResetPasswordAsync(model, Token);

        // Assert
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, UserEmailAddressNotFound);
    }

    /// <summary>
    /// The <see cref="UserService.ResetPasswordAsync"/> method should
    /// fail to reset the user's password if the
    /// inner <see cref="IUserManager{TRole}"/> instance reports error
    /// upon changing user password.
    /// </summary>
    [Fact]
    public async Task ResetPasswordAsync_FailsOnDependencyFailure()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to reset password"
        };

        Test.SetUserManagerResetPasswordAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        Test.ApplicationOptions.Timeout.ResetPasswordTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetPasswordTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetPasswordTimeout / 2);

        var password = "Password123!";
        var newPassword = "New" + password;

        var user = await Test.AddUserAsync(password: password);

        var identityToken = await Test.GeneratePasswordResetIdentityTokenAsync(user, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        var model = new UserResetPasswordModel(
            identityToken: identityToken,
            password: newPassword,
            passwordConfirmation: newPassword);

        // Act
        var result = await Test.Target.ResetPasswordAsync(model, Token);

        // Assert
        Assert.Equal(Failure, result.Status);

        Assert.Equal(password, Test.GetPassword(user.Id));
        Assert.ContainsErrorCode(result, DefaultError, error.Description);
    }

    /// <summary>
    /// The <see cref="UserService.CreateEmailAddressResetRequestAsync"/> method should
    /// create an email address reset request and submit corresponding domain event.
    /// </summary>
    [Fact]
    public async Task CreateEmailAddressResetRequestAsync()
    {
        // Arrange
        var newEmailAddress = "new@email.com";

        var user = await Test.AddUserAsync();

        var model = new UserResetEmailAddressRequestModel(
            emailAddress: newEmailAddress,
            emailAddressConfirmation: newEmailAddress);

        // Act
        var result = await Test.Target.CreateEmailAddressResetRequestAsync(user.Id, model, Token);

        // Assert
        Assert.Equal(Success, result.Status);

        var domainEvent = Assert.Single(Test.DomainEvents);
        Assert.IsType<EmailAddressResetRequestedEvent>(domainEvent);
    }

    /// <summary>
    /// The <see cref="UserService.CreateEmailAddressResetRequestAsync"/> method should
    /// fail to create an email address reset request if the input
    /// <see cref="UserResetEmailAddressRequestModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task CreateEmailAddressResetRequestAsync_FailsOnNull()
    {
        // Arrange
        var user = await Test.AddUserAsync();

        var model = null as UserResetEmailAddressRequestModel;

        // Act
        var result = await Test.Target.CreateEmailAddressResetRequestAsync(user.Id, model!, Token);

        // Assert
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidModel);
    }

    /// <summary>
    /// The <see cref="UserService.CreateEmailAddressResetRequestAsync"/> method should
    /// fail to create an email address reset request if the input
    /// email address is invalid.
    /// </summary>
    /// <param name="emailAddress">
    /// User's new email address.
    /// </param>
    [Theory, MemberData(nameof(CreateEmailAddressResetRequestAsync_FailsOnInvalidEmailAddress_MemberData))]
    public async Task CreateEmailAddressResetRequestAsync_FailsOnInvalidEmailAddress(string? emailAddress)
    {
        // Arrange
        var user = await Test.AddUserAsync();

        var model = new UserResetEmailAddressRequestModel(
            emailAddress: emailAddress!,
            emailAddressConfirmation: emailAddress!);

        // Act
        var result = await Test.Target.CreateEmailAddressResetRequestAsync(user.Id, model, Token);

        // Assert
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidEmailAddress);
    }

    /// <summary>
    /// The <see cref="UserService.CreateEmailAddressResetRequestAsync"/> method should
    /// fail to create an email address reset request if the input
    /// email address does not match it's confirmation value.
    /// </summary>
    [Fact]
    public async Task CreateEmailAddressResetRequestAsync_FailsOnInvalidEmailAddressConfirmation()
    {
        // Arrange
        var newEmailAddress = "new@email.com";

        var user = await Test.AddUserAsync();

        var model = new UserResetEmailAddressRequestModel(
            emailAddress: newEmailAddress,
            emailAddressConfirmation: newEmailAddress + "Mismatch");

        // Act
        var result = await Test.Target.CreateEmailAddressResetRequestAsync(user.Id, model, Token);

        // Assert
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, EmailAddressNotMatchConfirmation);
    }

    /// <summary>
    /// The <see cref="UserService.CreateEmailAddressResetRequestAsync"/> method should
    /// fail to create an email address reset request if the input
    /// email address is already in use.
    /// </summary>
    [Fact]
    public async Task CreateEmailAddressResetRequestAsync_FailsOnDuplicateEmailAddress()
    {
        // Arrange
        var emailAddress = "new@email.com";

        await Test.AddUserAsync(email: emailAddress);
        var user = await Test.AddUserAsync();

        var model = new UserResetEmailAddressRequestModel(
            emailAddress: emailAddress,
            emailAddressConfirmation: emailAddress);

        // Act
        var result = await Test.Target.CreateEmailAddressResetRequestAsync(user.Id, model, Token);

        // Assert
        Assert.Equal(Blocked, result.Status);

        Assert.ContainsErrorCode(result, DuplicateEmailAddress);
    }

    /// <summary>
    /// The <see cref="UserService.CreateEmailAddressResetRequestAsync"/> method should
    /// fail to create an email address reset request
    /// when no user with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task CreateEmailAddressResetRequestAsync_FailsOnNonExistingUser()
    {
        // Arrange
        var newEmailAddress = "new@email.com";

        var user = await Test.AddUserAsync();

        var model = new UserResetEmailAddressRequestModel(
            emailAddress: newEmailAddress,
            emailAddressConfirmation: newEmailAddress);

        await Test.DeleteUserAsync(user);

        // Act
        var result = await Test.Target.CreateEmailAddressResetRequestAsync(user.Id, model, Token);

        // Assert
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, UserIdNotFound);
    }

    /// <summary>
    /// The <see cref="UserService.ResetEmailAddressAsync"/> method should
    /// reset the user's email address.
    /// </summary>
    [Fact]
    public async Task ResetEmailAddressAsync()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout / 2);

        var oldEmailAddress = "old@email.com";
        var newEmailAddress = "new@email.com";

        var user = await Test.AddUserAsync(email: oldEmailAddress);

        var identityToken = await Test.GenerateEmailAddressResetIdentityTokenAsync(user, newEmailAddress, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.ResetEmailAddressAsync(identityToken, Token);

        // Assert
        Assert.Equal(Success, result.Status);

        Assert.Equal(newEmailAddress, Test.GetEmailAddress(user.Id));
        var domainEvent = Assert.Single(Test.DomainEvents);
        Assert.IsType<EmailAddressResetCompletedEvent>(domainEvent);
    }

    /// <summary>
    /// The <see cref="UserService.ResetEmailAddressAsync"/> method should
    /// fail to reset the user's email address if the input
    /// identity token is invalid.
    /// </summary>
    [Fact]
    public async Task ResetEmailAddressAsync_FailsOnInvalidIdentityToken()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout / 2);

        var oldEmailAddress = "old@email.com";
        var newEmailAddress = "new@email.com";

        var user = await Test.AddUserAsync(email: oldEmailAddress);

        var identityToken = await Test.GenerateInvalidEmailAddressResetIdentityTokenAsync(user, newEmailAddress, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.ResetEmailAddressAsync(identityToken, Token);

        // Assert
        Assert.Equal(InvalidInput, result.Status);

        Assert.Equal(oldEmailAddress, Test.GetEmailAddress(user.Id));
        Assert.ContainsErrorCode(result, InvalidToken);
    }

    /// <summary>
    /// The <see cref="UserService.ResetEmailAddressAsync"/> method should
    /// fail to reset the user's password if the input
    /// identity token is expired.
    /// </summary>
    [Fact]
    public async Task ResetEmailAddressAsync_FailsOnExpiredIdentityToken()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout * 2);

        var oldEmailAddress = "old@email.com";
        var newEmailAddress = "new@email.com";

        var user = await Test.AddUserAsync(email: oldEmailAddress);

        var identityToken = await Test.GenerateEmailAddressResetIdentityTokenAsync(user, newEmailAddress, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.ResetEmailAddressAsync(identityToken, Token);

        // Assert
        Assert.Equal(Blocked, result.Status);

        Assert.Equal(oldEmailAddress, Test.GetEmailAddress(user.Id));
        Assert.ContainsErrorCode(result, OutdatedToken);
    }

    /// <summary>
    /// The <see cref="UserService.ResetEmailAddressAsync"/> method should
    /// fail to reset the user's email address if the input
    /// email address is invalid.
    /// </summary>
    /// <param name="newEmailAddress">
    /// The new email address.
    /// </param>
    [Theory, MemberData(nameof(ResetEmailAddressAsync_FailsOnInvalidEmailAddress_MemberData))]
    public async Task ResetEmailAddressAsync_FailsOnInvalidEmailAddress(string? newEmailAddress)
    {
        // Arrange
        Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout / 2);

        var oldEmailAddress = "old@email.com";

        var user = await Test.AddUserAsync(email: oldEmailAddress);

        var identityToken = await Test.GenerateEmailAddressResetIdentityTokenAsync(user, newEmailAddress!, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.ResetEmailAddressAsync(identityToken, Token);

        // Assert
        Assert.Equal(InvalidInput, result.Status);

        Assert.Equal(oldEmailAddress, Test.GetEmailAddress(user.Id));
        Assert.ContainsErrorCode(result, InvalidToken);
    }

    /// <summary>
    /// The <see cref="UserService.ResetEmailAddressAsync"/> method should
    /// fail to reset the user's email address if the input
    /// email address is already in use.
    /// </summary>
    [Fact]
    public async Task ResetEmailAddressAsync_FailsOnDuplicateEmailAddress()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout / 2);

        var oldEmailAddress = "old@email.com";
        var newEmailAddress = "new@email.com";

        await Test.AddUserAsync(email: newEmailAddress);
        var user = await Test.AddUserAsync(email: oldEmailAddress);

        var identityToken = await Test.GenerateEmailAddressResetIdentityTokenAsync(user, newEmailAddress, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.ResetEmailAddressAsync(identityToken, Token);

        // Assert
        Assert.Equal(Blocked, result.Status);

        Assert.Equal(oldEmailAddress, Test.GetEmailAddress(user.Id));
        Assert.ContainsErrorCode(result, DuplicateEmailAddress);
    }

    /// <summary>
    /// The <see cref="UserService.ResetEmailAddressAsync"/> method should
    /// fail to reset the user's email address
    /// when no user with the specified email address exists.
    /// </summary>
    [Fact]
    public async Task ResetEmailAddressAsync_FailsOnNonExistingUser()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout / 2);

        var oldEmailAddress = "old@email.com";
        var newEmailAddress = "new@email.com";

        var user = await Test.AddUserAsync(email: oldEmailAddress);

        var identityToken = await Test.GenerateEmailAddressResetIdentityTokenAsync(user, newEmailAddress, identityTokenExpiryDate);

        await Test.DeleteUserAsync(user);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.ResetEmailAddressAsync(identityToken, Token);

        // Assert
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, UserEmailAddressNotFound);
    }

    /// <summary>
    /// The <see cref="UserService.ResetEmailAddressAsync"/> method should
    /// fail to reset the user's email address if the
    /// inner <see cref="IUserManager{TRole}"/> instance reports error
    /// upon changing user email address.
    /// </summary>
    [Fact]
    public async Task ResetEmailAddressAsync_FailsOnDependencyFailure()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to change email address"
        };

        Test.SetUserManagerChangeEmailAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout = TimeSpan.FromDays(1);

        var identityTokenExpiryDate = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout);
        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout / 2);

        var oldEmailAddress = "old@email.com";
        var newEmailAddress = "new@email.com";

        var user = await Test.AddUserAsync(email: oldEmailAddress);

        var identityToken = await Test.GenerateEmailAddressResetIdentityTokenAsync(user, newEmailAddress, identityTokenExpiryDate);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.ResetEmailAddressAsync(identityToken, Token);

        // Assert
        Assert.Equal(Failure, result.Status);

        Assert.Equal(oldEmailAddress, Test.GetEmailAddress(user.Id));
        Assert.ContainsErrorCode(result, DefaultError, error.Description);
    }

    /// <summary>
    /// The <see cref="UserService.UpdateAsync"/> method should
    /// update a user.
    /// </summary>
    /// <param name="isPendingDeletion">
    /// Indicates whether the user is pending deletion.
    /// </param>
    [Theory, MemberData(nameof(UpdateAsync_MemberData))]
    public async Task UpdateAsync(bool? isPendingDeletion)
    {
        // Arrange
        var oldUserName = "Old";
        var newUserName = "New";

        var user = await Test.AddUserAsync(userName: oldUserName);

        var model = new UserUpdateModel
        {
            IsPendingDeletion = isPendingDeletion,
            TwoFactorEnabled = true,
            UserName = newUserName
        };

        // Act
        var result = await Test.Target.UpdateAsync(user.Id, model, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.Equal(model.IsPendingDeletion == true, Test.GetDeletionRequestSubmitted(user.Id) is not null);
        Assert.Equal(model.TwoFactorEnabled, Test.GetTwoFactorEnabled(user.Id));
        Assert.Equal(model.UserName, Test.GetUserName(user.Id));
    }

    /// <summary>
    /// The <see cref="UserService.UpdateAsync"/> method should
    /// fail to update a user if the input
    /// <see cref="UserUpdateModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_FailsOnNull()
    {
        // Arrange
        var user = await Test.AddUserAsync();

        var model = null as UserUpdateModel;

        // Act
        var result = await Test.Target.UpdateAsync(user.Id, model!, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidModel);
    }

    /// <summary>
    /// The <see cref="UserService.UpdateAsync"/> method should
    /// fail to update a user
    /// when no user with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_FailsOnNonExistingUser()
    {
        // Arrange
        var id = Guid.Empty;

        var model = new UserUpdateModel
        {
            IsPendingDeletion = null,
            TwoFactorEnabled = true,
            UserName = "New"
        };

        // Act
        var result = await Test.Target.UpdateAsync(id, model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, UserIdNotFound);
    }

    /// <summary>
    /// The <see cref="UserService.UpdateAsync"/> method should
    /// fail to update a user if the input
    /// user-name is invalid.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_FailsOnInvalidUserName()
    {
        // Arrange
        var oldUserName = "Old";
        var newUserName = "!@#$%^&*()";

        var user = await Test.AddUserAsync(userName: oldUserName);

        var model = new UserUpdateModel
        {
            IsPendingDeletion = null,
            TwoFactorEnabled = true,
            UserName = newUserName
        };

        // Act
        var result = await Test.Target.UpdateAsync(user.Id, model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.Equal(oldUserName, Test.GetUserName(user.Id));
        Assert.ContainsErrorCode(result, InvalidUserName);
    }

    /// <summary>
    /// The <see cref="UserService.UpdateAsync"/> method should
    /// fail to update a user if the input
    /// user-name is in use.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_FailsOnDuplicateUserName()
    {
        // Arrange
        var oldUserName = "Old";
        var newUserName = "New";

        await Test.AddUserAsync(userName: newUserName);
        var user = await Test.AddUserAsync(userName: oldUserName);

        var model = new UserUpdateModel
        {
            IsPendingDeletion = null,
            TwoFactorEnabled = true,
            UserName = newUserName
        };

        // Act
        var result = await Test.Target.UpdateAsync(user.Id, model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Blocked, result.Status);

        Assert.Equal(oldUserName, Test.GetUserName(user.Id));
        Assert.ContainsErrorCode(result, DuplicateUserName);
    }

    /// <summary>
    /// The <see cref="UserService.UpdateAsync"/> method should
    /// fail to update a user if the
    /// inner <see cref="IUserManager{TRole}"/> instance reports error
    /// upon updating user data in the persistence storage.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_FailsOnDependencyFailure()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to update user"
        };

        Test.SetUserManagerUpdateAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        var oldUserName = "Old";
        var newUserName = "New";

        var user = await Test.AddUserAsync(userName: oldUserName);

        var model = new UserUpdateModel
        {
            IsPendingDeletion = null,
            TwoFactorEnabled = true,
            UserName = newUserName
        };

        // Act
        var result = await Test.Target.UpdateAsync(user.Id, model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Failure, result.Status);

        Assert.ContainsErrorCode(result, DefaultError, error.Description);
    }

    /// <summary>
    /// The <see cref="UserService.DeleteAsync"/> method should
    /// delete a user.
    /// </summary>
    [Fact]
    public async Task DeleteAsync()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.UserDeletionRequestTimeout = TimeSpan.FromDays(1);

        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.UserDeletionRequestTimeout / 2);

        var user = await Test.AddUserAsync(deletionRequestSubmitted: Test.UtcNow);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.DeleteAsync(user.Id, Token);

        // Assert
        Assert.Equal(Success, result.Status);

        Assert.False(Test.IdExists(user.Id));
    }

    /// <summary>
    /// The <see cref="UserService.DeleteAsync"/> method should
    /// fail to delete a user
    /// when no user with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_FailsOnNonExistingUser()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var result = await Test.Target.DeleteAsync(id, Token);

        // Assert
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, UserIdNotFound);
    }

    /// <summary>
    /// The <see cref="UserService.DeleteAsync"/> method should
    /// fail to delete a user
    /// when the specified user does not pending deletion.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_FailsOnUserNotPendingDeletion()
    {
        // Arrange
        var user = await Test.AddUserAsync(deletionRequestSubmitted: null);

        // Act
        var result = await Test.Target.DeleteAsync(user.Id, Token);

        // Assert
        Assert.Equal(InvalidInput, result.Status);

        Assert.True(Test.IdExists(user.Id));
        Assert.ContainsErrorCode(result, UserNotPendingDeletion);
    }

    /// <summary>
    /// The <see cref="UserService.DeleteAsync"/> method should
    /// fail to delete a user
    /// when the specified user's deletion request is expired
    /// and cancel such request.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_FailsOnExpiredDeletionRequest()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.UserDeletionRequestTimeout = TimeSpan.FromDays(1);

        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.UserDeletionRequestTimeout * 2);

        var user = await Test.AddUserAsync(deletionRequestSubmitted: Test.UtcNow);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.DeleteAsync(user.Id, Token);

        // Assert
        Assert.Equal(InvalidInput, result.Status);

        Assert.Null(Test.GetDeletionRequestSubmitted(user.Id));
        Assert.ContainsErrorCode(result, UserDeletionRequestExpired);
    }

    /// <summary>
    /// The <see cref="UserService.DeleteAsync"/> method should
    /// fail to delete a user
    /// when the specified user's deletion request is expired
    /// and fail to cancel expired deletion request if the
    /// inner <see cref="IUserManager{TRole}"/> instance reports error
    /// upon updating user data in the persistence storage.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_FailsOnExpiredDeletionRequest_LogsDeletionRequestCancellationFailure()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to update user"
        };

        Test.SetUserManagerUpdateAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        Test.ApplicationOptions.Timeout.UserDeletionRequestTimeout = TimeSpan.FromDays(1);

        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.UserDeletionRequestTimeout * 2);

        var user = await Test.AddUserAsync(deletionRequestSubmitted: Test.UtcNow);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.DeleteAsync(user.Id, Token);

        // Assert
        Assert.Equal(InvalidInput, result.Status);

        Assert.Contains(Test.LoggedMessages, message => message.RawMessage.Contains(error.Description, StringComparison.Ordinal));
        Assert.ContainsErrorCode(result, UserDeletionRequestExpired);
    }

    /// <summary>
    /// The <see cref="UserService.DeleteAsync"/> method should
    /// fail to delete a user if the
    /// inner <see cref="IUserManager{TRole}"/> instance reports error
    /// upon deleting user data from the persistence storage.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_FailsOnDependencyFailure()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to delete user"
        };

        Test.SetUserManagerDeleteAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        Test.ApplicationOptions.Timeout.UserDeletionRequestTimeout = TimeSpan.FromDays(1);

        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.UserDeletionRequestTimeout / 2);

        var user = await Test.AddUserAsync(deletionRequestSubmitted: Test.UtcNow);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.DeleteAsync(user.Id, Token);

        // Assert
        Assert.Equal(Failure, result.Status);

        Assert.True(Test.IdExists(user.Id));
        Assert.ContainsErrorCode(result, DefaultError, error.Description);
    }

    /// <summary>
    /// The <see cref="UserService.DeleteUnconfirmedUsersAsync"/> method should
    /// delete all users who did not confirm their email addresses.
    /// </summary>
    [Fact]
    public async Task DeleteUnconfirmedUsersAsync()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.EmailConfirmationTimeout = TimeSpan.FromDays(1);

        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.EmailConfirmationTimeout * 2);

        var user1 = await Test.AddUserAsync(email: "1@email.com",
                                            userName: "user1",
                                            emailConfirmed: false);

        var user2 = await Test.AddUserAsync(email: "2@email.com",
                                            userName: "user2",
                                            emailConfirmed: false);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.DeleteUnconfirmedUsersAsync(Token);

        // Assert
        Assert.Equal(Success, result.Status);

        Assert.Equal(2, result.Value);
        Assert.False(Test.IdExists(user1.Id));
        Assert.False(Test.IdExists(user2.Id));
    }

    /// <summary>
    /// The <see cref="UserService.CancelExpiredDeletionRequestsAsync"/> method should
    /// cancel users' expired deletion requests.
    /// </summary>
    [Fact]
    public async Task CancelExpiredDeletionRequestsAsync()
    {
        // Arrange
        Test.ApplicationOptions.Timeout.UserDeletionRequestTimeout = TimeSpan.FromDays(1);

        var actionTime = Test.UtcNow.Add(Test.ApplicationOptions.Timeout.UserDeletionRequestTimeout * 2);

        var user = await Test.AddUserAsync(deletionRequestSubmitted: Test.UtcNow);

        Test.UtcNow = actionTime;

        // Act
        var result = await Test.Target.CancelExpiredDeletionRequestsAsync(Token);

        // Assert
        Assert.Equal(Success, result.Status);

        Assert.Equal(1, result.Value);
        Assert.Null(Test.GetDeletionRequestSubmitted(user.Id));
    }
    #endregion
}