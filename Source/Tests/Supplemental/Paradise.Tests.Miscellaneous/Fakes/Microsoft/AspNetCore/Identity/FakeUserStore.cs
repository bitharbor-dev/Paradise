using Microsoft.AspNetCore.Identity;
using Paradise.DataAccess.Repositories;
using Paradise.Domain.Users;
using System.Security.Claims;

namespace Paradise.Tests.Miscellaneous.Fakes.Microsoft.AspNetCore.Identity;

/// <summary>
/// Fake <see cref="IUserStore{TUser}"/> of <see cref="User"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeUserStore"/> class.
/// </remarks>
/// <param name="domainDataSource">
/// Domain data source.
/// </param>
public sealed class FakeUserStore(IDomainDataSource domainDataSource)
    : IUserRoleStore<User>, IUserClaimStore<User>, IQueryableUserStore<User>, IUserPasswordStore<User>, IUserEmailStore<User>
{
    #region Fields
    private readonly IDomainDataSource _domainDataSource = domainDataSource;

    private readonly Dictionary<Guid, List<string>> _userRoles = new();
    private readonly Dictionary<Guid, List<Claim>> _userClaims = new();
    #endregion

    #region Properties
    /// <inheritdoc/>
    public IQueryable<User> Users
        => _domainDataSource.Set<User>();
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
    {
        _domainDataSource.Add(user);
        await _domainDataSource.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        user.ConcurrencyStamp = Guid.NewGuid().ToString();
        await _domainDataSource.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
    {
        _domainDataSource.Remove(user);
        await _domainDataSource.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        => Task.FromResult(user.Id.ToString());

    /// <inheritdoc/>
    public Task<string?> GetUserNameAsync(User user, CancellationToken cancellationToken)
        => Task.FromResult((string?)user.UserName);

    /// <inheritdoc/>
    public Task SetUserNameAsync(User user, string? userName, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(userName);

        user.UserName = userName;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<string?> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        => Task.FromResult(user.NormalizedUserName);

    /// <inheritdoc/>
    public Task SetNormalizedUserNameAsync(User user, string? normalizedName, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(normalizedName);

        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        => Task.FromResult(_domainDataSource.Set<User>().FirstOrDefault(u => u.Id.ToString() == userId));

    /// <inheritdoc/>
    public Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        => Task.FromResult(_domainDataSource.Set<User>().FirstOrDefault(u => u.NormalizedUserName == normalizedUserName));

    /// <inheritdoc/>
    public void Dispose() { }

    /// <inheritdoc/>
    public Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
        => Task.FromResult(_userClaims.TryGetValue(user.Id, out var claims) ? claims.ToList() as IList<Claim> : Array.Empty<Claim>());

    /// <inheritdoc/>
    public Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        if (_userClaims.TryGetValue(user.Id, out var storedClaims))
            storedClaims.AddRange(claims);
        else
            _userClaims.Add(user.Id, claims.ToList());

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
    {
        if (_userClaims.TryGetValue(user.Id, out var storedClaims))
        {
            var oldClaim = storedClaims.Find(c => c.Type == claim.Type);

            if (oldClaim is not null)
                storedClaims.Remove(oldClaim);

            storedClaims.Add(newClaim);
        }
        else
        {
            _userClaims.Add(user.Id, new() { newClaim });
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        if (_userClaims.TryGetValue(user.Id, out var storedClaims))
        {
            foreach (var claim in claims)
            {
                var claimToRemove = storedClaims.Find(c => c.Type == claim.Type);

                if (claimToRemove is not null)
                    storedClaims.Remove(claimToRemove);
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
        var set = _domainDataSource.Set<User>();
        var users = new List<User>();

        foreach (var userClaim in _userClaims)
        {
            if (userClaim.Value.Any(c => c.Type == claim.Type))
                users.Add(set.First(u => u.Id == userClaim.Key));
        }

        return Task.FromResult(users as IList<User>);
    }

    /// <inheritdoc/>
    public Task SetPasswordHashAsync(User user, string? passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<string?> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        => Task.FromResult(user.PasswordHash);

    /// <inheritdoc/>
    public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        => Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));

    /// <inheritdoc/>
    public Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        if (_userRoles.TryGetValue(user.Id, out var storedRoles))
            storedRoles.Add(roleName);
        else
            _userRoles.Add(user.Id, new() { roleName });

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
        if (_userRoles.TryGetValue(user.Id, out var storedRoles) && storedRoles.Contains(roleName))
            storedRoles.Remove(roleName);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        => Task.FromResult(_userRoles.TryGetValue(user.Id, out var roles) ? roles as IList<string> : Array.Empty<string>().ToList());

    /// <inheritdoc/>
    public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        => Task.FromResult(_userRoles.ContainsKey(user.Id) && _userRoles[user.Id].Contains(roleName));

    /// <inheritdoc/>
    public Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var set = _domainDataSource.Set<User>();
        var users = new List<User>();

        foreach (var userRole in _userRoles)
        {
            if (userRole.Value.Contains(roleName))
                users.Add(set.First(u => u.Id == userRole.Key));
        }

        return Task.FromResult(users as IList<User>);
    }

    /// <inheritdoc/>
    public Task SetEmailAsync(User user, string? email, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);

        user.Email = email;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<string?> GetEmailAsync(User user, CancellationToken cancellationToken)
        => Task.FromResult((string?)user.Email);

    /// <inheritdoc/>
    public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        => Task.FromResult(user.EmailConfirmed);

    /// <inheritdoc/>
    public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
    {
        user.EmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        => Task.FromResult(_domainDataSource.Set<User>().FirstOrDefault(u => u.NormalizedEmail == normalizedEmail));

    /// <inheritdoc/>
    public Task<string?> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        => Task.FromResult(user.NormalizedEmail);

    /// <inheritdoc/>
    public Task SetNormalizedEmailAsync(User user, string? normalizedEmail, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(normalizedEmail);

        user.NormalizedEmail = normalizedEmail;
        return Task.CompletedTask;
    }
    #endregion
}