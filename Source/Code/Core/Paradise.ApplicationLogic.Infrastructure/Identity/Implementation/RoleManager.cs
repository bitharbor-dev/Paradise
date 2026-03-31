using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Paradise.ApplicationLogic.Infrastructure.Identity.Implementation;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RoleManager{TRole}"/> class.
/// </remarks>
/// <param name="store">
/// The persistence store the manager will operate over.
/// </param>
/// <param name="roleValidators">
/// A collection of validators for roles.
/// </param>
/// <param name="keyNormalizer">
/// The normalizer to use when normalizing role names to keys.
/// </param>
/// <param name="errors">
/// The <see cref="IdentityErrorDescriber"/> used to provider error messages.
/// </param>
/// <param name="logger">
/// The logger used to log messages, warnings and errors.
/// </param>
internal sealed class RoleManager<TRole>(IRoleStore<TRole> store,
                                         IEnumerable<IRoleValidator<TRole>> roleValidators,
                                         ILookupNormalizer keyNormalizer,
                                         IdentityErrorDescriber errors,
                                         ILogger<RoleManager<TRole>> logger)
    : Microsoft.AspNetCore.Identity.RoleManager<TRole>(store,
                                                       roleValidators,
                                                       keyNormalizer,
                                                       errors,
                                                       logger),
    IRoleManager<TRole>
    where TRole : class;