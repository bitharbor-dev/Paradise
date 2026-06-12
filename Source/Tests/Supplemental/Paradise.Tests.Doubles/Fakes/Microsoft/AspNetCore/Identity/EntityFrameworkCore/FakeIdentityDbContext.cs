using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Paradise.Tests.Fixtures.Microsoft.AspNetCore.Identity;

namespace Paradise.Tests.Doubles.Fakes.Microsoft.AspNetCore.Identity.EntityFrameworkCore;

/// <summary>
/// Fake <see cref="IdentityDbContext{TUser, TRole, TKey}"/> implementation.
/// </summary>
public sealed class FakeIdentityDbContext : IdentityDbContext<TestUser, TestRole, Guid>
{
    #region Public methods
    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());

        base.OnConfiguring(optionsBuilder);
    }
    #endregion
}