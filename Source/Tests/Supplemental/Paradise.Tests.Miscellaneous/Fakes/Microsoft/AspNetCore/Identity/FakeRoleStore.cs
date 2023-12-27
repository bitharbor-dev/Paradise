using Microsoft.AspNetCore.Identity;
using Paradise.DataAccess.Repositories;
using Paradise.Domain.Roles;

namespace Paradise.Tests.Miscellaneous.Fakes.Microsoft.AspNetCore.Identity;

/// <summary>
/// Fake <see cref="IRoleStore{TRole}"/> of <see cref="Role"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeRoleStore"/> class.
/// </remarks>
/// <param name="domainDataSource">
/// Domain data source.
/// </param>
public sealed class FakeRoleStore(IDomainDataSource domainDataSource) : IRoleStore<Role>, IQueryableRoleStore<Role>
{
    #region Fields
    private readonly IDomainDataSource _domainDataSource = domainDataSource;
    #endregion

    #region Properties
    /// <inheritdoc/>
    public IQueryable<Role> Roles
        => _domainDataSource.GetQueryable<Role>();
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
    {
        _domainDataSource.Add(role);

        await _domainDataSource
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(role);

        role.ConcurrencyStamp = Guid.NewGuid().ToString();

        await _domainDataSource
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
    {
        _domainDataSource.Remove(role);

        await _domainDataSource
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(role);

        return Task.FromResult(role.Id.ToString());
    }

    /// <inheritdoc/>
    public Task<string?> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(role);

        return Task.FromResult((string?)role.Name);
    }

    /// <inheritdoc/>
    public Task SetRoleNameAsync(Role role, string? roleName, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(role);

        role.Name = roleName;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<string?> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(role);

        return Task.FromResult(role.NormalizedName);
    }

    /// <inheritdoc/>
    public Task SetNormalizedRoleNameAsync(Role role, string? normalizedName, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(role);

        role.NormalizedName = normalizedName;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<Role?> FindByIdAsync(string? roleId, CancellationToken cancellationToken)
        => Task.FromResult(_domainDataSource.GetQueryable<Role>().FirstOrDefault(r => r.Id.ToString() == roleId));

    /// <inheritdoc/>
    public Task<Role?> FindByNameAsync(string? normalizedRoleName, CancellationToken cancellationToken)
        => Task.FromResult(_domainDataSource.GetQueryable<Role>().FirstOrDefault(r => r.NormalizedName == normalizedRoleName));

    /// <inheritdoc/>
    public void Dispose() { }
    #endregion
}