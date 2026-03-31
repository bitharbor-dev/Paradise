using Paradise.ApplicationLogic.EventListeners.Domain.Identity.Users;
using Paradise.Domain.Events.Identity.Users;

namespace Paradise.ApplicationLogic.Tests.Unit.EventListeners.Domain.Identity.Users;

/// <summary>
/// <see cref="AssignDefaultUserRoles"/> test class.
/// </summary>
public sealed partial class AssignDefaultUserRolesTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="AssignDefaultUserRoles.ProcessAsync"/> method should
    /// assign all default roles to the user.
    /// </summary>
    [Fact]
    public async Task ProcessAsync()
    {
        // Arrange
        var role1 = await Test.AddRoleAsync(name: "RoleOne", isDefault: true);
        var role2 = await Test.AddRoleAsync(name: "RoleTwo", isDefault: true);
        var user = await Test.AddUserAsync();

        var domainEvent = new EmailAddressConfirmedEvent(default, user.Id);

        // Act
        await Test.Target.ProcessAsync(domainEvent, Token);

        // Assert
        Assert.True(await Test.UserIsInRoleAsync(user, role1));
        Assert.True(await Test.UserIsInRoleAsync(user, role2));
    }

    /// <summary>
    /// The <see cref="AssignDefaultUserRoles.ProcessAsync"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the
    /// role assignment fails.
    /// </summary>
    [Fact]
    public async Task ProcessAsync_ThrowsOnAssignmentError()
    {
        // Arrange
        var nonExistingUserId = Guid.Empty;

        await Test.AddRoleAsync(name: "Role", isDefault: true);

        var domainEvent = new EmailAddressConfirmedEvent(default, nonExistingUserId);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Test.Target.ProcessAsync(domainEvent, Token));
    }
    #endregion
}