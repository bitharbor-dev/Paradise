using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Base.EqualityComparers;

/// <summary>
/// Equality comparer for all value-objects.
/// </summary>
/// <typeparam name="TValueObject">
/// Value-object type.
/// </typeparam>
public sealed class ValueObjectEqualityComparer<TValueObject> : IEqualityComparer<TValueObject>
    where TValueObject : ValueObject
{
    #region Public methods
    /// <inheritdoc/>
    public bool Equals(TValueObject? x, TValueObject? y)
    {
        // Both objects are not null: compare them and return the result
        if (x is not null && y is not null)
        {
            return x.GetType() == y.GetType()
                && x.GetEqualityComponents().SequenceEqual(y.GetEqualityComponents());
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
    public int GetHashCode([DisallowNull] TValueObject obj)
    {
        var hash = new HashCode();
        hash.Add(obj.GetType());

        foreach (var item in obj.GetEqualityComponents())
            hash.Add(item);

        return hash.ToHashCode();
    }
    #endregion
}