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
    #region Fields
    /// <summary>
    /// Repository data source.
    /// </summary>
    protected readonly IDataSource _source;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of <see cref="ReadOnlyRepository{TEntity}"/> class
    /// with the specified data source.
    /// </summary>
    /// <param name="source">
    /// Repository data source.
    /// </param>
    protected ReadOnlyRepository(IDataSource source)
        => _source = source;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public IReadOnlyCollection<TEntity> GetAll()
        => _source.Set<TEntity>().ToList().AsReadOnly();

    /// <inheritdoc/>
    public IQueryable<TEntity> GetQueryable()
        => _source.Set<TEntity>();

    /// <inheritdoc/>
    public TEntity? GetById(Guid id)
        => _source.Set<TEntity>().SingleOrDefault(entity => entity.Id == id);

    /// <inheritdoc/>
    public bool Any()
        => _source.Set<TEntity>().Any();

    /// <inheritdoc/>
    public bool Any(Expression<Func<TEntity, bool>> predicate)
        => _source.Set<TEntity>().Any(predicate);

    /// <inheritdoc/>
    public int Count()
        => _source.Set<TEntity>().Count();

    /// <inheritdoc/>
    public int Count(Expression<Func<TEntity, bool>> predicate)
        => _source.Set<TEntity>().Count(predicate);

    /// <inheritdoc/>
    public TEntity First()
        => _source.Set<TEntity>().First();

    /// <inheritdoc/>
    public TEntity First(Expression<Func<TEntity, bool>> predicate)
        => _source.Set<TEntity>().First(predicate);

    /// <inheritdoc/>
    public TEntity? FirstOrDefault()
        => _source.Set<TEntity>().FirstOrDefault();

    /// <inheritdoc/>
    public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        => _source.Set<TEntity>().FirstOrDefault(predicate);

    /// <inheritdoc/>
    public TEntity Last()
        => _source.Set<TEntity>().Last();

    /// <inheritdoc/>
    public TEntity Last(Expression<Func<TEntity, bool>> predicate)
        => _source.Set<TEntity>().Last(predicate);

    /// <inheritdoc/>
    public TEntity? LastOrDefault()
        => _source.Set<TEntity>().LastOrDefault();

    /// <inheritdoc/>
    public TEntity? LastOrDefault(Expression<Func<TEntity, bool>> predicate)
        => _source.Set<TEntity>().LastOrDefault(predicate);

    /// <inheritdoc/>
    public TEntity Single()
        => _source.Set<TEntity>().Single();

    /// <inheritdoc/>
    public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        => _source.Set<TEntity>().Single(predicate);

    /// <inheritdoc/>
    public TEntity? SingleOrDefault()
        => _source.Set<TEntity>().SingleOrDefault();

    /// <inheritdoc/>
    public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        => _source.Set<TEntity>().SingleOrDefault(predicate);

    /// <inheritdoc/>
    public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().AnyAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().AnyAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().CountAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().CountAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity> FirstAsync(CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().FirstAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().FirstAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().FirstOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity> LastAsync(CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().LastAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().LastAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity?> LastOrDefaultAsync(CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().LastOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity?> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().LastOrDefaultAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity> SingleAsync(CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().SingleAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().SingleAsync(predicate, cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().SingleOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().SingleOrDefaultAsync(predicate, cancellationToken);
    #endregion
}