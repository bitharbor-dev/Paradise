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
/// <remarks>
/// Initializes a new instance of <see cref="ReadOnlyRepository{TEntity}"/> class
/// with the specified data source.
/// </remarks>
/// <param name="source">
/// Repository data source.
/// </param>
public abstract class ReadOnlyRepository<TEntity>(IDataSource source) : IReadOnlyRepository<TEntity>
    where TEntity : class, IDatabaseRecord
{
    #region Properties
    /// <summary>
    /// Repository data source.
    /// </summary>
    protected IDataSource Source { get; } = source;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public bool Any()
        => GetQueryableEntities().Any();

    /// <inheritdoc/>
    public bool Any(Expression<Func<TEntity, bool>> predicate)
        => GetQueryableEntities().Any(predicate);

    /// <inheritdoc/>
    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        => GetQueryableEntities().AnyAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => GetQueryableEntities().AnyAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public int Count()
        => GetQueryableEntities().Count();

    /// <inheritdoc/>
    public int Count(Expression<Func<TEntity, bool>> predicate)
        => GetQueryableEntities().Count(predicate);

    /// <inheritdoc/>
    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => GetQueryableEntities().CountAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => GetQueryableEntities().CountAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public TEntity First()
        => GetQueryableEntities().First();

    /// <inheritdoc/>
    public TEntity First(Expression<Func<TEntity, bool>> predicate)
        => GetQueryableEntities().First(predicate);

    /// <inheritdoc/>
    public Task<TEntity> FirstAsync(CancellationToken cancellationToken = default)
        => GetQueryableEntities().FirstAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => GetQueryableEntities().FirstAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public TEntity? FirstOrDefault()
        => GetQueryableEntities().FirstOrDefault();

    /// <inheritdoc/>
    public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        => GetQueryableEntities().FirstOrDefault(predicate);

    /// <inheritdoc/>
    public Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        => GetQueryableEntities().FirstOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => GetQueryableEntities().FirstOrDefaultAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public IReadOnlyCollection<TEntity> GetAll()
        => GetQueryableEntities().ToList().AsReadOnly();

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => (await GetQueryableEntities().ToListAsync(cancellationToken).ConfigureAwait(false)).AsReadOnly();

    /// <inheritdoc/>
    public TEntity? GetById(Guid id)
        => GetQueryableEntities().SingleOrDefault(entity => entity.Id == id);

    /// <inheritdoc/>
    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => GetQueryableEntities().SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    /// <inheritdoc/>
    public PagedListQueryResult<TEntity> GetPagedList(PagedListQuery<TEntity> query)
    {
        ArgumentNullException.ThrowIfNull(query);

        var set = GetQueryableEntities();

        query.Apply(ref set);

        var total = set.Count();

        set = set
            .Skip(query.PageSkip)
            .Take(query.PageSize);

        var data = set.ToList();

        return new(data, total);
    }

    /// <inheritdoc/>
    public async Task<PagedListQueryResult<TEntity>> GetPagedListAsync(PagedListQuery<TEntity> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var set = GetQueryableEntities();

        query.Apply(ref set);

        var total = await set
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);

        set = set
            .Skip(query.PageSkip)
            .Take(query.PageSize);

        var data = await set
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new(data, total);
    }

    /// <inheritdoc/>
    public IQueryable<TEntity> GetQueryable()
        => GetQueryableEntities();

    /// <inheritdoc/>
    public TEntity Last()
        => GetQueryableEntities().Last();

    /// <inheritdoc/>
    public TEntity Last(Expression<Func<TEntity, bool>> predicate)
        => GetQueryableEntities().Last(predicate);

    /// <inheritdoc/>
    public Task<TEntity> LastAsync(CancellationToken cancellationToken = default)
        => GetQueryableEntities().LastAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => GetQueryableEntities().LastAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public TEntity? LastOrDefault()
        => GetQueryableEntities().LastOrDefault();

    /// <inheritdoc/>
    public TEntity? LastOrDefault(Expression<Func<TEntity, bool>> predicate)
        => GetQueryableEntities().LastOrDefault(predicate);

    /// <inheritdoc/>
    public Task<TEntity?> LastOrDefaultAsync(CancellationToken cancellationToken = default)
        => GetQueryableEntities().LastOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity?> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => GetQueryableEntities().LastOrDefaultAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public TEntity Single()
        => GetQueryableEntities().Single();

    /// <inheritdoc/>
    public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        => GetQueryableEntities().Single(predicate);

    /// <inheritdoc/>
    public Task<TEntity> SingleAsync(CancellationToken cancellationToken = default)
        => GetQueryableEntities().SingleAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => GetQueryableEntities().SingleAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public TEntity? SingleOrDefault()
        => GetQueryableEntities().SingleOrDefault();

    /// <inheritdoc/>
    public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        => GetQueryableEntities().SingleOrDefault(predicate);

    /// <inheritdoc/>
    public Task<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
        => GetQueryableEntities().SingleOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => GetQueryableEntities().SingleOrDefaultAsync(predicate, cancellationToken);
    #endregion

    #region Private protected methods
    /// <summary>
    /// Gets the queryable entities set.
    /// </summary>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> of <typeparamref name="TEntity"/>.
    /// </returns>
    private protected IQueryable<TEntity> GetQueryableEntities()
        => Source.GetQueryable<TEntity>();
    #endregion
}