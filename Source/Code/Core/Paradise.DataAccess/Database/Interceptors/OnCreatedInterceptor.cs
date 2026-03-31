using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Paradise.DataAccess.Database.Interceptors.Base;
using Paradise.Domain.Base;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.DataAccess.Database.Interceptors;

/// <summary>
/// The <see cref="IInterceptor"/> implementation
/// which will set the creation time on all added
/// <see cref="IDomainObject"/> entities
/// upon saving data to the persistence storage.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OnCreatedInterceptor"/> class.
/// </remarks>
/// <param name="timeProvider">
/// Time provider.
/// </param>
internal sealed class OnCreatedInterceptor(TimeProvider timeProvider) : BaseSaveChangesInterceptor(timeProvider), ISingletonInterceptor
{
    #region Properties
    /// <inheritdoc/>
    [NotNull]
    public override Func<EntityEntry, bool>? EntityFilter { get; } = IsAddedDomainObject;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override void Intercept(EntityEntry entry, DbContextEventProperties properties)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(properties);

        if (entry.Entity is IDomainObject domainObject)
            domainObject.OnCreated(properties.TransactionTime);
    }
    #endregion

    #region Private methods
    /// <inheritdoc cref="EntityFilter"/>
    private static bool IsAddedDomainObject(EntityEntry entry)
    {
        var isDomainObject = entry.Entity is IDomainObject;
        var isAdded = entry.State is EntityState.Added;

        return isDomainObject && isAdded;
    }
    #endregion
}