using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Paradise.DataAccess.Database.Interceptors.Base;
using Paradise.Domain.Base;

namespace Paradise.DataAccess.Database.Interceptors;

/// <summary>
/// The <see cref="IInterceptor"/> implementation
/// which will call <see cref="IDatabaseRecord.ValidateState()"/>
/// method on all tracked <see cref="IDatabaseRecord"/> entities
/// upon saving data to the persistence storage.
/// </summary>
public class ValidateStateInterceptor : BaseSaveChangesInterceptor, ISingletonInterceptor
{
    #region Properties
    /// <inheritdoc/>
    protected override Func<EntityEntry, bool>? EntityFilter { get; } = IsDatabaseRecord;
    #endregion

    #region Protected methods
    /// <inheritdoc/>
    protected override void Intercept(EntityEntry entry, DbContextEventProperties properties)
    {
        if (entry.Entity is IDatabaseRecord record)
            record.ValidateState();
    }
    #endregion

    #region Private methods
    /// <inheritdoc cref="EntityFilter"/>
    private static bool IsDatabaseRecord(EntityEntry entry)
    {
        var isDatabaseRecord = entry.Entity is IDatabaseRecord;

        return isDatabaseRecord;
    }
    #endregion
}