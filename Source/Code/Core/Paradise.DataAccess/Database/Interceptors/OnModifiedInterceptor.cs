using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Paradise.DataAccess.Database.Interceptors.Base;
using Paradise.Domain.Base;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.DataAccess.Database.Interceptors;

/// <summary>
/// The <see cref="IInterceptor"/> implementation
/// which will set the modification time on all modified
/// <see cref="IDomainObject"/> entities
/// upon saving data to the persistence storage.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OnModifiedInterceptor"/> class.
/// </remarks>
/// <param name="timeProvider">
/// Time provider.
/// </param>
internal sealed class OnModifiedInterceptor(TimeProvider timeProvider) : BaseSaveChangesInterceptor(timeProvider), ISingletonInterceptor
{
    #region Properties
    /// <inheritdoc/>
    [NotNull]
    public override Func<EntityEntry, bool>? EntityFilter { get; } = IsModifiedDomainObject;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override void Intercept(EntityEntry entry, DbContextEventProperties properties)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(properties);

        if (entry.Entity is IDomainObject domainObject)
            domainObject.OnModified(properties.TransactionTime);
    }
    #endregion

    #region Private methods
    /// <inheritdoc cref="EntityFilter"/>
    private static bool IsModifiedDomainObject(EntityEntry entry)
    {
        var isDomainObject = entry.Entity is IDomainObject;
        var isModified = entry.State is EntityState.Modified;

        return isDomainObject && isModified;
    }
    #endregion
}