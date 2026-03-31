using Paradise.Models;
using System.Net;

namespace Paradise.WebApi.Infrastructure.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="OperationStatus"/> <see langword="enum"/>.
/// </summary>
internal static class OperationStatusExtensions
{
    #region Fields
    private static readonly Dictionary<OperationStatus, HttpStatusCode> _statusMap = new()
    {
        [OperationStatus.Success] = HttpStatusCode.OK,
        [OperationStatus.Failure] = HttpStatusCode.InternalServerError,
        [OperationStatus.InvalidInput] = HttpStatusCode.BadRequest,
        [OperationStatus.Created] = HttpStatusCode.Created,
        [OperationStatus.Missing] = HttpStatusCode.NotFound,
        [OperationStatus.Received] = HttpStatusCode.Accepted,
        [OperationStatus.Blocked] = HttpStatusCode.UnprocessableEntity,
        [OperationStatus.Prohibited] = HttpStatusCode.Forbidden,
        [OperationStatus.Unauthorized] = HttpStatusCode.Unauthorized
    };
    #endregion

    #region Public methods
    /// <summary>
    /// Gets the corresponding <see cref="HttpStatusCode"/> for the given <paramref name="status"/>.
    /// </summary>
    /// <param name="status">
    /// The <see cref="OperationStatus"/> to convert.
    /// </param>
    /// <returns>
    /// The corresponding <see cref="HttpStatusCode"/>.
    /// </returns>
    public static HttpStatusCode GetStatusCode(this OperationStatus status)
        => _statusMap[status];
    #endregion
}