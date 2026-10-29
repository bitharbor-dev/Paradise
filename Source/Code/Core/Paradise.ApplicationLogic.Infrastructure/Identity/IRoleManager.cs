using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.ApplicationLogic.Infrastructure.Identity;

/// <summary>
/// The <see cref="RoleManager{TRole}"/> abstraction.
/// </summary>
public interface IRoleManager<TRole>
{
    #region Properties
    /// <inheritdoc cref="RoleManager{TRole}.Roles"/>
    IQueryable<TRole> Roles { get; }
    #endregion

    #region Methods
    /// <inheritdoc cref="RoleManager{TRole}.CreateAsync"/>
    Task<IdentityResult> CreateAsync(TRole role);

    /// <inheritdoc cref="RoleManager{TRole}.UpdateAsync"/>
    Task<IdentityResult> UpdateAsync(TRole role);

    /// <inheritdoc cref="RoleManager{TRole}.DeleteAsync"/>
    Task<IdentityResult> DeleteAsync(TRole role);

    /// <inheritdoc cref="RoleManager{TRole}.NormalizeKey"/>
    [return: NotNullIfNotNull(nameof(key))]
    string? NormalizeKey(string? key);
    #endregion
}