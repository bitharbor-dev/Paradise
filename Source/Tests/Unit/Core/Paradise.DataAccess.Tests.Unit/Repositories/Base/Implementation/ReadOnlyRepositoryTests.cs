using Paradise.DataAccess.Repositories;
using Paradise.DataAccess.Repositories.Base.Implementation;
using Paradise.Domain.Base;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.DataAccess.Tests.Unit.Repositories.Base.Implementation;

/// <summary>
/// <see cref="ReadOnlyRepository{TEntity}"/> test class.
/// </summary>
/// <typeparam name="TRepository">
/// Repository type to execute tests on.
/// </typeparam>
/// <typeparam name="TEntity">
/// Entity which is managed by <typeparamref name="TRepository"/>.
/// </typeparam>
public abstract class ReadOnlyRepositoryTests<TRepository, TEntity>
    where TRepository : ReadOnlyRepository<TEntity>
    where TEntity : class, IDomainObject
{
    #region Fields
    private readonly FakeDataSource _source;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyRepositoryTests{TRepository, TEntity}"/> class.
    /// </summary>
    /// <param name="timeProvider">
    /// Time provider.
    /// </param>
    protected ReadOnlyRepositoryTests(TimeProvider timeProvider)
    {
        _source = new(timeProvider);

        Source = _source;

        var boxedRepository = Activator.CreateInstance(typeof(TRepository), Source);

        Repository = (TRepository)boxedRepository!;
    }
    #endregion

    #region Properties
    /// <summary>
    /// The <see cref="IEqualityComparer{T}"/> to be used to
    /// compare <typeparamref name="TEntity"/> instances.
    /// </summary>
    protected IEqualityComparer<TEntity?> Comparer { get; } = new TestEqualityComparer();

    /// <summary>
    /// An <see cref="IDataSource"/> instance used to
    /// arrange data and validate test results.
    /// </summary>
    protected IDataSource Source { get; }

    /// <summary>
    /// System under test.
    /// </summary>
    protected TRepository Repository { get; }

    /// <summary>
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </summary>
    protected CancellationToken Token { get; } = TestContext.Current.CancellationToken;
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="ReadOnlyRepository{TEntity}.GetAll"/> method should
    /// return the list of all entities in the repository.
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
    /// The <see cref="ReadOnlyRepository{TEntity}.GetAllAsync"/> method should
    /// return the list of all entities in the repository.
    /// </summary>
    [Fact]
    public async Task GetAllAsync()
    {
        // Arrange
        Source.Add(GetTestEntity());
        await Source.SaveChangesAsync(Token);

        // Act
        var result = (await Repository.GetAllAsync(Token)).Count;

        // Assert
        Assert.Equal(Source.GetQueryable<TEntity>().Count(), result);
    }

    /// <summary>
    /// The <see cref="ReadOnlyRepository{TEntity}.GetById"/> method should
    /// return an entity with the specified Id.
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
        Assert.Equal(entity, result, Comparer);
    }

    /// <summary>
    /// The <see cref="ReadOnlyRepository{TEntity}.GetById"/> method should
    /// return <see langword="null"/> if the
    /// repository contains no entities
    /// with the specified Id.
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
    /// The <see cref="ReadOnlyRepository{TEntity}.GetByIdAsync"/> method should
    /// return an entity with the specified Id.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        await Source.SaveChangesAsync(Token);

        // Act
        var result = await Repository.GetByIdAsync(entity.Id, Token);

        // Assert
        Assert.Equal(entity, result, Comparer);
    }

    /// <summary>
    /// The <see cref="ReadOnlyRepository{TEntity}.GetByIdAsync"/> method should
    /// return <see langword="null"/> if the
    /// repository contains no entities
    /// with the specified Id.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsNullOnNonMatchingId()
    {
        // Arrange
        var entityId = Guid.Empty;

        Source.Add(GetTestEntity());
        await Source.SaveChangesAsync(Token);

        // Act
        var result = await Repository.GetByIdAsync(entityId, Token);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// The <see cref="ReadOnlyRepository{TEntity}.GetPagedList"/> method should
    /// return the <see cref="PagedListQueryResult{TEntity}"/>
    /// containing the total number of entities which satisfies
    /// conditions given in the input <see cref="PagedListQuery{TEntity}"/>
    /// and the list of entities representing a particular "page".
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
    /// The <see cref="ReadOnlyRepository{TEntity}.GetPagedList"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="PagedListQuery{TEntity}"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void GetPagedList_ThrowsOnNull()
    {
        // Arrange
        var query = null as PagedListQuery<TEntity>;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => Repository.GetPagedList(query!));
    }

    /// <summary>
    /// The <see cref="ReadOnlyRepository{TEntity}.GetPagedList"/> method should
    /// return the <see cref="PagedListQueryResult{TEntity}"/>
    /// containing the total number of entities which satisfies
    /// conditions given in the input <see cref="PagedListQuery{TEntity}"/>
    /// and the list of entities representing a particular "page".
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

        var item = Assert.Single(result.CurrentPageItems);
        Assert.Equal(item.Id, entity1.Id);
    }

    /// <summary>
    /// The <see cref="ReadOnlyRepository{TEntity}.GetPagedList"/> method should
    /// return the <see cref="PagedListQueryResult{TEntity}"/>
    /// containing the total number of entities which satisfies
    /// conditions given in the input <see cref="PagedListQuery{TEntity}"/>
    /// and the list of entities representing a particular "page".
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
            OrderBy = nameof(IDomainObject.Id)
        };

        // Act
        var result = Repository.GetPagedList(query);

        // Assert
        Assert.Equal(2, result.Total);
        Assert.Equal(result.CurrentPageItems.Count(), query.PageSize);

        var isSameOrder = set
            .OrderBy(entity => entity.Id)
            .SequenceEqual(result.CurrentPageItems);

        Assert.True(isSameOrder);
    }

    /// <summary>
    /// The <see cref="ReadOnlyRepository{TEntity}.GetPagedList"/> method should
    /// return the <see cref="PagedListQueryResult{TEntity}"/>
    /// containing the total number of entities which satisfies
    /// conditions given in the input <see cref="PagedListQuery{TEntity}"/>
    /// and the list of entities representing a particular "page".
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
        Assert.All(result.CurrentPageItems, item => Assert.Contains(item, secondPage, Comparer));
    }

    /// <summary>
    /// The <see cref="ReadOnlyRepository{TEntity}.GetPagedListAsync"/> method should
    /// return the <see cref="PagedListQueryResult{TEntity}"/>
    /// containing the total number of entities which satisfies
    /// conditions given in the input <see cref="PagedListQuery{TEntity}"/>
    /// and the list of entities representing a particular "page".
    /// </summary>
    [Fact]
    public async Task GetPagedListAsync()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();
        Source.AddRange([entity1, entity2]);
        await Source.SaveChangesAsync(Token);

        var query = new PagedListQuery<TEntity>
        {
            PageSize = 2,
            PageNumber = 1
        };

        // Act
        var result = await Repository.GetPagedListAsync(query, Token);

        // Assert
        Assert.Equal(2, result.Total);
        Assert.Equal(result.CurrentPageItems.Count(), query.PageSize);
    }

    /// <summary>
    /// The <see cref="ReadOnlyRepository{TEntity}.GetPagedListAsync"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="PagedListQuery{TEntity}"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task GetPagedListAsync_ThrowsOnNull()
    {
        // Arrange
        var query = null as PagedListQuery<TEntity>;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(()
            => Repository.GetPagedListAsync(query!, Token));
    }

    /// <summary>
    /// The <see cref="ReadOnlyRepository{TEntity}.GetPagedListAsync"/> method should
    /// return the <see cref="PagedListQueryResult{TEntity}"/>
    /// containing the total number of entities which satisfies
    /// conditions given in the input <see cref="PagedListQuery{TEntity}"/>
    /// and the list of entities representing a particular "page".
    /// </summary>
    [Fact]
    public async Task GetPagedListAsync_WithFilter()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();
        Source.AddRange([entity1, entity2]);
        await Source.SaveChangesAsync(Token);

        var query = new PagedListQuery<TEntity>
        {
            Filter = e => e.Id == entity1.Id,
            PageSize = 2,
            PageNumber = 1
        };

        // Act
        var result = await Repository.GetPagedListAsync(query, Token);

        // Assert
        Assert.Equal(1, result.Total);

        var item = Assert.Single(result.CurrentPageItems);
        Assert.Equal(item.Id, entity1.Id);
    }

    /// <summary>
    /// The <see cref="ReadOnlyRepository{TEntity}.GetPagedListAsync"/> method should
    /// return the <see cref="PagedListQueryResult{TEntity}"/>
    /// containing the total number of entities which satisfies
    /// conditions given in the input <see cref="PagedListQuery{TEntity}"/>
    /// and the list of entities representing a particular "page".
    /// </summary>
    [Fact]
    public async Task GetPagedListAsync_WithOrderingProperty()
    {
        // Arrange
        var entity1 = GetTestEntity();
        var entity2 = GetTestEntity();

        var set = new[] { entity1, entity2 };
        Source.AddRange(set);
        await Source.SaveChangesAsync(Token);

        var query = new PagedListQuery<TEntity>
        {
            PageSize = 2,
            PageNumber = 1,
            OrderAscending = true,
            OrderBy = nameof(IDomainObject.Id)
        };

        // Act
        var result = await Repository.GetPagedListAsync(query, Token);

        // Assert
        Assert.Equal(2, result.Total);
        Assert.Equal(result.CurrentPageItems.Count(), query.PageSize);

        var isSameOrder = set
            .OrderBy(entity => entity.Id)
            .SequenceEqual(result.CurrentPageItems);

        Assert.True(isSameOrder);
    }

    /// <summary>
    /// The <see cref="ReadOnlyRepository{TEntity}.GetPagedListAsync"/> method should
    /// return the <see cref="PagedListQueryResult{TEntity}"/>
    /// containing the total number of entities which satisfies
    /// conditions given in the input <see cref="PagedListQuery{TEntity}"/>
    /// and the list of entities representing a particular "page".
    /// </summary>
    [Fact]
    public async Task GetPagedListAsync_WithPageNumber()
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
        await Source.SaveChangesAsync(Token);

        var query = new PagedListQuery<TEntity>
        {
            PageSize = PageSize,
            PageNumber = 2
        };

        // Act
        var result = await Repository.GetPagedListAsync(query, Token);

        // Assert
        Assert.Equal(firstPage.Length + secondPage.Length, result.Total);
        Assert.All(result.CurrentPageItems, item => Assert.Contains(item, secondPage, Comparer));
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
    protected abstract TEntity GetTestEntity(Guid? id = null, DateTimeOffset? created = null, DateTimeOffset? modified = null);
    #endregion

    #region Nested types
    /// <summary>
    /// Test-only equality comparer implementation.
    /// </summary>
    private sealed class TestEqualityComparer : IEqualityComparer<TEntity?>
    {
        #region Public methods
        /// <inheritdoc/>
        public bool Equals(TEntity? x, TEntity? y)
            => x?.Id == y?.Id;

        /// <inheritdoc/>
        public int GetHashCode([DisallowNull] TEntity obj)
            => obj.Id.GetHashCode();
        #endregion
    }
    #endregion
}