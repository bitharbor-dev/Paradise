namespace Paradise.Models;

/// <summary>
/// Declares possible operation statuses.
/// </summary>
public enum OperationStatus
{
    /// <summary>
    /// The operation completed successfully.
    /// </summary>
    Success,
    /// <summary>
    /// The operation failed due to an unexpected internal error.
    /// </summary>
    Failure,
    /// <summary>
    /// The provided input was invalid or incomplete.
    /// </summary>
    InvalidInput,
    /// <summary>
    /// A new entity was created as a result of the operation.
    /// </summary>
    Created,
    /// <summary>
    /// The requested entity could not be found.
    /// </summary>
    Missing,
    /// <summary>
    /// The operation was received and may be processed asynchronously.
    /// </summary>
    Received,
    /// <summary>
    /// The operation cannot proceed due to business rules or conflicts.
    /// </summary>
    Blocked,
    /// <summary>
    /// The operation is not allowed in the current context.
    /// </summary>
    Prohibited,
    /// <summary>
    /// The caller is not authorized to perform the operation.
    /// </summary>
    Unauthorized
}