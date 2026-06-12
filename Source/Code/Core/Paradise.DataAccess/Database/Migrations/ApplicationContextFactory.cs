using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.DataAccess.Database.Migrations;

/// <summary>
/// A design-time <see cref="ApplicationContext"/> factory to simplify migrations creation.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    #region Public methods
    /// <inheritdoc/>
    public ApplicationContext CreateDbContext(string[] args)
        => new(new DbContextOptionsBuilder<ApplicationContext>().UseSqlServer().Options);
    #endregion
}