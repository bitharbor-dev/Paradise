using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Paradise.DataAccess.Database.Interceptors.Base;
using Paradise.Domain.Base;

namespace Paradise.DataAccess.Database.Interceptors;

/// <summary>
/// The <see cref="IInterceptor"/> implementation
/// which will set the creation time on all added
/// <see cref="IDatabaseRecord"/> entities
/// upon saving data to the persistence storage.
/// </summary>
public class SetCreatedInterceptor : BaseSaveChangesInterceptor, ISingletonInterceptor
{
    #region Properties
    /// <inheritdoc/>
    protected override Func<EntityEntry, bool>? EntityFilter { get; } = IsAddedDatabaseRecord;
    #endregion

    #region Protected methods
    /// <inheritdoc/>
    protected override void Intercept(EntityEntry entry, DbContextEventProperties properties)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(properties);

        if (entry.Entity is IDatabaseRecord record)
            record.Created = properties.TransactionTime;
    }
    #endregion

    #region Private methods
    /// <inheritdoc cref="EntityFilter"/>
    private static bool IsAddedDatabaseRecord(EntityEntry entry)
    {
        var isDatabaseRecord = entry.Entity is IDatabaseRecord;
        var isAdded = entry.State is EntityState.Added;

        return isDatabaseRecord && isAdded;
    }
    #endregion
}