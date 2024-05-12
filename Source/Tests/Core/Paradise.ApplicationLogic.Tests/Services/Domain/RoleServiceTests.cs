using Microsoft.AspNetCore.Identity;
using Paradise.ApplicationLogic.Identity;
using Paradise.Domain.Roles;
using Paradise.Domain.Users;
using Paradise.Models.Domain.RoleModels;

namespace Paradise.ApplicationLogic.Tests.Services.Domain;

/// <summary>
/// Test class for the <see cref="RoleService"/> methods.
/// </summary>
public sealed class RoleServiceTests
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleServiceTests"/> class.
    /// </summary>
    /// <param name="output">
    /// Xunit output helper.
    /// </param>
    public RoleServiceTests(ITestOutputHelper output)
    {
        var domainDataSource = GetDomainDataSource();

        var identityOptions = GetIdentityOptions();

        var userManager = GetUserManager(output, domainDataSource, identityOptions);

        var roleManager = GetRoleManager(output, domainDataSource);

        UserManager = userManager;

        RoleManager = roleManager;

        Service = new(userManager, roleManager);
    }
    #endregion

    #region Properties
    /// <summary>
    /// A <see cref="Identity.UserManager"/> instance used to
    /// arrange data and validate test results.
    /// </summary>
    public UserManager UserManager { get; }

    /// <summary>
    /// A <see cref="RoleManager{Trole}"/> instance used to
    /// arrange data and validate test results.
    /// </summary>
    public RoleManager<Role> RoleManager { get; }

    /// <summary>
    /// A <see cref="RoleService"/> instance to be tested.
    /// </summary>
    public RoleService Service { get; }
    #endregion

    #region Public methods
    /// <summary>
    /// <see cref="RoleService.AssignAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Assigns a role with the specified Id
    /// to a user with the specified Id.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task AssignAsync()
    {
        // Arrange
        var user = new User("test@email.com", "TestUser0");

        var role = new Role("TestRole0", false);

        await RoleManager.CreateAsync(role);
        await UserManager.CreateAsync(user);

        // Act
        var result = await Service.AssignAsync(role.Id, user.Id);

        // Assert
        result.AssertSuccess(OK);
        Assert.NotNull(result.Value);
        Assert.Contains(result.Value, r => r.Name == role.Name);
    }

    /// <summary>
    /// <see cref="RoleService.AssignAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="Role"/>
    /// with the specified Id does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task AssignAsync_ThrowsOnNonExistingRole()
    {
        // Arrange
        var user = new User("test@email.com", "TestUser0");

        await UserManager.CreateAsync(user);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.AssignAsync(Guid.NewGuid(), user.Id));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, RoleIdNotFound);
    }

    /// <summary>
    /// <see cref="RoleService.AssignAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="User"/>
    /// with the specified Id does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task AssignAsync_ThrowsOnNonExistingUser()
    {
        // Arrange
        var role = new Role("TestRole0", false);

        await RoleManager.CreateAsync(role);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.AssignAsync(role.Id, Guid.NewGuid()));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, UserIdNotFound);
    }

    /// <summary>
    /// <see cref="RoleService.CreateAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Creates a new role.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var creationModel = new RoleCreationModel("TestRole0");

        // Act
        var result = await Service.CreateAsync(creationModel);

        // Assert
        result.AssertSuccess(Created);
        Assert.NotNull(result.Value);
        Assert.Equal(creationModel.Name, result.Value.Name);
    }

    /// <summary>
    /// <see cref="RoleService.CreateAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="RoleCreationModel"/> provided
    /// does not have a <see cref="RoleCreationModel.Name"/> specified.
    /// </para>
    /// </summary>
    [Fact]
    public async Task CreateAsync_ThrowsOnEmptyName()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.CreateAsync(new("")));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(UnprocessableEntity, InvalidRoleName);
    }

    /// <summary>
    /// <see cref="RoleService.CreateAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="RoleCreationModel"/> provided
    /// have a <see cref="RoleCreationModel.Name"/> which is already in use.
    /// </para>
    /// </summary>
    [Fact]
    public async Task CreateAsync_ThrowsOnTakenName()
    {
        // Arrange
        var roleName = "TestRole0";

        var role = new Role(roleName, false);

        var creationModel = new RoleCreationModel(roleName);

        await RoleManager.CreateAsync(role);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.CreateAsync(creationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(UnprocessableEntity, DuplicateRoleName);
    }

    /// <summary>
    /// <see cref="RoleService.DeleteAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Completely removes all data of a role
    /// with the specified Id.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task DeleteAsync()
    {
        // Arrange
        var role = new Role("TestRole0", false);

        await RoleManager.CreateAsync(role);

        // Act
        var result = await Service.DeleteAsync(role.Id);

        // Assert
        result.AssertSuccess(OK);
        Assert.DoesNotContain(RoleManager.Roles, r => r.Id == role.Id);
    }

    /// <summary>
    /// <see cref="RoleService.DeleteAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="Role"/>
    /// with the specified Id does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ThrowsOnNonExistingRole()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.DeleteAsync(Guid.NewGuid()));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, RoleIdNotFound);
    }

    /// <summary>
    /// <see cref="RoleService.GetAllAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// A complete list of roles is returned.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task GetAllAsync()
    {
        // Arrange
        var role1 = new Role("TestRole0", true);

        var role2 = new Role("TestRole1", false);

        await RoleManager.CreateAsync(role1);
        await RoleManager.CreateAsync(role2);

        // Act
        var result = await Service.GetAllAsync();

        // Assert
        result.AssertSuccess(OK);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count());
        Assert.Contains(result.Value, r => r.Name == role1.Name);
        Assert.Contains(result.Value, r => r.Name == role2.Name);
    }

    /// <summary>
    /// <see cref="RoleService.GetAllAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the list of roles, which <see cref="Role.IsDefault"/> value corresponds the
    /// given <paramref name="isDefault"/> value.
    /// </para>
    /// </para>
    /// </summary>
    [Theory, InlineData(true), InlineData(false)]
    public async Task GetAllAsync_ReturnsDefault(bool isDefault)
    {
        // Arrange
        var role1 = new Role("TestRole0", isDefault);

        var role2 = new Role("TestRole1", !isDefault);

        await RoleManager.CreateAsync(role1);
        await RoleManager.CreateAsync(role2);

        // Act
        var result = await Service.GetAllAsync(isDefault);

        // Assert
        result.AssertSuccess(OK);
        Assert.NotNull(result.Value);
        Assert.All(result.Value, r => Assert.True(r.IsDefault == isDefault));
    }

    /// <summary>
    /// <see cref="RoleService.GetByIdAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the role found.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task GetByIdAsync()
    {
        // Arrange
        var role = new Role("TestRole0", true);

        await RoleManager.CreateAsync(role);

        // Act
        var result = await Service.GetByIdAsync(role.Id);

        // Assert
        result.AssertSuccess(OK);
        Assert.NotNull(result.Value);
        Assert.Equal(role.Id, result.Value.Id);
    }

    /// <summary>
    /// <see cref="RoleService.GetByIdAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="Role"/>
    /// with the specified Id does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ThrowsOnNonExistingRole()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.GetByIdAsync(Guid.NewGuid()));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, RoleIdNotFound);
    }

    /// <summary>
    /// <see cref="RoleService.GetUserRolesAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns a complete list of roles, which belong
    /// to the specified user.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task GetUserRolesAsync()
    {
        // Arrange
        var user = new User("test@email.com", "TestUser0");

        var role1 = new Role("TestRole0", false);

        var role2 = new Role("TestRole1", false);

        await RoleManager.CreateAsync(role1);
        await RoleManager.CreateAsync(role2);
        await UserManager.CreateAsync(user);

        await UserManager.AddToRoleAsync(user, role1.Name);

        // Act
        var result = await Service.GetUserRolesAsync(user.Id);

        // Assert
        result.AssertSuccess(OK);
        Assert.NotNull(result.Value);
        Assert.Contains(result.Value, r => r.Name == role1.Name);
        Assert.DoesNotContain(result.Value, r => r.Name == role2.Name);
    }

    /// <summary>
    /// <see cref="RoleService.GetUserRolesAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="User"/>
    /// with the specified Id does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task GetUserRolesAsync_ThrowsOnNonExistingUser()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.GetUserRolesAsync(Guid.NewGuid()));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, UserIdNotFound);
    }

    /// <summary>
    /// <see cref="RoleService.UnassignAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Unassigns a role with the specified Id
    /// from a user with the specified Id.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task UnassignAsync()
    {
        // Arrange
        var user = new User("test@email.com", "TestUser0");

        var role1 = new Role("TestRole0", false);

        var role2 = new Role("TestRole1", false);

        await RoleManager.CreateAsync(role1);
        await RoleManager.CreateAsync(role2);
        await UserManager.CreateAsync(user);

        await UserManager.AddToRoleAsync(user, role1.Name);
        await UserManager.AddToRoleAsync(user, role2.Name);

        // Act
        var result = await Service.UnassignAsync(role1.Id, user.Id);

        // Assert
        result.AssertSuccess(OK);
        Assert.NotNull(result.Value);
        Assert.DoesNotContain(result.Value, r => r.Name == role1.Name);
        Assert.Contains(result.Value, r => r.Name == role2.Name);
    }

    /// <summary>
    /// <see cref="RoleService.UnassignAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="Role"/>
    /// with the specified Id does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task UnassignAsync_ThrowsOnNonExistingRole()
    {
        // Arrange
        var user = new User("test@email.com", "TestUser0");

        await UserManager.CreateAsync(user);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.UnassignAsync(Guid.NewGuid(), user.Id));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, RoleIdNotFound);
    }

    /// <summary>
    /// <see cref="RoleService.UnassignAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="User"/>
    /// with the specified Id does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task UnassignAsync_ThrowsOnNonExistingUser()
    {
        // Arrange
        var role = new Role("TestRole0", false);

        await RoleManager.CreateAsync(role);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.UnassignAsync(role.Id, Guid.NewGuid()));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, UserIdNotFound);
    }

    /// <summary>
    /// <see cref="RoleService.UpdateAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Updates the role with the new values.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var isDefault = true;

        var role = new Role("TestRole0", isDefault);

        var roleIsDefaultInitialValue = role.IsDefault;

        var updateModel = new RoleUpdateModel
        {
            IsDefault = !role.IsDefault
        };

        await RoleManager.CreateAsync(role);

        // Act
        var result = await Service.UpdateAsync(role.Id, updateModel);

        // Assert
        result.AssertSuccess(OK);
        Assert.NotNull(result.Value);
        Assert.NotEqual(roleIsDefaultInitialValue, result.Value.IsDefault);
    }

    /// <summary>
    /// <see cref="RoleService.UpdateAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="Role"/>
    /// with the specified Id does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ThrowsOnNonExistingRole()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.UpdateAsync(Guid.NewGuid(), new()));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, RoleIdNotFound);
    }
    #endregion
}