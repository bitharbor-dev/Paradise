using Paradise.Domain.Base;

namespace Paradise.DataAccess.Repositories.Base;

/// <summary>
/// A repository data source abstraction.
/// </summary>
public interface IDataSource
{
    #region Methods
    /// <summary>
    /// Prepares the persistence storage to be used
    /// by the current <see cref="IDataSource"/> instance.
    /// </summary>
    void PreparePersistenceStorage();

    /// <summary>
    /// Asynchronously prepares the persistence storage to be used
    /// by the current <see cref="IDataSource"/> instance.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    Task PreparePersistenceStorageAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets queryable entities set.
    /// </summary>
    /// <typeparam name="TEntity">
    /// Entity type.
    /// </typeparam>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> of <typeparamref name="TEntity"/>.
    /// </returns>
    IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class, IDatabaseRecord;

    /// <summary>
    /// Marks the given <paramref name="entity"/> as added,
    /// so that it will be saved to the persistence storage
    /// when the <see cref="SaveChanges"/> or
    /// <see cref="SaveChangesAsync"/> method is called.
    /// </summary>
    /// <typeparam name="TEntity">
    /// Entity type.
    /// </typeparam>
    /// <param name="entity">
    /// The <typeparamref name="TEntity"/> to be marked.
    /// </param>
    void Add<TEntity>(TEntity entity) where TEntity : class, IDatabaseRecord;

    /// <summary>
    /// Marks the given <paramref name="entities"/> as added,
    /// so that they will be saved to the persistence storage
    /// when the <see cref="SaveChanges"/> or
    /// <see cref="SaveChangesAsync"/> method is called.
    /// </summary>
    /// <typeparam name="TEntity">
    /// Entity type.
    /// </typeparam>
    /// <param name="entities">
    /// The <see cref="IEnumerable{T}"/> of <typeparamref name="TEntity"/> to be marked.
    /// </param>
    void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IDatabaseRecord;

    /// <summary>
    /// Marks the given <paramref name="entity"/> as removed,
    /// so that it will be removed from the persistence storage
    /// when the <see cref="SaveChanges"/> or
    /// <see cref="SaveChangesAsync"/> method is called.
    /// </summary>
    /// <typeparam name="TEntity">
    /// Entity type.
    /// </typeparam>
    /// <param name="entity">
    /// The <typeparamref name="TEntity"/> to be marked.
    /// </param>
    void Remove<TEntity>(TEntity entity) where TEntity : class, IDatabaseRecord;

    /// <summary>
    /// Marks the given <paramref name="entities"/> as removed,
    /// so that they will be removed from the persistence storage
    /// when the <see cref="SaveChanges"/> or
    /// <see cref="SaveChangesAsync"/> method is called.
    /// </summary>
    /// <typeparam name="TEntity">
    /// Entity type.
    /// </typeparam>
    /// <param name="entities">
    /// The <see cref="IEnumerable{T}"/> of <typeparamref name="TEntity"/> to be marked.
    /// </param>
    void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IDatabaseRecord;

    /// <summary>
    /// Saves all changes into the persistence storage.
    /// </summary>
    /// <returns>
    /// The number of state entries written to the persistence storage.
    /// </returns>
    int SaveChanges();

    /// <summary>
    /// Asynchronously saves all changes into the persistence storage.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The number of state entries written to the persistence storage.
    /// </returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    #endregion
}