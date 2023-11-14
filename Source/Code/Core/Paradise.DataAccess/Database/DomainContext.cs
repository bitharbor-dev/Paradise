using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Paradise.DataAccess.Database.ChangeTracking;
using Paradise.DataAccess.Database.Configuration;
using Paradise.DataAccess.Repositories;
using Paradise.DataAccess.Repositories.Base;
using Paradise.Domain.Roles;
using Paradise.Domain.Users;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.DataAccess.Database;

/// <summary>
/// Manages all domain entities.
/// </summary>
public sealed class DomainContext : IdentityDbContext<User, Role, Guid>, IDomainDataSource
{
    #region Constants
    /// <summary>
    /// Domain database connection string name.
    /// </summary>
    public const string ConnectionStringName = "DomainConnectionString";
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainContext"/> class.
    /// </summary>
    /// <param name="options">
    /// The options to be used by a <see cref="DomainContext"/>.
    /// </param>
    public DomainContext([NotNull] DbContextOptions<DomainContext> options) : base(options)
    {
        ChangeTracker.Tracked += ChangeTrackerEvents.OnTracked;
        ChangeTracker.StateChanged += ChangeTrackerEvents.OnStateChanged;
    }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override void Dispose()
    {
        ChangeTracker.Tracked -= ChangeTrackerEvents.OnTracked;
        ChangeTracker.StateChanged -= ChangeTrackerEvents.OnStateChanged;

        base.Dispose();

        GC.SuppressFinalize(this);
    }

    #region IDataSource
    /// <inheritdoc/>
    public void PreparePersistenceStorage()
        => Database.Migrate();

    /// <inheritdoc/>
    public Task PreparePersistenceStorageAsync(CancellationToken cancellationToken = default)
        => Database.MigrateAsync(cancellationToken);

    IQueryable<TEntity> IDataSource.Set<TEntity>()
        => Set<TEntity>();

    void IDataSource.Add<TEntity>(TEntity entity)
        => Set<TEntity>().Add(entity);

    void IDataSource.AddRange<TEntity>(IEnumerable<TEntity> entities)
        => Set<TEntity>().AddRange(entities);

    void IDataSource.Remove<TEntity>(TEntity entity)
        => Set<TEntity>().Remove(entity);

    void IDataSource.RemoveRange<TEntity>(IEnumerable<TEntity> entities)
        => Set<TEntity>().RemoveRange(entities);

    int IDataSource.SaveChanges()
        => SaveChanges();

    Task<int> IDataSource.SaveChangesAsync(CancellationToken cancellationToken)
        => SaveChangesAsync(cancellationToken);
    #endregion

    #endregion

    #region Protected methods
    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);
        DomainContextConfiguration.OnModelCreating(builder);
    }
    #endregion
}