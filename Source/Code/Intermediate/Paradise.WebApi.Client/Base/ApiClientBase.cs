using Microsoft.Extensions.Options;
using Paradise.Common.Extensions;
using Paradise.Models;
using Paradise.Options.Models;
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

    private Uri _apiUrl;

    private readonly HttpClient _httpClient;
    private JsonSerializerOptions _jsonSerializerOptions;
    private readonly string _schemeName;

    private readonly IDisposable? _applicationOptionsReloadToken;
    private readonly IDisposable? _jsonSerializerOptionsReloadToken;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClientBase"/> class.
    /// </summary>
    /// <param name="applicationOptions">
    /// The accessor used to access the <see cref="ApplicationOptions"/>.
    /// </param>
    /// <param name="jsonSerializerOptions">
    /// The accessor used to access the <see cref="JsonSerializerOptions"/>.
    /// </param>
    /// <param name="httpClient">
    /// <see cref="HttpClient"/> instance the <see cref="ApiClientBase"/>
    /// will operate over.
    /// </param>
    /// <param name="schemeName">
    /// The authentication scheme name for this client.
    /// </param>
    protected ApiClientBase(IOptionsMonitor<ApplicationOptions> applicationOptions,
                            IOptionsMonitor<JsonSerializerOptions> jsonSerializerOptions,
                            HttpClient httpClient,
                            string schemeName)
    {
        ArgumentNullException.ThrowIfNull(applicationOptions);
        ArgumentNullException.ThrowIfNull(jsonSerializerOptions);

        _apiUrl = applicationOptions.CurrentValue.ApiUrl;

        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions.CurrentValue;
        _schemeName = schemeName;

        _applicationOptionsReloadToken = applicationOptions.OnChange(UpdateApplicationOptions);
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
        {
            _applicationOptionsReloadToken?.Dispose();
            _jsonSerializerOptionsReloadToken?.Dispose();
        }

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
    /// <param name="uri">
    /// Request <see cref="Uri"/>.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// The <see cref="Result{TValue}"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result<TValue>> GetAsync<TValue>(Uri uri, string? accessToken = null)
        => ExecuteRequestAsync<TValue>(HttpMethod.Get, uri, accessToken);

    /// <summary>
    /// Executes GET request.
    /// </summary>
    /// <param name="uri">
    /// Request <see cref="Uri"/>.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// The <see cref="Result"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result> GetAsync(Uri uri, string? accessToken = null)
        => ExecuteRequestAsync(HttpMethod.Get, uri, accessToken);

    /// <summary>
    /// Executes POST request.
    /// </summary>
    /// <typeparam name="TValue">
    /// The <see cref="Result{TValue}.Value"/> type.
    /// </typeparam>
    /// <param name="uri">
    /// Request <see cref="Uri"/>.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// The <see cref="Result{TValue}"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result<TValue>> PostAsync<TValue>(Uri uri, object content, string? accessToken = null)
        => ExecuteRequestAsync<TValue>(HttpMethod.Post, uri, accessToken, content);

    /// <summary>
    /// Executes POST request.
    /// </summary>
    /// <param name="uri">
    /// Request <see cref="Uri"/>.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// The <see cref="Result"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result> PostAsync(Uri uri, object content, string? accessToken = null)
        => ExecuteRequestAsync(HttpMethod.Post, uri, accessToken, content);

    /// <summary>
    /// Executes PUT request.
    /// </summary>
    /// <typeparam name="TValue">
    /// The <see cref="Result{TValue}.Value"/> type.
    /// </typeparam>
    /// <param name="uri">
    /// Request <see cref="Uri"/>.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// The <see cref="Result{TValue}"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result<TValue>> PutAsync<TValue>(Uri uri, object content, string? accessToken = null)
        => ExecuteRequestAsync<TValue>(HttpMethod.Put, uri, accessToken, content);

    /// <summary>
    /// Executes PUT request.
    /// </summary>
    /// <param name="uri">
    /// Request <see cref="Uri"/>.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// The <see cref="Result"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result> PutAsync(Uri uri, object content, string? accessToken = null)
        => ExecuteRequestAsync(HttpMethod.Put, uri, accessToken, content);

    /// <summary>
    /// Executes PATCH request.
    /// </summary>
    /// <typeparam name="TValue">
    /// The <see cref="Result{TValue}.Value"/> type.
    /// </typeparam>
    /// <param name="uri">
    /// Request <see cref="Uri"/>.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// The <see cref="Result{TValue}"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result<TValue>> PatchAsync<TValue>(Uri uri, object? content = null, string? accessToken = null)
        => ExecuteRequestAsync<TValue>(HttpMethod.Patch, uri, accessToken, content);

    /// <summary>
    /// Executes PATCH request.
    /// </summary>
    /// <param name="uri">
    /// Request <see cref="Uri"/>.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// The <see cref="Result"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result> PatchAsync(Uri uri, object? content = null, string? accessToken = null)
        => ExecuteRequestAsync(HttpMethod.Patch, uri, accessToken, content);

    /// <summary>
    /// Executes DELETE request.
    /// </summary>
    /// <typeparam name="TValue">
    /// The <see cref="Result{TValue}.Value"/> type.
    /// </typeparam>
    /// <param name="uri">
    /// Request <see cref="Uri"/>.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// The <see cref="Result{TValue}"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result<TValue>> DeleteAsync<TValue>(Uri uri, string? accessToken = null)
        => ExecuteRequestAsync<TValue>(HttpMethod.Delete, uri, accessToken);

    /// <summary>
    /// Executes DELETE request.
    /// </summary>
    /// <param name="uri">
    /// Request <see cref="Uri"/>.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// The <see cref="Result"/> containing the deserialized response.
    /// </returns>
    private protected Task<Result> DeleteAsync(Uri uri, string? accessToken = null)
        => ExecuteRequestAsync(HttpMethod.Delete, uri, accessToken);

    /// <summary>
    /// Creates a <see cref="Uri"/> based on the <see cref="ApplicationOptions.ApiUrl"/>,
    /// <paramref name="route"/>? <paramref name="routeParameters"/> and <paramref name="queryParameters"/>.
    /// </summary>
    /// <param name="route">
    /// API action route.
    /// </param>
    /// <param name="queryParameters">
    /// API action query parameters.
    /// </param>
    /// <param name="routeParameters">
    /// API action route parameters.
    /// </param>
    /// <returns>
    /// The <see cref="Uri"/> created from the <see cref="ApplicationOptions.ApiUrl"/>,
    /// <paramref name="route"/>? <paramref name="routeParameters"/> and <paramref name="queryParameters"/>.
    /// </returns>
    private protected Uri CreateUri(string route, Dictionary<string, object?>? queryParameters = null, Dictionary<string, object?>? routeParameters = null)
    {
        static string Selector(KeyValuePair<string, object?> parameter)
            => $"{parameter.Key}={parameter.Value}";

        if (routeParameters is not null)
        {
            foreach (var parameter in routeParameters)
            {
                if (parameter.Value is null)
                    continue;

                var placeholder = $"{{{parameter.Key}}}";
                var replacement = parameter.Value.ToString();

                route = route.Replace(placeholder, replacement, StringComparison.OrdinalIgnoreCase);
            }
        }

        if (queryParameters is not null)
        {
            var notNullParameters = queryParameters
                .Where(parameter => parameter.Value is not null)
                .Select(Selector);

            if (notNullParameters.Any())
                route = $"{route}?{string.Join('&', notNullParameters)}";
        }

        return new(_apiUrl, route);
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
    /// <param name="uri">
    /// Request <see cref="Uri"/>.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <returns>
    /// A deserialized <see cref="Result{TValue}"/> from the HTTP request response.
    /// </returns>
    private async Task<Result<TValue>> ExecuteRequestAsync<TValue>(HttpMethod method, Uri uri, string? accessToken = null, object? content = null)
    {
        using var request = CreateRequest(method, uri, accessToken, content);
        using var response = await _httpClient.SendAsync(request);

        var result = await ParseResultAsync<TValue>(response);

        return result;
    }

    /// <summary>
    /// Executes the HTTP request and returns the response content
    /// deserialized into <see cref="Result"/>.
    /// </summary>
    /// <param name="method">
    /// Request method.
    /// </param>
    /// <param name="uri">
    /// Request <see cref="Uri"/>.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <returns>
    /// A deserialized <see cref="Result"/> from the HTTP request response.
    /// </returns>
    private async Task<Result> ExecuteRequestAsync(HttpMethod method, Uri uri, string? accessToken = null, object? content = null)
    {
        using var request = CreateRequest(method, uri, accessToken, content);
        using var response = await _httpClient.SendAsync(request);

        var result = await ParesResultAsync(response);

        return result;
    }

    /// <summary>
    /// Updates the current <see cref="ApplicationOptions"/> with the given <paramref name="options"/>.
    /// </summary>
    /// <param name="options">
    /// The <see cref="ApplicationOptions"/> instance.
    /// </param>
    private void UpdateApplicationOptions(ApplicationOptions options)
        => _apiUrl = options.ApiUrl;

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
    /// <param name="requestUri">
    /// Request <see cref="Uri"/>.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <param name="content">
    /// Request content.
    /// </param>
    /// <returns>
    /// The <see cref="HttpRequestMessage"/>.
    /// </returns>
    private HttpRequestMessage CreateRequest(HttpMethod method, Uri requestUri, string? accessToken = null, object? content = null)
    {
        var request = new HttpRequestMessage(method, requestUri);

        if (accessToken.IsNotNullOrWhiteSpace())
            request.Headers.Authorization = new(_schemeName, accessToken);

        if (content is not null)
            request.Content = new StringContent(Serialize(content, _jsonSerializerOptions), null, Result.ContentType);

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
    /// <returns>
    /// A deserialized <see cref="Result{TValue}"/> from the given <paramref name="response"/>.
    /// </returns>
    /// <exception cref="JsonException">
    /// Occurs when deserialization fails, which means that the response is badly formatted.
    /// </exception>
    private async Task<Result<TValue>> ParseResultAsync<TValue>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStreamAsync();

        if (content.Length is 0)
        {
            var errorResult = new Result<TValue>();
            errorResult.AddError(response.StatusCode, ErrorCode.DefaultError);
            return errorResult;
        }

        var result = await DeserializeAsync<Result<TValue>>(content, _jsonSerializerOptions);

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
    /// <returns>
    /// A deserialized <see cref="Result"/> from the given <paramref name="response"/>.
    /// </returns>
    /// <exception cref="JsonException">
    /// Occurs when deserialization fails, which means that the response is badly formatted.
    /// </exception>
    private async Task<Result> ParesResultAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStreamAsync();

        if (content.Length is 0)
        {
            var errorResult = new Result();
            errorResult.AddError(response.StatusCode, ErrorCode.DefaultError);
            return errorResult;
        }

        var result = await DeserializeAsync<Result>(content, _jsonSerializerOptions);

        if (result is null)
        {
            var message = GetFailedToDeserializeMessage(typeof(Result));

            throw new JsonException(message);
        }

        return result;
    }
    #endregion
}