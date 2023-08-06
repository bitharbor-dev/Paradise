using Paradise.Domain.Base.EqualityComparers;

namespace Paradise.Domain.Base;

/// <summary>
/// Base class for all value-objects.
/// </summary>
public abstract class ValueObject : IDatabaseRecord, IEquatable<ValueObject>
{
    #region Properties
    /// <inheritdoc/>
    public Guid Id { get; set; }

    /// <inheritdoc/>
    public DateTime Created { get; set; }

    /// <inheritdoc/>
    public DateTime Modified { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public virtual void ValidateState() { }

    /// <inheritdoc/>
    public bool Equals(ValueObject? other)
        => this == other;

    /// <inheritdoc/>
    public sealed override bool Equals(object? obj)
        => obj is ValueObject valueObject && Equals(valueObject);

    /// <inheritdoc/>
    public sealed override int GetHashCode()
        => new ValueObjectEqualityComparer<ValueObject>().GetHashCode(this);

    /// <summary>
    /// Gets an array of <see cref="ValueObject"/>
    /// properties to determine its equality compared to the other objects
    /// of the same type.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="object"/>
    /// to determine <see cref="ValueObject"/> equality.
    /// </returns>
    public abstract IEnumerable<object?> GetEqualityComponents();
    #endregion

    #region Operators
    /// <summary>
    /// Compares the given <paramref name="left"/> and <paramref name="right"/>
    /// objects for equality.
    /// </summary>
    /// <param name="left">
    /// The first <see cref="ValueObject"/> to be compared.
    /// </param>
    /// <param name="right">
    /// The second <see cref="ValueObject"/> to be compared.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> equals <paramref name="right"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool operator ==(ValueObject? left, ValueObject? right)
        => new ValueObjectEqualityComparer<ValueObject>().Equals(left, right);

    /// <summary>
    /// Compares the given <paramref name="left"/> and <paramref name="right"/>
    /// objects for inequality.
    /// </summary>
    /// <param name="left">
    /// The first <see cref="ValueObject"/> to be compared.
    /// </param>
    /// <param name="right">
    /// The second <see cref="ValueObject"/> to be compared.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> does not equal <paramref name="right"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool operator !=(ValueObject? left, ValueObject? right)
        => !(left == right);
    #endregion
}