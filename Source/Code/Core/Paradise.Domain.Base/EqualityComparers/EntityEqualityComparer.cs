using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Base.EqualityComparers;

/// <summary>
/// Equality comparer for all entities.
/// </summary>
public sealed class EntityEqualityComparer : IEqualityComparer<IEntity>
{
    #region Properties
    /// <summary>
    /// Singleton <see cref="EntityEqualityComparer"/> instance.
    /// </summary>
    public static EntityEqualityComparer Instance { get; } = new();
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public bool Equals(IEntity? x, IEntity? y)
    {
        // Both objects are not null: compare them and return the result
        if (x is not null && y is not null)
        {
            return x.GetType() == y.GetType()
                && x.Id == y.Id;
        }
        // One object is null and another one is not: just return false
        else if (x is null ^ y is null)
        {
            return false;
        }

        // Both objects are null: just return true
        return true;
    }

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] IEntity obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return HashCode.Combine(obj.GetType(), obj.Id);
    }
    #endregion
}