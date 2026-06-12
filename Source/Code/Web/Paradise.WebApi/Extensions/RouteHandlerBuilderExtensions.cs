using Microsoft.AspNetCore.Http.Metadata;
using Paradise.Models;
using Paradise.WebApi.Base.Extensions;
using Paradise.WebApi.Infrastructure;

namespace Paradise.WebApi.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="RouteHandlerBuilder"/> <see langword="class"/>.
/// </summary>
internal static class RouteHandlerBuilderExtensions
{
    #region Public methods
    /// <summary>
    /// Adds an <see cref="IProducesResponseTypeMetadata"/> to <see cref="EndpointBuilder.Metadata"/> for all endpoints
    /// produced by <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="RouteHandlerBuilder"/>.
    /// </param>
    /// <param name="status">
    /// The <see cref="OperationStatus"/> that represents the status code.
    /// </param>
    /// <param name="contentType">
    /// The response content type. Defaults to "application/json" if responseType is not null, otherwise defaults to null.
    /// </param>
    /// <param name="additionalContentTypes">
    /// Additional response content types the endpoint produces for the supplied status code.
    /// </param>
    /// <returns>
    /// A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.
    /// </returns>
    public static RouteHandlerBuilder Produces(
        this RouteHandlerBuilder builder, OperationStatus status, string? contentType = null, params string[] additionalContentTypes)
        => builder.ProducesInternal(status, null, contentType, additionalContentTypes);

    /// <summary>
    /// Adds an <see cref="IProducesResponseTypeMetadata"/> with a <see cref="ApplicationProblemDetails"/> type
    /// to <see cref="EndpointBuilder.Metadata"/> for all endpoints produced by <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="RouteHandlerBuilder"/>.
    /// </param>
    /// <param name="status">
    /// The <see cref="OperationStatus"/> that represents the status code.
    /// </param>
    /// <param name="contentType">
    /// The response content type. Defaults to "application/problem+json".
    /// </param>
    /// <returns>
    /// A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.
    /// </returns>
    public static RouteHandlerBuilder ProducesProblem(
        this RouteHandlerBuilder builder, OperationStatus status, string? contentType = null)
        => builder.ProducesInternal(status, typeof(ApplicationProblemDetails), contentType);

    /// <summary>
    /// Adds an <see cref="IProducesResponseTypeMetadata"/> to <see cref="EndpointBuilder.Metadata"/> for all endpoints
    /// produced by <paramref name="builder"/>.
    /// </summary>
    /// <typeparam name="TValue">
    /// The type of the response.
    /// </typeparam>
    /// <param name="builder">
    /// The <see cref="RouteHandlerBuilder"/>.
    /// </param>
    /// <param name="status">
    /// The <see cref="OperationStatus"/> that represents the status code.
    /// </param>
    /// <param name="contentType">
    /// The response content type. Defaults to "application/json".
    /// </param>
    /// <param name="additionalContentTypes">
    /// Additional response content types the endpoint produces for the supplied status code.
    /// </param>
    /// <returns>
    /// A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.
    /// </returns>
    public static RouteHandlerBuilder Produces<TValue>(
        this RouteHandlerBuilder builder, OperationStatus status, string? contentType = null, params string[] additionalContentTypes)
        => builder.ProducesInternal(status, typeof(TValue), contentType, additionalContentTypes);
    #endregion

    #region Private methods
    /// <summary>
    /// Adds an <see cref="IProducesResponseTypeMetadata"/> to <see cref="EndpointBuilder.Metadata"/> for all endpoints
    /// produced by <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="RouteHandlerBuilder"/>.
    /// </param>
    /// <param name="status">
    /// The <see cref="OperationStatus"/> that represents the status code.
    /// </param>
    /// <param name="responseType">
    /// The type of the response. Defaults to null.
    /// </param>
    /// <param name="contentType">
    /// The response content type. Defaults to "application/json" if responseType is not null, otherwise defaults to null.
    /// </param>
    /// <param name="additionalContentTypes">
    /// Additional response content types the endpoint produces for the supplied status code.
    /// </param>
    /// <returns>
    /// A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.
    /// </returns>
    private static RouteHandlerBuilder ProducesInternal(this RouteHandlerBuilder builder,
                                                        OperationStatus status,
                                                        Type? responseType = null,
                                                        string? contentType = null,
                                                        params string[] additionalContentTypes)
    {
        var statusCode = status.GetStatusCode();

        return builder.Produces(statusCode, responseType, contentType, additionalContentTypes);
    }
    #endregion
}