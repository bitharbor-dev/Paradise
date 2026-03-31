using Paradise.Domain.Base.EqualityComparers;

namespace Paradise.Domain.Base;

/// <summary>
/// Base class for all entities.
/// </summary>
public abstract class Entity : IEntity, IEquatable<Entity>
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
    public bool Equals(Entity? other)
        => this == other;

    /// <inheritdoc/>
    public sealed override bool Equals(object? obj)
        => obj is Entity entity && Equals(entity);

    /// <inheritdoc/>
    public sealed override int GetHashCode()
        => EntityEqualityComparer.Instance.GetHashCode(this);
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
        => EntityEqualityComparer.Instance.Equals(left, right);

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