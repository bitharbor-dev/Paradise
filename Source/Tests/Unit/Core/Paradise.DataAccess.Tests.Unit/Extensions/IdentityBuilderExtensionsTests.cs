using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Paradise.DataAccess.Database;
using Paradise.DataAccess.Extensions;
using Paradise.Domain.Identity.Roles;
using Paradise.Domain.Identity.Users;
using Identity = Microsoft.AspNetCore.Identity;

namespace Paradise.DataAccess.Tests.Unit.Extensions;

/// <summary>
/// <see cref="IdentityBuilderExtensions"/> test class.
/// </summary>
public sealed class IdentityBuilderExtensionsTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="IdentityBuilderExtensions.AddDataAccessStores"/> method should
    /// register the default <see cref="Identity.IUserStore{TUser}"/>
    /// and <see cref="Identity.IRoleStore{TRole}"/> implementations.
    /// </summary>
    [Fact]
    public void AddDataAccessStores()
    {
        // Arrange
        var services = new ServiceCollection();

        var builder = services
            .AddDbContext<DomainContext>(options => options.UseInMemoryDatabase(TestContext.Current.Test!.UniqueID))
            .AddIdentity<User, Role>();

        // Act
        builder.AddDataAccessStores();
        var provider = services.BuildServiceProvider();

        // Assert
        using var scope = provider.CreateScope();
        var scopedProvier = scope.ServiceProvider;

        Assert.NotNull(scopedProvier.GetService<Identity.IUserStore<User>>());
        Assert.NotNull(scopedProvier.GetService<Identity.IRoleStore<Role>>());
    }

    /// <summary>
    /// The <see cref="IdentityBuilderExtensions.AddDataAccessStores"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="Identity.IdentityBuilder"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void AddDataAccessStores_ThrowsOnNull()
    {
        // Arrange
        var builder = null as Identity.IdentityBuilder;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            IdentityBuilderExtensions.AddDataAccessStores(builder!));
    }
    #endregion
}