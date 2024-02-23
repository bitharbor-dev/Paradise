namespace Paradise.Domain.Base;

/// <summary>
/// Provides default properties and/or methods for all entities.
/// </summary>
public interface IEntity
{
    #region Properties
    /// <summary>
    /// Unique identifier.
    /// </summary>
    Guid Id { get; set; }
    #endregion
}