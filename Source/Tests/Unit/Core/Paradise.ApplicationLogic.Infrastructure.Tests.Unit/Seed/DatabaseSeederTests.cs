using Microsoft.AspNetCore.Identity;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.ApplicationLogic.Infrastructure.Seed.Implementation;
using Paradise.ApplicationLogic.Infrastructure.Services;
using Paradise.DataAccess.Seed.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess.Seed.Models.Domain.Identity.Roles;
using Paradise.DataAccess.Seed.Models.Domain.Identity.Users;
using Paradise.Models;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using static Paradise.Models.ErrorCode;
using static Paradise.Models.OperationStatus;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.Seed;

/// <summary>
/// <see cref="DatabaseSeeder"/> test class.
/// </summary>
public sealed partial class DatabaseSeederTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="SeedUsersAsync_FailsOnInvalidPassword"/> method.
    /// </summary>
    public static TheoryData<string?> SeedUsersAsync_FailsOnInvalidPassword_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="DatabaseSeeder.EnsureStorageAvailableAsync"/> method should
    /// setup the persistence storage so it will become available for further operations.
    /// </summary>
    [Fact]
    public async Task EnsureStorageAvailableAsync()
    {
        // Arrange

        // Act
        await Test.Target.EnsureStorageAvailableAsync(Token);

        // Assert
        Assert.True(Test.DomainStoragePrepared);
        Assert.True(Test.InfrastructureStoragePrepared);
    }

    /// <summary>
    /// The <see cref="DatabaseSeeder.SeedRolesAsync"/> method should
    /// create roles in persistence storage and
    /// return the number of added roles.
    /// </summary>
    [Fact]
    public async Task SeedRolesAsync()
    {
        // Arrange
        var model = GetRoleModel();

        // Act
        var result = await Test.Target.SeedRolesAsync([model], Token);

        // Assert
        Assert.True(Test.RoleExists(model.Name, model.IsDefault));
        Assert.Equal(1, result);
    }

    /// <summary>
    /// The <see cref="DatabaseSeeder.SeedRolesAsync"/> method should
    /// only create roles which are not present in persistence storage and
    /// return the number of added roles.
    /// </summary>
    [Fact]
    public async Task SeedRolesAsync_SkipsExistingRoles()
    {
        // Arrange
        var model = GetRoleModel();

        Test.AddRole(model.Name, model.IsDefault);

        // Act
        var result = await Test.Target.SeedRolesAsync([model], Token);

        // Assert
        Assert.True(Test.RoleExists(model.Name, model.IsDefault));
        Assert.Equal(0, result);
    }

    /// <summary>
    /// The <see cref="DatabaseSeeder.SeedRolesAsync"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="IEnumerable{T}"/> of <see cref="SeedRoleModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task SeedRolesAsync_ThrowsOnNull()
    {
        // Arrange
        var models = null as IEnumerable<SeedRoleModel>;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(()
            => Test.Target.SeedRolesAsync(models!, Token));
    }

    /// <summary>
    /// The <see cref="DatabaseSeeder.SeedRolesAsync"/> method should
    /// fail to create a new role if the
    /// inner <see cref="IRoleManager{TRole}"/> instance reports error
    /// upon saving role data to the persistence storage.
    /// </summary>
    [Fact]
    public async Task SeedRolesAsync_FailsOnDependencyFailure()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to create role"
        };

        Test.SetRoleManagerCreateAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        var model = GetRoleModel();

        // Act
        var result = await Test.Target.SeedRolesAsync([model], Token);

        // Assert
        Assert.False(Test.RoleExists(model.Name, model.IsDefault));
        Assert.Equal(0, result);
        Assert.True(Test.ExceptionLogged<InvalidOperationException>(error.Description));
    }

    /// <summary>
    /// The <see cref="DatabaseSeeder.SeedUsersAsync"/> method should
    /// create users in persistence storage and
    /// return the number of added users.
    /// </summary>
    [Fact]
    public async Task SeedUsersAsync()
    {
        // Arrange
        var model = GetUserModel();

        // Act
        var result = await Test.Target.SeedUsersAsync([model], Token);

        // Assert
        Assert.True(Test.UserExists(model.EmailAddress, model.UserName));
        Assert.Equal(1, result);
    }

    /// <summary>
    /// The <see cref="DatabaseSeeder.SeedUsersAsync"/> method should
    /// only create users which are not present in persistence storage and
    /// return the number of added users.
    /// </summary>
    [Fact]
    public async Task SeedUsersAsync_SkipsExistingUsers()
    {
        // Arrange
        var model = GetUserModel();

        Test.AddUser(model.EmailAddress, model.UserName);

        // Act
        var result = await Test.Target.SeedUsersAsync([model], Token);

        // Assert
        Assert.True(Test.UserExists(model.EmailAddress, model.UserName));
        Assert.Equal(0, result);
    }

    /// <summary>
    /// The <see cref="DatabaseSeeder.SeedUsersAsync"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="IEnumerable{T}"/> of <see cref="SeedUserModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task SeedUsersAsync_ThrowsOnNull()
    {
        // Arrange
        var models = null as IEnumerable<SeedUserModel>;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(()
            => Test.Target.SeedUsersAsync(models!, Token));
    }

    /// <summary>
    /// The <see cref="DatabaseSeeder.SeedUsersAsync"/> method should
    /// fail to create a new user if the
    /// input <paramref name="password"/> is invalid.
    /// </summary>
    /// <param name="password">
    /// Invalid password value.
    /// </param>
    [Theory, MemberData(nameof(SeedUsersAsync_FailsOnInvalidPassword_MemberData))]
    public async Task SeedUsersAsync_FailsOnInvalidPassword(string? password)
    {
        // Arrange
        var model = GetUserModel(password: password);

        // Act
        var result = await Test.Target.SeedUsersAsync([model], Token);

        // Assert
        Assert.False(Test.UserExists(model.EmailAddress, model.UserName));
        Assert.Equal(0, result);
    }

    /// <summary>
    /// The <see cref="DatabaseSeeder.SeedUsersAsync"/> method should
    /// fail to create a new user if the
    /// inner <see cref="IUserManager{TUser}"/> instance reports error
    /// upon saving user data to the persistence storage.
    /// </summary>
    [Fact]
    public async Task SeedUsersAsync_FailsOnDependencyFailure_UponCreation()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to create user"
        };

        Test.SetUserManagerCreateAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        var model = GetUserModel();

        // Act
        var result = await Test.Target.SeedUsersAsync([model], Token);

        // Assert
        Assert.False(Test.UserExists(model.EmailAddress, model.UserName));
        Assert.True(Test.ExceptionLogged<InvalidOperationException>(error.Description));
        Assert.Equal(0, result);
    }

    /// <summary>
    /// The <see cref="DatabaseSeeder.SeedUsersAsync"/> method should
    /// fail to complete user creation if the
    /// inner <see cref="IUserManager{TUser}"/> instance reports error
    /// upon assigning default roles and
    /// the ones specified in the <see cref="SeedUserModel.Roles"/>.
    /// </summary>
    [Fact]
    public async Task SeedUsersAsync_FailsOnDependencyFailure_UponRolesAssign()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to assign role"
        };

        Test.SetUserManagerAddToRolesAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        var model = GetUserModel(roles: ["User"]);

        // Act
        var result = await Test.Target.SeedUsersAsync([model], Token);

        // Assert
        Assert.False(Test.UserExists(model.EmailAddress, model.UserName));
        Assert.True(Test.ExceptionLogged<InvalidOperationException>(error.Description));
        Assert.Equal(0, result);
    }

    /// <summary>
    /// The <see cref="DatabaseSeeder.SeedUsersAsync"/> method should
    /// fail to complete user creation if the
    /// inner <see cref="IUserManager{TUser}"/> instance reports error
    /// upon assigning the default roles and
    /// the ones specified in the <see cref="SeedUserModel.Roles"/>
    /// and then failing to delete the user after unsuccessful seeding operation.
    /// </summary>
    [Fact]
    public async Task SeedUsersAsync_FailsOnDependencyFailure_UponCleanup()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to delete the user after role assignment error"
        };

        Test.SetUserManagerAddToRolesAsyncResult(() => Task.FromResult(IdentityResult.Failed()));
        Test.SetUserManagerDeleteAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        var model = GetUserModel(roles: ["User"]);

        // Act
        var result = await Test.Target.SeedUsersAsync([model], Token);

        // Assert
        Assert.True(Test.UserExists(model.EmailAddress, model.UserName));
        Assert.True(Test.ExceptionLogged<InvalidOperationException>(error.Description));
        Assert.Equal(0, result);
    }

    /// <summary>
    /// The <see cref="DatabaseSeeder.SeedEmailTemplatesAsync"/> method should
    /// create email templates in persistence storage and
    /// return the number of added email templates.
    /// </summary>
    [Fact]
    public async Task SeedEmailTemplatesAsync()
    {
        // Arrange
        var model = GetEmailTemplateModel();

        // Act
        var result = await Test.Target.SeedEmailTemplatesAsync([model], Token);

        // Assert
        Assert.True(Test.EmailTemplateExists(model.TemplateName, model.CultureId, model.TemplateText, model.Subject));
        Assert.Equal(1, result);
    }

    /// <summary>
    /// The <see cref="DatabaseSeeder.SeedEmailTemplatesAsync"/> method should
    /// only create email templates which are not present in persistence storage and
    /// return the number of added email templates, existing email templates
    /// should be updated with the new data.
    /// </summary>
    [Fact]
    public async Task SeedEmailTemplatesAsync_UpdatesExistingEmailTemplates()
    {
        // Arrange
        var templateName = "TemplateName";
        var cultureId = 1;
        var templateText = "TemplateText";
        var subject = "Subject";

        var updatedTemplateText = "UpdatedTemplateText";

        var model = GetEmailTemplateModel(templateName, cultureId, updatedTemplateText, subject);

        Test.AddEmailTemplate(templateName, cultureId, templateText, subject);

        // Act
        var result = await Test.Target.SeedEmailTemplatesAsync([model], Token);

        // Assert
        Assert.True(Test.EmailTemplateExists(templateName, cultureId, updatedTemplateText, subject));
        Assert.False(Test.EmailTemplateExists(templateName, cultureId, templateText, subject));
        Assert.Equal(0, result);
    }

    /// <summary>
    /// The <see cref="DatabaseSeeder.SeedEmailTemplatesAsync"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="IEnumerable{T}"/> of <see cref="SeedEmailTemplateModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task SeedEmailTemplatesAsync_ThrowsOnNull()
    {
        // Arrange
        var models = null as IEnumerable<SeedEmailTemplateModel>;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(()
            => Test.Target.SeedEmailTemplatesAsync(models!, Token));
    }

    /// <summary>
    /// The <see cref="DatabaseSeeder.SeedEmailTemplatesAsync"/> method should
    /// fail to complete email template creation if the
    /// inner <see cref="IEmailTemplateService"/> instance reports error
    /// upon saving email template data to the persistence storage.
    /// </summary>
    [Fact]
    public async Task SeedEmailTemplatesAsync_FailsOnDependencyFailure_UponCreate()
    {
        // Arrange
        var error = new ApplicationError(DefaultError, "Failure");
        var errorResult = new Result<EmailTemplateModel>(Failure, [error], null);

        var model = GetEmailTemplateModel();

        Test.SetEmailTemplateServiceCreateAsyncResult(() => Task.FromResult(errorResult));

        // Act
        var result = await Test.Target.SeedEmailTemplatesAsync([model], Token);

        // Assert
        Assert.False(Test.EmailTemplateExists(model.TemplateName, model.CultureId, model.TemplateText, model.Subject));
        Assert.True(Test.ExceptionLogged<InvalidOperationException>(error));
        Assert.Equal(0, result);
    }

    /// <summary>
    /// The <see cref="DatabaseSeeder.SeedEmailTemplatesAsync"/> method should
    /// fail to complete email template creation if the
    /// inner <see cref="IEmailTemplateService"/> instance reports error
    /// upon updating email template data in the persistence storage.
    /// </summary>
    [Fact]
    public async Task SeedEmailTemplatesAsync_FailsOnDependencyFailure_UponUpdate()
    {
        // Arrange
        var error = new ApplicationError(DefaultError, "Failure");
        var errorResult = new Result<EmailTemplateModel>(Failure, [error], null);

        var templateName = "TemplateName";
        var cultureId = 1;
        var templateText = "TemplateText";
        var subject = "Subject";

        var updatedTemplateText = "UpdatedTemplateText";

        var model = GetEmailTemplateModel(templateName, cultureId, updatedTemplateText, subject);

        Test.AddEmailTemplate(templateName, cultureId, templateText, subject);

        Test.SetEmailTemplateServiceUpdateAsyncResult(() => Task.FromResult(errorResult));

        // Act
        var result = await Test.Target.SeedEmailTemplatesAsync([model], Token);

        // Assert
        Assert.True(Test.EmailTemplateExists(templateName, cultureId, templateText, subject));
        Assert.False(Test.EmailTemplateExists(templateName, cultureId, updatedTemplateText, subject));
        Assert.True(Test.ExceptionLogged<InvalidOperationException>(error));
        Assert.Equal(0, result);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Initializes a new instance of the <see cref="SeedRoleModel"/> class.
    /// </summary>
    /// <param name="name">
    /// Role name.
    /// </param>
    /// <param name="isDefault">
    /// Indicates whether the role is default and should be
    /// assigned automatically when a user has been created.
    /// </param>
    /// <returns>
    /// A new instance of the <see cref="SeedRoleModel"/> class.
    /// </returns>
    private static SeedRoleModel GetRoleModel(string? name = "Name",
                                              bool isDefault = false)
    {
        return new(name!,
                   isDefault);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeedUserModel"/> class.
    /// </summary>
    /// <param name="emailAddress">
    /// User's email address.
    /// </param>
    /// <param name="userName">
    /// User's user-name.
    /// </param>
    /// <param name="password">
    /// User's password.
    /// </param>
    /// <param name="isEmailConfirmed">
    /// Indicates whether the user's email address has been confirmed.
    /// </param>
    /// <param name="roles">
    /// The list of the user's roles.
    /// </param>
    /// <returns>
    /// A new instance of the <see cref="SeedUserModel"/> class.
    /// </returns>
    private static SeedUserModel GetUserModel(string? emailAddress = "EmailAddress",
                                              string? userName = "UserName",
                                              string? password = "Password",
                                              bool isEmailConfirmed = false,
                                              IEnumerable<string>? roles = null)
    {
        return new(emailAddress!,
                   userName!,
                   password!,
                   isEmailConfirmed,
                   roles);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeedEmailTemplateModel"/> class.
    /// </summary>
    /// <param name="templateName">
    /// Template name.
    /// </param>
    /// <param name="cultureId">
    /// Template culture LCID.
    /// </param>
    /// <param name="templateText">
    /// Template text.
    /// </param>
    /// <param name="subject">
    /// Email subject.
    /// </param>
    /// <param name="isBodyHtml">
    /// Indicates whether the email body is an HTML document.
    /// </param>
    /// <param name="placeholderName">
    /// Placeholder name to be replaced with values during the message formatting.
    /// </param>
    /// <param name="placeholdersNumber">
    /// The number of placeholders to be replaced with values during the message formatting.
    /// </param>
    /// <param name="subjectPlaceholderName">
    /// Subject placeholder name to be replaced with values during the message formatting.
    /// </param>
    /// <param name="subjectPlaceholdersNumber">
    /// The number of subject placeholders to be replaced with values during the message formatting.
    /// </param>
    /// <returns>
    /// A new instance of the <see cref="SeedEmailTemplateModel"/> class.
    /// </returns>
    private static SeedEmailTemplateModel GetEmailTemplateModel(string? templateName = "TemplateName",
                                                                int? cultureId = null,
                                                                string? templateText = "TemplateText",
                                                                string? subject = "Subject",
                                                                bool isBodyHtml = false,
                                                                string? placeholderName = null,
                                                                ushort placeholdersNumber = 0,
                                                                string? subjectPlaceholderName = null,
                                                                ushort subjectPlaceholdersNumber = 0)
    {
        return new(templateName!,
                   cultureId,
                   subject!,
                   isBodyHtml,
                   placeholderName,
                   placeholdersNumber,
                   subjectPlaceholderName,
                   subjectPlaceholdersNumber,
                   templateText);
    }
    #endregion
}