using Paradise.DataAccess;
using Paradise.DataAccess.Repositories.Base;
using Paradise.Domain.Base;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess.Repositories.Base;

/// <summary>
/// Fake <see cref="IRepository{TEntity}"/> implementation.
/// </summary>
/// <typeparam name="TEntity">
/// Entity type.
/// </typeparam>
public abstract class FakeRepositoryBase<TEntity> : FakeReadOnlyRepositoryBase<TEntity>, IRepository<TEntity>
    where TEntity : class, IDomainObject
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeReadOnlyRepositoryBase{TEntity}"/> class.
    /// </summary>
    /// <param name="source">
    /// Repository data source.
    /// </param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructors can not be protected.")]
    protected FakeRepositoryBase(IDataSource source) : base(source) { }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Add(TEntity entity)
        => _source.Add(entity);

    /// <inheritdoc/>
    public void AddRange(IEnumerable<TEntity> entities)
        => _source.AddRange(entities);

    /// <inheritdoc/>
    public void ForEach(Action<TEntity> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        foreach (var entity in _source.GetQueryable<TEntity>())
            action(entity);
    }

    /// <inheritdoc/>
    public void ForEach(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        foreach (var entity in _source.GetQueryable<TEntity>().Where(predicate))
            action(entity);
    }

    /// <inheritdoc/>
    public Task ForEachAsync(Action<TEntity> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        foreach (var entity in _source.GetQueryable<TEntity>())
            action(entity);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task ForEachAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        foreach (var entity in _source.GetQueryable<TEntity>().Where(predicate))
            action(entity);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Remove(TEntity entity)
        => _source.Remove(entity);

    /// <inheritdoc/>
    public void RemoveById(Guid id)
    {
        var entity = _source.GetQueryable<TEntity>().SingleOrDefault(e => e.Id == id);
        if (entity is not null)
            _source.Remove(entity);
    }

    /// <inheritdoc/>
    public void RemoveRange(IEnumerable<TEntity> entities)
        => _source.RemoveRange(entities);

    /// <inheritdoc/>
    public void RemoveWhere(Expression<Func<TEntity, bool>> predicate)
        => _source.RemoveRange(_source.GetQueryable<TEntity>().Where(predicate));
    #endregion
}