using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Paradise.Tests.Miscellaneous.TestImplementations.Core.DataAccess.Database.Interceptors.Base;

/// <summary>
/// Provides event data for the <see cref="TestBaseSaveChangesInterceptor.Intercepted"/> event.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EntryInterceptedEventArgs"/> class.
/// </remarks>
/// <param name="entry">
/// The intercepted entry.
/// </param>
public sealed class EntryInterceptedEventArgs(EntityEntry entry) : EventArgs
{
    #region Properties
    /// <summary>
    /// The intercepted entry.
    /// </summary>
    public EntityEntry Entry { get; } = entry;
    #endregion
}