using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
/// <remarks>
/// Initializes a new instance of the <see cref="DomainContext"/> class.
/// </remarks>
/// <param name="options">
/// The options to be used by a <see cref="DomainContext"/>.
/// </param>
public sealed class DomainContext([NotNull] DbContextOptions<DomainContext> options) : IdentityDbContext<User, Role, Guid>(options), IDomainDataSource
{
    #region Constants
    /// <summary>
    /// Domain database connection string name.
    /// </summary>
    public const string ConnectionStringName = "DomainConnectionString";

    /// <summary>
    /// The name of a scheme tied to this database context.
    /// </summary>
    public const string SchemeName = "domain";
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
    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);
        DomainContextConfiguration.OnModelCreating(builder);
    }
    #endregion
}