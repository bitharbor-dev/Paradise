using Microsoft.AspNetCore.Identity;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.DataAccess;
using Paradise.Domain.Identity.Roles;
using Paradise.Domain.Identity.Users;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using static Xunit.TestContext;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.ApplicationLogic.Infrastructure.Identity;

/// <summary>
/// Fake <see cref="IUserManager{TUser}"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeUserManager"/> class.
/// </remarks>
/// <param name="timeProvider">
/// Time provider.
/// </param>
/// <param name="source">
/// An <see cref="IDataSource"/> instance used to
/// arrange data and validate test results.
/// </param>
/// <param name="options">
/// An <see cref="IdentityOptions"/> instance used to
/// configure the fake identity.
/// </param>
public sealed class FakeUserManager(TimeProvider timeProvider, IDataSource source, IdentityOptions options) : IUserManager<User>
{
    #region Properties
    /// <inheritdoc/>
    public IQueryable<User> Users
        => source.GetQueryable<User>();

    /// <inheritdoc/>
    public IdentityOptions Options
        => options;

    /// <summary>
    /// <see cref="CreateAsync"/> result.
    /// </summary>
    public Func<Task<IdentityResult>>? CreateAsyncResult { get; set; }

    /// <summary>
    /// <see cref="UpdateAsync"/> result.
    /// </summary>
    public Func<Task<IdentityResult>>? UpdateAsyncResult { get; set; }

    /// <summary>
    /// <see cref="DeleteAsync"/> result.
    /// </summary>
    public Func<Task<IdentityResult>>? DeleteAsyncResult { get; set; }

    /// <summary>
    /// <see cref="GetClaimsAsync"/> result.
    /// </summary>
    public Func<Task<IList<Claim>>>? GetClaimsAsyncResult { get; set; }

    /// <summary>
    /// <see cref="AddToRoleAsync"/> result.
    /// </summary>
    public Func<Task<IdentityResult>>? AddToRoleAsyncResult { get; set; }

    /// <summary>
    /// <see cref="AddToRolesAsync"/> result.
    /// </summary>
    public Func<Task<IdentityResult>>? AddToRolesAsyncResult { get; set; }

    /// <summary>
    /// <see cref="RemoveFromRoleAsync"/> result.
    /// </summary>
    public Func<Task<IdentityResult>>? RemoveFromRoleAsyncResult { get; set; }

    /// <summary>
    /// <see cref="AccessFailedAsync"/> result.
    /// </summary>
    public Func<Task<IdentityResult>>? AccessFailedAsyncResult { get; set; }

    /// <summary>
    /// <see cref="ConfirmEmailAsync"/> result.
    /// </summary>
    public Func<Task<IdentityResult>>? ConfirmEmailAsyncResult { get; set; }

    /// <summary>
    /// <see cref="ChangeEmailAsync"/> result.
    /// </summary>
    public Func<Task<IdentityResult>>? ChangeEmailAsyncResult { get; set; }

    /// <summary>
    /// <see cref="ResetPasswordAsync"/> result.
    /// </summary>
    public Func<Task<IdentityResult>>? ResetPasswordAsyncResult { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task<User?> FindByIdAsync(Guid userId)
    {
        var user = source
            .GetQueryable<User>()
            .FirstOrDefault(user => user.Id == userId);

        return Task.FromResult(user);
    }

    /// <inheritdoc/>
    public Task<User?> FindByNameAsync(string userName)
    {
        var user = source
            .GetQueryable<User>()
            .FirstOrDefault(user => user.UserName == userName);

        return Task.FromResult(user);
    }

    /// <inheritdoc/>
    public Task<User?> FindByEmailAsync(string email)
    {
        var user = source
            .GetQueryable<User>()
            .FirstOrDefault(user => user.Email == email);

        return Task.FromResult(user);
    }

    /// <inheritdoc/>
    public Task<User?> FindByPhoneNumberAsync(string? phoneNumber)
    {
        var user = source
            .GetQueryable<User>()
            .FirstOrDefault(user => user.PhoneNumber == phoneNumber);

        return Task.FromResult(user);
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> CreateAsync(User user, string password)
    {
        if (CreateAsyncResult is not null)
        {
            return await CreateAsyncResult()
                .ConfigureAwait(false);
        }

        ArgumentNullException.ThrowIfNull(user);

        user.PasswordHash = password;
        UpdateNormalizedProperties(user);

        source.Add(user);
        await source.SaveChangesAsync(Current.CancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> UpdateAsync(User user)
    {
        if (UpdateAsyncResult is not null)
        {
            return await UpdateAsyncResult()
                .ConfigureAwait(false);
        }

        ArgumentNullException.ThrowIfNull(user);

        UpdateNormalizedProperties(user);

        await source.SaveChangesAsync(Current.CancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> DeleteAsync(User user)
    {
        if (DeleteAsyncResult is not null)
        {
            return await DeleteAsyncResult()
                .ConfigureAwait(false);
        }

        source.Remove(user);
        await source.SaveChangesAsync(Current.CancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public Task<IList<Claim>> GetClaimsAsync(User user)
    {
        return GetClaimsAsyncResult is not null
            ? GetClaimsAsyncResult()
            : Task.FromResult<IList<Claim>>([]);
    }

    /// <inheritdoc/>
    public Task<IList<string>> GetRolesAsync(User user)
    {
        var userRoles = source
            .GetQueryable<IdentityUserRole<Guid>>()
            .Where(userRole => userRole.UserId == user.Id)
            .ToList();

        var roles = source
            .GetQueryable<Role>()
            .Where(role => userRoles.Any(userRole => role.Id == userRole.RoleId))
            .Select(role => role.Name)
            .ToList();

        return Task.FromResult<IList<string>>(roles);
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> AddToRoleAsync(User user, string role)
    {
        if (AddToRoleAsyncResult is not null)
        {
            return await AddToRoleAsyncResult()
                .ConfigureAwait(false);
        }

        ArgumentNullException.ThrowIfNull(user);

        var roleEntity = source
            .GetQueryable<Role>()
            .FirstOrDefault(r => r.Name == role);

        if (roleEntity is null)
            return IdentityResult.Failed();

        source.Add(new IdentityUserRole<Guid>()
        {
            RoleId = roleEntity.Id,
            UserId = user.Id
        });

        await source.SaveChangesAsync(Current.CancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> AddToRolesAsync(User user, IEnumerable<string> roles)
    {
        if (AddToRolesAsyncResult is not null)
        {
            return await AddToRolesAsyncResult()
                .ConfigureAwait(false);
        }

        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(roles);

        foreach (var roleName in roles)
        {
            var role = source
                .GetQueryable<Role>()
                .FirstOrDefault(role => role.Name == roleName);

            if (role is null)
                continue;

            var userRole = source
                .GetQueryable<IdentityUserRole<Guid>>()
                .FirstOrDefault(userRole => userRole.RoleId == role.Id && userRole.UserId == user.Id);

            if (userRole is null)
            {
                userRole = new()
                {
                    RoleId = role.Id,
                    UserId = user.Id
                };

                source.Add(userRole);
            }
        }

        await source.SaveChangesAsync(Current.CancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> RemoveFromRoleAsync(User user, string role)
    {
        if (RemoveFromRoleAsyncResult is not null)
        {
            return await RemoveFromRoleAsyncResult()
                .ConfigureAwait(false);
        }

        var roleEntity = source
            .GetQueryable<Role>()
            .FirstOrDefault(entity => entity.Name == role);

        if (roleEntity is null)
            return IdentityResult.Failed();

        var userRoleEntity = source
            .GetQueryable<IdentityUserRole<Guid>>()
            .FirstOrDefault(entity => entity.RoleId == roleEntity.Id && entity.UserId == user.Id);

        if (userRoleEntity is not null)
        {
            source.Remove(userRoleEntity);
            await source.SaveChangesAsync(Current.CancellationToken)
                .ConfigureAwait(false);
        }

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public Task<bool> IsLockedOutAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult(user.LockoutEnd >= timeProvider.GetUtcNow());
    }

    /// <inheritdoc/>
    public Task<bool> CheckPasswordAsync(User user, string password)
    {
        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult(user.PasswordHash == password);
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> AccessFailedAsync(User user)
    {
        if (AccessFailedAsyncResult is not null)
        {
            return await AccessFailedAsyncResult()
                .ConfigureAwait(false);
        }

        ArgumentNullException.ThrowIfNull(user);

        user.AccessFailedCount++;
        UpdateNormalizedProperties(user);

        await source.SaveChangesAsync(Current.CancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(email))]
    public string? NormalizeEmail(string? email)
        => email;

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(name))]
    public string? NormalizeName(string? name)
        => name;

    /// <inheritdoc/>
    public Task<string> GenerateEmailConfirmationTokenAsync(User user)
        => GenerateFakeTokenAsync(user);

    /// <inheritdoc/>
    public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
    {
        if (ConfirmEmailAsyncResult is not null)
        {
            return await ConfirmEmailAsyncResult()
                .ConfigureAwait(false);
        }

        ArgumentNullException.ThrowIfNull(user);

        if (token != user.Id.ToString())
            return IdentityResult.Failed();

        user.EmailConfirmed = true;

        await source.SaveChangesAsync(Current.CancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public Task<string> GenerateChangeEmailTokenAsync(User user, string newEmail)
    {
        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult($"{user.Id} {newEmail}");
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> ChangeEmailAsync(User user, string newEmail, string token)
    {
        if (ChangeEmailAsyncResult is not null)
        {
            return await ChangeEmailAsyncResult()
                .ConfigureAwait(false);
        }

        ArgumentNullException.ThrowIfNull(user);

        if (token != $"{user.Id} {newEmail}")
            return IdentityResult.Failed();

        user.Email = newEmail;

        await source.SaveChangesAsync(Current.CancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public Task<string> GeneratePasswordResetTokenAsync(User user)
        => GenerateFakeTokenAsync(user);

    /// <inheritdoc/>
    public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
    {
        if (ResetPasswordAsyncResult is not null)
        {
            return await ResetPasswordAsyncResult()
                .ConfigureAwait(false);
        }

        ArgumentNullException.ThrowIfNull(user);

        if (token != user.Id.ToString())
            return IdentityResult.Failed();

        user.PasswordHash = newPassword;

        await source.SaveChangesAsync(Current.CancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public Task<IdentityResult> ValidatePasswordAsync(User user, string? password)
    {
        ArgumentNullException.ThrowIfNull(password);

        var errors = new List<IdentityError>();
        var passwordOptions = Options.Password;
        var describer = new IdentityErrorDescriber();

        if (string.IsNullOrWhiteSpace(password) || password.Length < passwordOptions.RequiredLength)
            errors.Add(describer.PasswordTooShort(passwordOptions.RequiredLength));

        if (passwordOptions.RequireNonAlphanumeric && password.All(IsLetterOrDigit))
            errors.Add(describer.PasswordRequiresNonAlphanumeric());

        if (passwordOptions.RequireDigit && !password.Any(IsDigit))
            errors.Add(describer.PasswordRequiresDigit());

        if (passwordOptions.RequireLowercase && !password.Any(IsLower))
            errors.Add(describer.PasswordRequiresLower());

        if (passwordOptions.RequireUppercase && !password.Any(IsUpper))
            errors.Add(describer.PasswordRequiresUpper());

        if (passwordOptions.RequiredUniqueChars >= 1 && password.Distinct().Count() < passwordOptions.RequiredUniqueChars)
            errors.Add(describer.PasswordRequiresUniqueChars(passwordOptions.RequiredUniqueChars));

        return Task.FromResult(errors.Count > 0
            ? IdentityResult.Failed([.. errors])
            : IdentityResult.Success);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Generates the fake action-token based on the given <paramref name="user"/> Id.
    /// </summary>
    /// <param name="user">
    /// The <see cref="User"/> for whom to generate a token.
    /// </param>
    /// <returns>
    /// The fake action-token based on the given <paramref name="user"/> Id.
    /// </returns>
    private static Task<string> GenerateFakeTokenAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult(user.Id.ToString());
    }

    /// <summary>
    /// Returns a flag indicating whether the supplied character is a digit.
    /// </summary>
    /// <param name="character">
    /// The character to check if it is a digit.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the character is a digit,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    private static bool IsDigit(char character)
        => character is >= '0' and <= '9';

    /// <summary>
    /// Returns a flag indicating whether the supplied character is a lower case ASCII letter.
    /// </summary>
    /// <param name="character">
    /// The character to check if it is a lower case ASCII letter.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the character is a lower case ASCII letter,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    private static bool IsLower(char character)
        => character is >= 'a' and <= 'z';

    /// <summary>
    /// Returns a flag indicating whether the supplied character is an upper case ASCII letter.
    /// </summary>
    /// <param name="character">
    /// The character to check if it is an upper case ASCII letter.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the character is an upper case ASCII letter,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    private static bool IsUpper(char character)
        => character is >= 'A' and <= 'Z';

    /// <summary>
    /// Returns a flag indicating whether the supplied character is an ASCII letter or digit.
    /// </summary>
    /// <param name="character">
    /// The character to check if it is an ASCII letter or digit.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the character is an ASCII letter or digit,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    private static bool IsLetterOrDigit(char character)
        => IsUpper(character) || IsLower(character) || IsDigit(character);

    /// <summary>
    /// Sets the normalized properties values of the given <paramref name="user"/>
    /// to the same values as non-normalized ones.
    /// </summary>
    /// <param name="user">
    /// The <see cref="User"/> to update.
    /// </param>
    private static void UpdateNormalizedProperties(User user)
    {
        user.NormalizedEmail = user.Email;
        user.NormalizedUserName = user.UserName;
    }
    #endregion
}