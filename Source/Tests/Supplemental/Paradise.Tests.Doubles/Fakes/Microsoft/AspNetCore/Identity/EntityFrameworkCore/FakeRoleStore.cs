using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Paradise.Tests.Surrogates.Microsoft.AspNetCore.Identity;

namespace Paradise.Tests.Doubles.Fakes.Microsoft.AspNetCore.Identity.EntityFrameworkCore;

/// <summary>
/// Fake <see cref="RoleStore{TRole, TContext, TKey}"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeRoleStore"/> class.
/// </remarks>
/// <param name="context">
/// The <see cref="DbContext"/>.
/// </param>
/// <param name="describer">
/// The <see cref="IdentityErrorDescriber"/>.
/// </param>
public sealed class FakeRoleStore(DbContext context, IdentityErrorDescriber? describer = null)
    : RoleStore<TestRole, DbContext, Guid>(context, describer);