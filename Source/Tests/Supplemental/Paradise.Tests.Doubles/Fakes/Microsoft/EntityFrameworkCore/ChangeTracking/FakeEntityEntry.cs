using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Tests.Doubles.Fakes.Microsoft.EntityFrameworkCore.ChangeTracking;

/// <summary>
/// Fake <see cref="EntityEntry"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeEntityEntry"/> class.
/// </remarks>
/// <param name="entity">
/// The entity being tracked by this entry.
/// </param>
/// <param name="state">
/// State that this entity is being tracked in.
/// </param>
[method: SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
public sealed class FakeEntityEntry(object entity, EntityState state) : EntityEntry(null!)
{
    #region Properties
    /// <inheritdoc/>
    public override object Entity
        => entity;

    /// <inheritdoc/>
    public override EntityState State
    {
        get => state;
        set => state = value;
    }
    #endregion
}