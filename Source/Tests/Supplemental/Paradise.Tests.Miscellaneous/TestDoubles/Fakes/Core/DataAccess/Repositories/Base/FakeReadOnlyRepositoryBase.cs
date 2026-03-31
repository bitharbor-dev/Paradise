using Paradise.DataAccess;
using Paradise.DataAccess.Repositories;
using Paradise.DataAccess.Repositories.Base;
using Paradise.Domain.Base;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess.Repositories.Base;

/// <summary>
/// Fake <see cref="IReadOnlyRepository{TEntity}"/> implementation.
/// </summary>
/// <typeparam name="TEntity">
/// Entity type.
/// </typeparam>
public abstract class FakeReadOnlyRepositoryBase<TEntity> : IReadOnlyRepository<TEntity>
    where TEntity : class, IDomainObject
{
    #region Fields
    private protected readonly IDataSource _source;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeReadOnlyRepositoryBase{TEntity}"/> class.
    /// </summary>
    /// <param name="source">
    /// Repository data source.
    /// </param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructors can not be protected.")]
    protected FakeReadOnlyRepositoryBase(IDataSource source)
        => _source = source;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public IReadOnlyCollection<TEntity> GetAll()
        => _source.GetQueryable<TEntity>().ToList();

    /// <inheritdoc/>
    public Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyCollection<TEntity>>(_source.GetQueryable<TEntity>().ToList().AsReadOnly());

    /// <inheritdoc/>
    public TEntity? GetById(Guid id)
        => _source.GetQueryable<TEntity>().FirstOrDefault(entity => entity.Id == id);

    /// <inheritdoc/>
    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_source.GetQueryable<TEntity>().FirstOrDefault(entity => entity.Id == id));

    /// <inheritdoc/>
    public PagedListQueryResult<TEntity> GetPagedList(PagedListQuery<TEntity> query)
    {
        ArgumentNullException.ThrowIfNull(query);

        var set = _source.GetQueryable<TEntity>();

        query.Apply(ref set);

        var total = set.Count();

        set = set
            .Skip(query.PageSkip)
            .Take(query.PageSize);

        var data = set.ToList();

        return new(data, total);
    }

    /// <inheritdoc/>
    public Task<PagedListQueryResult<TEntity>> GetPagedListAsync(PagedListQuery<TEntity> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var set = _source.GetQueryable<TEntity>();

        query.Apply(ref set);

        var total = set.Count();

        set = set
            .Skip(query.PageSkip)
            .Take(query.PageSize);

        var data = set.ToList();

        return Task.FromResult(new PagedListQueryResult<TEntity>(data, total));
    }
    #endregion
}