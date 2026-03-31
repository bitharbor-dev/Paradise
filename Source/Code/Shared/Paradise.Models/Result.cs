using System.Text.Json.Serialization;

namespace Paradise.Models;

/// <summary>
/// Represents a general operation result without a value.
/// </summary>
public sealed class Result : ResultBase
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="status">
    /// Indicates the result status. Provides more information on the nature of the errors, in case of any.
    /// <para>
    /// Provides more information on the nature of the errors,
    /// in case of any.
    /// </para>
    /// </param>
    public Result(OperationStatus status) : base(status) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="status">
    /// Indicates the result status. Provides more information on the nature of the errors, in case of any.
    /// </param>
    /// <param name="error">
    /// Error code to create an <see cref="ApplicationError"/> instance.
    /// </param>
    /// <param name="arguments">
    /// Error description format arguments.
    /// </param>
    public Result(OperationStatus status, ErrorCode error, params object?[] arguments) : base(status, error, arguments) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="status">
    /// Indicates the result status. Provides more information on the nature of the errors, in case of any.
    /// </param>
    /// <param name="errors">
    /// Errors associated with the result.
    /// </param>
    [JsonConstructor]
    public Result(OperationStatus status, IEnumerable<ApplicationError> errors) : base(status, errors) { }
    #endregion
}