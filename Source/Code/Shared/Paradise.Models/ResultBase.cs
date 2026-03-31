using Paradise.Models.Extensions;
using System.Text.Json.Serialization;

namespace Paradise.Models;

/// <summary>
/// Base class for operation results.
/// <para>
/// Encapsulates status, errors, and an optional exception.
/// </para>
/// </summary>
public abstract class ResultBase
{
    #region Fields
    private readonly List<ApplicationError> _errors = [];
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultBase"/> class.
    /// </summary>
    protected ResultBase() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultBase"/> class.
    /// </summary>
    /// <param name="status">
    /// Indicates the result status. Provides more information on the nature of the errors, in case of any.
    /// </param>
    protected ResultBase(OperationStatus status)
        => Status = status;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultBase"/> class.
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
    protected ResultBase(OperationStatus status, ErrorCode error, params object?[] arguments) : this(status)
        => AddError(error, arguments);

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultBase"/> class.
    /// </summary>
    /// <param name="status">
    /// Indicates the result status. Provides more information on the nature of the errors, in case of any.
    /// <para>
    /// Provides more information on the nature of the errors,
    /// in case of any.
    /// </para>
    /// </param>
    /// <param name="errors">
    /// Errors associated with the result.
    /// </param>
    protected ResultBase(OperationStatus status, IEnumerable<ApplicationError> errors)
    {
        Status = status;
        _errors.AddRange(errors);
    }
    #endregion

    #region Indexes
    /// <summary>
    /// Gets the <see cref="ApplicationError"/> at the specified index.
    /// </summary>
    /// <param name="index">
    /// The zero-based index of the element to get.
    /// </param>
    /// <returns>
    /// An <see cref="ApplicationError"/> at the specified index.
    /// </returns>
    public ApplicationError this[int index]
        => _errors[index];
    #endregion

    #region Properties
    /// <summary>
    /// Indicates the result status. Provides more information on the nature of the errors, in case of any.
    /// </summary>
    public OperationStatus Status { get; set; }

    /// <summary>
    /// Exception associated with the result.
    /// </summary>
    [JsonIgnore]
    public Exception? CapturedException { get; private set; }

    /// <summary>
    /// Errors associated with the result.
    /// </summary>
    public IEnumerable<ApplicationError> Errors
        => _errors.AsReadOnly();

    /// <summary>
    /// Indicates whether the operation completed successfully (no errors present).
    /// </summary>
    public bool IsSuccess
        => _errors.Count is 0;
    #endregion

    #region Public methods
    /// <summary>
    /// Adds a new <see cref="ApplicationError"/> instance to the <see cref="Errors"/>
    /// based on the given <paramref name="error"/> and <paramref name="arguments"/>.
    /// </summary>
    /// <param name="error">
    /// Error code to create an <see cref="ApplicationError"/> instance.
    /// </param>
    /// <param name="arguments">
    /// Error description format arguments.
    /// </param>
    public void AddError(ErrorCode error, params object?[] arguments)
    {
        var description = error.GetFormattedDisplayValue(arguments);

        _errors.Add(new(error, description));
    }

    /// <summary>
    /// Adds the given <paramref name="error"/> instance to the <see cref="Errors"/>.
    /// </summary>
    /// <param name="error">
    /// The <see cref="ApplicationError"/> instance to be added.
    /// </param>
    public void AddError(ApplicationError error)
        => _errors.Add(error);

    /// <summary>
    /// Adds the given <paramref name="errors"/> to the result.
    /// </summary>
    /// <param name="errors">
    /// Errors to be added.
    /// </param>
    public void AddErrors(IEnumerable<ApplicationError> errors)
        => _errors.AddRange(errors);

    /// <summary>
    /// Adds the given <paramref name="exception"/> to the result.
    /// </summary>
    /// <param name="exception">
    /// The <see cref="Exception"/> to be added.
    /// </param>
    public void AddException(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        CapturedException = exception;

        var message =
#if !DEBUG
            exception.Message;
#else
            exception.ToString();
#endif

        _errors.Add(new(ErrorCode.DefaultError, message));
    }
    #endregion
}