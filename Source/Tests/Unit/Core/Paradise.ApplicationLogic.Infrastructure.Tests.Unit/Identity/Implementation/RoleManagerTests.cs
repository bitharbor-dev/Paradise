using Paradise.ApplicationLogic.Infrastructure.Identity.Implementation;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.AspNetCore.Identity;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.Extensions.Logging;
using Paradise.Tests.Miscellaneous.TestImplementations.Microsoft.AspNetCore.Identity;
using CoreIdentity = Microsoft.AspNetCore.Identity;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.Identity.Implementation;

/// <summary>
/// <see cref="RoleManager{TRole}"/> test class.
/// </summary>
public sealed class RoleManagerTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="RoleManager{TRole}"/> constructor should
    /// successfully initialize a new instance of the class.
    /// </summary>
    [Fact]
    public void Constructor()
    {
        // Arrange
        var errors = new CoreIdentity.IdentityErrorDescriber();

        using var context = new FakeIdentityDbContext();
        using var store = new FakeRoleStore(context, errors);
        var roleValidators = new List<CoreIdentity.RoleValidator<TestRole>>();
        var keyNormalizer = new CoreIdentity.UpperInvariantLookupNormalizer();
        var logger = new FakeLogger<RoleManager<TestRole>>();

        // Act
        var exception = Record.Exception(()
            => new RoleManager<TestRole>(store, roleValidators, keyNormalizer, errors, logger));

        // Assert
        Assert.Null(exception);
    }
    #endregion
}