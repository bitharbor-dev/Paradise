using Microsoft.AspNetCore.Http;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationProblemDetails"/> class.
    /// </summary>
    /// <param name="validationProblem">
    /// The <see cref="HttpValidationProblemDetails"/> produced by the framework.
    /// </param>
    public ApplicationProblemDetails(HttpValidationProblemDetails validationProblem)
    {
        ArgumentNullException.ThrowIfNull(validationProblem);

        Type = validationProblem.Type;
        Title = validationProblem.Title;
        Status = validationProblem.Status;
        Detail = validationProblem.Detail;
        Instance = validationProblem.Instance;
        Extensions = validationProblem.Extensions;
        Errors = GetErrors(validationProblem.Errors);
    }
    #endregion

    #region Properties
    /// <summary>
    /// The collection of application-specific errors associated with the problem details.
    /// </summary>
    public IEnumerable<ApplicationError> Errors { get; }
    #endregion

    #region Private methods
    /// <summary>
    /// Maps framework validation errors into a sequence of <see cref="ApplicationError"/> objects.
    /// </summary>
    /// <param name="validationErrors">
    /// A dictionary produced by model validation where the key is the model/member name
    /// and the value is an array of error messages for that key.
    /// </param>
    /// <returns>
    /// A sequence of <see cref="ApplicationError"/> instances created from all validation messages.
    /// Each returned error uses <see cref="ErrorCode.InvalidModel"/> as the error code.
    /// </returns>
    private static IEnumerable<ApplicationError> GetErrors(IDictionary<string, string[]> validationErrors)
    {
        var values = validationErrors.Values;
        var descriptions = values.SelectMany(errors => errors);

        return descriptions.Select(description => new ApplicationError(ErrorCode.InvalidModel, description));
    }
    #endregion
}