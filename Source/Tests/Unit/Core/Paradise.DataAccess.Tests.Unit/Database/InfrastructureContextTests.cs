using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Paradise.DataAccess.Database;
using Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Base;

namespace Paradise.DataAccess.Tests.Unit.Database;

/// <summary>
/// <see cref="InfrastructureContext"/> test class.
/// </summary>
public sealed partial class InfrastructureContextTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="InfrastructureContext.PreparePersistenceStorage"/> method should
    /// create the database to operate over using the default model configuration.
    /// </summary>
    [Fact]
    public void PreparePersistenceStorage()
    {
        // Arrange
        using var context = Test.CreateContext(ensureCreated: false, useFakeModel: false);

        // Act
        context.PreparePersistenceStorage();

        // Assert
        Assert.True(context.Database.EnsureDeleted());
    }

    /// <summary>
    /// The <see cref="InfrastructureContext.PreparePersistenceStorageAsync"/> method should
    /// create the database to operate over using the default model configuration.
    /// </summary>
    [Fact]
    public async Task PreparePersistenceStorageAsync()
    {
        // Arrange
        using var context = Test.CreateContext(ensureCreated: false, useFakeModel: false);

        // Act
        await context.PreparePersistenceStorageAsync(Token);

        // Assert
        Assert.True(await context.Database.EnsureDeletedAsync(Token));
    }

    /// <summary>
    /// The <see cref="InfrastructureContext.GetQueryable"/> method should
    /// return the <see cref="IQueryable{T}"/> instance representing
    /// the queryable set of the entities which type was passed as generic parameter.
    /// </summary>
    [Fact]
    public void GetQueryable()
    {
        // Arrange
        using var context = Test.CreateContext();

        // Act
        var queryable = context.GetQueryable<TestNamedEntity>();

        // Assert
        Assert.NotNull(queryable);
        Assert.IsType<IQueryable<TestNamedEntity>>(queryable, exactMatch: false);
    }

    /// <summary>
    /// The <see cref="IDataSource.Add"/> method should
    /// persist the entity in the database.
    /// </summary>
    [Fact]
    public void Add()
    {
        // Arrange
        using var context = Test.CreateContext();
        var entity = new TestNamedEntity();

        // Act
        ((IDataSource)context).Add(entity);
        context.SaveChanges();

        // Assert
        Assert.Contains(entity, context.Set<TestNamedEntity>());
    }

    /// <summary>
    /// The <see cref="IDataSource.AddRange"/> method should
    /// persist the set of entities in the database.
    /// </summary>
    [Fact]
    public void AddRange()
    {
        // Arrange
        using var context = Test.CreateContext();
        var entities = new[]
        {
            new TestNamedEntity(),
            new TestNamedEntity()
        };

        // Act
        ((IDataSource)context).AddRange(entities);
        context.SaveChanges();

        // Assert
        Assert.Collection(entities, entity => Assert.Contains(entity, context.Set<TestNamedEntity>()),
                                    entity => Assert.Contains(entity, context.Set<TestNamedEntity>()));
    }

    /// <summary>
    /// The <see cref="IDataSource.Remove"/> method should
    /// remove the entity from the database.
    /// </summary>
    [Fact]
    public void Remove()
    {
        // Arrange
        var entity = Test.AddEntity();

        using var context = Test.CreateContext();

        // Act
        ((IDataSource)context).Remove(entity);
        context.SaveChanges();

        // Assert
        Assert.DoesNotContain(entity, context.Set<TestNamedEntity>());
    }

    /// <summary>
    /// The <see cref="IDataSource.RemoveRange"/> method should
    /// remove the set of entities from the database.
    /// </summary>
    [Fact]
    public void RemoveRange()
    {
        // Arrange
        var entities = new[]
        {
            Test.AddEntity(),
            Test.AddEntity()
        };

        using var context = Test.CreateContext();

        // Act
        ((IDataSource)context).RemoveRange(entities);
        context.SaveChanges();

        // Assert
        Assert.Collection(entities, entity => Assert.DoesNotContain(entity, context.Set<TestNamedEntity>()),
                                    entity => Assert.DoesNotContain(entity, context.Set<TestNamedEntity>()));
    }

    /// <summary>
    /// The <see cref="InfrastructureContext.DataProtectionKeys"/> property should
    /// return the list of the persisted <see cref="DataProtectionKey"/> instances.
    /// </summary>
    [Fact]
    public void DataProtectionKeys()
    {
        // Arrange
        using var context = Test.CreateContext(ensureCreated: true, useFakeModel: false);

        var dataProtectionKey = new DataProtectionKey
        {
            FriendlyName = "FriendlyName"
        };

        context.Add(dataProtectionKey);
        context.SaveChanges();

        // Act & Assert
        var persistedKey = Assert.Single(context.DataProtectionKeys);
        Assert.Equal(dataProtectionKey.FriendlyName, persistedKey.FriendlyName);
    }
    #endregion
}