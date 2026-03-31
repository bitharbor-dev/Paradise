using Paradise.Domain.Base;

namespace Paradise.DataAccess.Repositories.Base;

/// <summary>
/// Base read-only repository interface.
/// </summary>
/// <typeparam name="TEntity">
/// Entity type.
/// </typeparam>
public interface IReadOnlyRepository<TEntity>
    where TEntity : class, IDomainObject
{
    #region Methods
    /// <summary>
    /// Returns all entities in the repository.
    /// </summary>
    /// <returns>
    /// All entities in the repository.
    /// </returns>
    IReadOnlyCollection<TEntity> GetAll();

    /// <summary>
    /// Asynchronously returns all entities in the repository.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains all entities in the repository.
    /// </returns>
    Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds an entity with the given <paramref name="id"/>.
    /// If no entity is found, then <see langword="null"/> is returned.
    /// </summary>
    /// <param name="id">
    /// The Id of the entity to be found.
    /// </param>
    /// <returns>
    /// The entity found, or <see langword="null"/>.
    /// </returns>
    TEntity? GetById(Guid id);

    /// <summary>
    /// Asynchronously finds an entity with the given <paramref name="id"/>.
    /// If no entity is found, then <see langword="null"/> is returned.
    /// </summary>
    /// <param name="id">
    /// The Id of the entity to be found.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The entity found, or <see langword="null"/>.
    /// </returns>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the paged list of entities of the repository
    /// using the <paramref name="query"/> parameters.
    /// </summary>
    /// <param name="query">
    /// Contains paged list query properties.
    /// </param>
    /// <returns>
    /// The paged list of entities of the repository
    /// filtered by the given <paramref name="query"/> parameters.
    /// </returns>
    PagedListQueryResult<TEntity> GetPagedList(PagedListQuery<TEntity> query);

    /// <summary>
    /// Asynchronously gets the paged list of entities of the repository
    /// using the <paramref name="query"/> parameters.
    /// </summary>
    /// <param name="query">
    /// Contains paged list query properties.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the paged list of entities of the repository
    /// filtered by the given <paramref name="query"/> parameters.
    /// </returns>
    Task<PagedListQueryResult<TEntity>> GetPagedListAsync(PagedListQuery<TEntity> query, CancellationToken cancellationToken = default);
    #endregion
}