using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Paradise.DataAccess.Database.Interceptors.Base;

/// <summary>
/// Base save changes interceptor implementation.
/// </summary>
public abstract class BaseSaveChangesInterceptor : SaveChangesInterceptor
{
    #region Properties
    /// <summary>
    /// Entity entry filter to be applied on the tacked entries collection
    /// before state interception.
    /// </summary>
    protected virtual Func<EntityEntry, bool>? EntityFilter { get; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public sealed override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        OnSavingChanges(eventData, new()
        {
            TransactionTime = DateTime.UtcNow
        });

        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc/>
    public sealed override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        OnSavingChanges(eventData, new()
        {
            TransactionTime = DateTime.UtcNow
        });

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    #endregion

    #region Protected methods
    /// <summary>
    /// Intercepts the <paramref name="entry"/> state.
    /// </summary>
    /// <param name="entry">
    /// The input <see cref="EntityEntry"/> instance.
    /// </param>
    /// <param name="properties">
    /// Contains additional transaction information.
    /// </param>
    protected abstract void Intercept(EntityEntry entry, DbContextEventProperties properties);
    #endregion

    #region Private methods
    /// <summary>
    /// Handles additional actions upon data saving.
    /// </summary>
    /// <param name="eventData">
    /// Contextual information about the <see cref="DbContext" /> being used.
    /// </param>
    /// <param name="properties">
    /// Contains additional transaction information.
    /// </param>
    private void OnSavingChanges(DbContextEventData eventData, DbContextEventProperties properties)
    {
        var context = eventData.Context;

        if (context is null)
            return;

        var entries = EntityFilter is not null
            ? context.ChangeTracker.Entries().Where(EntityFilter)
            : context.ChangeTracker.Entries();

        foreach (var entry in entries)
            Intercept(entry, properties);
    }
    #endregion
}