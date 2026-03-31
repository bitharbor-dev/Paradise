using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Paradise.DataAccess.Database;

namespace Paradise.DataAccess.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IdentityBuilder"/> <see langword="class"/>.
/// </summary>
public static class IdentityBuilderExtensions
{
    #region Public methods
    /// <summary>
    /// Adds a data access internal implementation of identity information stores.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IdentityBuilder"/> instance this method extends.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityBuilder"/> instance this method extends.
    /// </returns>
    public static IdentityBuilder AddDataAccessStores(this IdentityBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddEntityFrameworkStores<DomainContext>();
    }
    #endregion
}