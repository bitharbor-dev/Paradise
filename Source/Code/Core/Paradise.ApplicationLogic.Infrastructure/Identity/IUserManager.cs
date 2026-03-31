using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Paradise.ApplicationLogic.Infrastructure.Identity;

/// <summary>
/// The <see cref="UserManager{TUser}"/> abstraction.
/// </summary>
public interface IUserManager<TUser> where TUser : class
{
    #region Properties
    /// <inheritdoc cref="UserManager{TUser}.Users"/>
    IQueryable<TUser> Users { get; }

    /// <inheritdoc cref="UserManager{TUser}.Options"/>
    IdentityOptions Options { get; }
    #endregion

    #region Methods
    /// <inheritdoc cref="UserManager{TUser}.FindByIdAsync(string)"/>
    Task<TUser?> FindByIdAsync(Guid userId);

    /// <inheritdoc cref="UserManager{TUser}.FindByNameAsync(string)"/>
    Task<TUser?> FindByNameAsync(string userName);

    /// <inheritdoc cref="UserManager{TUser}.FindByEmailAsync(string)"/>
    Task<TUser?> FindByEmailAsync(string email);

    /// <summary>
    /// Finds and returns a user, if any, who has the specified phone number.
    /// </summary>
    /// <param name="phoneNumber">
    /// The phone number to search for.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation,
    /// containing the user matching the specified <paramref name="phoneNumber"/> if it exists.
    /// </returns>
    Task<TUser?> FindByPhoneNumberAsync(string? phoneNumber);

    /// <inheritdoc cref="UserManager{TUser}.CreateAsync(TUser, string)"/>
    Task<IdentityResult> CreateAsync(TUser user, string password);

    /// <inheritdoc cref="UserManager{TUser}.UpdateAsync(TUser)"/>
    Task<IdentityResult> UpdateAsync(TUser user);

    /// <inheritdoc cref="UserManager{TUser}.DeleteAsync(TUser)"/>
    Task<IdentityResult> DeleteAsync(TUser user);

    /// <inheritdoc cref="UserManager{TUser}.GetClaimsAsync(TUser)"/>
    Task<IList<Claim>> GetClaimsAsync(TUser user);

    /// <inheritdoc cref="UserManager{TUser}.GetRolesAsync(TUser)"/>
    Task<IList<string>> GetRolesAsync(TUser user);

    /// <inheritdoc cref="UserManager{TUser}.AddToRoleAsync(TUser, string)"/>
    Task<IdentityResult> AddToRoleAsync(TUser user, string role);

    /// <inheritdoc cref="UserManager{TUser}.AddToRolesAsync(TUser, IEnumerable{string})"/>
    Task<IdentityResult> AddToRolesAsync(TUser user, IEnumerable<string> roles);

    /// <inheritdoc cref="UserManager{TUser}.RemoveFromRoleAsync(TUser, string)"/>
    Task<IdentityResult> RemoveFromRoleAsync(TUser user, string role);

    /// <inheritdoc cref="UserManager{TUser}.IsLockedOutAsync(TUser)"/>
    Task<bool> IsLockedOutAsync(TUser user);

    /// <inheritdoc cref="UserManager{TUser}.CheckPasswordAsync(TUser, string)"/>
    Task<bool> CheckPasswordAsync(TUser user, string password);

    /// <inheritdoc cref="UserManager{TUser}.AccessFailedAsync(TUser)"/>
    Task<IdentityResult> AccessFailedAsync(TUser user);

    /// <inheritdoc cref="UserManager{TUser}.NormalizeEmail(string?)"/>
    [return: NotNullIfNotNull(nameof(email))]
    string? NormalizeEmail(string? email);

    /// <inheritdoc cref="UserManager{TUser}.NormalizeName(string?)"/>
    [return: NotNullIfNotNull(nameof(name))]
    string? NormalizeName(string? name);

    /// <inheritdoc cref="UserManager{TUser}.GenerateEmailConfirmationTokenAsync(TUser)"/>
    Task<string> GenerateEmailConfirmationTokenAsync(TUser user);

    /// <inheritdoc cref="UserManager{TUser}.ConfirmEmailAsync(TUser, string)"/>
    Task<IdentityResult> ConfirmEmailAsync(TUser user, string token);

    /// <inheritdoc cref="UserManager{TUser}.GenerateChangeEmailTokenAsync(TUser, string)"/>
    Task<string> GenerateChangeEmailTokenAsync(TUser user, string newEmail);

    /// <inheritdoc cref="UserManager{TUser}.ChangeEmailAsync(TUser, string, string)"/>
    Task<IdentityResult> ChangeEmailAsync(TUser user, string newEmail, string token);

    /// <inheritdoc cref="UserManager{TUser}.GeneratePasswordResetTokenAsync(TUser)"/>
    Task<string> GeneratePasswordResetTokenAsync(TUser user);

    /// <inheritdoc cref="UserManager{TUser}.ResetPasswordAsync(TUser, string, string)"/>
    Task<IdentityResult> ResetPasswordAsync(TUser user, string token, string newPassword);

    /// <inheritdoc cref="UserManager{TUser}.ValidatePasswordAsync(TUser, string)"/>
    Task<IdentityResult> ValidatePasswordAsync(TUser user, string? password);
    #endregion
}