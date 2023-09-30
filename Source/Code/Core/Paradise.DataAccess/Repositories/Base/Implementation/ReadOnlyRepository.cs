using Microsoft.EntityFrameworkCore;
using Paradise.Domain.Base;
using System.Linq.Expressions;

namespace Paradise.DataAccess.Repositories.Base.Implementation;

/// <summary>
/// Base read-only repository class.
/// </summary>
/// <typeparam name="TEntity">
/// Entity type.
/// </typeparam>
public abstract class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity>
    where TEntity : class, IDatabaseRecord
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of <see cref="ReadOnlyRepository{TEntity}"/> class
    /// with the specified data source.
    /// </summary>
    /// <param name="source">
    /// Repository data source.
    /// </param>
    protected ReadOnlyRepository(IDataSource source)
        => Source = source;
    #endregion

    #region Properties
    /// <summary>
    /// Repository data source.
    /// </summary>
    protected IDataSource Source { get; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public bool Any()
        => Source.Set<TEntity>().Any();

    /// <inheritdoc/>
    public bool Any(Expression<Func<TEntity, bool>> predicate)
        => Source.Set<TEntity>().Any(predicate);

    /// <inheritdoc/>
    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        => Source.Set<TEntity>().AnyAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => Source.Set<TEntity>().AnyAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public int Count()
        => Source.Set<TEntity>().Count();

    /// <inheritdoc/>
    public int Count(Expression<Func<TEntity, bool>> predicate)
        => Source.Set<TEntity>().Count(predicate);

    /// <inheritdoc/>
    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => Source.Set<TEntity>().CountAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => Source.Set<TEntity>().CountAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public TEntity First()
        => Source.Set<TEntity>().First();

    /// <inheritdoc/>
    public TEntity First(Expression<Func<TEntity, bool>> predicate)
        => Source.Set<TEntity>().First(predicate);

    /// <inheritdoc/>
    public Task<TEntity> FirstAsync(CancellationToken cancellationToken = default)
        => Source.Set<TEntity>().FirstAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => Source.Set<TEntity>().FirstAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public TEntity? FirstOrDefault()
        => Source.Set<TEntity>().FirstOrDefault();

    /// <inheritdoc/>
    public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        => Source.Set<TEntity>().FirstOrDefault(predicate);

    /// <inheritdoc/>
    public Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        => Source.Set<TEntity>().FirstOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => Source.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public IReadOnlyCollection<TEntity> GetAll()
        => Source.Set<TEntity>().ToList().AsReadOnly();

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => (await Source.Set<TEntity>().ToListAsync(cancellationToken)).AsReadOnly();

    /// <inheritdoc/>
    public TEntity? GetById(Guid id)
        => Source.Set<TEntity>().SingleOrDefault(entity => entity.Id == id);

    /// <inheritdoc/>
    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Source.Set<TEntity>().SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    /// <inheritdoc/>
    public PagedListQueryResult<TEntity> GetPagedList(PagedListQuery<TEntity> query)
    {
        var set = Source.Set<TEntity>();

        query.Apply(ref set);

        var total = set.Count();

        set = set.Skip(query.PageSkip).Take(query.PageSize);

        var data = set.ToList();

        return new(data, total);
    }

    /// <inheritdoc/>
    public async Task<PagedListQueryResult<TEntity>> GetPagedListAsync(PagedListQuery<TEntity> query, CancellationToken cancellationToken = default)
    {
        var set = Source.Set<TEntity>();

        query.Apply(ref set);

        var total = await set.CountAsync(cancellationToken);

        set = set.Skip(query.PageSkip).Take(query.PageSize);

        var data = await set.ToListAsync(cancellationToken);

        return new(data, total);
    }

    /// <inheritdoc/>
    public IQueryable<TEntity> GetQueryable()
        => Source.Set<TEntity>();

    /// <inheritdoc/>
    public TEntity Last()
        => Source.Set<TEntity>().Last();

    /// <inheritdoc/>
    public TEntity Last(Expression<Func<TEntity, bool>> predicate)
        => Source.Set<TEntity>().Last(predicate);

    /// <inheritdoc/>
    public Task<TEntity> LastAsync(CancellationToken cancellationToken = default)
        => Source.Set<TEntity>().LastAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => Source.Set<TEntity>().LastAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public TEntity? LastOrDefault()
        => Source.Set<TEntity>().LastOrDefault();

    /// <inheritdoc/>
    public TEntity? LastOrDefault(Expression<Func<TEntity, bool>> predicate)
        => Source.Set<TEntity>().LastOrDefault(predicate);

    /// <inheritdoc/>
    public Task<TEntity?> LastOrDefaultAsync(CancellationToken cancellationToken = default)
        => Source.Set<TEntity>().LastOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity?> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => Source.Set<TEntity>().LastOrDefaultAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public TEntity Single()
        => Source.Set<TEntity>().Single();

    /// <inheritdoc/>
    public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        => Source.Set<TEntity>().Single(predicate);

    /// <inheritdoc/>
    public Task<TEntity> SingleAsync(CancellationToken cancellationToken = default)
        => Source.Set<TEntity>().SingleAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => Source.Set<TEntity>().SingleAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public TEntity? SingleOrDefault()
        => Source.Set<TEntity>().SingleOrDefault();

    /// <inheritdoc/>
    public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        => Source.Set<TEntity>().SingleOrDefault(predicate);

    /// <inheritdoc/>
    public Task<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
        => Source.Set<TEntity>().SingleOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => Source.Set<TEntity>().SingleOrDefaultAsync(predicate, cancellationToken);
    #endregion
}