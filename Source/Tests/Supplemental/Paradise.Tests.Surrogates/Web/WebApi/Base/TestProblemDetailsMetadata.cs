using Paradise.Models;
using Paradise.WebApi.Base;
using System.Text.Json.Serialization;

namespace Paradise.Tests.Surrogates.Web.WebApi.Base;

/// <summary>
/// Test <see cref="IProblemDetailsMetadata"/> class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TestProblemDetailsMetadata"/> class.
/// </remarks>
/// <param name="errors">
/// The collection of application-specific errors associated with the problem details.
/// </param>
[method: JsonConstructor]
public sealed class TestProblemDetailsMetadata(IEnumerable<ApplicationError> errors) : IProblemDetailsMetadata
{
    #region Properties
    /// <inheritdoc/>
    public IEnumerable<ApplicationError> Errors { get; } = errors;

    /// <inheritdoc/>
    [JsonExtensionData]
    public IDictionary<string, object?> Extensions { get; set; } = new Dictionary<string, object?>();
    #endregion
}