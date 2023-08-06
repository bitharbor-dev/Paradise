using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Base.EqualityComparers;

/// <summary>
/// Equality comparer for all entities.
/// </summary>
/// <typeparam name="TEntity">
/// Entity type.
/// </typeparam>
public sealed class EntityEqualityComparer<TEntity> : IEqualityComparer<TEntity>
    where TEntity : IDatabaseRecord
{
    #region Public methods
    /// <inheritdoc/>
    public bool Equals(TEntity? x, TEntity? y)
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
    public int GetHashCode([DisallowNull] TEntity obj)
        => HashCode.Combine(obj.GetType(), obj.Id);
    #endregion
}