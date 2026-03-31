using Microsoft.AspNetCore.Mvc;
using Paradise.Models;
using Paradise.WebApi.Infrastructure.Extensions;

namespace Paradise.WebApi.Infrastructure.Filters.Metadata;

/// <summary>
/// Provides simple API for describing controller methods responses.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class ResultResponseAttribute : ProducesResponseTypeAttribute
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultResponseAttribute"/> class.
    /// </summary>
    /// <param name="status">
    /// Operation status.
    /// </param>
    /// <param name="description">
    /// The description of the response.
    /// </param>
    public ResultResponseAttribute(OperationStatus status, string? description = null)
        : base(typeof(Result), (int)status.GetStatusCode())
    {
        Status = status;
        Description = description;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Operation status.
    /// </summary>
    public OperationStatus Status { get; }
    #endregion
}