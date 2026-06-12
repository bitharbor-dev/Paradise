using Microsoft.AspNetCore.Mvc;
using Paradise.Models;
using Paradise.WebApi.Base;

namespace Paradise.WebApi.Infrastructure;

/// <summary>
/// Represents a <see cref="ProblemDetails"/> instance extended with application-specific errors.
/// </summary>
public sealed class ApplicationProblemDetails : ProblemDetails, IProblemDetailsMetadata
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationProblemDetails"/> class.
    /// </summary>
    /// <param name="status">
    /// The HTTP status code associated with the problem details.
    /// </param>
    /// <param name="errors">
    /// The collection of application-specific errors associated with the problem details.
    /// </param>
    public ApplicationProblemDetails(int status, IEnumerable<ApplicationError> errors)
    {
        Status = status;
        Errors = errors;
    }
    #endregion

    #region Properties
    /// <summary>
    /// The collection of application-specific errors associated with the problem details.
    /// </summary>
    public IEnumerable<ApplicationError> Errors { get; }
    #endregion
}