using Paradise.DataAccess.Repositories.Base.Implementation;
using Paradise.Domain.Base;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Paradise.DataAccess.Tests.Unit.Repositories.Base.Implementation;

/// <summary>
/// <see cref="Repository{TEntity}"/> test class.
/// </summary>
/// <typeparam name="TRepository">
/// Type of repository to run tests against.
/// </typeparam>
/// <typeparam name="TEntity">
/// Type of entity which is managed by <typeparamref name="TRepository"/>.
/// </typeparam>
public abstract class RepositoryTests<TRepository, TEntity> : ReadOnlyRepositoryTests<TRepository, TEntity>
    where TRepository : Repository<TEntity>
    where TEntity : class, IDomainObject
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryTests{TRepository, TEntity}"/> class.
    /// </summary>
    /// <param name="timeProvider">
    /// Time provider.
    /// </param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructors can not be protected.")]
    protected RepositoryTests(TimeProvider timeProvider) : base(timeProvider) { }
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="Repository{TEntity}.Add"/> method should
    /// mark the input <typeparamref name="TEntity"/> instance as added
    /// and preserve it to persistence storage upon saving changes.
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
        Assert.Contains(entity, Source.GetQueryable<TEntity>(), Comparer);
    }

    /// <summary>
    /// The <see cref="Repository{TEntity}.Add"/> method should
    /// throw the <see cref="ArgumentException"/> if the input
    /// <typeparamref name="TEntity"/> instance is already contained within repository.
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
            => Source.SaveChanges());
    }

    /// <summary>
    /// The <see cref="Repository{TEntity}.AddRange"/> method should
    /// mark the input range of <typeparamref name="TEntity"/> instances as added
    /// and preserve them to persistence storage upon saving changes.
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
        Assert.All(entities, entity => Assert.Contains(entity, Source.GetQueryable<TEntity>(), Comparer));
    }

    /// <summary>
    /// The <see cref="Repository{TEntity}.AddRange"/> method should
    /// throw the <see cref="ArgumentException"/> if the input
    /// range of <typeparamref name="TEntity"/> instances is already contained within repository.
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
            => Source.SaveChanges());
    }

    /// <summary>
    /// The <see cref="Repository{TEntity}.ForEach(Action{TEntity})"/> method should
    /// executes the specified <see cref="Action{T}"/> on each entity in the repository.
    /// </summary>
    [Fact]
    public void ForEach()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        var created = DateTimeOffset.UnixEpoch;

        // Act
        Repository.ForEach(entry => entry.OnCreated(created));
        Source.SaveChanges();

        // Assert
        Assert.All(Source.GetQueryable<TEntity>(), entity => Assert.True(entity.Created == created));
    }

    /// <summary>
    /// The <see cref="Repository{TEntity}.ForEach(Expression{Func{TEntity, bool}}, Action{TEntity})"/> method should
    /// execute the specified <see cref="Action{T}"/> on each entity in the repository
    /// which satisfies the specified condition.
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
        var created = DateTimeOffset.UnixEpoch;

        Expression<Func<TEntity, bool>> predicate =
            entry => entry.Id != id;

        // Act
        Repository.ForEach(predicate, entry => entry.OnCreated(created));
        Source.SaveChanges();

        // Assert
        Assert.All(Source.GetQueryable<TEntity>().Where(predicate), entity => Assert.True(entity.Created == created));
    }

    /// <summary>
    /// The <see cref="Repository{TEntity}.ForEachAsync(Action{TEntity}, CancellationToken)"/> method should
    /// execute the specified <see cref="Action{T}"/> on each entity in the repository.
    /// </summary>
    [Fact]
    public async Task ForEachAsync()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        await Source.SaveChangesAsync(Token);

        var created = DateTimeOffset.UnixEpoch;

        // Act
        await Repository.ForEachAsync(entry => entry.OnCreated(created), Token);
        await Source.SaveChangesAsync(Token);

        // Assert
        Assert.All(Source.GetQueryable<TEntity>(), entity => Assert.True(entity.Created == created));
    }

    /// <summary>
    /// The <see cref="Repository{TEntity}.ForEachAsync(Expression{Func{TEntity, bool}}, Action{TEntity}, CancellationToken)"/> method should
    /// execute the specified <see cref="Action{T}"/> on each entity in the repository
    /// which satisfies the specified condition.
    /// </summary>
    [Fact]
    public async Task ForEachAsync_WithCondition()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        await Source.SaveChangesAsync(Token);

        var id = entity1.Id;
        var created = DateTimeOffset.UnixEpoch;

        Expression<Func<TEntity, bool>> predicate =
            entry => entry.Id != id;

        // Act
        await Repository.ForEachAsync(predicate, entry => entry.OnCreated(created), Token);
        await Source.SaveChangesAsync(Token);

        // Assert
        Assert.All(Source.GetQueryable<TEntity>().Where(predicate), entity => Assert.True(entity.Created == created));
    }

    /// <summary>
    /// The <see cref="Repository{TEntity}.Remove"/> method should
    /// mark the input <typeparamref name="TEntity"/> instance as removed
    /// and discard it from persistence storage upon saving changes.
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
        Assert.DoesNotContain(entity, Source.GetQueryable<TEntity>(), Comparer);
    }

    /// <summary>
    /// The <see cref="Repository{TEntity}.RemoveById"/> method should
    /// mark the <typeparamref name="TEntity"/> instance with the specified Id as removed
    /// and discard it from persistence storage upon saving changes.
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
        Assert.DoesNotContain(entity, Source.GetQueryable<TEntity>(), Comparer);
    }

    /// <summary>
    /// The <see cref="Repository{TEntity}.RemoveRange"/> method should
    /// mark the input range of <typeparamref name="TEntity"/> instances as removed
    /// and discard them from persistence storage upon saving changes.
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
        Assert.All(entities, entity => Assert.DoesNotContain(entity, Source.GetQueryable<TEntity>(), Comparer));
    }

    /// <summary>
    /// The <see cref="Repository{TEntity}.RemoveRange"/> method should
    /// mark the <typeparamref name="TEntity"/> instances which satisfies the specified condition as removed
    /// and discard them from persistence storage upon saving changes.
    /// </summary>
    [Fact]
    public void RemoveWhere()
    {
        // Arrange
        var modified = DateTimeOffset.UnixEpoch;

        var entitiesToRemove = new[] { GetTestEntity(modified: modified), GetTestEntity(modified: modified) };

        var entitiesToKeep = new[] { GetTestEntity(modified: modified.AddDays(1)), GetTestEntity(modified: modified.AddDays(1)) };

        Source.AddRange(entitiesToRemove);
        Source.AddRange(entitiesToKeep);
        Source.SaveChanges();

        // Act
        Repository.RemoveWhere(entity => entity.Modified == modified);
        Source.SaveChanges();

        // Assert
        Assert.All(entitiesToRemove, entity => Assert.DoesNotContain(entity, Source.GetQueryable<TEntity>(), Comparer));
        Assert.All(entitiesToKeep, entity => Assert.Contains(entity, Source.GetQueryable<TEntity>(), Comparer));
    }
    #endregion
}