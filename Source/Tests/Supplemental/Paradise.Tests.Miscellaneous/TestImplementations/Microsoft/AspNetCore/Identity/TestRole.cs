using Microsoft.AspNetCore.Identity;

namespace Paradise.Tests.Miscellaneous.TestImplementations.Microsoft.AspNetCore.Identity;

/// <summary>
/// Test <see cref="IdentityRole{TKey}"/> implementation.
/// </summary>
public sealed class TestRole : IdentityRole<Guid>;