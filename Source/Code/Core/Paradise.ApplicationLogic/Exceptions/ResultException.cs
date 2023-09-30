using Microsoft.AspNetCore.Identity;
using Paradise.Models;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Paradise.ApplicationLogic.Exceptions;

/// <inheritdoc/>
public sealed class ResultException : Exception
{
    #region Fields
    private readonly List<Tuple<ErrorCode, object?[]>> _errorData = new();
    private readonly List<IdentityResult> _identityErrors = new();
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class.
    /// </summary>
    internal ResultException() { }

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
        => _errorData.Count is not 0 || _identityErrors.Count is not 0;
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

        foreach (var identityResult in _identityErrors)
            result.AddIdentityResult(identityResult, StatusCode);

        foreach (var error in _errorData)
            result.AddError(StatusCode, error.Item1, error.Item2);

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
        _identityErrors.Add(identityResult);
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
        _errorData.Add(new(errorCode, args));
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