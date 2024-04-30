using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Base.EqualityComparers;

/// <summary>
/// Equality comparer for all entities.
/// </summary>
public sealed class EntityEqualityComparer : IEqualityComparer<IEntity>
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityEqualityComparer"/> class.
    /// </summary>
    /// <remarks>
    /// Internal constructor is required to restrict the usage
    /// to a single static <see cref="Instance"/>.
    /// </remarks>
    internal EntityEqualityComparer() { }
    #endregion

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