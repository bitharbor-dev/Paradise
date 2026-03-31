using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Paradise.ApplicationLogic.Infrastructure.Identity.Implementation;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserManager{TUser}"/> class.
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
internal sealed class UserManager<TUser>(IUserStore<TUser> store,
                                         IOptions<IdentityOptions> identityOptions,
                                         IPasswordHasher<TUser> passwordHasher,
                                         IEnumerable<IUserValidator<TUser>> userValidators,
                                         IEnumerable<IPasswordValidator<TUser>> passwordValidators,
                                         ILookupNormalizer keyNormalizer,
                                         IdentityErrorDescriber errors,
                                         IServiceProvider services,
                                         ILogger<UserManager<TUser>> logger)
    : Microsoft.AspNetCore.Identity.UserManager<TUser>(store,
                                                       identityOptions,
                                                       passwordHasher,
                                                       userValidators,
                                                       passwordValidators,
                                                       keyNormalizer,
                                                       errors,
                                                       services,
                                                       logger),
    IUserManager<TUser>
    where TUser : IdentityUser<Guid>
{
    #region Fields
    private readonly IServiceProvider _services = services;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override async Task<IdentityResult> CreateAsync(TUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var creationResult = await base
            .CreateAsync(user)
            .ConfigureAwait(false);

        if (!creationResult.Succeeded)
            return creationResult;

        var claimsAdditionResult = await AddDefaultUserClaimsAsync(user)
            .ConfigureAwait(false);

        if (!claimsAdditionResult.Succeeded)
        {
            await DeleteAsync(user)
                .ConfigureAwait(false);

            return claimsAdditionResult;
        }

        return IdentityResult.Success;
    }

    ///<inheritdoc cref="UserManager{TUser}.FindByIdAsync"/>
    public Task<TUser?> FindByIdAsync(Guid userId)
        => FindByIdAsync(userId.ToString());

    /// <inheritdoc/>
    public async Task<TUser?> FindByPhoneNumberAsync(string? phoneNumber)
    {
        ThrowIfDisposed();

        ArgumentException.ThrowIfNullOrEmpty(phoneNumber);

        var user = await Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, CancellationToken)
            .ConfigureAwait(false);

        if (user is null && Options.Stores.ProtectPersonalData)
        {
            var keyRing = _services.GetRequiredService<ILookupProtectorKeyRing>();
            var protector = _services.GetRequiredService<ILookupProtector>();

            foreach (var key in keyRing.GetAllKeyIds())
            {
                var protectedPhoneNumber = protector.Protect(key, phoneNumber);

                user = await Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == protectedPhoneNumber, CancellationToken)
                    .ConfigureAwait(false);

                if (user is not null)
                    return user;
            }
        }

        return user;
    }

    Task<IdentityResult> IUserManager<TUser>.ValidatePasswordAsync(TUser user, string? password)
        => ValidatePasswordAsync(user, password);
    #endregion

    #region Private methods
    /// <summary>
    /// Adds the default application claims to the given <paramref name="user"/>.
    /// </summary>
    /// <param name="user">
    /// The user to add claims to.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation,
    /// containing the <see cref="IdentityResult"/> of the operation.
    /// </returns>
    private Task<IdentityResult> AddDefaultUserClaimsAsync(TUser user)
    {
        var claims = new Claim[]
        {
            new(Options.ClaimsIdentity.UserIdClaimType, user.Id.ToString()),
            new(Options.ClaimsIdentity.UserNameClaimType, user.UserName!),
            new(Options.ClaimsIdentity.EmailClaimType, user.Email!),
        };

        return AddClaimsAsync(user, claims);
    }
    #endregion
}