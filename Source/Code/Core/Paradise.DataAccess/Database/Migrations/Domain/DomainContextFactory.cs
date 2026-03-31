using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Paradise.DataAccess.Database.Migrations.Domain;

/// <summary>
/// A design-time <see cref="DomainContext"/> factory to simplify migrations creation.
/// </summary>
internal sealed class DomainContextFactory : IDesignTimeDbContextFactory<DomainContext>
{
    #region Public methods
    /// <inheritdoc/>
    public DomainContext CreateDbContext(string[] args)
        => new(new DbContextOptionsBuilder<DomainContext>().UseSqlServer().Options);
    #endregion
}