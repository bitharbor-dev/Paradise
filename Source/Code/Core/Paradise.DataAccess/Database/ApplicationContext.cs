using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Paradise.DataAccess.Database.Configuration;
using Paradise.DataAccess.Repositories;
using Paradise.DataAccess.Repositories.Base;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.DataAccess.Database;

/// <summary>
/// Manages all entities of the application logic.
/// </summary>
public sealed class ApplicationContext : DbContext, IApplicationDataSource, IDataProtectionKeyContext
{
    #region Constants
    /// <summary>
    /// Application database connection string name.
    /// </summary>
    public const string ConnectionStringName = "ApplicationConnectionString";
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationContext"/> class
    /// using the specified options.
    /// </summary>
    /// <param name="options">
    /// The options to be used by an <see cref="ApplicationContext"/>.
    /// </param>
    public ApplicationContext([NotNull] DbContextOptions<ApplicationContext> options) : base(options)
        => DataProtectionKeys = Set<DataProtectionKey>();
    #endregion

    #region Properties
    /// <inheritdoc/>
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    #endregion

    #region Public methods

    #region IDataSource
    /// <inheritdoc/>
    public void PreparePersistenceStorage()
        => Database.Migrate();

    /// <inheritdoc/>
    public Task PreparePersistenceStorageAsync(CancellationToken cancellationToken = default)
        => Database.MigrateAsync(cancellationToken);

    IQueryable<TEntity> IDataSource.GetQueryable<TEntity>()
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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);
        ApplicationContextConfiguration.OnModelCreating(modelBuilder);
    }
    #endregion
}