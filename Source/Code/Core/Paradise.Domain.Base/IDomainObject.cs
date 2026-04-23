namespace Paradise.Domain.Base;

/// <summary>
/// Provides default properties for all domain objects.
/// </summary>
public interface IDomainObject
{
    #region Properties
    /// <summary>
    /// Unique identifier.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Creation date.
    /// </summary>
    DateTimeOffset Created { get; }

    /// <summary>
    /// Last changed date.
    /// </summary>
    DateTimeOffset Modified { get; }
    #endregion

    #region Methods
    /// <summary>
    /// Validates current object's state.
    /// <para>
    /// Should throw a <see cref="InvalidOperationException"/>
    /// in case of state validation errors.
    /// </para>
    /// </summary>
    void ValidateState();

    /// <summary>
    /// Occurs when an object is created.
    /// </summary>
    /// <param name="utcNow">
    /// Current UTC time.
    /// </param>
    void OnCreated(DateTimeOffset utcNow);

    /// <summary>
    /// Occurs when an object is modified.
    /// </summary>
    /// <param name="utcNow">
    /// Current UTC time.
    /// </param>
    void OnModified(DateTimeOffset utcNow);
    #endregion
}