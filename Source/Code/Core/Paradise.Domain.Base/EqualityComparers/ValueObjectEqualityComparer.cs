using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Base.EqualityComparers;

/// <summary>
/// Equality comparer for all value-objects.
/// </summary>
public sealed class ValueObjectEqualityComparer : IEqualityComparer<IValueObject>
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueObjectEqualityComparer"/> class.
    /// </summary>
    /// <remarks>
    /// Internal constructor is required to restrict the usage
    /// to a single static <see cref="Instance"/>.
    /// </remarks>
    internal ValueObjectEqualityComparer() { }
    #endregion

    #region Properties
    /// <summary>
    /// Singleton <see cref="ValueObjectEqualityComparer"/> instance.
    /// </summary>
    public static ValueObjectEqualityComparer Instance { get; } = new();
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public bool Equals(IValueObject? x, IValueObject? y)
    {
        // Both objects are not null: compare them and return the result
        if (x is not null && y is not null)
        {
            var xComponents = x.GetEqualityComponents();
            var yComponents = y.GetEqualityComponents();

            return x.GetType() == y.GetType()
                && xComponents.SequenceEqual(yComponents);
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
    public int GetHashCode([DisallowNull] IValueObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var hash = new HashCode();
        hash.Add(obj.GetType());

        foreach (var item in obj.GetEqualityComponents())
            hash.Add(item);

        return hash.ToHashCode();
    }
    #endregion
}