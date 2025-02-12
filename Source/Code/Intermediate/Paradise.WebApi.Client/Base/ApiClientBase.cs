using Microsoft.Extensions.Options;
using Paradise.Models;
using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static Paradise.Localization.ExceptionsHandling.ExceptionMessagesProvider;
using static System.Text.Json.JsonSerializer;

namespace Paradise.WebApi.Client.Base;

/// <summary>
/// Base API client class.
/// </summary>
public abstract class ApiClientBase : IDisposable
{
    #region Fields
    private bool _disposed;

    private readonly HttpClient _httpClient;
    private JsonSerializerOptions _jsonSerializerOptions;

    private readonly IDisposable? _jsonSerializerOptionsReloadToken;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClientBase"/> class.
    /// </summary>
    /// <param name="jsonSerializerOptions">
    /// The accessor used to access the <see cref="JsonSerializerOptions"/>.
    /// </param>
    /// <param name="httpClient">
    /// <see cref="HttpClient"/> instance the <see cref="ApiClientBase"/>
    /// will operate over.
    /// </param>
    protected ApiClientBase(IOptionsMonitor<JsonSerializerOptions> jsonSerializerOptions, HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(jsonSerializerOptions);

        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions.CurrentValue;
        _jsonSerializerOptionsReloadToken = jsonSerializerOptions.OnChange(UpdateJsonSerializerSettings);
    }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }
    #endregion

    #region Protected methods
    /// <summary>
    /// <inheritdoc cref="Dispose()"/>
    /// </summary>
    /// <param name="disposing">
    /// Indicates whether the managed resources are disposing as well.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
            _jsonSerializerOptionsReloadToken?.Dispose();

        _disposed = true;
    }
    #endregion

    #region Private protected methods
    /// <summary>
    /// Executes GET request.
    /// </summary>
    /// <typeparam name="TValue">
    /// The <see cref="Result{TValue}.Value"/> type.
    /// </typeparam>
    /// <param name="route">
    /// Request route.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="Result{TValue}"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result<TValue>> GetAsync<TValue>(string route, CancellationToken cancellationToken = default)
        => ExecuteRequestAsync<TValue>(HttpMethod.Get, route, cancellationToken: cancellationToken);

    /// <summary>
    /// Executes GET request.
    /// </summary>
    /// <param name="route">
    /// Request route.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="Result"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result> GetAsync(string route, CancellationToken cancellationToken = default)
        => ExecuteRequestAsync(HttpMethod.Get, route, cancellationToken: cancellationToken);

    /// <summary>
    /// Executes POST request.
    /// </summary>
    /// <typeparam name="TValue">
    /// The <see cref="Result{TValue}.Value"/> type.
    /// </typeparam>
    /// <param name="route">
    /// Request route.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="Result{TValue}"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result<TValue>> PostAsync<TValue>(string route, object content, CancellationToken cancellationToken = default)
        => ExecuteRequestAsync<TValue>(HttpMethod.Post, route, content, cancellationToken);

    /// <summary>
    /// Executes POST request.
    /// </summary>
    /// <param name="route">
    /// Request route.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="Result"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result> PostAsync(string route, object content, CancellationToken cancellationToken = default)
        => ExecuteRequestAsync(HttpMethod.Post, route, content, cancellationToken);

    /// <summary>
    /// Executes PUT request.
    /// </summary>
    /// <typeparam name="TValue">
    /// The <see cref="Result{TValue}.Value"/> type.
    /// </typeparam>
    /// <param name="route">
    /// Request route.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="Result{TValue}"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result<TValue>> PutAsync<TValue>(string route, object content, CancellationToken cancellationToken = default)
        => ExecuteRequestAsync<TValue>(HttpMethod.Put, route, content, cancellationToken);

    /// <summary>
    /// Executes PUT request.
    /// </summary>
    /// <param name="route">
    /// Request route.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="Result"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result> PutAsync(string route, object content, CancellationToken cancellationToken = default)
        => ExecuteRequestAsync(HttpMethod.Put, route, content, cancellationToken);

    /// <summary>
    /// Executes PATCH request.
    /// </summary>
    /// <typeparam name="TValue">
    /// The <see cref="Result{TValue}.Value"/> type.
    /// </typeparam>
    /// <param name="route">
    /// Request route.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="Result{TValue}"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result<TValue>> PatchAsync<TValue>(string route, object? content = null, CancellationToken cancellationToken = default)
        => ExecuteRequestAsync<TValue>(HttpMethod.Patch, route, content, cancellationToken);

    /// <summary>
    /// Executes PATCH request.
    /// </summary>
    /// <param name="route">
    /// Request route.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="Result"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result> PatchAsync(string route, object? content = null, CancellationToken cancellationToken = default)
        => ExecuteRequestAsync(HttpMethod.Patch, route, content, cancellationToken);

    /// <summary>
    /// Executes DELETE request.
    /// </summary>
    /// <typeparam name="TValue">
    /// The <see cref="Result{TValue}.Value"/> type.
    /// </typeparam>
    /// <param name="route">
    /// Request route.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="Result{TValue}"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result<TValue>> DeleteAsync<TValue>(string route, CancellationToken cancellationToken = default)
        => ExecuteRequestAsync<TValue>(HttpMethod.Delete, route, cancellationToken: cancellationToken);

    /// <summary>
    /// Executes DELETE request.
    /// </summary>
    /// <param name="route">
    /// Request route.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="Result"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result> DeleteAsync(string route, CancellationToken cancellationToken = default)
        => ExecuteRequestAsync(HttpMethod.Delete, route, cancellationToken: cancellationToken);

    /// <summary>
    /// Creates a route based on the given <paramref name="routeTemplate"/>,
    /// <paramref name="routeParameters"/> and <paramref name="queryParameters"/>.
    /// </summary>
    /// <param name="routeTemplate">
    /// API action route template.
    /// </param>
    /// <param name="queryParameters">
    /// API action query parameters.
    /// </param>
    /// <param name="routeParameters">
    /// API action route parameters.
    /// </param>
    /// <returns>
    /// A route based on the given <paramref name="routeTemplate"/>,
    /// <paramref name="routeParameters"/> and <paramref name="queryParameters"/>.
    /// </returns>
    private protected static string CreateRoute(string routeTemplate,
                                                Dictionary<string, object?>? queryParameters = null,
                                                Dictionary<string, object?>? routeParameters = null)
    {
        static string Selector(KeyValuePair<string, object?> parameter)
            => $"{parameter.Key}={parameter.Value}";

        var routeBuilder = new StringBuilder(routeTemplate);

        if (routeParameters is not null)
        {
            foreach (var parameter in routeParameters)
            {
                if (parameter.Value is null)
                    continue;

                routeBuilder.Replace($"{{{parameter.Key}}}", parameter.Value.ToString());
            }
        }

        if (queryParameters is not null)
        {
            var notNullParameters = queryParameters
                .Where(parameter => parameter.Value is not null)
                .Select(Selector);

            if (notNullParameters.Any())
                routeBuilder.Append(CultureInfo.InvariantCulture, $"?{string.Join('&', notNullParameters)}");
        }

        return routeBuilder.ToString();
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Executes the HTTP request and returns the response content
    /// deserialized into <see cref="Result{TValue}"/>.
    /// </summary>
    /// <typeparam name="TValue">
    /// The <see cref="Result{TValue}.Value"/> type.
    /// </typeparam>
    /// <param name="method">
    /// Request method.
    /// </param>
    /// <param name="route">
    /// Request route.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A deserialized <see cref="Result{TValue}"/> from the HTTP request response.
    /// </returns>
    private async Task<Result<TValue>> ExecuteRequestAsync<TValue>(HttpMethod method, string route, object? content = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(method, route, content);

        using var response = await _httpClient
            .SendAsync(request, cancellationToken)
            .ConfigureAwait(false);

        var result = await ParseResultAsync<TValue>(response, cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// Executes the HTTP request and returns the response content
    /// deserialized into <see cref="Result"/>.
    /// </summary>
    /// <param name="method">
    /// Request method.
    /// </param>
    /// <param name="route">
    /// Request route.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A deserialized <see cref="Result"/> from the HTTP request response.
    /// </returns>
    private async Task<Result> ExecuteRequestAsync(HttpMethod method, string route, object? content = null, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(method, route, content);

        using var response = await _httpClient
            .SendAsync(request, cancellationToken)
            .ConfigureAwait(false);

        var result = await ParseResultAsync(response, cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// Updates the current <see cref="JsonSerializerOptions"/> with the given <paramref name="options"/>.
    /// </summary>
    /// <param name="options">
    /// The <see cref="JsonSerializerOptions"/> instance.
    /// </param>
    private void UpdateJsonSerializerSettings(JsonSerializerOptions options)
        => _jsonSerializerOptions = options;

    /// <summary>
    /// Creates the <see cref="HttpRequestMessage"/>.
    /// </summary>
    /// <param name="method">
    /// Request method.
    /// </param>
    /// <param name="route">
    /// Request route.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <returns>
    /// The <see cref="HttpRequestMessage"/>.
    /// </returns>
    private HttpRequestMessage CreateRequest(HttpMethod method, string route, object? content = null)
    {
        var request = new HttpRequestMessage(method, route);

        if (content is HttpContent httpContent)
            request.Content = httpContent;
        else if (content is not null)
            request.Content = JsonContent.Create(content, new(Result.ContentType), _jsonSerializerOptions);

        return request;
    }

    /// <summary>
    /// Deserializes the content of the given HTTP <paramref name="response"/>
    /// into the <see cref="Result{TValue}"/> object.
    /// </summary>
    /// <typeparam name="TValue">
    /// The <see cref="Result{TValue}.Value"/> type.
    /// </typeparam>
    /// <param name="response">
    /// HTTP response.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A deserialized <see cref="Result{TValue}"/> from the given <paramref name="response"/>.
    /// </returns>
    /// <exception cref="JsonException">
    /// Occurs when deserialization fails, which means that the response is badly formatted.
    /// </exception>
    private async Task<Result<TValue>> ParseResultAsync<TValue>(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        var content = await response
            .Content
            .ReadAsStreamAsync(cancellationToken)
            .ConfigureAwait(false);

        if (content.Length is 0)
        {
            var errorResult = new Result<TValue>();
            errorResult.AddError(response.StatusCode, ErrorCode.DefaultError);
            return errorResult;
        }

        var result = await DeserializeAsync<Result<TValue>>(content, _jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);

        if (result is null)
        {
            var message = GetFailedToDeserializeMessage(typeof(Result<TValue>));

            throw new JsonException(message);
        }

        return result;
    }

    /// <summary>
    /// Deserializes the content of the given HTTP <paramref name="response"/>
    /// into the <see cref="Result"/> object.
    /// </summary>
    /// <param name="response">
    /// HTTP response.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A deserialized <see cref="Result"/> from the given <paramref name="response"/>.
    /// </returns>
    /// <exception cref="JsonException">
    /// Occurs when deserialization fails, which means that the response is badly formatted.
    /// </exception>
    private async Task<Result> ParseResultAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        var content = await response
            .Content
            .ReadAsStreamAsync(cancellationToken)
            .ConfigureAwait(false);

        if (content.Length is 0)
        {
            var errorResult = new Result();
            errorResult.AddError(response.StatusCode, ErrorCode.DefaultError);
            return errorResult;
        }

        var result = await DeserializeAsync<Result>(content, _jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);

        if (result is null)
        {
            var message = GetFailedToDeserializeMessage(typeof(Result));

            throw new JsonException(message);
        }

        return result;
    }
    #endregion
}