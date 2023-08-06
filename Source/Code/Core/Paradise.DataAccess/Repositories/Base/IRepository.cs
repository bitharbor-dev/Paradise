using Paradise.Domain.Base;
using System.Linq.Expressions;

namespace Paradise.DataAccess.Repositories.Base;

/// <summary>
/// Base repository interface.
/// </summary>
/// <typeparam name="TEntity">
/// Entity type.
/// </typeparam>
public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>
    where TEntity : class, IDatabaseRecord
{
    #region Methods
    /// <summary>
    /// Marks the given <paramref name="entity"/> as added,
    /// so that it will be saved to the persistence storage
    /// when <see cref="Commit"/> or <see cref="CommitAsync"/> method is called.
    /// </summary>
    /// <param name="entity">
    /// The <typeparamref name="TEntity"/> to be marked.
    /// </param>
    void Add(TEntity entity);

    /// <summary>
    /// Marks the given <paramref name="entities"/> as added,
    /// so that they will be saved to the persistence storage
    /// when <see cref="Commit"/> or <see cref="CommitAsync"/> method is called.
    /// </summary>
    /// <param name="entities">
    /// The <see cref="IEnumerable{T}"/> of <typeparamref name="TEntity"/> to be marked.
    /// </param>
    void AddRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Marks the given <paramref name="entity"/> as removed,
    /// so that it will be removed from the persistence storage
    /// when <see cref="Commit"/> or <see cref="CommitAsync"/> method is called.
    /// </summary>
    /// <param name="entity">
    /// The entity to remove.
    /// </param>
    void Remove(TEntity entity);

    /// <summary>
    /// Marks the given <paramref name="entities"/> as removed,
    /// so that they will be removed from the persistence storage
    /// when <see cref="Commit"/> or <see cref="CommitAsync"/> method is called.
    /// </summary>
    /// <param name="entities">
    /// The <see cref="IEnumerable{T}"/> of <typeparamref name="TEntity"/> to be marked.
    /// </param>
    void RemoveRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Marks the entity with the given <paramref name="id"/> as removed,
    /// so that it will be removed from the persistence storage
    /// when <see cref="Commit"/> or <see cref="CommitAsync"/> method is called.
    /// </summary>
    /// <param name="id">
    /// The Id of the entity to be marked.
    /// </param>
    void RemoveById(Guid id);

    /// <summary>
    /// Marks the entities that satisfy the condition in
    /// <paramref name="predicate"/> as removed,
    /// so that they will be removed from the persistence storage
    /// when <see cref="Commit"/> or <see cref="CommitAsync"/> method is called.
    /// </summary>
    /// <param name="predicate">
    /// A function to test each element for a condition.
    /// </param>
    void RemoveWhere(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Enumerates the repository and performs
    /// the specified <paramref name="action"/>
    /// on each <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="action">
    /// The action to perform on each <typeparamref name="TEntity"/>.
    /// </param>
    void ForEach(Action<TEntity> action);

    /// <summary>
    /// Filters the repository with the given <paramref name="predicate"/>,
    /// enumerates the query results and performs
    /// the specified <paramref name="action"/>
    /// on each <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="predicate">
    /// A function to test each element for a condition.
    /// </param>
    /// <param name="action">
    /// The action to perform on each <typeparamref name="TEntity"/>.
    /// </param>
    void ForEach(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action);

    /// <summary>
    /// Saves all changes made in this repository to the persistence storage.
    /// </summary>
    /// <returns>
    /// The number of state entries written to the persistence storage.
    /// </returns>
    int Commit();

    /// <summary>
    /// Asynchronously enumerates the repository and performs
    /// the specified <paramref name="action"/>
    /// on each <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="action">
    /// The action to perform on each <typeparamref name="TEntity"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    Task ForEachAsync(Action<TEntity> action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously filters the repository with the given <paramref name="predicate"/>,
    /// enumerates the query results and performs
    /// the specified <paramref name="action"/>
    /// on each <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="predicate">
    /// A function to test each element for a condition.
    /// </param>
    /// <param name="action">
    /// The action to perform on each <typeparamref name="TEntity"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    Task ForEachAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes made in this repository to the persistence storage.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous save operation.
    /// The task result contains the number of state entries written to the persistence storage.
    /// </returns>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    #endregion
}