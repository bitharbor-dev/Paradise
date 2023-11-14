using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Paradise.Models;

/// <summary>
/// A class to access HTTP response methods.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="HttpResponseWrapper"/> class.
/// </remarks>
/// <param name="response">
/// The inner <see cref="HttpResponse"/> instance.
/// </param>
public sealed class HttpResponseWrapper(HttpResponse response) : IHttpResponseWrapper
{
    #region Properties
    /// <inheritdoc/>
    public bool HasStarted
        => response.HasStarted;

    /// <inheritdoc/>
    public IHeaderDictionary RequestHeaders
        => response.HttpContext.Request.Headers;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task WriteResultAsync(Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var serviceProvider = response.HttpContext.RequestServices;

        var jsonSerializerOptions = serviceProvider.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        return result.WriteResponseContentAsync(response, jsonSerializerOptions);
    }

    /// <inheritdoc/>
    public Task CompleteAsync()
        => response.CompleteAsync();
    #endregion
}