using Paradise.ApplicationLogic.DataConverters.Domain.Identity.Users;
using Paradise.DataAccess.Seed.Models.Domain.Identity.Users;
using Paradise.Domain.Identity.Users;
using Paradise.Models.Domain.Identity.Users;

namespace Paradise.ApplicationLogic.DataConverters.Tests.Unit.Domain.Identity.Users;

/// <summary>
/// <see cref="UserDataConverter"/> test class.
/// </summary>
public sealed class UserDataConverterTests
{
    #region Properties
    /// <inheritdoc cref="DateTimeOffset.UnixEpoch"/>
    public static DateTimeOffset UnixEpoch { get; } = DateTimeOffset.UnixEpoch;
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="UserDataConverter.ToEntity(UserRegistrationModel)"/> method should
    /// return a new <see cref="User"/> instance populated
    /// with the data from the input <see cref="UserRegistrationModel"/> object.
    /// </summary>
    [Fact]
    public void ToEntity_FromRegistrationModel()
    {
        // Arrange
        var model = new UserRegistrationModel(
            userName: "UserName",
            emailAddress: "EmailAddress",
            phoneNumber: "PhoneNumber",
            password: "Password",
            passwordConfirmation: "PasswordConfirmation");

        // Act
        var result = model.ToEntity();

        // Assert
        Assert.Equal(model.UserName, result.UserName);
        Assert.Equal(model.EmailAddress, result.Email);
        Assert.Equal(model.PhoneNumber, result.PhoneNumber);
    }

    /// <summary>
    /// The <see cref="UserDataConverter.ToEntity(UserRegistrationModel)"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="UserRegistrationModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void ToEntity_FromRegistrationModel_ThrowsOnNull()
    {
        // Arrange
        var model = null as UserRegistrationModel;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => model!.ToEntity());
    }

    /// <summary>
    /// The <see cref="UserDataConverter.ToEntity(SeedUserModel)"/> method should
    /// return a new <see cref="User"/> instance populated
    /// with the data from the input <see cref="SeedUserModel"/> object.
    /// </summary>
    [Fact]
    public void ToEntity_FromSeedModel()
    {
        // Arrange
        var model = new SeedUserModel(
            emailAddress: "EmailAddress",
            userName: "UserName",
            password: "Password",
            isEmailConfirmed: true,
            roles: []);

        // Act
        var result = model.ToEntity();

        // Assert
        Assert.Equal(model.UserName, result.UserName);
        Assert.Equal(model.EmailAddress, result.Email);
        Assert.Equal(model.IsEmailConfirmed, result.EmailConfirmed);
    }

    /// <summary>
    /// The <see cref="UserDataConverter.ToEntity(SeedUserModel)"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="SeedUserModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void ToEntity_FromSeedModel_ThrowsOnNull()
    {
        // Arrange
        var model = null as SeedUserModel;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => model!.ToEntity());
    }

    /// <summary>
    /// The <see cref="UserDataConverter.ToModel"/> method should
    /// return a new <see cref="UserModel"/> instance populated
    /// with the data from the input <see cref="User"/> object.
    /// </summary>
    [Fact]
    public void ToModel()
    {
        // Arrange
        var entity = new User(email: "Email", userName: "UserName")
        {
            EmailConfirmed = true,
            DeletionRequestSubmitted = UnixEpoch,
            PhoneNumber = "PhoneNumber",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = true
        };

        // Act
        var result = entity.ToModel();

        // Assert
        Assert.Equal(entity.Email, result.EmailAddress);
        Assert.Equal(entity.UserName, result.UserName);
        Assert.Equal(entity.Created, result.RegistrationDate);
        Assert.Equal(entity.EmailConfirmed, result.IsEmailAddressConfirmed);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.IsPendingDeletion, result.IsPendingDeletion);
        Assert.Equal(entity.PhoneNumber, result.PhoneNumber);
        Assert.Equal(entity.PhoneNumberConfirmed, result.IsPhoneNumberConfirmed);
        Assert.Equal(entity.TwoFactorEnabled, result.IsTwoFactorEnabled);
    }

    /// <summary>
    /// The <see cref="UserDataConverter.ToModel"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="User"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void ToModel_ThrowsOnNull()
    {
        // Arrange
        var entity = null as User;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => entity!.ToModel());
    }
    #endregion
}