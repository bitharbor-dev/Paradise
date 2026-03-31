using Paradise.DataAccess;
using Paradise.Domain.Base;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.System.Collections.Generic;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess;

/// <summary>
/// Fake <see cref="IDataSource"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeDataSource"/> class.
/// </remarks>
/// <param name="timeProvider">
/// Time provider.
/// </param>
public sealed class FakeDataSource(TimeProvider timeProvider) : IDataSource
{
    #region Fields
    private readonly Dictionary<Type, List<Entry>> _store = [];
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void PreparePersistenceStorage()
        => PersistenceStoragePrepared?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc/>
    public Task PreparePersistenceStorageAsync(CancellationToken cancellationToken = default)
    {
        PersistenceStoragePreparedAsync?.Invoke(this, EventArgs.Empty);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class
    {
        var set = GetOrCreateSet(typeof(TEntity));
        var entities = set
            .Where(entry => entry.State is not EntryState.Removed)
            .Select(entry => (TEntity)entry.Entity);

        return new FakeAsyncEnumerable<TEntity>(entities);
    }

    /// <inheritdoc/>
    public void Add<TEntity>(TEntity entity) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entity);

        var set = GetOrCreateSet(typeof(TEntity));

        set.Add(new(entity, EntryState.Added));
    }

    /// <inheritdoc/>
    public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entities);

        foreach (var entity in entities)
            Add(entity);
    }

    /// <inheritdoc/>
    public void Remove<TEntity>(TEntity entity) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entity);

        var set = GetOrCreateSet(typeof(TEntity));
        var entry = set.FirstOrDefault(e => ReferenceEquals(e.Entity, entity));

        if (entry is null)
            return;

        if (entry.State == EntryState.Added)
            set.Remove(entry);
        else
            entry.State = EntryState.Removed;
    }

    /// <inheritdoc/>
    public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entities);

        foreach (var entity in entities)
            Remove(entity);
    }

    /// <inheritdoc/>
    public int SaveChanges()
    {
        var result = SaveChangesInternal();

        ChangesSaved?.Invoke(this, EventArgs.Empty);

        return result;
    }

    /// <inheritdoc/>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = SaveChangesInternal();

        ChangesSavedAsync?.Invoke(this, EventArgs.Empty);

        return Task.FromResult(result);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Persists all changes in the in-memory store and updates entity states accordingly.
    /// </summary>
    /// <returns>
    /// The total number of entries affected (added, modified, or removed) by this operation.
    /// </returns>
    private int SaveChangesInternal()
    {
        var count = 0;

        foreach (var set in _store.Values)
        {
            foreach (var entry in set.ToList())
            {
                switch (entry.State)
                {
                    case EntryState.Added:
                        {
                            PersistAdded(set, entry);
                            count++;
                            break;
                        }
                    case EntryState.Modified:
                        {
                            PersistModified(entry);
                            count++;
                            break;
                        }
                    case EntryState.Removed:
                        {
                            if (set.Remove(entry))
                                count++;

                            break;
                        }
                    case EntryState.Persisted:
                    default:
                        break;
                }
            }
        }

        return count;
    }

    /// <summary>
    /// Gets the backing entry set for the specified entity type,
    /// creating a new one if it does not already exist.
    /// </summary>
    /// <param name="entityType">
    /// The CLR type of the entity whose set is requested.
    /// </param>
    /// <returns>
    /// The mutable list of <see cref="Entry"/> instances associated
    /// with the given <paramref name="entityType"/>.
    /// </returns>
    private List<Entry> GetOrCreateSet(Type entityType)
    {
        if (!_store.TryGetValue(entityType, out var set))
        {
            set = [];
            _store[entityType] = set;
        }

        return set;
    }

    /// <summary>
    /// Persists an entity that is currently in the <see cref="EntryState.Added"/> state.
    /// </summary>
    /// <param name="set">
    /// The backing set that owns the specified <paramref name="entry"/>.
    /// </param>
    /// <param name="entry">
    /// The entry to persist.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when another entry in the same set references the same entity instance,
    /// indicating an invalid or inconsistent state.
    /// </exception>
    /// <remarks>
    /// This method transitions the entry to the <see cref="EntryState.Persisted"/> state
    /// and, if the entity implements <see cref="IDomainObject"/>, assigns identity and
    /// creation metadata using the current <see cref="TimeProvider"/>.
    /// </remarks>
    private void PersistAdded(List<Entry> set, Entry entry)
    {
        if (set.Except([entry]).Any(e => ReferenceEquals(e.Entity, entry.Entity)))
            throw new ArgumentException(string.Empty, nameof(entry));

        entry.State = EntryState.Persisted;

        if (entry.Entity is IDomainObject entity)
            entity.OnCreated(timeProvider.GetUtcNow());
    }

    /// <summary>
    /// Persists an entity that is currently in the <see cref="EntryState.Modified"/> state.
    /// </summary>
    /// <param name="entry">
    /// The entry to persist.
    /// </param>
    /// <remarks>
    /// This method transitions the entry to the <see cref="EntryState.Persisted"/> state
    /// and, if the entity implements <see cref="IDomainObject"/>, updates
    /// modification metadata using the current <see cref="TimeProvider"/>.
    /// </remarks>
    private void PersistModified(Entry entry)
    {
        entry.State = EntryState.Persisted;

        if (entry.Entity is IDomainObject entity)
            entity.OnModified(timeProvider.GetUtcNow());
    }
    #endregion

    #region Events
    /// <summary>
    /// Occurs when persistence storage is prepared.
    /// </summary>
    public event EventHandler? PersistenceStoragePrepared;

    /// <summary>
    /// Occurs when persistence storage is asynchronously prepared.
    /// </summary>
    public event EventHandler? PersistenceStoragePreparedAsync;

    /// <summary>
    /// Occurs when changes are saved.
    /// </summary>
    public event EventHandler? ChangesSaved;

    /// <summary>
    /// Occurs when changes are asynchronously saved.
    /// </summary>
    public event EventHandler? ChangesSavedAsync;
    #endregion

    #region Nested types
    /// <summary>
    /// Represents an entity with state tracking.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="Entry"/> class.
    /// </remarks>
    /// <param name="entity">
    /// The entity being tracked.
    /// </param>
    /// <param name="state">
    /// Entity tracking state.
    /// </param>
    private sealed class Entry(object entity, EntryState state)
    {
        #region Properties
        /// <summary>
        /// The entity being tracked.
        /// </summary>
        public object Entity { get; } = entity;

        /// <summary>
        /// Entity tracking state.
        /// </summary>
        public EntryState State { get; set; } = state;
        #endregion
    }

    /// <summary>
    /// States for entity tracking.
    /// </summary>
    private enum EntryState
    {
        /// <summary>
        /// Added entity.
        /// </summary>
        Added,
        /// <summary>
        /// Modified entity.
        /// </summary>
        Modified,
        /// <summary>
        /// Removed entity.
        /// </summary>
        Removed,
        /// <summary>
        /// Persisted entity.
        /// </summary>
        Persisted
    }
    #endregion
}