using Paradise.Models;
using Paradise.WebApi.Base;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Paradise.WebApi.Client.Base;

/// <summary>
/// Represents the metadata for problem details,
/// including a collection of application errors and any additional extensions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ProblemDetailsMetadata"/> class.
/// </remarks>
/// <param name="errors">
/// The collection of application-specific errors associated with the problem details.
/// </param>
[SuppressMessage("Microsoft.Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated via JSON serializer.")]
[method: JsonConstructor]
internal sealed class ProblemDetailsMetadata(IEnumerable<ApplicationError> errors) : IProblemDetailsMetadata
{
    #region Properties
    /// <inheritdoc/>
    public IEnumerable<ApplicationError> Errors { get; } = errors;

    /// <inheritdoc/>
    [JsonExtensionData]
    public IDictionary<string, object?> Extensions { get; set; } = new Dictionary<string, object?>();
    #endregion
}