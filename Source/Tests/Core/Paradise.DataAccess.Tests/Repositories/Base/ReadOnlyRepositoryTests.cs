using Paradise.DataAccess.Repositories;
using Paradise.DataAccess.Repositories.Base;
using Paradise.DataAccess.Repositories.Base.Implementation;
using Paradise.Domain.Base;
using Paradise.Tests.Miscellaneous.Fakes.Core.DataAccess.Repositories.Base;
using System.Linq.Expressions;

namespace Paradise.DataAccess.Tests.Repositories.Base;

/// <summary>
/// Base read-only repository test class.
/// </summary>
/// <typeparam name="TRepository">
/// Repository type to execute tests on.
/// </typeparam>
/// <typeparam name="TEntity">
/// Entity which is managed by <typeparamref name="TRepository"/>.
/// </typeparam>
public abstract class ReadOnlyRepositoryTests<TRepository, TEntity>
    where TRepository : ReadOnlyRepository<TEntity>
    where TEntity : class, IDatabaseRecord
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ReadOnlyRepositoryTests{TRepository, TEntity}"/>
    /// class.
    /// </summary>
    protected ReadOnlyRepositoryTests()
    {
        Source = new FakeDataSource();

        var boxedRepository = Activator.CreateInstance(typeof(TRepository), Source);

        Repository = (TRepository)boxedRepository!;
    }
    #endregion

    #region Properties
    /// <summary>
    /// A <see cref="IDataSource"/> instance used to
    /// arrange data and validate test results.
    /// </summary>
    protected IDataSource Source { get; }

    /// <summary>
    /// A <typeparamref name="TRepository"/> instance to be tested.
    /// </summary>
    protected TRepository Repository { get; }
    #endregion

    #region Public methods
    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Any()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/> since the repository
    /// contains no entities.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Any_ReturnsFalseOnEmptyRepository()
    {
        // Arrange

        // Act
        var result = Repository.Any();

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Any()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="true"/> since the repository
    /// contains entities.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Any_ReturnsTrueOnNonEmptyRepository()
    {
        // Arrange
        var entity = GetTestEntity();
        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = Repository.Any();

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Any(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/> since the repository
    /// contains no entities
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Any_WithCondition_ReturnsFalse()
    {
        // Arrange
        var entity = GetTestEntity();
        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = Repository.Any(e => e.Id == Guid.Empty);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Any(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="true"/> since the repository
    /// contains entities
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Any_WithCondition_ReturnsTrue()
    {
        // Arrange
        var entity = GetTestEntity();
        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = Repository.Any(e => e.Id == entity.Id);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.AnyAsync(CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/> since the repository
    /// contains no entities.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void AnyAsync_ReturnsFalseOnEmptyRepository()
    {
        // Arrange

        // Act
        var result = await Repository.AnyAsync();

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.AnyAsync(CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="true"/> since the repository
    /// contains entities.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void AnyAsync_ReturnsTrueOnNonEmptyRepository()
    {
        // Arrange
        var entity = GetTestEntity();
        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = await Repository.AnyAsync();

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.AnyAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/> since the repository
    /// contains no entities
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void AnyAsync_WithCondition_ReturnsFalse()
    {
        // Arrange
        var entity = GetTestEntity();
        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = await Repository.AnyAsync(e => e.Id == Guid.Empty);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.AnyAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="true"/> since the repository
    /// contains entities
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void AnyAsync_WithCondition_ReturnsTrue()
    {
        // Arrange
        var entity = GetTestEntity();
        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = await Repository.AnyAsync(e => e.Id == entity.Id);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Count()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the number of entities in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Count()
    {
        // Arrange
        var entity = GetTestEntity();
        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = Repository.Count();

        // Assert
        Assert.Equal(1, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Count(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the number of entities in the repository
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Count_WithCondition()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();
        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = Repository.Count(e => e.Id == entity1.Id);

        // Assert
        Assert.Equal(1, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.CountAsync(CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the number of entities in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void CountAsync()
    {
        // Arrange
        var entity = GetTestEntity();
        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = await Repository.CountAsync();

        // Assert
        Assert.Equal(1, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.CountAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the number of entities in the repository
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void CountAsync_WithCondition()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();
        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = await Repository.CountAsync(e => e.Id == entity1.Id);

        // Assert
        Assert.Equal(1, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.First()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the first entity in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void First()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = Repository.First();

        // Assert
        Assert.Equal(entity1, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.First()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities.
    /// </para>
    /// </summary>
    [Fact]
    public void First_ThrowsOnEmptyRepository()
    {
        // Arrange

        // Act

        // Assert
        Assert.Throws<InvalidOperationException>(
            Repository.First);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.First(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the first entity in the repository
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void First_WithCondition()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = Repository.First(e => e.Id == entity1.Id);

        // Assert
        Assert.Equal(entity1, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.First(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities.
    /// </para>
    /// </summary>
    [Fact]
    public void First_WithCondition_ThrowsOnEmptyRepository()
    {
        // Arrange
        var entityId = Guid.NewGuid();

        // Act

        // Assert
        Assert.Throws<InvalidOperationException>(()
            => Repository.First(entity => entity.Id == entityId));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.First(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities
    /// which satisfies the specified condition.
    /// </para>
    /// </summary>
    [Fact]
    public void First_WithCondition_ThrowsOnNonMatchingCondition()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        Source.SaveChanges();

        // Act

        // Assert
        Assert.Throws<InvalidOperationException>(()
            => Repository.First(e => e.Id != entity.Id));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.FirstAsync(CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the first entity in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void FirstAsync()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = await Repository.FirstAsync();

        // Assert
        Assert.Equal(entity1, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.FirstAsync(CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities.
    /// </para>
    /// </summary>
    [Fact]
    public async void FirstAsync_ThrowsOnEmptyRepository()
    {
        // Arrange

        // Act

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Repository.FirstAsync());
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.FirstAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the first entity in the repository
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void FirstAsync_WithCondition()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = await Repository.FirstAsync(e => e.Id == entity1.Id);

        // Assert
        Assert.Equal(entity1, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.FirstAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities.
    /// </para>
    /// </summary>
    [Fact]
    public async void FirstAsync_WithCondition_ThrowsOnEmptyRepository()
    {
        // Arrange
        var entityId = Guid.NewGuid();

        // Act

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Repository.FirstAsync(entity => entity.Id == entityId));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.FirstAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities
    /// which satisfies the specified condition.
    /// </para>
    /// </summary>
    [Fact]
    public async void FirstAsync_WithCondition_ThrowsOnNonMatchingCondition()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        Source.SaveChanges();

        // Act

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Repository.FirstAsync(e => e.Id != entity.Id));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.FirstOrDefault()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the first entity in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void FirstOrDefault()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = Repository.FirstOrDefault();

        // Assert
        Assert.Equal(entity1, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.FirstOrDefault()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void FirstOrDefault_ReturnsNullOnEmptyRepository()
    {
        // Arrange

        // Act
        var result = Repository.FirstOrDefault();

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.FirstOrDefault(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the first entity in the repository
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void FirstOrDefault_WithCondition()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = Repository.FirstOrDefault(e => e.Id == entity1.Id);

        // Assert
        Assert.Equal(entity1, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.FirstOrDefault(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void FirstOrDefault_WithCondition_ReturnsNullOnEmptyRepository()
    {
        // Arrange
        var entityId = Guid.NewGuid();

        // Act
        var result = Repository.FirstOrDefault(e => e.Id == entityId);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.FirstOrDefault(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void FirstOrDefault_WithCondition_ReturnsNullOnNonMatchingCondition()
    {
        // Arrange
        var entity = GetTestEntity();
        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = Repository.FirstOrDefault(e => e.Id != entity.Id);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.FirstOrDefaultAsync(CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the first entity in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void FirstOrDefaultAsync()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = await Repository.FirstOrDefaultAsync();

        // Assert
        Assert.Equal(entity1, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.FirstOrDefaultAsync(CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void FirstOrDefaultAsync_ReturnsNullOnEmptyRepository()
    {
        // Arrange

        // Act
        var result = await Repository.FirstOrDefaultAsync();

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.FirstOrDefaultAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the first entity in the repository
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void FirstOrDefaultAsync_WithCondition()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = await Repository.FirstOrDefaultAsync(e => e.Id == entity1.Id);

        // Assert
        Assert.Equal(entity1, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.FirstOrDefaultAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void FirstOrDefaultAsync_WithCondition_ReturnsNullOnEmptyRepository()
    {
        // Arrange
        var entityId = Guid.NewGuid();

        // Act
        var result = await Repository.FirstOrDefaultAsync(e => e.Id == entityId);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.FirstOrDefaultAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void FirstOrDefaultAsync_WithCondition_ReturnsNullOnNonMatchingCondition()
    {
        // Arrange
        var entity = GetTestEntity();
        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = await Repository.FirstOrDefaultAsync(e => e.Id != entity.Id);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.GetAll"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the list of all entities in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetAll()
    {
        // Arrange
        Source.Add(GetTestEntity());
        Source.SaveChanges();

        // Act
        var result = Repository.GetAll().Count;

        // Assert
        Assert.Equal(Source.GetQueryable<TEntity>().Count(), result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.GetAllAsync(CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the list of all entities in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void GetAllAsync()
    {
        // Arrange
        Source.Add(GetTestEntity());
        Source.SaveChanges();

        // Act
        var result = (await Repository.GetAllAsync()).Count;

        // Assert
        Assert.Equal(Source.GetQueryable<TEntity>().Count(), result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.GetById"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns an entity
    /// with the specified <see cref="IDatabaseRecord.Id"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetById()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = Repository.GetById(entity.Id);

        // Assert
        Assert.Equal(entity, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.GetById"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities
    /// with the specified <see cref="IDatabaseRecord.Id"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetById_ReturnsNullOnNonMatchingId()
    {
        // Arrange
        var entityId = Guid.Empty;

        Source.Add(GetTestEntity());
        Source.SaveChanges();

        // Act
        var result = Repository.GetById(entityId);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.GetByIdAsync(Guid, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns an entity
    /// with the specified <see cref="IDatabaseRecord.Id"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void GetByIdAsync()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = await Repository.GetByIdAsync(entity.Id);

        // Assert
        Assert.Equal(entity, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.GetByIdAsync(Guid, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities
    /// with the specified <see cref="IDatabaseRecord.Id"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void GetByIdAsync_ReturnsNullOnNonMatchingId()
    {
        // Arrange
        var entityId = Guid.Empty;

        Source.Add(GetTestEntity());
        Source.SaveChanges();

        // Act
        var result = await Repository.GetByIdAsync(entityId);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.GetPagedList"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the <see cref="PagedListQueryResult{TEntity}"/>
    /// containing the total number of entities which satisfies
    /// conditions given in the input <see cref="PagedListQuery{TEntity}"/>
    /// and a list of entities representing particular "page".
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetPagedList()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();
        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        var query = new PagedListQuery<TEntity>
        {
            PageSize = 2,
            PageNumber = 1
        };

        // Act
        var result = Repository.GetPagedList(query);

        // Assert
        Assert.Equal(2, result.Total);
        Assert.Equal(result.CurrentPageItems.Count(), query.PageSize);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.GetPagedList"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the <see cref="PagedListQueryResult{TEntity}"/>
    /// containing the total number of entities which satisfies
    /// conditions given in the input <see cref="PagedListQuery{TEntity}"/>
    /// and a list of entities representing particular "page".
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetPagedList_WithFilter()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();
        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        var query = new PagedListQuery<TEntity>
        {
            Filter = e => e.Id == entity1.Id,
            PageSize = 2,
            PageNumber = 1
        };

        // Act
        var result = Repository.GetPagedList(query);

        // Assert
        Assert.Equal(1, result.Total);
        Assert.Single(result.CurrentPageItems);
        Assert.Equal(result.CurrentPageItems.First().Id, entity1.Id);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.GetPagedList"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the <see cref="PagedListQueryResult{TEntity}"/>
    /// containing the total number of entities which satisfies
    /// conditions given in the input <see cref="PagedListQuery{TEntity}"/>
    /// and a list of entities representing particular "page".
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetPagedList_WithOrderingProperty()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        var set = new[] { entity1, entity2 };
        Source.AddRange(set);
        Source.SaveChanges();

        var query = new PagedListQuery<TEntity>
        {
            PageSize = 2,
            PageNumber = 1,
            OrderAscending = true,
            OrderBy = nameof(IDatabaseRecord.Id)
        };

        // Act
        var result = Repository.GetPagedList(query);

        // Assert
        Assert.Equal(2, result.Total);
        Assert.Equal(result.CurrentPageItems.Count(), query.PageSize);
        Assert.True(set.OrderBy(entity => entity.Id).SequenceEqual(result.CurrentPageItems));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.GetPagedList"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the <see cref="PagedListQueryResult{TEntity}"/>
    /// containing the total number of entities which satisfies
    /// conditions given in the input <see cref="PagedListQuery{TEntity}"/>
    /// and a list of entities representing particular "page".
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetPagedList_WithPageNumber()
    {
        // Arrange
        const int PageSize = 3;

        var firstPage = new TEntity[PageSize]
        {
            GetTestEntity(),
            GetTestEntity(),
            GetTestEntity()
        };

        var secondPage = new TEntity[PageSize]
        {
            GetTestEntity(),
            GetTestEntity(),
            GetTestEntity()
        };

        Source.AddRange(firstPage);
        Source.AddRange(secondPage);
        Source.SaveChanges();

        var query = new PagedListQuery<TEntity>
        {
            PageSize = PageSize,
            PageNumber = 2
        };

        // Act
        var result = Repository.GetPagedList(query);

        // Assert
        Assert.Equal(firstPage.Length + secondPage.Length, result.Total);
        Assert.All(result.CurrentPageItems, item => Assert.Contains(item, secondPage));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.GetPagedListAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the <see cref="PagedListQueryResult{TEntity}"/>
    /// containing the total number of entities which satisfies
    /// conditions given in the input <see cref="PagedListQuery{TEntity}"/>
    /// and a list of entities representing particular "page".
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void GetPagedListAsync()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();
        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        var query = new PagedListQuery<TEntity>
        {
            PageSize = 2,
            PageNumber = 1
        };

        // Act
        var result = await Repository.GetPagedListAsync(query);

        // Assert
        Assert.Equal(2, result.Total);
        Assert.Equal(result.CurrentPageItems.Count(), query.PageSize);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.GetPagedListAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the <see cref="PagedListQueryResult{TEntity}"/>
    /// containing the total number of entities which satisfies
    /// conditions given in the input <see cref="PagedListQuery{TEntity}"/>
    /// and a list of entities representing particular "page".
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void GetPagedListAsync_WithFilter()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();
        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        var query = new PagedListQuery<TEntity>
        {
            Filter = e => e.Id == entity1.Id,
            PageSize = 2,
            PageNumber = 1
        };

        // Act
        var result = await Repository.GetPagedListAsync(query);

        // Assert
        Assert.Equal(1, result.Total);
        Assert.Single(result.CurrentPageItems);
        Assert.Equal(result.CurrentPageItems.First().Id, entity1.Id);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.GetPagedListAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the <see cref="PagedListQueryResult{TEntity}"/>
    /// containing the total number of entities which satisfies
    /// conditions given in the input <see cref="PagedListQuery{TEntity}"/>
    /// and a list of entities representing particular "page".
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void GetPagedListAsync_WithOrderingProperty()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        var set = new[] { entity1, entity2 };
        Source.AddRange(set);
        Source.SaveChanges();

        var query = new PagedListQuery<TEntity>
        {
            PageSize = 2,
            PageNumber = 1,
            OrderAscending = true,
            OrderBy = nameof(IDatabaseRecord.Id)
        };

        // Act
        var result = await Repository.GetPagedListAsync(query);

        // Assert
        Assert.Equal(2, result.Total);
        Assert.Equal(result.CurrentPageItems.Count(), query.PageSize);
        Assert.True(set.OrderBy(entity => entity.Id).SequenceEqual(result.CurrentPageItems));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.GetPagedListAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the <see cref="PagedListQueryResult{TEntity}"/>
    /// containing the total number of entities which satisfies
    /// conditions given in the input <see cref="PagedListQuery{TEntity}"/>
    /// and a list of entities representing particular "page".
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void GetPagedListAsync_WithPageNumber()
    {
        // Arrange
        const int PageSize = 3;

        var firstPage = new TEntity[PageSize]
        {
            GetTestEntity(),
            GetTestEntity(),
            GetTestEntity()
        };

        var secondPage = new TEntity[PageSize]
        {
            GetTestEntity(),
            GetTestEntity(),
            GetTestEntity()
        };

        Source.AddRange(firstPage);
        Source.AddRange(secondPage);
        Source.SaveChanges();

        var query = new PagedListQuery<TEntity>
        {
            PageSize = PageSize,
            PageNumber = 2
        };

        // Act
        var result = await Repository.GetPagedListAsync(query);

        // Assert
        Assert.Equal(firstPage.Length + secondPage.Length, result.Total);
        Assert.All(result.CurrentPageItems, item => Assert.Contains(item, secondPage));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Last()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the last entity in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Last()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = Repository.Last();

        // Assert
        Assert.Equal(entity2, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Last()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities.
    /// </para>
    /// </summary>
    [Fact]
    public void Last_ThrowsOnEmptyRepository()
    {
        // Arrange

        // Act

        // Assert
        Assert.Throws<InvalidOperationException>(
            Repository.Last);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Last(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the last entity in the repository
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Last_WithCondition()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = Repository.Last(e => e.Id == entity2.Id);

        // Assert
        Assert.Equal(entity2, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Last(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities.
    /// </para>
    /// </summary>
    [Fact]
    public void Last_WithCondition_ThrowsOnEmptyRepository()
    {
        // Arrange
        var entityId = Guid.NewGuid();

        // Act

        // Assert
        Assert.Throws<InvalidOperationException>(()
            => Repository.Last(entity => entity.Id == entityId));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Last(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities
    /// which satisfies the specified condition.
    /// </para>
    /// </summary>
    [Fact]
    public void Last_WithCondition_ThrowsOnNonMatchingCondition()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        Source.SaveChanges();

        // Act

        // Assert
        Assert.Throws<InvalidOperationException>(()
            => Repository.Last(e => e.Id != entity.Id));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.LastAsync(CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the last entity in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void LastAsync()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = await Repository.LastAsync();

        // Assert
        Assert.Equal(entity2, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.LastAsync(CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities.
    /// </para>
    /// </summary>
    [Fact]
    public async void LastAsync_ThrowsOnEmptyRepository()
    {
        // Arrange

        // Act

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Repository.LastAsync());
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.LastAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the last entity in the repository
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void LastAsync_WithCondition()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = await Repository.LastAsync(e => e.Id == entity2.Id);

        // Assert
        Assert.Equal(entity2, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.LastAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities.
    /// </para>
    /// </summary>
    [Fact]
    public async void LastAsync_WithCondition_ThrowsOnEmptyRepository()
    {
        // Arrange
        var entityId = Guid.NewGuid();

        // Act

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Repository.LastAsync(entity => entity.Id == entityId));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.LastAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities
    /// which satisfies the specified condition.
    /// </para>
    /// </summary>
    [Fact]
    public async void LastAsync_WithCondition_ThrowsOnNonMatchingCondition()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        Source.SaveChanges();

        // Act

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Repository.LastAsync(e => e.Id != entity.Id));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.LastOrDefault()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the last entity in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void LastOrDefault()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = Repository.LastOrDefault();

        // Assert
        Assert.Equal(entity2, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.LastOrDefault()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void LastOrDefault_ReturnsNullOnEmptyRepository()
    {
        // Arrange

        // Act
        var result = Repository.LastOrDefault();

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.LastOrDefault(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the last entity in the repository
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void LastOrDefault_WithCondition()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = Repository.LastOrDefault(e => e.Id == entity2.Id);

        // Assert
        Assert.Equal(entity2, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.LastOrDefault(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void LastOrDefault_WithCondition_ReturnsNullOnEmptyRepository()
    {
        // Arrange
        var entityId = Guid.NewGuid();

        // Act
        var result = Repository.LastOrDefault(e => e.Id == entityId);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.LastOrDefault(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void LastOrDefault_WithCondition_ReturnsNullOnNonMatchingCondition()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = Repository.LastOrDefault(e => e.Id != entity.Id);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.LastOrDefaultAsync(CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the last entity in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void LastOrDefaultAsync()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = await Repository.LastOrDefaultAsync();

        // Assert
        Assert.Equal(entity2, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.LastOrDefaultAsync(CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void LastOrDefaultAsync_ReturnsNullOnEmptyRepository()
    {
        // Arrange

        // Act
        var result = await Repository.LastOrDefaultAsync();

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.LastOrDefaultAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the last entity in the repository
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void LastOrDefaultAsync_WithCondition()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = await Repository.LastOrDefaultAsync(e => e.Id == entity2.Id);

        // Assert
        Assert.Equal(entity2, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.LastOrDefaultAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void LastOrDefaultAsync_WithCondition_ReturnsNullOnEmptyRepository()
    {
        // Arrange
        var entityId = Guid.NewGuid();

        // Act
        var result = await Repository.LastOrDefaultAsync(e => e.Id == entityId);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.LastOrDefaultAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void LastOrDefaultAsync_WithCondition_ReturnsNullOnNonMatchingCondition()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = await Repository.LastOrDefaultAsync(e => e.Id != entity.Id);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Single()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns a single entity in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Single()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = Repository.Single();

        // Assert
        Assert.NotNull(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Single()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities.
    /// </para>
    /// </summary>
    [Fact]
    public void Single_ThrowsOnEmptyRepository()
    {
        // Arrange

        // Act

        // Assert
        Assert.Throws<InvalidOperationException>(
            Repository.Single);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Single()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains more than one entity.
    /// </para>
    /// </summary>
    [Fact]
    public void Single_ThrowsOnMultiple()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act

        // Assert
        Assert.Throws<InvalidOperationException>(Repository.Single);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Single(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns a single entity in the repository
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Single_WithCondition()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = Repository.Single(e => e.Id == entity1.Id);

        // Assert
        Assert.Equal(entity1, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Single(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities.
    /// </para>
    /// </summary>
    [Fact]
    public void Single_WithCondition_ThrowsOnEmptyRepository()
    {
        // Arrange
        var entityId = Guid.NewGuid();

        // Act

        // Assert
        Assert.Throws<InvalidOperationException>(()
            => Repository.Single(entity => entity.Id == entityId));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Single(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains more than one entity
    /// which satisfies the specified condition.
    /// </para>
    /// </summary>
    [Fact]
    public void Single_WithCondition_ThrowsOnMultiple()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act

        // Assert
        Assert.Throws<InvalidOperationException>(()
            => Repository.Single(e => e.Id != Guid.Empty));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.Single(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities
    /// which satisfies the specified condition.
    /// </para>
    /// </summary>
    [Fact]
    public void Single_WithCondition_ThrowsOnNonMatchingCondition()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        Source.SaveChanges();

        // Act

        // Assert
        Assert.Throws<InvalidOperationException>(()
            => Repository.Single(e => e.Id != entity.Id));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleAsync(CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns a single entity in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void SingleAsync()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = await Repository.SingleAsync();

        // Assert
        Assert.NotNull(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleAsync(CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities.
    /// </para>
    /// </summary>
    [Fact]
    public async void SingleAsync_ThrowsOnEmptyRepository()
    {
        // Arrange

        // Act

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Repository.SingleAsync());
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleAsync(CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains more than one entity.
    /// </para>
    /// </summary>
    [Fact]
    public async void SingleAsync_ThrowsOnMultiple()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Repository.SingleAsync());
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns a single entity in the repository
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void SingleAsync_WithCondition()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = await Repository.SingleAsync(e => e.Id == entity1.Id);

        // Assert
        Assert.Equal(entity1, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities.
    /// </para>
    /// </summary>
    [Fact]
    public async void SingleAsync_WithCondition_ThrowsOnEmptyRepository()
    {
        // Arrange
        var entityId = Guid.NewGuid();

        // Act

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Repository.SingleAsync(entity => entity.Id == entityId));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains more than one entity
    /// which satisfies the specified condition.
    /// </para>
    /// </summary>
    [Fact]
    public async void SingleAsync_WithCondition_ThrowsOnMultiple()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Repository.SingleAsync(e => e.Id != Guid.Empty));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains no entities
    /// which satisfies the specified condition.
    /// </para>
    /// </summary>
    [Fact]
    public async void SingleAsync_WithCondition_ThrowsOnNonMatchingCondition()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        Source.SaveChanges();

        // Act

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Repository.SingleAsync(e => e.Id != entity.Id));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleOrDefault()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns a single entity in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void SingleOrDefault()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = Repository.SingleOrDefault();

        // Assert
        Assert.NotNull(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleOrDefault()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void SingleOrDefault_ReturnsNullOnEmptyRepository()
    {
        // Arrange

        // Act
        var result = Repository.SingleOrDefault();

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleOrDefault()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains more than one entity.
    /// </para>
    /// </summary>
    [Fact]
    public void SingleOrDefault_ThrowsOnMultiple()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act

        // Assert
        Assert.Throws<InvalidOperationException>(Repository.SingleOrDefault);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleOrDefault(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns a single entity in the repository
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void SingleOrDefault_WithCondition()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = Repository.SingleOrDefault(e => e.Id == entity1.Id);

        // Assert
        Assert.Equal(entity1, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleOrDefault(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void SingleOrDefault_WithCondition_ReturnsNullOnEmptyRepository()
    {
        // Arrange
        var entityId = Guid.NewGuid();

        // Act
        var result = Repository.SingleOrDefault(e => e.Id == entityId);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleOrDefault(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void SingleOrDefault_WithCondition_ReturnsNullOnNonMatchingCondition()
    {
        // Arrange
        var entity = GetTestEntity();
        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = Repository.SingleOrDefault(e => e.Id != entity.Id);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleOrDefault(Expression{Func{TEntity, bool}})"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains more than one entity
    /// which satisfies the specified condition.
    /// </para>
    /// </summary>
    [Fact]
    public void SingleOrDefault_WithCondition_ThrowsOnMultiple()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act

        // Assert
        Assert.Throws<InvalidOperationException>(()
            => Repository.SingleOrDefault(e => e.Id != Guid.Empty));
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleOrDefault()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns a single entity in the repository.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void SingleOrDefaultAsync()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = Repository.SingleOrDefault();

        // Assert
        Assert.NotNull(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleOrDefault()"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void SingleOrDefaultAsync_ReturnsNullOnEmptyRepository()
    {
        // Arrange

        // Act
        var result = Repository.SingleOrDefault();

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleOrDefaultAsync(CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains more than one entity.
    /// </para>
    /// </summary>
    [Fact]
    public async void SingleOrDefaultAsync_ThrowsOnMultiple()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Repository.SingleOrDefaultAsync());
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleOrDefaultAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns a single entity in the repository
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void SingleOrDefaultAsync_WithCondition()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act
        var result = await Repository.SingleOrDefaultAsync(e => e.Id == entity1.Id);

        // Assert
        Assert.Equal(entity1, result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleOrDefaultAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void SingleOrDefaultAsync_WithCondition_ReturnsNullOnEmptyRepository()
    {
        // Arrange
        var entityId = Guid.NewGuid();

        // Act
        var result = await Repository.SingleOrDefaultAsync(e => e.Id == entityId);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleOrDefaultAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository
    /// contains no entities
    /// which satisfies the specified condition.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async void SingleOrDefaultAsync_WithCondition_ReturnsNullOnNonMatchingCondition()
    {
        // Arrange
        var entity = GetTestEntity();
        Source.Add(entity);
        Source.SaveChanges();

        // Act
        var result = await Repository.SingleOrDefaultAsync(e => e.Id != entity.Id);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// <see cref="ReadOnlyRepository{TEntity}.SingleOrDefaultAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="InvalidOperationException"/> since the repository
    /// contains more than one entity
    /// which satisfies the specified condition.
    /// </para>
    /// </summary>
    [Fact]
    public async void SingleOrDefaultAsync_WithCondition_ThrowsOnMultiple()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        Source.AddRange([entity1, entity2]);
        Source.SaveChanges();

        // Act

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Repository.SingleOrDefaultAsync(e => e.Id != Guid.Empty));
    }
    #endregion

    #region Protected methods
    /// <summary>
    /// Gets the new <typeparamref name="TEntity"/>
    /// instance to be used during tests execution.
    /// </summary>
    /// <param name="id">
    /// Unique identifier.
    /// </param>
    /// <param name="created">
    /// Creation date.
    /// </param>
    /// <param name="modified">
    /// Last changed date.
    /// </param>
    /// <returns>
    /// New <typeparamref name="TEntity"/> instance.
    /// </returns>
    protected abstract TEntity GetTestEntity(Guid? id = null, DateTime? created = null, DateTime? modified = null);
    #endregion
}