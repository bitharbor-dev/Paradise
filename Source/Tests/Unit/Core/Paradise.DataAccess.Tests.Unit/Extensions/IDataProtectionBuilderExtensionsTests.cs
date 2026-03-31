using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.DataAccess.Database;
using Paradise.DataAccess.Extensions;

namespace Paradise.DataAccess.Tests.Unit.Extensions;

/// <summary>
/// <see cref="IDataProtectionBuilderExtensions"/> test class.
/// </summary>
public sealed class IDataProtectionBuilderExtensionsTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="IDataProtectionBuilderExtensions.PersistKeysToDataAccess"/> method should
    /// register the EF Core based <see cref="IXmlRepository"/> implementation.
    /// </summary>
    [Fact]
    public void PersistKeysToDataAccess()
    {
        // Arrange
        var services = new ServiceCollection();

        var builder = services
            .AddDbContext<InfrastructureContext>(options => options.UseInMemoryDatabase(TestContext.Current.Test!.UniqueID))
            .AddDataProtection();

        // Act
        builder.PersistKeysToDataAccess();
        var provider = services.BuildServiceProvider();

        // Assert
        using var scope = provider.CreateScope();
        var scopedProvier = scope.ServiceProvider;

        var xmlRepository = scopedProvier.GetRequiredService<IOptions<KeyManagementOptions>>().Value.XmlRepository;

        Assert.IsType<EntityFrameworkCoreXmlRepository<InfrastructureContext>>(xmlRepository);
    }
    #endregion
}