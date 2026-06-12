using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Paradise.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.DataAccess.Database.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.DataAccess.Database;

/// <summary>
/// Application-wise <see cref="DbContext"/> implementation.
/// </summary>
internal sealed class ApplicationContext : IdentityDbContext<User, Role, Guid>, IDataSource, IDataProtectionKeyContext
{
    #region Constants
    /// <summary>
    /// Database connection string name.
    /// </summary>
    public const string ConnectionStringName = "DatabaseConnectionString";

    /// <summary>
    /// Domain scheme name.
    /// </summary>
    public const string DomainSchemeName = "domain";

    /// <summary>
    /// Infrastructure scheme name.
    /// </summary>
    public const string InfrastructureSchemeName = "infrastructure";
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
    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);
        ApplicationContextConfiguration.OnModelCreating(builder);
    }
    #endregion
}