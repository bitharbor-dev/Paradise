using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Paradise.DataAccess.Database.Configuration;
using Paradise.Domain.Identity.Roles;
using Paradise.Domain.Identity.Users;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.DataAccess.Database;

/// <summary>
/// Manages all domain entities in the database.
/// </summary>
internal sealed class DomainContext : IdentityDbContext<User, Role, Guid>, IDataSource
{
    #region Constants
    /// <summary>
    /// Domain database connection string name.
    /// </summary>
    public const string ConnectionStringName = "DomainConnectionString";

    /// <summary>
    /// Database scheme name.
    /// </summary>
    public const string SchemeName = "domain";
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainContext"/> class
    /// using the specified options.
    /// </summary>
    /// <param name="options">
    /// The options to be used by an <see cref="DomainContext"/>.
    /// </param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Omitted for code style consistency.")]
    public DomainContext([NotNull] DbContextOptions<DomainContext> options) : base(options) { }
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
        DomainContextConfiguration.OnModelCreating(builder);
    }
    #endregion
}