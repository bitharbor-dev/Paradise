using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Paradise.Domain.Base;

namespace Paradise.DataAccess.Database.ChangeTracking;

/// <summary>
/// Contains methods to define <see cref="EntityState"/> dependent behavior.
/// </summary>
internal static class ChangeTrackerEvents
{
    #region Public methods
    /// <summary>
    /// <see cref="ChangeTracker.Tracked"/> event handler.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// An object that contains the event data.
    /// </param>
    public static void OnTracked(object? sender, EntityTrackedEventArgs e)
    {
        if (!e.FromQuery)
        {
            if (e.Entry.Entity is IDatabaseRecord record && e.Entry.State is EntityState.Added)
            {
                record.ValidateState();
                record.Created = DateTime.UtcNow;
            }
        }
    }

    /// <summary>
    /// <see cref="ChangeTracker.StateChanged"/> event handler.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// An object that contains the event data.
    /// </param>
    public static void OnStateChanged(object? sender, EntityStateChangedEventArgs e)
    {
        if (e.Entry.Entity is IDatabaseRecord record && e.Entry.State is EntityState.Modified)
        {
            record.ValidateState();
            record.Modified = DateTime.UtcNow;
        }
    }
    #endregion
}