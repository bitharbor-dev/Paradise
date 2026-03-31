using Microsoft.AspNetCore.Identity;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.DataAccess;
using Paradise.Domain.Identity.Roles;
using System.Diagnostics.CodeAnalysis;
using static Xunit.TestContext;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.ApplicationLogic.Infrastructure.Identity;

/// <summary>
/// Fake <see cref="IRoleManager{TRole}"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeRoleManager"/> class.
/// </remarks>
/// <param name="source">
/// An <see cref="IDataSource"/> instance used to
/// arrange data and validate test results.
/// </param>
public sealed class FakeRoleManager(IDataSource source) : IRoleManager<Role>
{
    #region Properties
    /// <inheritdoc/>
    public IQueryable<Role> Roles
        => source.GetQueryable<Role>();

    /// <summary>
    /// <see cref="CreateAsync"/> method.
    /// </summary>
    public Func<Task<IdentityResult>>? CreateAsyncResult { get; set; }

    /// <summary>
    /// <see cref="UpdateAsync"/> method.
    /// </summary>
    public Func<Task<IdentityResult>>? UpdateAsyncResult { get; set; }

    /// <summary>
    /// <see cref="DeleteAsync"/> method.
    /// </summary>
    public Func<Task<IdentityResult>>? DeleteAsyncResult { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async Task<IdentityResult> CreateAsync(Role role)
    {
        if (CreateAsyncResult is not null)
        {
            return await CreateAsyncResult()
                .ConfigureAwait(false);
        }

        ArgumentNullException.ThrowIfNull(role);

        source.Add(role);

        UpdateNormalizedProperties(role);

        await source.SaveChangesAsync(Current.CancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> UpdateAsync(Role role)
    {
        if (UpdateAsyncResult is not null)
        {
            return await UpdateAsyncResult()
                .ConfigureAwait(false);
        }

        ArgumentNullException.ThrowIfNull(role);

        UpdateNormalizedProperties(role);

        await source.SaveChangesAsync(Current.CancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> DeleteAsync(Role role)
    {
        if (DeleteAsyncResult is not null)
        {
            return await DeleteAsyncResult()
                .ConfigureAwait(false);
        }

        ArgumentNullException.ThrowIfNull(role);

        source.Remove(role);

        await source.SaveChangesAsync(Current.CancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(key))]
    public string? NormalizeKey(string? key)
        => key;
    #endregion

    #region Private methods
    /// <summary>
    /// Sets the normalized properties values of the given <paramref name="role"/>
    /// to the same values as non-normalized ones.
    /// </summary>
    /// <param name="role">
    /// The <see cref="Role"/> to update.
    /// </param>
    private static void UpdateNormalizedProperties(Role role)
        => role.NormalizedName = role.Name;
    #endregion
}