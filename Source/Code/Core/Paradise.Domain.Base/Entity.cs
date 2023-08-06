using Paradise.Domain.Base.EqualityComparers;

namespace Paradise.Domain.Base;

/// <summary>
/// Base class for all entities.
/// </summary>
public abstract class Entity : IDatabaseRecord, IEquatable<Entity>
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
    public bool Equals(Entity? other)
        => this == other;

    /// <inheritdoc/>
    public sealed override bool Equals(object? obj)
        => obj is Entity entity && Equals(entity);

    /// <inheritdoc/>
    public sealed override int GetHashCode()
        => new EntityEqualityComparer<Entity>().GetHashCode(this);
    #endregion

    #region Operators
    /// <summary>
    /// Compares the given <paramref name="left"/> and <paramref name="right"/>
    /// objects for equality.
    /// </summary>
    /// <param name="left">
    /// The first <see cref="Entity"/> to be compared.
    /// </param>
    /// <param name="right">
    /// The second <see cref="Entity"/> to be compared.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> equals <paramref name="right"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool operator ==(Entity? left, Entity? right)
        => new EntityEqualityComparer<Entity>().Equals(left, right);

    /// <summary>
    /// Compares the given <paramref name="left"/> and <paramref name="right"/>
    /// objects for inequality.
    /// </summary>
    /// <param name="left">
    /// The first <see cref="Entity"/> to be compared.
    /// </param>
    /// <param name="right">
    /// The second <see cref="Entity"/> to be compared.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> does not equal <paramref name="right"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool operator !=(Entity? left, Entity? right)
        => !(left == right);
    #endregion
}