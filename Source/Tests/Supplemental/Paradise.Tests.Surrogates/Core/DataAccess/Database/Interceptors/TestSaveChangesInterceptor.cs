using Microsoft.EntityFrameworkCore.ChangeTracking;
using Paradise.DataAccess.Database.Interceptors.Base;

namespace Paradise.Tests.Surrogates.Core.DataAccess.Database.Interceptors;

/// <summary>
/// Test <see cref="BaseSaveChangesInterceptor"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TestSaveChangesInterceptor"/> class.
/// </remarks>
/// <param name="timeProvider">
/// Time provider.
/// </param>
public sealed class TestSaveChangesInterceptor(TimeProvider timeProvider) : BaseSaveChangesInterceptor(timeProvider)
{
    #region Properties
    /// <summary>
    /// <see cref="EntityFilter"/> result.
    /// </summary>
    public Func<EntityEntry, bool>? EntityFilterResult { get; set; }

    /// <inheritdoc/>
    public override Func<EntityEntry, bool>? EntityFilter
        => EntityFilterResult ?? base.EntityFilter;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override void Intercept(EntityEntry entry, DbContextEventProperties properties)
        => Intercepted?.Invoke(this, new(entry));
    #endregion

    #region Events
    /// <summary>
    /// Occurs when an entity entry is intercepted.
    /// </summary>
    public event EventHandler<EntryInterceptedEventArgs>? Intercepted;
    #endregion
}