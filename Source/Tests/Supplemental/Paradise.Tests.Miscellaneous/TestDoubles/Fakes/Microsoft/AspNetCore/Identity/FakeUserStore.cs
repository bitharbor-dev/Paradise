using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Paradise.Tests.Miscellaneous.TestImplementations.Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.AspNetCore.Identity;

/// <summary>
/// Fake <see cref="UserStore{TUser, TRole, TContext, TKey}"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeUserStore"/> class.
/// </remarks>
/// <param name="context">
/// The <see cref="DbContext"/>.
/// </param>
/// <param name="describer">
/// The <see cref="IdentityErrorDescriber"/>.
/// </param>
public sealed class FakeUserStore(DbContext context, IdentityErrorDescriber? describer = null) : UserStore<TestUser, TestRole, DbContext, Guid>(context, describer)
{
    #region Properties
    /// <summary>
    /// <see cref="CreateAsync"/> result.
    /// </summary>
    public Func<Task<IdentityResult>>? CreateAsyncResult { get; set; }

    /// <summary>
    /// <see cref="UpdateAsync"/> result.
    /// </summary>
    public Func<Task<IdentityResult>>? UpdateAsyncResult { get; set; }

    /// <summary>
    /// <see cref="AddClaimsAsync"/> result.
    /// </summary>
    public Func<Task>? AddClaimsAsyncResult { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override Task<IdentityResult> CreateAsync(TestUser user, CancellationToken cancellationToken = default)
        => CreateAsyncResult is not null
        ? CreateAsyncResult()
        : base.CreateAsync(user, cancellationToken);

    /// <inheritdoc/>
    public override Task<IdentityResult> UpdateAsync(TestUser user, CancellationToken cancellationToken = default)
        => UpdateAsyncResult is not null
        ? UpdateAsyncResult()
        : base.UpdateAsync(user, cancellationToken);

    /// <inheritdoc/>
    public override Task AddClaimsAsync(TestUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        => AddClaimsAsyncResult is not null
        ? AddClaimsAsyncResult()
        : base.AddClaimsAsync(user, claims, cancellationToken);
    #endregion
}