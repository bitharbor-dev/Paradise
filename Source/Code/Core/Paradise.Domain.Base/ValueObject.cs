using Paradise.Domain.Base.EqualityComparers;

namespace Paradise.Domain.Base;

/// <summary>
/// Base class for all value-objects.
/// </summary>
public abstract class ValueObject : IValueObject, IEquatable<ValueObject>
{
    #region Properties
    /// <inheritdoc/>
    public Guid Id { get; protected set; }

    /// <inheritdoc/>
    public DateTimeOffset Created { get; protected set; }

    /// <inheritdoc/>
    public DateTimeOffset Modified { get; protected set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public virtual void ValidateState() { }

    /// <inheritdoc/>
    public virtual void OnCreated(DateTimeOffset utcNow)
    {
        Id = Guid.CreateVersion7(utcNow);
        Created = utcNow;

        ValidateState();
    }

    /// <inheritdoc/>
    public virtual void OnModified(DateTimeOffset utcNow)
    {
        Modified = utcNow;

        ValidateState();
    }

    /// <inheritdoc/>
    public bool Equals(ValueObject? other)
        => this == other;

    /// <inheritdoc/>
    public sealed override bool Equals(object? obj)
        => obj is ValueObject valueObject && Equals(valueObject);

    /// <inheritdoc/>
    public sealed override int GetHashCode()
        => ValueObjectEqualityComparer.Instance.GetHashCode(this);

    /// <inheritdoc/>
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
        => ValueObjectEqualityComparer.Instance.Equals(left, right);

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