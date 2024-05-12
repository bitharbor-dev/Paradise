using Paradise.ApplicationLogic.Identity;
using Paradise.DataAccess.Repositories.Application.Implementation;
using Paradise.DataAccess.Repositories.Domain.Implementation;
using Paradise.DataAccess.Seed.Models.Application.MessageTemplates;
using Paradise.DataAccess.Seed.Models.Domain.Roles;
using Paradise.DataAccess.Seed.Models.Domain.Users;
using Paradise.Domain.Users;

namespace Paradise.ApplicationLogic.Tests.Services.Application;

/// <summary>
/// Test class for the <see cref="DatabaseService"/> methods.
/// </summary>
public sealed class DatabaseServiceTests
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseServiceTests"/> class.
    /// </summary>
    /// <param name="output">
    /// Xunit output helper.
    /// </param>
    public DatabaseServiceTests(ITestOutputHelper output)
    {
        var applicationDataSource = GetApplicationDataSource();
        var domainDataSource = GetDomainDataSource();

        var identityOptions = GetIdentityOptions();

        var userManager = GetUserManager(output, domainDataSource, identityOptions);

        var roleManager = GetRoleManager(output, domainDataSource);

        var logger = GetLogger<DatabaseService>(output);
        var userRefreshTokensRepository = new UserRefreshTokensRepository(domainDataSource);
        var emailTemplatesRepository = new EmailTemplatesRepository(applicationDataSource);
        var emailTemplateService = new EmailTemplateService(emailTemplatesRepository);

        Manager = userManager;

        Service = new(logger,
                      roleManager,
                      userManager,
                      applicationDataSource,
                      domainDataSource,
                      userRefreshTokensRepository,
                      emailTemplatesRepository,
                      emailTemplateService);
    }
    #endregion

    #region Properties
    /// <summary>
    /// A <see cref="UserManager"/> instance used to
    /// arrange data and validate test results.
    /// </summary>
    public UserManager Manager { get; }

    /// <summary>
    /// A <see cref="DatabaseService"/> instance to be tested.
    /// </summary>
    public DatabaseService Service { get; }
    #endregion

    #region Public methods
    /// <summary>
    /// <see cref="DatabaseService.DeleteUnconfirmedUsersAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// All users with unconfirmed email addresses
    /// are deleted after the email confirmation period is exceeded.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task DeleteUnconfirmedUsersAsync()
    {
        // Arrange
        var confirmationPeriod = TimeSpan.FromDays(1);

        var user = new User("test@email.com", "TestUser")
        {
            EmailConfirmed = false
        };

        await Manager.CreateAsync(user);
        user.Created -= confirmationPeriod;
        await Manager.UpdateAsync(user);

        // Act
        var result = await Service.DeleteUnconfirmedUsersAsync(confirmationPeriod);

        // Assert
        Assert.Equal(1, result);
    }

    /// <summary>
    /// <see cref="DatabaseService.DeleteUnconfirmedUsersAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Affects none of the users since all of them have not
    /// exceeded the email confirmation period yet.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task DeleteUnconfirmedUsersAsync_AffectsNone()
    {
        // Arrange
        var confirmationPeriod = TimeSpan.FromDays(1);

        var user = new User("test@email.com", "TestUser")
        {
            EmailConfirmed = false
        };

        await Manager.CreateAsync(user);

        // Act
        var result = await Service.DeleteUnconfirmedUsersAsync(confirmationPeriod);

        // Assert
        Assert.Equal(0, result);
    }

    /// <summary>
    /// <see cref="DatabaseService.ResetUsersPendingDeletionAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// All users which were in ready-to-delete state are
    /// reset due to their deletion period is exceeded.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task ResetUsersPendingDeletionAsync()
    {
        // Arrange
        var requestLifetime = TimeSpan.FromDays(1);
        var requestTime = DateTime.UtcNow - requestLifetime;

        var user = new User("test@email.com", "TestUser")
        {
            EmailConfirmed = true,
            DeletionRequestSubmitted = requestTime
        };

        await Manager.CreateAsync(user);

        // Act
        var result = await Service.ResetUsersPendingDeletionAsync(requestLifetime);

        // Assert
        Assert.Equal(1, result);
    }

    /// <summary>
    /// <see cref="DatabaseService.ResetUsersPendingDeletionAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Affects none of the users since all of them have not
    /// exceeded the deletion request lifetime.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task ResetUsersPendingDeletionAsync_AffectsNone()
    {
        // Arrange
        var requestLifetime = TimeSpan.FromDays(1);
        var requestTime = DateTime.UtcNow;
        var user = new User("test@email.com", "TestUser")
        {
            EmailConfirmed = true,
            DeletionRequestSubmitted = requestTime
        };

        await Manager.CreateAsync(user);

        // Act
        var result = await Service.ResetUsersPendingDeletionAsync(requestLifetime);

        // Assert
        Assert.Equal(0, result);
    }

    /// <summary>
    /// <see cref="DatabaseService.SeedEmailTemplatesAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// All given email templates are added into the database.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task SeedEmailTemplatesAsync()
    {
        // Arrange
        var emailTemplates = new SeedEmailTemplateModel[]
        {
            new("TestTemplate0", "Test", "Test")
            {
                CultureId = 1,
                IsBodyHtml = false,
                PlaceholderName = "{placeholder}",
                PlaceholdersNumber = 0,
                SubjectPlaceholderName = null,
                SubjectPlaceholdersNumber = 0
            },
            new("TestTemplate1", "Test", "Test")
            {
                CultureId = 1,
                IsBodyHtml = false,
                PlaceholderName = "{placeholder}",
                PlaceholdersNumber = 0,
                SubjectPlaceholderName = null,
                SubjectPlaceholdersNumber = 0,
            }
        };

        // Act
        var result = await Service.SeedEmailTemplatesAsync(emailTemplates);

        // Assert
        Assert.Equal(emailTemplates.Length, result);
    }

    /// <summary>
    /// <see cref="DatabaseService.SeedEmailTemplatesAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Does not add duplicates to the database.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task SeedEmailTemplatesAsync_DoesNotAddDuplicates()
    {
        // Arrange
        var emailTemplate = new SeedEmailTemplateModel("TestTemplate0", "Test", "Test")
        {
            CultureId = 1,
            IsBodyHtml = false,
            PlaceholderName = "{placeholder}",
            PlaceholdersNumber = 0,
            SubjectPlaceholderName = null,
            SubjectPlaceholdersNumber = 0
        };

        var emailTemplates = new[] { emailTemplate, emailTemplate };

        // Act
        var result = await Service.SeedEmailTemplatesAsync(emailTemplates);

        // Assert
        Assert.Equal(emailTemplates.Distinct().Count(), result);
    }

    /// <summary>
    /// <see cref="DatabaseService.SeedRolesAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// All given roles are added into the database.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task SeedRolesAsync()
    {
        // Arrange
        var roles = new SeedRoleModel[]
        {
            new("TestRole0"),
            new("TestRole1")
        };

        // Act
        var result = await Service.SeedRolesAsync(roles);

        // Assert
        Assert.Equal(roles.Length, result);
    }

    /// <summary>
    /// <see cref="DatabaseService.SeedRolesAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Does not add duplicates to the database.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task SeedRolesAsync_DoesNotAddDuplicates()
    {
        // Arrange
        var role = new SeedRoleModel("TestRole0");

        var roles = new[] { role, role };

        // Act
        var result = await Service.SeedRolesAsync(roles);

        // Assert
        Assert.Equal(roles.Distinct().Count(), result);
    }

    /// <summary>
    /// <see cref="DatabaseService.SeedUsersAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// All given users are added into the database.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task SeedUsersAsync()
    {
        // Arrange
        var users = new SeedUserModel[]
        {
            new("TestUser0", "test@email.com", "TestPassword123!")
            {
                IsEmailConfirmed = true
            },
            new("TestUser1", "test1@email.com", "TestPassword123!")
            {
                IsEmailConfirmed = true
            }
        };

        // Act
        var result = await Service.SeedUsersAsync(users);

        // Assert
        Assert.Equal(users.Length, result);
    }

    /// <summary>
    /// <see cref="DatabaseService.SeedUsersAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Does not add duplicates to the database.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task SeedUsersAsync_DoesNotAddDuplicates()
    {
        // Arrange
        var user = new SeedUserModel("TestUser0", "test@email.com", "TestPassword123!")
        {
            IsEmailConfirmed = true
        };

        var users = new[] { user, user };

        // Act
        var result = await Service.SeedUsersAsync(users);

        // Assert
        Assert.Equal(users.Distinct().Count(), result);
    }
    #endregion
}