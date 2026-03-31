using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Paradise.Tests.Miscellaneous.TestImplementations.Microsoft.AspNetCore.Identity;
using Xunit;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.AspNetCore.Identity.EntityFrameworkCore;

/// <summary>
/// Fake <see cref="IdentityDbContext{TUser, TRole, TKey}"/> implementation.
/// </summary>
public sealed class FakeIdentityDbContext : IdentityDbContext<TestUser, TestRole, Guid>
{
    #region Public methods
    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(TestContext.Current.Test!.UniqueID);

        base.OnConfiguring(optionsBuilder);
    }
    #endregion
}