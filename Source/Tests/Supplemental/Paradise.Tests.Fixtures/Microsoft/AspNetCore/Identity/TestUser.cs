using Microsoft.AspNetCore.Identity;

namespace Paradise.Tests.Fixtures.Microsoft.AspNetCore.Identity;

/// <summary>
/// Test <see cref="IdentityUser{TKey}"/> implementation.
/// </summary>
public sealed class TestUser : IdentityUser<Guid>;