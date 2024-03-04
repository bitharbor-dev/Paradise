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
        => Source.GetQueryable<TEntity>().Any();

    /// <inheritdoc/>
    public bool Any(Expression<Func<TEntity, bool>> predicate)
        => Source.GetQueryable<TEntity>().Any(predicate);

    /// <inheritdoc/>
    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        => Source.GetQueryable<TEntity>().AnyAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => Source.GetQueryable<TEntity>().AnyAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public int Count()
        => Source.GetQueryable<TEntity>().Count();

    /// <inheritdoc/>
    public int Count(Expression<Func<TEntity, bool>> predicate)
        => Source.GetQueryable<TEntity>().Count(predicate);

    /// <inheritdoc/>
    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => Source.GetQueryable<TEntity>().CountAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => Source.GetQueryable<TEntity>().CountAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public TEntity First()
        => Source.GetQueryable<TEntity>().First();

    /// <inheritdoc/>
    public TEntity First(Expression<Func<TEntity, bool>> predicate)
        => Source.GetQueryable<TEntity>().First(predicate);

    /// <inheritdoc/>
    public Task<TEntity> FirstAsync(CancellationToken cancellationToken = default)
        => Source.GetQueryable<TEntity>().FirstAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => Source.GetQueryable<TEntity>().FirstAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public TEntity? FirstOrDefault()
        => Source.GetQueryable<TEntity>().FirstOrDefault();

    /// <inheritdoc/>
    public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        => Source.GetQueryable<TEntity>().FirstOrDefault(predicate);

    /// <inheritdoc/>
    public Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        => Source.GetQueryable<TEntity>().FirstOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => Source.GetQueryable<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public IReadOnlyCollection<TEntity> GetAll()
        => Source.GetQueryable<TEntity>().ToList().AsReadOnly();

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => (await Source.GetQueryable<TEntity>().ToListAsync(cancellationToken).ConfigureAwait(false)).AsReadOnly();

    /// <inheritdoc/>
    public TEntity? GetById(Guid id)
        => Source.GetQueryable<TEntity>().SingleOrDefault(entity => entity.Id == id);

    /// <inheritdoc/>
    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Source.GetQueryable<TEntity>().SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    /// <inheritdoc/>
    public PagedListQueryResult<TEntity> GetPagedList(PagedListQuery<TEntity> query)
    {
        ArgumentNullException.ThrowIfNull(query);

        var set = Source.GetQueryable<TEntity>();

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

        var set = Source.GetQueryable<TEntity>();

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
        => Source.GetQueryable<TEntity>();

    /// <inheritdoc/>
    public TEntity Last()
        => Source.GetQueryable<TEntity>().Last();

    /// <inheritdoc/>
    public TEntity Last(Expression<Func<TEntity, bool>> predicate)
        => Source.GetQueryable<TEntity>().Last(predicate);

    /// <inheritdoc/>
    public Task<TEntity> LastAsync(CancellationToken cancellationToken = default)
        => Source.GetQueryable<TEntity>().LastAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => Source.GetQueryable<TEntity>().LastAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public TEntity? LastOrDefault()
        => Source.GetQueryable<TEntity>().LastOrDefault();

    /// <inheritdoc/>
    public TEntity? LastOrDefault(Expression<Func<TEntity, bool>> predicate)
        => Source.GetQueryable<TEntity>().LastOrDefault(predicate);

    /// <inheritdoc/>
    public Task<TEntity?> LastOrDefaultAsync(CancellationToken cancellationToken = default)
        => Source.GetQueryable<TEntity>().LastOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity?> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => Source.GetQueryable<TEntity>().LastOrDefaultAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public TEntity Single()
        => Source.GetQueryable<TEntity>().Single();

    /// <inheritdoc/>
    public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        => Source.GetQueryable<TEntity>().Single(predicate);

    /// <inheritdoc/>
    public Task<TEntity> SingleAsync(CancellationToken cancellationToken = default)
        => Source.GetQueryable<TEntity>().SingleAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => Source.GetQueryable<TEntity>().SingleAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public TEntity? SingleOrDefault()
        => Source.GetQueryable<TEntity>().SingleOrDefault();

    /// <inheritdoc/>
    public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        => Source.GetQueryable<TEntity>().SingleOrDefault(predicate);

    /// <inheritdoc/>
    public Task<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
        => Source.GetQueryable<TEntity>().SingleOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => Source.GetQueryable<TEntity>().SingleOrDefaultAsync(predicate, cancellationToken);
    #endregion
}