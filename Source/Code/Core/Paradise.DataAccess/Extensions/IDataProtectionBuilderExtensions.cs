using Microsoft.AspNetCore.DataProtection;
using Paradise.DataAccess.Database;

namespace Paradise.DataAccess.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IDataProtectionBuilder"/> <see langword="interface"/>.
/// </summary>
public static class IDataProtectionBuilderExtensions
{
    #region Public methods
    /// <summary>
    /// Configures the data protection system to persist keys to a data access internal data-store.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IDataProtectionBuilder"/> instance to modify.
    /// </param>
    /// <returns>
    /// The <see cref="IDataProtectionBuilder"/> instance configured to
    /// persist keys to a data access internal data-store.
    /// </returns>
    public static IDataProtectionBuilder PersistKeysToDataAccess(this IDataProtectionBuilder builder)
        => builder.PersistKeysToDbContext<InfrastructureContext>();
    #endregion
}