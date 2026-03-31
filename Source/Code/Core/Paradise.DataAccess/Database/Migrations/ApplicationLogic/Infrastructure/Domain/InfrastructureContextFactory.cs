using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Paradise.DataAccess.Database.Migrations.ApplicationLogic.Infrastructure.Domain;

/// <summary>
/// A design-time <see cref="InfrastructureContext"/> factory to simplify migrations creation.
/// </summary>
internal sealed class InfrastructureContextFactory : IDesignTimeDbContextFactory<InfrastructureContext>
{
    #region Public methods
    /// <inheritdoc/>
    public InfrastructureContext CreateDbContext(string[] args)
        => new(new DbContextOptionsBuilder<InfrastructureContext>().UseSqlServer().Options);
    #endregion
}