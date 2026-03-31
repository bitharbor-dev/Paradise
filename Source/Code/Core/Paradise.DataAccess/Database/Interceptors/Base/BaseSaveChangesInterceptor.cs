using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.DataAccess.Database.Interceptors.Base;

/// <summary>
/// Base save changes interceptor implementation.
/// </summary>
public abstract class BaseSaveChangesInterceptor : SaveChangesInterceptor
{
    #region Fields
    private readonly TimeProvider _timeProvider;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSaveChangesInterceptor"/> class.
    /// </summary>
    /// <param name="timeProvider">
    /// Time provider.
    /// </param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructors can not be protected.")]
    protected BaseSaveChangesInterceptor(TimeProvider timeProvider)
        => _timeProvider = timeProvider;
    #endregion

    #region Properties
    /// <summary>
    /// Entity entry filter to be applied on the tacked entries collection
    /// before state interception.
    /// </summary>
    public virtual Func<EntityEntry, bool>? EntityFilter { get; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public sealed override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        OnSavingChanges(eventData, new()
        {
            TransactionTime = _timeProvider.GetUtcNow()
        });

        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc/>
    public sealed override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
                                                                                 InterceptionResult<int> result,
                                                                                 CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        OnSavingChanges(eventData, new()
        {
            TransactionTime = _timeProvider.GetUtcNow()
        });

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Intercepts the <paramref name="entry"/> state.
    /// </summary>
    /// <param name="entry">
    /// The input <see cref="EntityEntry"/> instance.
    /// </param>
    /// <param name="properties">
    /// Contains additional transaction information.
    /// </param>
    public abstract void Intercept(EntityEntry entry, DbContextEventProperties properties);
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