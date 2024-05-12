using Paradise.DataAccess.Repositories;
using Paradise.DataAccess.Repositories.Base;
using Paradise.Domain.Base;
using Paradise.Tests.Miscellaneous.Fakes.Microsoft.EntityFrameworkCore.Query;

namespace Paradise.Tests.Miscellaneous.Fakes.Core.DataAccess.Repositories.Base;

/// <summary>
/// Fake <see cref="IDataSource"/> implementation.
/// </summary>
public sealed class FakeDataSource : IDataSource, IApplicationDataSource, IDomainDataSource
{
    #region Fields
    private readonly DataSetCache _cache = new();
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void PreparePersistenceStorage() { }

    /// <inheritdoc/>
    public Task PreparePersistenceStorageAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc/>
    public IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class, IDatabaseRecord
        => _cache.GetQueryable<TEntity>();

    /// <inheritdoc/>
    public void Add<TEntity>(TEntity entity) where TEntity : class, IDatabaseRecord
        => _cache.Add(entity);

    /// <inheritdoc/>
    public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IDatabaseRecord
    {
        ArgumentNullException.ThrowIfNull(entities);

        foreach (var entity in entities)
            _cache.Add(entity);
    }

    /// <inheritdoc/>
    public void Remove<TEntity>(TEntity entity) where TEntity : class, IDatabaseRecord
        => _cache.Remove(entity);

    /// <inheritdoc/>
    public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IDatabaseRecord
    {
        ArgumentNullException.ThrowIfNull(entities);

        foreach (var entity in entities)
            _cache.Remove(entity);
    }

    /// <inheritdoc/>
    public int SaveChanges()
    {
        var counter = 0;

        foreach (var set in _cache.GetDataSets())
        {
            var clonedSet = set.ToList();

            foreach (var entry in clonedSet)
            {
                switch (entry.State)
                {
                    case DataSetCache.EntryState.Added:
                        {
                            if (entry.Entity.Id != Guid.Empty)
                                throw new ArgumentException(nameof(entry));

                            if (set.Except([entry]).Any(e => ReferenceEquals(e.Entity, entry.Entity)))
                                throw new ArgumentException(nameof(entry));

                            entry.Entity.ValidateState();
                            entry.Entity.Created = DateTime.UtcNow;
                            entry.Entity.Id = Guid.NewGuid();
                            entry.State = DataSetCache.EntryState.Persisted;

                            counter++;
                            break;
                        }
                    case DataSetCache.EntryState.Modified:
                        {
                            entry.Entity.ValidateState();
                            entry.Entity.Modified = DateTime.UtcNow;
                            entry.State = DataSetCache.EntryState.Persisted;

                            counter++;
                            break;
                        }
                    case DataSetCache.EntryState.Removed:
                        {
                            set.Remove(entry);

                            counter++;
                            break;
                        }
                    case DataSetCache.EntryState.Persisted:
                    default:
                        break;
                }
            }
        }

        return counter;
    }

    /// <inheritdoc/>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(SaveChanges());
    #endregion

    #region Nested types
    /// <summary>
    /// Manages the in-memory cache for all data sets
    /// in the current data <see cref="FakeDataSource"/> instance.
    /// </summary>
    private sealed class DataSetCache
    {
        #region Fields
        private readonly Dictionary<Type, List<Entry<IDatabaseRecord>>> _dataSets = [];
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the list of all data sets in the current <see cref="FakeDataSource"/> instance.
        /// </summary>
        /// <returns>
        /// A <see cref="List{T}"/> of <see cref="Entry{TEntity}"/> of <see cref="IDatabaseRecord"/>.
        /// </returns>
        public List<List<Entry<IDatabaseRecord>>> GetDataSets()
            => [.. _dataSets.Values];

        /// <summary>
        /// Get the queryable set of <typeparamref name="TEntity"/> records.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IQueryable{Task}"/> of <typeparamref name="TEntity"/>.
        /// </returns>
        public FakeAsyncQueryProvider<TEntity> GetQueryable<TEntity>() where TEntity : class, IDatabaseRecord
            => new(GetOrAdd<TEntity>().Select(entry => entry.Entity));

        /// <summary>
        /// Adds the given <paramref name="entity"/>
        /// to the current <see cref="FakeDataSource"/> instance.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type.
        /// </typeparam>
        /// <param name="entity">
        /// The <typeparamref name="TEntity"/> to be added.
        /// </param>
        public void Add<TEntity>(TEntity entity) where TEntity : class, IDatabaseRecord
        {
            if (_dataSets.TryGetValue(typeof(TEntity), out var set))
            {
                set.Add(entity);
            }
            else
            {
                set = [entity];

                _dataSets.Add(typeof(TEntity), set);
            }
        }

        /// <summary>
        /// Removes the given <paramref name="entity"/>
        /// from the current <see cref="FakeDataSource"/> instance.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type.
        /// </typeparam>
        /// <param name="entity">
        /// The <typeparamref name="TEntity"/> to be removed.
        /// </param>
        public void Remove<TEntity>(TEntity entity) where TEntity : class, IDatabaseRecord
        {
            if (_dataSets.TryGetValue(typeof(TEntity), out var set))
            {
                var record = set.Find(entry => entry.Entity.Id == entity.Id);
                if (record is not null)
                {
                    if (record.State is EntryState.Added)
                        set.Remove(record);
                    else
                        record.State = EntryState.Removed;
                }
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Gets the <see cref="List{T}"/> of <see cref="Entry{TEntity}"/> of <typeparamref name="TEntity"/>
        /// or adds a new one to the cache if not exists and then returns it.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type.
        /// </typeparam>
        /// <returns>
        /// The <see cref="List{T}"/> of <see cref="Entry{TEntity}"/> of <typeparamref name="TEntity"/>.
        /// </returns>
        private List<Entry<TEntity>> GetOrAdd<TEntity>() where TEntity : class, IDatabaseRecord
        {
            if (_dataSets.TryGetValue(typeof(TEntity), out var set))
            {
                return set.Select(entry => new Entry<TEntity>((TEntity)entry.Entity, entry.State)).ToList();
            }
            else
            {
                set = [];

                _dataSets.Add(typeof(TEntity), set);

                return set.Select(entry => new Entry<TEntity>((TEntity)entry.Entity, entry.State)).ToList();
            }
        }
        #endregion

        #region Nested types
        /// <summary>
        /// An <typeparamref name="TEntity"/> wrapper
        /// to enable the entity state managing mechanism.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type.
        /// </typeparam>
        /// <remarks>
        /// Initializes a new instance of the <see cref="Entry{TEntity}"/> class.
        /// </remarks>
        /// <param name="entity">
        /// Entity.
        /// </param>
        /// <param name="state">
        /// Entity state.
        /// </param>
        public sealed class Entry<TEntity>(TEntity entity, EntryState state) where TEntity : class, IDatabaseRecord
        {
            #region Properties
            /// <summary>
            /// Entity.
            /// </summary>
            public TEntity Entity { get; } = entity;

            /// <summary>
            /// Entity state.
            /// </summary>
            public EntryState State { get; set; } = state;
            #endregion

            #region Operators
            /// <summary>
            /// Implicitly converts the given <paramref name="entity"/>
            /// into the new <see cref="Entry{TEntity}"/> instance.
            /// </summary>
            /// <param name="entity">
            /// The <typeparamref name="TEntity"/> to be converted.
            /// </param>
            public static implicit operator Entry<TEntity>(TEntity entity)
                => new(entity, EntryState.Added);
            #endregion
        }

        /// <summary>
        /// Defines all possible entity states.
        /// </summary>
        public enum EntryState
        {
            /// <summary>
            /// Entity added.
            /// </summary>
            Added,
            /// <summary>
            /// Entity modified.
            /// </summary>
            Modified,
            /// <summary>
            /// Entity removed.
            /// </summary>
            Removed,
            /// <summary>
            /// Entity saved.
            /// </summary>
            Persisted
        }
        #endregion
    }
    #endregion
}