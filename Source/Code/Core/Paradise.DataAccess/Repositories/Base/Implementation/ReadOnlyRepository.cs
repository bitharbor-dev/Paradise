using Microsoft.EntityFrameworkCore;
using Paradise.Domain.Base;

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
    where TEntity : class, IDomainObject
{
    #region Properties
    /// <summary>
    /// Repository data source.
    /// </summary>
    protected IDataSource Source { get; } = source;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public IReadOnlyCollection<TEntity> GetAll()
        => GetQueryableEntities().AsNoTrackingWithIdentityResolution().ToList().AsReadOnly();

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var data = await GetQueryableEntities()
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return data.AsReadOnly();
    }

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