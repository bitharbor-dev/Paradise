using Microsoft.EntityFrameworkCore;
using Paradise.Domain.Base;
using System.Linq.Expressions;

namespace Paradise.DataAccess.Repositories.Base.Implementation;

/// <summary>
/// Base repository class.
/// </summary>
/// <typeparam name="TEntity">
/// Entity type.
/// </typeparam>
public abstract class Repository<TEntity> : ReadOnlyRepository<TEntity>, IRepository<TEntity>
    where TEntity : class, IDatabaseRecord
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of <see cref="Repository{TEntity}"/> class
    /// with the specified data source.
    /// </summary>
    /// <param name="source">
    /// Repository data source.
    /// </param>
    protected Repository(IDataSource source) : base(source) { }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Add(TEntity entity)
        => _source.Add(entity);

    /// <inheritdoc/>
    public void AddRange(IEnumerable<TEntity> entities)
        => _source.AddRange(entities);

    /// <inheritdoc/>
    public void Remove(TEntity entity)
        => _source.Remove(entity);

    /// <inheritdoc/>
    public void RemoveRange(IEnumerable<TEntity> entities)
        => _source.RemoveRange(entities);

    /// <inheritdoc/>
    public void RemoveById(Guid id)
    {
        var entity = _source.Set<TEntity>().SingleOrDefault(e => e.Id == id);
        if (entity is not null)
            _source.Remove(entity);
    }

    /// <inheritdoc/>
    public void RemoveWhere(Expression<Func<TEntity, bool>> predicate)
        => _source.RemoveRange(_source.Set<TEntity>().Where(predicate));

    /// <inheritdoc/>
    public void ForEach(Action<TEntity> action)
    {
        foreach (var entity in _source.Set<TEntity>())
            action(entity);
    }

    /// <inheritdoc/>
    public void ForEach(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action)
    {
        foreach (var entity in _source.Set<TEntity>().Where(predicate))
            action(entity);
    }

    /// <inheritdoc/>
    public int Commit()
        => _source.SaveChanges();

    /// <inheritdoc/>
    public Task ForEachAsync(Action<TEntity> action, CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().ForEachAsync(action, cancellationToken);

    /// <inheritdoc/>
    public Task ForEachAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action, CancellationToken cancellationToken = default)
        => _source.Set<TEntity>().Where(predicate).ForEachAsync(action, cancellationToken);

    /// <inheritdoc/>
    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => _source.SaveChangesAsync(cancellationToken);
    #endregion
}