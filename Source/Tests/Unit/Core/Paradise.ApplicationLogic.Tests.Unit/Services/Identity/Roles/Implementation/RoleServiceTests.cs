using Microsoft.AspNetCore.Identity;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.ApplicationLogic.Services.Identity.Roles.Implementation;
using Paradise.Models.Domain.Identity.Roles;
using Paradise.Tests.Miscellaneous;
using static Paradise.Models.ErrorCode;
using static Paradise.Models.OperationStatus;

namespace Paradise.ApplicationLogic.Tests.Unit.Services.Identity.Roles.Implementation;

/// <summary>
/// <see cref="RoleService"/> test class.
/// </summary>
public sealed partial class RoleServiceTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="CreateAsync_FailsOnInvalidName"/> method.
    /// </summary>
    public static TheoryData<string?> CreateAsync_FailsOnInvalidName_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="RoleService.GetAllAsync"/> method should
    /// return the list of roles.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsAll()
    {
        // Arrange
        var role1 = await Test.AddRoleAsync(isDefault: true);
        var role2 = await Test.AddRoleAsync(isDefault: false);

        var isDefault = null as bool?;

        // Act
        var result = await Test.Target.GetAllAsync(isDefault, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.Collection(result.Value, roleModel => Assert.Equal(role1.Name, roleModel.Name),
                                        roleModel => Assert.Equal(role2.Name, roleModel.Name));
    }

    /// <summary>
    /// The <see cref="RoleService.GetAllAsync"/> method should
    /// return the list of default roles.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsDefault()
    {
        // Arrange
        await Test.AddRoleAsync(isDefault: true);
        await Test.AddRoleAsync(isDefault: false);

        var isDefault = true;

        // Act
        var result = await Test.Target.GetAllAsync(isDefault, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.All(result.Value, role => Assert.True(role.IsDefault));
    }

    /// <summary>
    /// The <see cref="RoleService.GetAllAsync"/> method should
    /// return the list of non-default roles.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsNonDefault()
    {
        // Arrange
        await Test.AddRoleAsync(isDefault: true);
        await Test.AddRoleAsync(isDefault: false);

        var isDefault = false;

        // Act
        var result = await Test.Target.GetAllAsync(isDefault, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.All(result.Value, role => Assert.False(role.IsDefault));
    }

    /// <summary>
    /// The <see cref="RoleService.GetByIdAsync"/> method should
    /// return a role with the specified Id.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync()
    {
        // Arrange
        var role = await Test.AddRoleAsync();

        // Act
        var result = await Test.Target.GetByIdAsync(role.Id, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.Equal(role.Id, result.Value.Id);
    }

    /// <summary>
    /// The <see cref="RoleService.GetByIdAsync"/> method should
    /// fail to retrieve a role
    /// when no role with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_FailsOnNonExistingRole()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var result = await Test.Target.GetByIdAsync(id, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, RoleIdNotFound);
    }

    /// <summary>
    /// The <see cref="RoleService.GetUserRolesAsync"/> method should
    /// return the list of roles assigned to a user with the specified Id.
    /// </summary>
    [Fact]
    public async Task GetUserRolesAsync()
    {
        // Arrange
        var user = await Test.AddUserAsync();
        var role = await Test.AddRoleAsync();
        await Test.AssignRoleAsync(user, role);

        // Act
        var result = await Test.Target.GetUserRolesAsync(user.Id, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        var roleModel = Assert.Single(result.Value);
        Assert.Equal(role.Name, roleModel.Name);
        Assert.Equal(role.IsDefault, roleModel.IsDefault);
    }

    /// <summary>
    /// The <see cref="RoleService.GetUserRolesAsync"/> method should
    /// fail to retrieve a user's roles
    /// when no user with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task GetUserRolesAsync_FailsOnNonExistingUser()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var result = await Test.Target.GetUserRolesAsync(id, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, UserIdNotFound);
    }

    /// <summary>
    /// The <see cref="RoleService.CreateAsync"/> method should
    /// create a new role.
    /// </summary>
    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var model = new RoleCreationModel("Name", false);

        // Act
        var result = await Test.Target.CreateAsync(model, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Created, result.Status);

        Assert.True(Test.RoleExists(result.Value.Name));
    }

    /// <summary>
    /// The <see cref="RoleService.CreateAsync"/> method should
    /// fail to create a new role if the input
    /// <see cref="RoleCreationModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task CreateAsync_FailsOnNull()
    {
        // Arrange
        var model = null as RoleCreationModel;

        // Act
        var result = await Test.Target.CreateAsync(model!, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidModel);
    }

    /// <summary>
    /// The <see cref="RoleService.CreateAsync"/> method should
    /// fail to create a new role if the
    /// input <see cref="RoleCreationModel"/> does not
    /// contain a valid role name.
    /// </summary>
    /// <param name="name">
    /// Role name.
    /// </param>
    [Theory, MemberData(nameof(CreateAsync_FailsOnInvalidName_MemberData))]
    public async Task CreateAsync_FailsOnInvalidName(string? name)
    {
        // Arrange
        var model = new RoleCreationModel(name!, false);

        // Act
        var result = await Test.Target.CreateAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.False(Test.RoleExists(name));
        Assert.ContainsErrorCode(result, InvalidRoleName);
    }

    /// <summary>
    /// The<see cref = "RoleService.CreateAsync" /> method should
    /// fail to create a role if the
    /// input<see cref = "RoleCreationModel" /> specifies the
    /// role name which is already in use by existing role.
    /// </summary>
    [Fact]
    public async Task CreateAsync_FailsOnDuplicateName()
    {
        // Arrange
        var role = await Test.AddRoleAsync();
        var model = new RoleCreationModel(role.Name, false);

        // Act
        var result = await Test.Target.CreateAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Blocked, result.Status);

        Assert.True(Test.RoleExists(role.Name));
        Assert.ContainsErrorCode(result, DuplicateRoleName);
    }

    /// <summary>
    /// The <see cref="RoleService.CreateAsync"/> method should
    /// fail to create a new role if the
    /// inner <see cref="IRoleManager{TRole}"/> instance reports error
    /// upon saving role data to the persistence storage.
    /// </summary>
    [Fact]
    public async Task CreateAsync_FailsOnDependencyFailure()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to create role"
        };

        Test.SetRoleManagerCreateAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        var model = new RoleCreationModel("Name", false);

        // Act
        var result = await Test.Target.CreateAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Failure, result.Status);

        Assert.False(Test.RoleExists(model.Name));
        Assert.ContainsErrorCode(result, DefaultError, error.Description);
    }

    /// <summary>
    /// The <see cref="RoleService.UpdateAsync"/> method should
    /// update a role.
    /// </summary>
    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var isDefault = true;

        var role = await Test.AddRoleAsync(isDefault: isDefault);
        var model = new RoleUpdateModel { IsDefault = !isDefault };

        // Act
        var result = await Test.Target.UpdateAsync(role.Id, model, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.Equal(model.IsDefault, result.Value.IsDefault);
    }

    /// <summary>
    /// The <see cref="RoleService.UpdateAsync"/> method should
    /// fail to update a role if the input
    /// <see cref="RoleUpdateModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_FailsOnNull()
    {
        // Arrange
        var role = await Test.AddRoleAsync();
        var model = null as RoleUpdateModel;

        // Act
        var result = await Test.Target.UpdateAsync(role.Id, model!, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(InvalidInput, result.Status);

        Assert.ContainsErrorCode(result, InvalidModel);
    }

    /// <summary>
    /// The <see cref="RoleService.UpdateAsync"/> method should
    /// fail to update a role
    /// when no role with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_FailsOnNonExistingRole()
    {
        // Arrange
        var id = Guid.Empty;
        var model = new RoleUpdateModel
        {
            IsDefault = true
        };

        // Act
        var result = await Test.Target.UpdateAsync(id, model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, RoleIdNotFound);
    }

    /// <summary>
    /// The <see cref="RoleService.UpdateAsync"/> method should
    /// fail to update a role if the
    /// inner <see cref="IRoleManager{TRole}"/> instance reports error
    /// upon saving role data to the persistence storage.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_FailsOnDependencyFailure()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to update role"
        };

        Test.SetRoleManagerUpdateAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        var role = await Test.AddRoleAsync();
        var model = new RoleUpdateModel
        {
            IsDefault = true
        };

        // Act
        var result = await Test.Target.UpdateAsync(role.Id, model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Failure, result.Status);

        Assert.ContainsErrorCode(result, DefaultError, error.Description);
    }

    /// <summary>
    /// The <see cref="RoleService.DeleteAsync"/> method should
    /// delete a role.
    /// </summary>
    [Fact]
    public async Task DeleteAsync()
    {
        // Arrange
        var role1 = await Test.AddRoleAsync("RoleOne");
        var role2 = await Test.AddRoleAsync("RoleTwo");

        // Act
        var result = await Test.Target.DeleteAsync(role1.Id, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.Contains(result.Value, role => role.Name == role2.Name);

        Assert.False(Test.RoleExists(role1.Name));
        Assert.True(Test.RoleExists(role2.Name));
    }

    /// <summary>
    /// The <see cref="RoleService.DeleteAsync"/> method should
    /// fail to delete a role
    /// when no role with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_FailsOnNonExistingRole()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var result = await Test.Target.DeleteAsync(id, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, RoleIdNotFound);
    }

    /// <summary>
    /// The <see cref="RoleService.DeleteAsync"/> method should
    /// fail to delete a role if the
    /// inner <see cref="IRoleManager{TRole}"/> instance reports error
    /// upon deleting role data from the persistence storage.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_FailsOnDependencyFailure()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to delete role"
        };

        Test.SetRoleManagerDeleteAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        var role = await Test.AddRoleAsync();

        // Act
        var result = await Test.Target.DeleteAsync(role.Id, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Failure, result.Status);

        Assert.True(Test.RoleExists(role.Name));
        Assert.ContainsErrorCode(result, DefaultError, error.Description);
    }

    /// <summary>
    /// The <see cref="RoleService.AssignAsync"/> method should
    /// assign a role to a user.
    /// </summary>
    [Fact]
    public async Task AssignAsync()
    {
        // Arrange
        var role1 = await Test.AddRoleAsync("RoleOne");
        var role2 = await Test.AddRoleAsync("RoleTwo");
        var user = await Test.AddUserAsync();

        // Act
        var result = await Test.Target.AssignAsync(role1.Id, user.Id, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.DoesNotContain(result.Value, role => role.Name == role2.Name);

        Assert.True(await Test.UserIsInRoleAsync(user, role1));
        Assert.False(await Test.UserIsInRoleAsync(user, role2));
    }

    /// <summary>
    /// The <see cref="RoleService.AssignAsync"/> method should
    /// fail to assign a role
    /// when no role with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task AssignAsync_FailsOnNonExistingRole()
    {
        // Arrange
        var roleId = Guid.Empty;
        var user = await Test.AddUserAsync();

        // Act
        var result = await Test.Target.AssignAsync(roleId, user.Id, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, RoleIdNotFound);
    }

    /// <summary>
    /// The <see cref="RoleService.AssignAsync"/> method should
    /// fail to assign a role
    /// when no user with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task AssignAsync_FailsOnNonExistingUser()
    {
        // Arrange
        var role = await Test.AddRoleAsync();
        var userId = Guid.Empty;

        // Act
        var result = await Test.Target.AssignAsync(role.Id, userId, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, UserIdNotFound);
    }

    /// <summary>
    /// The <see cref="RoleService.AssignAsync"/> method should
    /// fail to assign a role if the
    /// inner <see cref="IUserManager{TUser}"/> instance reports error
    /// upon assigning role to the user.
    /// </summary>
    [Fact]
    public async Task AssignAsync_FailsOnDependencyFailure()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to assign role"
        };

        Test.SetUserManagerAddToRoleAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        var role = await Test.AddRoleAsync();
        var user = await Test.AddUserAsync();

        // Act
        var result = await Test.Target.AssignAsync(role.Id, user.Id, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Failure, result.Status);

        Assert.False(await Test.UserIsInRoleAsync(user, role));
        Assert.ContainsErrorCode(result, DefaultError, error.Description);
    }

    /// <summary>
    /// The <see cref="RoleService.UnassignAsync"/> method should
    /// unassign a role from a user.
    /// </summary>
    [Fact]
    public async Task UnassignAsync()
    {
        // Arrange
        var role1 = await Test.AddRoleAsync("RoleOne");
        var role2 = await Test.AddRoleAsync("RoleTwo");
        var user = await Test.AddUserAsync();
        await Test.AssignRoleAsync(user, role1);
        await Test.AssignRoleAsync(user, role2);

        // Act
        var result = await Test.Target.UnassignAsync(role1.Id, user.Id, Token);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(Success, result.Status);

        Assert.DoesNotContain(result.Value, role => role.Name == role1.Name);

        Assert.False(await Test.UserIsInRoleAsync(user, role1));
        Assert.True(await Test.UserIsInRoleAsync(user, role2));
    }

    /// <summary>
    /// The <see cref="RoleService.UnassignAsync"/> method should
    /// fail to unassign a role
    /// when no role with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task UnassignAsync_FailsOnNonExistingRole()
    {
        // Arrange
        var roleId = Guid.Empty;
        var user = await Test.AddUserAsync();

        // Act
        var result = await Test.Target.UnassignAsync(roleId, user.Id, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, RoleIdNotFound);
    }

    /// <summary>
    /// The <see cref="RoleService.UnassignAsync"/> method should
    /// fail to unassign a role
    /// when no user with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task UnassignAsync_FailsOnNonExistingUser()
    {
        // Arrange
        var role = await Test.AddRoleAsync();
        var userId = Guid.Empty;

        // Act
        var result = await Test.Target.UnassignAsync(role.Id, userId, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Missing, result.Status);

        Assert.ContainsErrorCode(result, UserIdNotFound);
    }

    /// <summary>
    /// The <see cref="RoleService.UnassignAsync"/> method should
    /// fail to unassign a role if the
    /// inner <see cref="IUserManager{TUser}"/> instance reports error
    /// upon unassigning role to the user.
    /// </summary>
    [Fact]
    public async Task UnassignAsync_FailsOnDependencyFailure()
    {
        // Arrange
        var error = new IdentityError
        {
            Description = "Failed to unassign role"
        };

        Test.SetUserManagerRemoveFromRoleAsyncResult(() => Task.FromResult(IdentityResult.Failed(error)));

        var role = await Test.AddRoleAsync();
        var user = await Test.AddUserAsync();
        await Test.AssignRoleAsync(user, role);

        // Act
        var result = await Test.Target.UnassignAsync(role.Id, user.Id, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(Failure, result.Status);

        Assert.True(await Test.UserIsInRoleAsync(user, role));
        Assert.ContainsErrorCode(result, DefaultError, error.Description);
    }
    #endregion
}