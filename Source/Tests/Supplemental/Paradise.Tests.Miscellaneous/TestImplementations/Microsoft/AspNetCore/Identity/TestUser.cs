using Microsoft.AspNetCore.Identity;

namespace Paradise.Tests.Miscellaneous.TestImplementations.Microsoft.AspNetCore.Identity;

/// <summary>
/// Test <see cref="IdentityUser{TKey}"/> implementation.
/// </summary>
public sealed class TestUser : IdentityUser<Guid>;