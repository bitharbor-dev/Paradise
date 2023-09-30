using Paradise.DataAccess.Repositories.Base.Implementation;
using Paradise.Domain.Base;
using Paradise.Domain.Base.EqualityComparers;
using System.Linq.Expressions;

namespace Paradise.DataAccess.Tests.Repositories.Base;

/// <summary>
/// Base repository test class.
/// </summary>
/// <typeparam name="TRepository">
/// Repository type to execute tests on.
/// </typeparam>
/// <typeparam name="TEntity">
/// Entity which is managed by <typeparamref name="TRepository"/>.
/// </typeparam>
public abstract class RepositoryTests<TRepository, TEntity> : ReadOnlyRepositoryTests<TRepository, TEntity>
    where TRepository : Repository<TEntity>
    where TEntity : class, IDatabaseRecord
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryTests{TRepository, TEntity}"/> class.
    /// </summary>
    protected RepositoryTests()
        => Comparer = new();
    #endregion

    #region Properties
    /// <summary>
    /// The <see cref="EntityEqualityComparer{TEntity}"/> to be used to
    /// compare <typeparamref name="TEntity"/> instances.
    /// </summary>
    public EntityEqualityComparer<IDatabaseRecord> Comparer { get; }
    #endregion

    #region Public methods
    /// <summary>
    /// <see cref="Repository{TEntity}.Add"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Adds an entity to the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Add()
    {
        // Arrange
        var entity = GetTestEntity();

        // Act
        Repository.Add(entity);
        Source.SaveChanges();

        // Assert
        Assert.Contains(entity, Source.Set<TEntity>(), Comparer);
    }

    /// <summary>
    /// <see cref="Repository{TEntity}.Add"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="ArgumentException"/> since the repository
    /// already contains the specified entity.
    /// </para>
    /// </summary>
    [Fact]
    public void Add_ThrowsOnDuplicate()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        Source.SaveChanges();

        // Act
        Repository.Add(entity);

        // Assert
        Assert.Throws<ArgumentException>(()
            => Repository.Commit());
    }

    /// <summary>
    /// <see cref="Repository{TEntity}.AddRange"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Adds a range of entities to the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void AddRange()
    {
        // Arrange
        var entities = new[]
        {
            GetTestEntity(),
            GetTestEntity(),
            GetTestEntity(),
            GetTestEntity()
        };

        // Act
        Repository.AddRange(entities);
        Source.SaveChanges();

        // Assert
        Assert.All(entities, entity => Assert.Contains(entity, Source.Set<TEntity>(), Comparer));
    }

    /// <summary>
    /// <see cref="Repository{TEntity}.AddRange"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="ArgumentException"/> since the repository
    /// already contains the specified entities.
    /// </para>
    /// </summary>
    [Fact]
    public void AddRange_ThrowsOnDuplicate()
    {
        // Arrange
        var entities = new[]
        {
            GetTestEntity(),
            GetTestEntity(),
            GetTestEntity(),
            GetTestEntity()
        };

        Source.AddRange(entities);
        Source.SaveChanges();

        Repository.AddRange(entities);

        // Act

        // Assert
        Assert.Throws<ArgumentException>(()
            => Repository.Commit());
    }

    /// <summary>
    /// <see cref="Repository{TEntity}.Commit"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Saves all changes made in the repository to the persistence storage and
    /// returns the number of state entries written to it.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Commit()
    {
        // Arrange
        var entity = GetTestEntity();
        Source.Add(entity);

        // Act
        var result = Repository.Commit();

        // Assert
        Assert.Equal(1, result);
    }

    /// <summary>
    /// <see cref="Repository{TEntity}.CommitAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Saves all changes made in the repository to the persistence storage and
    /// returns the number of state entries written to it.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void CommitAsync()
    {
        // Arrange
        var entity = GetTestEntity();
        Source.Add(entity);

        // Act
        var result = await Repository.CommitAsync();

        // Assert
        Assert.Equal(1, result);
    }

    /// <summary>
    /// <see cref="Repository{TEntity}.ForEach(Action{TEntity})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Executes the specified <see cref="Action{T}"/>
    /// on each entity in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void ForEach()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        var created = DateTime.UtcNow;

        // Act
        Repository.ForEach(entry => entry.Created = created);
        Source.SaveChanges();

        // Assert
        Assert.All(Source.Set<TEntity>(), entity => Assert.True(entity.Created == created));
    }

    /// <summary>
    /// <see cref="Repository{TEntity}.ForEach(Expression{Func{TEntity, bool}}, Action{TEntity})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Executes the specified <see cref="Action{T}"/>
    /// on each entity in the repository
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void ForEach_WithCondition()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        var id = entity1.Id;
        var created = DateTime.UtcNow;

        Expression<Func<TEntity, bool>> predicate =
            entry => entry.Id != id;

        // Act
        Repository.ForEach(predicate, entry => entry.Created = created);
        Source.SaveChanges();

        // Assert
        Assert.All(Source.Set<TEntity>().Where(predicate), entity => Assert.True(entity.Created == created));
    }

    /// <summary>
    /// <see cref="Repository{TEntity}.ForEachAsync(Action{TEntity}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Executes the specified <see cref="Action{T}"/>
    /// on each entity in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void ForEachAsync()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        var created = DateTime.UtcNow;

        // Act
        await Repository.ForEachAsync(entry => entry.Created = created);
        Source.SaveChanges();

        // Assert
        Assert.All(Source.Set<TEntity>(), entity => Assert.True(entity.Created == created));
    }

    /// <summary>
    /// <see cref="Repository{TEntity}.ForEachAsync(Expression{Func{TEntity, bool}}, Action{TEntity}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Executes the specified <see cref="Action{T}"/>
    /// on each entity in the repository
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void ForEachAsync_WithCondition()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        var id = entity1.Id;
        var created = DateTime.UtcNow;

        Expression<Func<TEntity, bool>> predicate =
            entry => entry.Id != id;

        // Act
        await Repository.ForEachAsync(predicate, entry => entry.Created = created);
        Source.SaveChanges();

        // Assert
        Assert.All(Source.Set<TEntity>().Where(predicate), entity => Assert.True(entity.Created == created));
    }

    /// <summary>
    /// <see cref="Repository{TEntity}.Remove"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Removes an entity from the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Remove()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        Source.SaveChanges();

        // Act
        Repository.Remove(entity);
        Source.SaveChanges();

        // Assert
        Assert.DoesNotContain(entity, Source.Set<TEntity>(), Comparer);
    }

    /// <summary>
    /// <see cref="Repository{TEntity}.RemoveById"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Removes an entity with the specified <see cref="IDatabaseRecord.Id"/>
    /// from the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void RemoveById()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        Source.SaveChanges();

        // Act
        Repository.RemoveById(entity.Id);
        Source.SaveChanges();

        // Assert
        Assert.DoesNotContain(entity, Source.Set<TEntity>(), Comparer);
    }

    /// <summary>
    /// <see cref="Repository{TEntity}.RemoveRange"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Removes a range of entities from the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void RemoveRange()
    {
        // Arrange
        var entities = new[]
        {
            GetTestEntity(),
            GetTestEntity(),
            GetTestEntity(),
            GetTestEntity()
        };

        Source.AddRange(entities);
        Source.SaveChanges();

        // Act
        Repository.RemoveRange(entities);
        Source.SaveChanges();

        // Assert
        Assert.All(entities, entity => Assert.DoesNotContain(entity, Source.Set<TEntity>(), Comparer));
    }

    /// <summary>
    /// <see cref="Repository{TEntity}.RemoveWhere"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Removes a range of entities from the repository
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void RemoveWhere()
    {
        // Arrange
        var modified = DateTime.UtcNow;

        var entitiesToRemove = new[] { GetTestEntity(modified: modified), GetTestEntity(modified: modified) };

        var entitiesToKeep = new[] { GetTestEntity(), GetTestEntity() };

        Source.AddRange(entitiesToRemove);
        Source.AddRange(entitiesToKeep);
        Source.SaveChanges();

        // Act
        Repository.RemoveWhere(entity => entity.Modified == modified);
        Source.SaveChanges();

        // Assert
        Assert.All(entitiesToRemove, entity => Assert.DoesNotContain(entity, Source.Set<TEntity>(), Comparer));
        Assert.All(entitiesToKeep, entity => Assert.Contains(entity, Source.Set<TEntity>(), Comparer));
    }
    #endregion
}