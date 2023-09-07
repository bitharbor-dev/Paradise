using Paradise.Domain.Base;
using System.Linq.Expressions;

namespace Paradise.DataAccess.Repositories.Base;

/// <summary>
/// Base read-only repository interface.
/// </summary>
/// <typeparam name="TEntity">
/// Entity type.
/// </typeparam>
public interface IReadOnlyRepository<TEntity>
    where TEntity : class, IDatabaseRecord
{
    #region Methods
    /// <summary>
    /// Determines whether the repository contains any entities.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the repository contains any entities,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    bool Any();

    /// <summary>
    /// Determines whether any entity in the repository satisfies a condition.
    /// </summary>
    /// <param name="predicate">
    /// A function to test each entity for a condition.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if any entities in the repository
    /// pass the test in the specified <paramref name="predicate"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    bool Any(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Asynchronously determines whether the repository contains any entities.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains <see langword="true"/>
    /// if the repository contains any entities,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously determines whether any entity in the repository satisfies a condition.
    /// </summary>
    /// <param name="predicate">
    /// A function to test each entity for a condition.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains <see langword="true"/>
    /// if any entities in the repository pass the test
    /// in the specified <paramref name="predicate"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the number of entities in the repository.
    /// </summary>
    /// <returns>
    /// The number of entities in the repository.
    /// </returns>
    int Count();

    /// <summary>
    /// Returns the number of entities in the repository that satisfies a condition.
    /// </summary>
    /// <param name="predicate">
    /// A function to test each entity for a condition.
    /// </param>
    /// <returns>
    /// The number of entities in the repository
    /// that satisfies the condition in the predicate function.
    /// </returns>
    int Count(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Asynchronously returns the number of entities
    /// in the repository.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the number of entities in the repository.
    /// </returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously returns the number of entities
    /// in the repository that satisfy a condition.
    /// </summary>
    /// <param name="predicate">
    /// A function to test each entity for a condition.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the number of entities in the repository
    /// that satisfy the condition in the predicate function.
    /// </returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first entity in the repository.
    /// <para>
    /// The implementations of this method should throw
    /// an exception if the repository contains no entities.
    /// </para>
    /// </summary>
    /// <returns>
    /// The first entity in the repository.
    /// </returns>
    TEntity First();

    /// <summary>
    /// Returns the first entity in the repository
    /// that satisfies a specified condition.
    /// <para>
    /// The implementations of this method should throw
    /// an exception if the repository contains no entities
    /// or no entity satisfies the condition in <paramref name="predicate"/>.
    /// </para>
    /// </summary>
    /// <param name="predicate">
    /// A function to test each entity for a condition.
    /// </param>
    /// <returns>
    /// The first entity in the repository
    /// that passes the test in <paramref name="predicate"/>.
    /// </returns>
    TEntity First(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Asynchronously returns the first entity in the repository.
    /// <para>
    /// The implementations of this method should throw
    /// an exception if the repository contains no entities.
    /// </para>
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the first entity in the repository.
    /// </returns>
    Task<TEntity> FirstAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously returns the first entity in the repository
    /// that satisfies a specified condition.
    /// <para>
    /// The implementations of this method should throw
    /// an exception if the repository contains no entities
    /// or no entity satisfies the condition in <paramref name="predicate"/>.
    /// </para>
    /// </summary>
    /// <param name="predicate">
    /// A function to test each entity for a condition.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the first entity in the repository
    /// that passes the test in <paramref name="predicate"/>.
    /// </returns>
    Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first entity in the repository,
    /// or a <see langword="default"/> value if the repository contains no entities.
    /// </summary>
    /// <returns>
    /// <see langword="default"/>(<typeparamref name="TEntity"/>)
    /// if the repository is empty.
    /// Otherwise, the first entity in the repository.
    /// </returns>
    TEntity? FirstOrDefault();

    /// <summary>
    /// Returns the first entity in the repository
    /// that satisfies a specified condition,
    /// or a <see langword="default"/> value if no such entity is found.
    /// </summary>
    /// <param name="predicate">
    /// A function to test each entity for a condition.
    /// </param>
    /// <returns>
    /// <see langword="default"/>(<typeparamref name="TEntity"/>)
    /// if the repository is empty or if no entity passes the test
    /// specified by <paramref name="predicate"/>.
    /// Otherwise, the first entity in the repository that passes the test
    /// specified by <paramref name="predicate"/>.
    /// </returns>
    TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Asynchronously returns the first entity in the repository,
    /// or a <see langword="default"/> value if the repository contains no entities.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains
    /// <see langword="default"/>(<typeparamref name="TEntity"/>)
    /// if the repository is empty.
    /// Otherwise, the first entity in the repository.
    /// </returns>
    Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously returns the first entity in the repository
    /// that satisfies a specified condition,
    /// or a <see langword="default"/> value if no such entity is found.
    /// </summary>
    /// <param name="predicate">
    /// A function to test each entity for a condition.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains
    /// <see langword="default"/>(<typeparamref name="TEntity"/>)
    /// if the repository is empty or if no entity passes the test
    /// specified by <paramref name="predicate"/>.
    /// Otherwise, the first entity in the repository that passes the test
    /// specified by <paramref name="predicate"/>.
    /// </returns>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

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

    /// <summary>
    /// Gets queryable entities set.
    /// </summary>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> that represents the current repository.
    /// </returns>
    IQueryable<TEntity> GetQueryable();

    /// <summary>
    /// Returns the last entity in the repository.
    /// <para>
    /// The implementations of this method should throw
    /// an exception if the repository contains no entities.
    /// </para>
    /// </summary>
    /// <returns>
    /// The last entity in the repository.
    /// </returns>
    TEntity Last();

    /// <summary>
    /// Returns the last entity in the repository
    /// that satisfies a specified condition.
    /// <para>
    /// The implementations of this method should throw
    /// an exception if the repository contains no entities
    /// or no entity satisfies the condition in <paramref name="predicate"/>.
    /// </para>
    /// </summary>
    /// <param name="predicate">
    /// A function to test an entity for a condition.
    /// </param>
    /// <returns>
    /// The last entity in the repository
    /// that passes the test in <paramref name="predicate"/>.
    /// </returns>
    TEntity Last(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Asynchronously returns the last entity in the repository.
    /// <para>
    /// The implementations of this method should throw
    /// an exception if the repository contains no entities.
    /// </para>
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the last entity in the repository.
    /// </returns>
    Task<TEntity> LastAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously returns the last entity in the repository
    /// that satisfies a specified condition.
    /// <para>
    /// The implementations of this method should throw
    /// an exception if the repository contains no entities
    /// or no entity satisfies the condition in <paramref name="predicate"/>.
    /// </para>
    /// </summary>
    /// <param name="predicate">
    /// A function to test an entity for a condition.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the last entity in the repository
    /// that passes the test in <paramref name="predicate"/>.
    /// </returns>
    Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the last entity in the repository,
    /// or a <see langword="default"/> value if the repository contains no entities.
    /// </summary>
    /// <returns>
    /// <see langword="default"/>(<typeparamref name="TEntity"/>)
    /// if the repository is empty.
    /// Otherwise, the last entity in the repository.
    /// </returns>
    TEntity? LastOrDefault();

    /// <summary>
    /// Returns the last entity in the repository
    /// that satisfies a specified condition,
    /// or a <see langword="default"/> value if no such entity is found.
    /// </summary>
    /// <param name="predicate">
    /// A function to test an entity for a condition.
    /// </param>
    /// <returns>
    /// <see langword="default"/>(<typeparamref name="TEntity"/>)
    /// if the repository is empty or if no entity passes the test
    /// specified by <paramref name="predicate"/>.
    /// Otherwise, the last entity in the repository that passes the test
    /// specified by <paramref name="predicate"/>.
    /// </returns>
    TEntity? LastOrDefault(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Asynchronously returns the last entity in the repository,
    /// or a <see langword="default"/> value if the repository contains no entities.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains
    /// <see langword="default"/>(<typeparamref name="TEntity"/>)
    /// if the repository is empty.
    /// Otherwise, the last entity in the repository.
    /// </returns>
    Task<TEntity?> LastOrDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously returns the last entity in the repository
    /// that satisfies a specified condition,
    /// or a <see langword="default"/> value if no such entity is found.
    /// </summary>
    /// <param name="predicate">
    /// A function to test an entity for a condition.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains
    /// <see langword="default"/>(<typeparamref name="TEntity"/>)
    /// if the repository is empty or if no entity passes the test
    /// specified by <paramref name="predicate"/>.
    /// Otherwise, the last entity in the repository that passes the test
    /// specified by <paramref name="predicate"/>.
    /// </returns>
    Task<TEntity?> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the only entity of the repository.
    /// <para>
    /// The implementations of this method should throw
    /// an exception if there is not exactly one entity in the repository.
    /// </para>
    /// </summary>
    /// <returns>
    /// The single entity of the repository.
    /// </returns>
    TEntity Single();

    /// <summary>
    /// Returns the only entity of the repository
    /// that satisfies a specified condition.
    /// <para>
    /// The implementations of this method should throw
    /// an exception if more than one such entity exists.
    /// </para>
    /// </summary>
    /// <param name="predicate">
    /// A function to test an entity for a condition.
    /// </param>
    /// <returns>
    /// The single entity
    /// of the repository that satisfies the condition
    /// in <paramref name="predicate"/>.
    /// </returns>
    TEntity Single(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Asynchronously returns the only entity of the repository.
    /// <para>
    /// The implementations of this method should throw
    /// an exception if there is not exactly one entity in the repository.
    /// </para>
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the single entity of the repository.
    /// </returns>
    Task<TEntity> SingleAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously returns the only entity of the repository
    /// that satisfies a specified condition.
    /// <para>
    /// The implementations of this method should throw
    /// an exception if more than one such entity exists.
    /// </para>
    /// </summary>
    /// <param name="predicate">
    /// A function to test an entity for a condition.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the single entity
    /// of the repository that satisfies the condition
    /// in <paramref name="predicate"/>.
    /// </returns>
    Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the only entity of the repository,
    /// or a <see langword="default"/> value if the repository is empty.
    /// <para>
    /// The implementations of this method should throw
    /// an exception if there is more than one entity in the repository.
    /// </para>
    /// </summary>
    /// <returns>
    /// The single entity of the repository,
    /// or <see langword="default"/>(<typeparamref name="TEntity"/>)
    /// if the repository contains no entities.
    /// </returns>
    TEntity? SingleOrDefault();

    /// <summary>
    /// Returns the only entity of the repository
    /// that satisfies a specified condition,
    /// or a <see langword="default"/> value if no such entity exists.
    /// <para>
    /// The implementations of this method should throw
    /// an exception if more than one entity satisfies the condition.
    /// </para>
    /// </summary>
    /// <param name="predicate">
    /// A function to test an entity for a condition.
    /// </param>
    /// <returns>
    /// The single entity of the repository
    /// that satisfies the condition in the predicate,
    /// or <see langword="default"/>(<typeparamref name="TEntity"/>)
    /// if no such entity is found.
    /// </returns>
    TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Asynchronously returns the only entity of the repository,
    /// or a <see langword="default"/> value if the repository is empty.
    /// <para>
    /// The implementations of this method should throw
    /// an exception if there is more than one entity in the repository.
    /// </para>
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the single entity of the repository,
    /// or <see langword="default"/>(<typeparamref name="TEntity"/>)
    /// if the repository contains no entities.
    /// </returns>
    Task<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously returns the only entity of the repository
    /// that satisfies a specified condition,
    /// or a <see langword="default"/> value if no such entity exists.
    /// <para>
    /// The implementations of this method should throw
    /// an exception if more than one entity satisfies the condition.
    /// </para>
    /// </summary>
    /// <param name="predicate">
    /// A function to test an entity for a condition.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the single entity of the repository
    /// that satisfies the condition in the predicate,
    /// or <see langword="default"/>(<typeparamref name="TEntity"/>)
    /// if no such entity is found.
    /// </returns>
    Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    #endregion
}