using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.ApplicationLogic.Infrastructure.Identity.Implementation;
using Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Microsoft.AspNetCore.Identity;
using Paradise.Tests.Miscellaneous.TestImplementations.Microsoft.AspNetCore.Identity;
using IdentityResult = Microsoft.AspNetCore.Identity.IdentityResult;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.Identity.Implementation;

/// <summary>
/// <see cref="UserManager{TUser}"/> test class.
/// </summary>
public sealed partial class UserManagerTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="UserManager{TUser}.CreateAsync"/> method should
    /// create a new user and add default user claims.
    /// </summary>
    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var user = new TestUser { Email = "Email", UserName = "UserName" };

        var claimsAdded = false;

        Test.SetUserStoreAddClaimsAsyncResult(() =>
        {
            claimsAdded = true;

            return Task.CompletedTask;
        });

        // Act
        var result = await Test.Target.CreateAsync(user);

        // Assert
        Assert.True(result.Succeeded);
        Assert.True(claimsAdded);
        Assert.True(Test.UserExists(user.Email, user.UserName));
    }

    /// <summary>
    /// The <see cref="UserManager{TUser}.CreateAsync"/> method should
    /// fail to create a new user if the
    /// inner <see cref="Microsoft.AspNetCore.Identity.IUserStore{TRole}"/> instance reports error
    /// upon saving user data to the persistence storage.
    /// </summary>
    [Fact]
    public async Task CreateAsync_FailsOnDependencyFailure()
    {
        // Arrange
        var user = new TestUser { Email = "Email", UserName = "UserName" };

        Test.SetUserStoreCreateAsyncResult(() => Task.FromResult(IdentityResult.Failed()));

        // Act
        var result = await Test.Target.CreateAsync(user);

        // Assert
        Assert.False(result.Succeeded);
        Assert.False(Test.UserExists(user.Email, user.UserName));
    }

    /// <summary>
    /// The <see cref="UserManager{TUser}.CreateAsync"/> method should
    /// create a new user, but fail to add default user claims and
    /// delete the newly created user due to the failure.
    /// </summary>
    [Fact]
    public async Task CreateAsync_RollbacksOnDependencyFailure()
    {
        // Arrange
        var user = new TestUser { Email = "Email", UserName = "UserName" };

        Test.SetUserStoreUpdateAsyncResult(() => Task.FromResult(IdentityResult.Failed()));

        // Act
        var result = await Test.Target.CreateAsync(user);

        // Assert
        Assert.False(result.Succeeded);
        Assert.False(Test.UserExists(user.Email, user.UserName));
    }

    /// <summary>
    /// The <see cref="UserManager{TUser}.FindByIdAsync"/> method should
    /// return a user with the specified Id.
    /// </summary>
    [Fact]
    public async Task FindByIdAsync()
    {
        // Arrange
        var user = Test.AddUser();

        // Act
        var foundUser = await Test.Target.FindByIdAsync(user.Id);

        // Assert
        Assert.NotNull(foundUser);
        Assert.Equal(user.Id, foundUser.Id);
    }

    /// <summary>
    /// The <see cref="UserManager{TUser}.FindByIdAsync"/> method should
    /// return <see langword="null"/> when no
    /// user with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task FindByIdAsync_ReturnsNull()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var foundUser = await Test.Target.FindByIdAsync(id);

        // Assert
        Assert.Null(foundUser);
    }

    /// <summary>
    /// The <see cref="UserManager{TUser}.FindByPhoneNumberAsync"/> method should
    /// return a user with the specified phone number.
    /// </summary>
    [Fact]
    public async Task FindByPhoneNumberAsync()
    {
        // Arrange
        var user = Test.AddUser();

        // Act
        var foundUser = await Test.Target.FindByPhoneNumberAsync(user.PhoneNumber);

        // Assert
        Assert.NotNull(foundUser);
        Assert.Equal(user.Id, foundUser.Id);
    }

    /// <summary>
    /// The <see cref="UserManager{TUser}.FindByPhoneNumberAsync"/> method should
    /// return <see langword="null"/> when no
    /// user with the specified phone number exists.
    /// </summary>
    [Fact]
    public async Task FindByPhoneNumberAsync_ReturnsNull()
    {
        // Arrange
        var phoneNumber = "PhoneNumber";

        // Act
        var foundUser = await Test.Target.FindByPhoneNumberAsync(phoneNumber);

        // Assert
        Assert.Null(foundUser);
    }

    /// <summary>
    /// The <see cref="UserManager{TUser}.FindByPhoneNumberAsync"/> method should
    /// return a user with the specified phone number
    /// when personal data protection is enabled.
    /// </summary>
    [Fact]
    public async Task FindByPhoneNumberAsync_WithProtectedPhoneNumber()
    {
        // Arrange
        Test.Target.Options.Stores.ProtectPersonalData = true;

        var phoneNumber = "PhoneNumber";
        var protectedPhoneNumber = Test.ConvertToProtectedData(phoneNumber);

        var user = Test.AddUser(phoneNumber: protectedPhoneNumber);

        // Act
        var foundUser = await Test.Target.FindByPhoneNumberAsync(phoneNumber);

        // Assert
        Assert.NotNull(foundUser);
        Assert.Equal(protectedPhoneNumber, foundUser.PhoneNumber);
        Assert.Equal(user.Id, foundUser.Id);
    }

    /// <summary>
    /// The <see cref="UserManager{TUser}.FindByPhoneNumberAsync"/> method should
    /// return <see langword="null"/> when no
    /// user with the specified phone number exists
    /// and personal data protection is enabled.
    /// </summary>
    [Fact]
    public async Task FindByPhoneNumberAsync_WithProtectedPhoneNumber_ReturnsNull()
    {
        // Arrange
        Test.Target.Options.Stores.ProtectPersonalData = true;

        var phoneNumber = "PhoneNumber";

        // Act
        var foundUser = await Test.Target.FindByPhoneNumberAsync(phoneNumber);

        // Assert
        Assert.Null(foundUser);
    }

    /// <summary>
    /// The <see cref="IUserManager{TUser}.ValidatePasswordAsync"/> method should
    /// return successful password validation result.
    /// </summary>
    [Fact]
    public async Task ValidatePasswordAsync()
    {
        // Arrange
        Test.Target.PasswordValidators.Add(new StubPasswordValidator());

        var user = Test.AddUser();

        var password = "password";

        // Act
        var result = await ((IUserManager<TestUser>)Test.Target).ValidatePasswordAsync(user, password);

        // Assert
        Assert.True(result.Succeeded);
    }

    /// <summary>
    /// The <see cref="IUserManager{TUser}.ValidatePasswordAsync"/> method should
    /// return unsuccessful password validation result.
    /// </summary>
    [Fact]
    public async Task ValidatePasswordAsync_FailsOnInvalidPassword()
    {
        // Arrange
        Test.Target.PasswordValidators.Add(new StubPasswordValidator(IdentityResult.Failed()));

        var user = Test.AddUser();

        var password = "password";

        // Act
        var result = await ((IUserManager<TestUser>)Test.Target).ValidatePasswordAsync(user, password);

        // Assert
        Assert.False(result.Succeeded);
    }
    #endregion
}