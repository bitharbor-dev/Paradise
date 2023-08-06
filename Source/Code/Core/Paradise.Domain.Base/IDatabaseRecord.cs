namespace Paradise.Domain.Base;

/// <summary>
/// Provides default properties for all records
/// within the persistence storage.
/// </summary>
public interface IDatabaseRecord
{
    #region Properties
    /// <summary>
    /// Unique identifier.
    /// </summary>
    Guid Id { get; set; }

    /// <summary>
    /// Creation date.
    /// </summary>
    DateTime Created { get; set; }

    /// <summary>
    /// Last changed date.
    /// </summary>
    DateTime Modified { get; set; }
    #endregion

    #region Methods
    /// <summary>
    /// Validates current record's state.
    /// <para>
    /// Automatically called during create/update operations
    /// in case record is being tracked by ORM.
    /// </para>
    /// </summary>
    void ValidateState();
    #endregion
}