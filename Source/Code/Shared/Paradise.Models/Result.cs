using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.Models.Extensions;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Paradise.Models;

/// <summary>
/// General action result class.
/// </summary>
public class Result : IActionResult
{
    #region Constants
    /// <summary>
    /// Default content type for this type of result.
    /// </summary>
    public const string ContentType = MediaTypeNames.Application.Json;
    #endregion

    #region Fields
    private protected readonly List<ApplicationError> _errors = new();
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    public Result() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    public Result(HttpStatusCode statusCode)
       => SetStatusCode(statusCode);
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
    /// Result exception.
    /// </summary>
    [JsonIgnore]
    public Exception? Exception { get; private set; }

    /// <summary>
    /// Result errors.
    /// </summary>
    public IEnumerable<ApplicationError> Errors
        => _errors.AsReadOnly();

    /// <inheritdoc/>
    public HttpStatusCode? StatusCode { get; private set; }

    /// <summary>
    /// Indicates whether the result has any errors.
    /// </summary>
    public bool IsSuccess
        => _errors.Count is 0;
    #endregion

    #region Public methods
    /// <summary>
    /// Sets the given <paramref name="statusCode"/> value
    /// into the <see cref="StatusCode"/> property.
    /// </summary>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    public void SetStatusCode(HttpStatusCode? statusCode)
    {
        if (statusCode.HasValue)
            StatusCode = statusCode.Value;
    }

    /// <summary>
    /// Adds a new <see cref="ApplicationError"/> instance to the <see cref="Errors"/>
    /// based on the given <paramref name="error"/> and <paramref name="args"/>.
    /// </summary>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    /// <param name="error">
    /// Error code to create an <see cref="ApplicationError"/> instance.
    /// </param>
    /// <param name="args">
    /// Error description format arguments.
    /// </param>
    public void AddError(HttpStatusCode statusCode, ErrorCode error, params object?[] args)
    {
        SetStatusCode(statusCode);

        var description = error.GetFormattedErrorDescription(args);

        _errors.Add(new(error, description));
    }

    /// <summary>
    /// Adds the given <paramref name="errors"/> to the result.
    /// </summary>
    /// <param name="errors">
    /// Errors to be added.
    /// </param>
    /// <param name="statusCode">
    /// Result status code.
    /// <para>
    /// If not <see langword="null"/> - will overwrite
    /// the current <see cref="StatusCode"/> value.
    /// </para>
    /// </param>
    public void AddErrors(IEnumerable<ApplicationError> errors, HttpStatusCode? statusCode = null)
    {
        SetStatusCode(statusCode);
        _errors.AddRange(errors);
    }

    /// <summary>
    /// Adds the given <paramref name="result"/>
    /// errors to the current <see cref="Result"/> instance
    /// and overwrites it's <see cref="StatusCode"/> in case
    /// not <see langword="null"/> value.
    /// </summary>
    /// <param name="result">
    /// The <see cref="Result"/> to be merged
    /// with the current instance.
    /// </param>
    public void Merge(Result result)
    {
        _errors.AddRange(result.Errors);

        if (result.StatusCode.HasValue)
            StatusCode = result.StatusCode.Value;
    }

    /// <summary>
    /// Adds the <see cref="IdentityResult.Errors"/>
    /// from the given <paramref name="identityResult"/>.
    /// </summary>
    /// <param name="identityResult">
    /// The <see cref="IdentityResult"/> to add errors from.
    /// </param>
    /// <param name="statusCode">
    /// Result status code.
    /// <para>
    /// If not <see langword="null"/> - will overwrite
    /// the current <see cref="StatusCode"/> value.
    /// </para>
    /// </param>
    public void AddIdentityResult(IdentityResult identityResult, HttpStatusCode? statusCode = null)
    {
        SetStatusCode(statusCode);

        foreach (var error in identityResult.Errors)
        {
            if (Enum.TryParse(error.Code, out ErrorCode errorCode))
                _errors.Add(new(errorCode, error.Description));
        }
    }

    /// <summary>
    /// Adds the given <paramref name="exception"/> to the result.
    /// </summary>
    /// <param name="exception">
    /// The <see cref="System.Exception"/> to be added.
    /// </param>
    /// <param name="statusCode">
    /// Result status code.
    /// <para>
    /// If not <see langword="null"/> - will overwrite
    /// the current <see cref="StatusCode"/> value.
    /// </para>
    /// </param>
    public void AddException(Exception exception, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        Exception = exception;
        SetStatusCode(statusCode);

        var message =
#if !DEBUG
            exception.Message;
#else
            exception.ToString();
#endif

        _errors.Add(new(ErrorCode.DefaultError, message));
    }

    /// <inheritdoc/>
    public Task ExecuteResultAsync(ActionContext context)
    {
        var jsonOptions = context.HttpContext.RequestServices.GetService<IOptions<JsonSerializerOptions>>()?.Value;

        return WriteResponseContentAsync(context.HttpContext.Response, jsonOptions);
    }

    /// <summary>
    /// Writes the current <see cref="Result"/> instance
    /// into the given <paramref name="response"/> content.
    /// </summary>
    /// <param name="response">
    /// The <see cref="HttpResponse"/> instance.
    /// </param>
    /// <param name="options">
    /// The <see cref="JsonSerializerOptions"/> instance.
    /// </param>
    public async Task WriteResponseContentAsync(HttpResponse response, JsonSerializerOptions? options = null)
    {
        if (response.HasStarted)
            return;

        if (StatusCode.HasValue)
            response.StatusCode = (int)StatusCode.Value;

        response.ContentType = ContentType;

        var json = JsonSerializer.Serialize(this, GetType(), options);

        await response.WriteAsync(json);
    }

    /// <summary>
    /// Converts the given <paramref name="modelState"/> into
    /// the <see cref="Result"/> instance.
    /// </summary>
    /// <param name="modelState">
    /// <see cref="ModelStateDictionary"/> to be converted.
    /// </param>
    /// <returns>
    /// <see cref="Result"/> with the data from the <paramref name="modelState"/>.
    /// </returns>
    public static Result FromModelState(ModelStateDictionary modelState)
    {
        var result = new Result();

        var errors = modelState
            .Where(state => state.Value is not null)
            .SelectMany(state => state.Value!.Errors.Select(error => error.ErrorMessage));

        foreach (var error in errors)
        {
            result.AddError(HttpStatusCode.BadRequest,
                            ErrorCode.InvalidModel,
                            error);
        }

        return result;
    }

    /// <summary>
    /// Converts the given <paramref name="statusCode"/>
    /// into a <see cref="Result"/> instance.
    /// </summary>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> to be converted into a <see cref="Result"/> instance.
    /// </param>
    /// <returns>
    /// A new <see cref="Result"/> instance converted from the given <paramref name="statusCode"/>.
    /// </returns>
    public static Result FromHttpStatusCode(HttpStatusCode statusCode)
        => new(statusCode);
    #endregion

    #region Operators
    /// <summary>
    /// Implicitly converts the given <paramref name="statusCode"/>
    /// into a <see cref="Result"/> instance.
    /// </summary>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> to be converted into a <see cref="Result"/> instance.
    /// </param>
    public static implicit operator Result(HttpStatusCode statusCode)
        => new(statusCode);
    #endregion
}

/// <summary>
/// General action result class.
/// </summary>
/// <typeparam name="TValue">
/// The <see cref="Result{TValue}.Value"/> type.
/// </typeparam>
public sealed class Result<TValue> : Result
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class.
    /// </summary>
    public Result() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class.
    /// </summary>
    /// <param name="value">
    /// Result value.
    /// </param>
    /// <param name="statusCode">
    /// Result status code.
    /// </param>
    public Result(TValue? value, HttpStatusCode statusCode)
    {
        Value = value;
        SetStatusCode(statusCode);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class
    /// with the given result <paramref name="value"/>,
    /// <paramref name="errors"/> and <paramref name="statusCode"/>.
    /// </summary>
    /// <param name="value">
    /// Result value.
    /// </param>
    /// <param name="errors">
    /// Result errors.
    /// </param>
    /// <param name="statusCode">
    /// Result status code.
    /// </param>
    [JsonConstructor]
    public Result(TValue? value, IEnumerable<ApplicationError> errors, HttpStatusCode? statusCode)
    {
        Value = value;
        _errors.AddRange(errors);
        SetStatusCode(statusCode);
    }
    #endregion

    #region Properties
    /// <summary>
    /// Result value.
    /// </summary>
    public TValue? Value { get; private set; }
    #endregion

    #region Public methods
    /// <summary>
    /// Sets the result value and status code.
    /// </summary>
    /// <param name="value">
    /// Value to be set.
    /// </param>
    /// <param name="statusCode">
    /// Result status code.
    /// </param>
    public void SetValue(TValue value, HttpStatusCode statusCode)
    {
        SetStatusCode(statusCode);
        Value = value;
    }
    #endregion
}