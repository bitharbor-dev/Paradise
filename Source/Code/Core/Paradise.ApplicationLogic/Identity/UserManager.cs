using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Extensions;
using Paradise.Domain.Users;
using System.Security.Claims;

namespace Paradise.ApplicationLogic.Identity;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserManager"/> class.
/// </remarks>
/// <param name="store">
/// The persistence store the manager will operate over.
/// </param>
/// <param name="identityOptions">
/// The accessor used to access the <see cref="IdentityOptions"/>.
/// </param>
/// <param name="passwordHasher">
/// The password hashing implementation to use when saving passwords.
/// </param>
/// <param name="userValidators">
/// A collection of <see cref="IUserValidator{TUser}"/>
/// to validate users against.
/// </param>
/// <param name="passwordValidators">
/// A collection of <see cref="IPasswordValidator{TUser}"/>
/// to validate passwords against.
/// </param>
/// <param name="keyNormalizer">
/// The <see cref="ILookupNormalizer"/> to use
/// when generating index keys for users.
/// </param>
/// <param name="errors">
/// The <see cref="IdentityErrorDescriber"/> used to provider error
/// messages.
/// </param>
/// <param name="services">
/// The <see cref="IServiceProvider"/> used to resolve services.
/// </param>
/// <param name="logger">
/// The logger used to log messages, warnings and errors.
/// </param>
public sealed class UserManager(IUserStore<User> store,
                                IOptions<IdentityOptions> identityOptions,
                                IPasswordHasher<User> passwordHasher,
                                IEnumerable<IUserValidator<User>> userValidators,
                                IEnumerable<IPasswordValidator<User>> passwordValidators,
                                ILookupNormalizer keyNormalizer,
                                IdentityErrorDescriber errors,
                                IServiceProvider services,
                                ILogger<UserManager> logger)
    : UserManager<User>(store, identityOptions, passwordHasher, userValidators,
                        passwordValidators, keyNormalizer, errors, services, logger)
{
    #region Public methods
    /// <inheritdoc/>
    public override async Task<IdentityResult> CreateAsync(User user)
    {
        var creationResult = await base.CreateAsync(user);
        if (!creationResult.Succeeded)
            return creationResult;

        var claimsAdditionResult = await AddDefaultUserClaimsAsync(user);
        if (!claimsAdditionResult.Succeeded)
        {
            Logger.LogClaimsAdditionFailure();
            await DeleteAsync(user);
            return claimsAdditionResult;
        }

        return IdentityResult.Success;
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Adds the default application claims to the given <paramref name="user"/>.
    /// </summary>
    /// <param name="user">
    /// The user to add claims to.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation,
    /// containing the <see cref="IdentityResult"/> of the operation.
    /// </returns>
    private async Task<IdentityResult> AddDefaultUserClaimsAsync(User user)
    {
        var claims = new Claim[]
        {
            new(Options.ClaimsIdentity.UserIdClaimType, user.Id.ToString()),
            new(Options.ClaimsIdentity.UserNameClaimType, user.UserName),
            new(Options.ClaimsIdentity.EmailClaimType, user.Email),
        };

        return await AddClaimsAsync(user, claims);
    }
    #endregion
}