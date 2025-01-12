using Microsoft.AspNetCore.Mvc;
using Paradise.Models;
using System.Net;

namespace Paradise.WebApi.Filters.Annotation;

/// <summary>
/// Provides simple API for describing controller methods.
/// </summary>
/// <typeparam name="TValue">
/// The <see cref="Result{TValue}.Value"/> type.
/// </typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ResultResponseAttribute{T}"/> class.
/// </remarks>
/// <param name="statusCode">
/// Response status code.
/// </param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
internal sealed class ResultResponseAttribute<TValue>(HttpStatusCode statusCode) : ProducesResponseTypeAttribute(typeof(Result<TValue>), (int)statusCode)
{
    #region Properties
    /// <summary>
    /// Gets the HTTP status code of the response.
    /// </summary>
    public new HttpStatusCode StatusCode { get; } = statusCode;
    #endregion
}

/// <summary>
/// Provides simple API for describing controller methods.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ResultResponseAttribute"/> class.
/// </remarks>
/// <param name="statusCode">
/// Response status code.
/// </param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
internal sealed class ResultResponseAttribute(HttpStatusCode statusCode) : ProducesResponseTypeAttribute(typeof(Result), (int)statusCode)
{
    #region Properties
    /// <summary>
    /// Gets the HTTP status code of the response.
    /// </summary>
    public new HttpStatusCode StatusCode { get; } = statusCode;
    #endregion
}