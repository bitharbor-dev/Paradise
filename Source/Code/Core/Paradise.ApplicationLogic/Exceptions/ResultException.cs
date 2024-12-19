using Microsoft.AspNetCore.Identity;
using Paradise.Models;
using Paradise.Models.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;

namespace Paradise.ApplicationLogic.Exceptions;

/// <inheritdoc/>
public sealed class ResultException : Exception
{
    #region Fields
    private readonly List<ApplicationError> _errors = [];
    private readonly StringBuilder _exceptionMessageBuilder = new();
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class.
    /// </summary>
    internal ResultException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">
    /// <inheritdoc/>
    /// </param>
    internal ResultException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class
    /// with a specified error message and a reference to the inner exception that is the
    /// cause of this exception.
    /// </summary>
    /// <param name="message">
    /// <inheritdoc/>
    /// </param>
    /// <param name="innerException">
    /// <inheritdoc/>
    /// </param>
    internal ResultException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class.
    /// </summary>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    /// <param name="errorCode">
    /// <see cref="ErrorCode"/> that references
    /// the corresponding error description.
    /// </param>
    /// <param name="args">
    /// An object array that contains zero or more objects to format.
    /// </param>
    internal ResultException(HttpStatusCode statusCode, ErrorCode errorCode, params object?[] args)
        => AddError(statusCode, errorCode, args);

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class.
    /// </summary>
    /// <param name="identityResult">
    /// The <see cref="IdentityResult"/> to add errors from.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    private ResultException(IdentityResult identityResult, HttpStatusCode statusCode)
        => AddError(identityResult, statusCode);
    #endregion

    #region Properties
    /// <summary>
    /// Status code.
    /// </summary>
    public HttpStatusCode StatusCode { get; private set; }

    /// <summary>
    /// Indicates whether the current instance has any errors.
    /// </summary>
    public bool HaveErrors
        => _errors.Count is not 0;

    /// <inheritdoc/>
    public override string Message
        => _exceptionMessageBuilder.ToString();
    #endregion

    #region Public methods
    /// <summary>
    /// Gets a <see cref="Result"/> instance representing
    /// the current exception.
    /// </summary>
    /// <returns>
    /// A <see cref="Result"/> instance representing
    /// the current exception.
    /// </returns>
    public Result GetResult()
    {
        var result = new Result();

        result.AddErrors(_errors, StatusCode);

        if (InnerException is not null)
            result.AddException(InnerException, StatusCode);

        return result;
    }
    #endregion

    #region Internal methods
    /// <summary>
    /// Adds an identity error to the current instance.
    /// </summary>
    /// <param name="identityResult">
    /// The <see cref="IdentityResult"/> to add errors from.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    internal void AddError(IdentityResult identityResult, HttpStatusCode statusCode = HttpStatusCode.ServiceUnavailable)
    {
        foreach (var error in identityResult.AsErrors())
        {
            _exceptionMessageBuilder.AppendLine(error);
            _errors.Add(error);
        }

        StatusCode = statusCode;
    }

    /// <summary>
    /// Adds an error to the current instance.
    /// </summary>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    /// <param name="errorCode">
    /// <see cref="ErrorCode"/> that references
    /// the corresponding error description.
    /// </param>
    /// <param name="args">
    /// An object array that contains zero or more objects to format.
    /// </param>
    internal void AddError(HttpStatusCode statusCode, ErrorCode errorCode, params object?[] args)
    {
        var error = new ApplicationError(errorCode, errorCode.GetFormattedErrorDescription(args));

        _exceptionMessageBuilder.AppendLine(error);
        _errors.Add(error);

        StatusCode = statusCode;
    }

    /// <summary>
    /// Throws a <see cref="ResultException"/>.
    /// </summary>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    /// <param name="errorCode">
    /// <see cref="ErrorCode"/> that references
    /// the corresponding error description.
    /// </param>
    /// <param name="args">
    /// An object array that contains zero or more objects to format.
    /// </param>
    [DoesNotReturn]
    internal static void Throw(HttpStatusCode statusCode, ErrorCode errorCode, params object?[] args)
        => throw new ResultException(statusCode, errorCode, args);

    /// <summary>
    /// Throws a <see cref="ResultException"/>.
    /// </summary>
    /// <param name="identityResult">
    /// The <see cref="IdentityResult"/> to add errors from.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    [DoesNotReturn]
    internal static void Throw(IdentityResult identityResult, HttpStatusCode statusCode = HttpStatusCode.ServiceUnavailable)
        => throw new ResultException(identityResult, statusCode);
    #endregion
}