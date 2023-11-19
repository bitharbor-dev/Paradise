using Microsoft.AspNetCore.Http;
using Paradise.Models;

namespace Paradise.ApplicationLogic.Authorization.Models;

/// <summary>
/// An interface to access HTTP response methods.
/// </summary>
public interface IHttpResponseWrapper
{
    #region Properties
    /// <summary>
    /// Gets a value indicating whether response headers have been sent to the client.
    /// </summary>
    bool HasStarted { get; }

    /// <summary>
    /// Gets the request headers.
    /// </summary>
    IHeaderDictionary RequestHeaders { get; }

    /// <summary>
    /// Gets the endpoint meta-data.
    /// </summary>
    IReadOnlyList<object>? EndpointMetadata { get; }
    #endregion

    #region Methods
    /// <summary>
    /// Writes the given <paramref name="result"/> into the current response instance.
    /// </summary>
    /// <param name="result">
    /// The <see cref="Result"/> instance to be written.
    /// </param>
    Task WriteResultAsync(Result result);

    /// <summary>
    /// Flush any remaining response headers, data, or trailers.
    /// This may throw if the response is in an invalid state such as a Content-Length mismatch.
    /// </summary>
    Task CompleteAsync();
    #endregion
}