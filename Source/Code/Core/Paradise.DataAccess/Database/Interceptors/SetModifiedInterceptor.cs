using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Paradise.DataAccess.Database.Interceptors.Base;
using Paradise.Domain.Base;

namespace Paradise.DataAccess.Database.Interceptors;

/// <summary>
/// The <see cref="IInterceptor"/> implementation
/// which will set the modification time on all modified
/// <see cref="IDatabaseRecord"/> entities
/// upon saving data to the persistence storage.
/// </summary>
public class SetModifiedInterceptor : BaseSaveChangesInterceptor, ISingletonInterceptor
{
    #region Properties
    /// <inheritdoc/>
    protected override Func<EntityEntry, bool>? EntityFilter { get; } = IsModifiedDatabaseRecord;
    #endregion

    #region Protected methods
    /// <inheritdoc/>
    protected override void Intercept(EntityEntry entry, DbContextEventProperties properties)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(properties);

        if (entry.Entity is IDatabaseRecord record)
            record.Modified = properties.TransactionTime;
    }
    #endregion

    #region Private methods
    /// <inheritdoc cref="EntityFilter"/>
    private static bool IsModifiedDatabaseRecord(EntityEntry entry)
    {
        var isDatabaseRecord = entry.Entity is IDatabaseRecord;
        var isModified = entry.State is EntityState.Modified;

        return isDatabaseRecord && isModified;
    }
    #endregion
}