using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Paradise.DataAccess.Database.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.DataAccess.Database;

/// <summary>
/// Manages all infrastructure entities in the database.
/// </summary>
internal sealed class InfrastructureContext : DbContext, IDataSource, IDataProtectionKeyContext
{
    #region Constants
    /// <summary>
    /// Infrastructure database connection string name.
    /// </summary>
    public const string ConnectionStringName = "InfrastructureConnectionString";

    /// <summary>
    /// Database scheme name.
    /// </summary>
    public const string SchemeName = "infrastructure";
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="InfrastructureContext"/> class
    /// using the specified options.
    /// </summary>
    /// <param name="options">
    /// The options to be used by an <see cref="InfrastructureContext"/>.
    /// </param>
    public InfrastructureContext([NotNull] DbContextOptions<InfrastructureContext> options) : base(options)
        => DataProtectionKeys = Set<DataProtectionKey>();
    #endregion

    #region Properties
    /// <inheritdoc/>
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void PreparePersistenceStorage()
        => Database.Migrate();

    /// <inheritdoc/>
    public Task PreparePersistenceStorageAsync(CancellationToken cancellationToken = default)
        => Database.MigrateAsync(cancellationToken);

    /// <inheritdoc/>
    public IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class
        => Set<TEntity>();

    void IDataSource.Add<TEntity>(TEntity entity)
        => Set<TEntity>().Add(entity);

    void IDataSource.AddRange<TEntity>(IEnumerable<TEntity> entities)
        => Set<TEntity>().AddRange(entities);

    void IDataSource.Remove<TEntity>(TEntity entity)
        => Set<TEntity>().Remove(entity);

    void IDataSource.RemoveRange<TEntity>(IEnumerable<TEntity> entities)
        => Set<TEntity>().RemoveRange(entities);
    #endregion

    #region Protected methods
    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);
        InfrastructureContextConfiguration.OnModelCreating(modelBuilder);
    }
    #endregion
}