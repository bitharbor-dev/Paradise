using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.Models;
using Paradise.WebApi.Infrastructure.Extensions;
using System.Net.Mime;
using System.Text.Json;

namespace Paradise.WebApi.Infrastructure;

/// <summary>
/// The <see cref="IActionResult"/> wrapper implementation around <see cref="ResultBase"/> instances.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ApplicationActionResult"/> class.
/// </remarks>
/// <param name="result">
/// The inner <see cref="ResultBase"/> instance.
/// </param>
public sealed class ApplicationActionResult(ResultBase result) : IActionResult
{
    #region Public methods
    /// <inheritdoc/>
    public Task ExecuteResultAsync(ActionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var response = context.HttpContext.Response;

        var serviceProvider = context.HttpContext.RequestServices;
        var jsonSerializerOptions = serviceProvider.GetService<IOptions<JsonSerializerOptions>>()?.Value;

        return WriteResponseContentAsync(response, jsonSerializerOptions);
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
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public async Task WriteResponseContentAsync(HttpResponse response, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (response.HasStarted)
            return;

        response.StatusCode = (int)result.Status.GetStatusCode();

        await response.WriteAsJsonAsync(result, result.GetType(), options, MediaTypeNames.Application.Json)
            .ConfigureAwait(false);

        await response.CompleteAsync()
            .ConfigureAwait(false);
    }
    #endregion
}