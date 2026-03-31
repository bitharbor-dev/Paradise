using Paradise.Models;

namespace Paradise.Tests.Miscellaneous.TestImplementations.Shared.Models;

/// <summary>
/// Test <see cref="ResultBase"/> implementation.
/// </summary>
public sealed class TestResult : ResultBase
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="TestResult"/> class.
    /// </summary>
    public TestResult() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestResult"/> class.
    /// </summary>
    /// <param name="status">
    /// Indicates the result status. Provides more information on the nature of the errors, in case of any.
    /// <para>
    /// Provides more information on the nature of the errors,
    /// in case of any.
    /// </para>
    /// </param>
    public TestResult(OperationStatus status) : base(status) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestResult"/> class.
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
    public TestResult(OperationStatus status, ErrorCode error, params object?[] arguments) : base(status, error, arguments) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestResult"/> class.
    /// </summary>
    /// <param name="status">
    /// Indicates the result status. Provides more information on the nature of the errors, in case of any.
    /// </param>
    /// <param name="errors">
    /// Errors associated with the result.
    /// </param>
    public TestResult(OperationStatus status, IEnumerable<ApplicationError> errors) : base(status, errors) { }
    #endregion
}