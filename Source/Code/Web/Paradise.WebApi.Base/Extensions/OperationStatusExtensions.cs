using Microsoft.AspNetCore.Http;
using Paradise.Models;

namespace Paradise.WebApi.Base.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="OperationStatus"/> <see langword="enum"/>.
/// </summary>
public static class OperationStatusExtensions
{
    #region Fields
    private static readonly Dictionary<OperationStatus, int> _statusCodeMap = new()
    {
        [OperationStatus.Success] = StatusCodes.Status200OK,
        [OperationStatus.Failure] = StatusCodes.Status500InternalServerError,
        [OperationStatus.InvalidInput] = StatusCodes.Status400BadRequest,
        [OperationStatus.Created] = StatusCodes.Status201Created,
        [OperationStatus.Missing] = StatusCodes.Status404NotFound,
        [OperationStatus.Received] = StatusCodes.Status202Accepted,
        [OperationStatus.Blocked] = StatusCodes.Status422UnprocessableEntity,
        [OperationStatus.Prohibited] = StatusCodes.Status403Forbidden,
        [OperationStatus.Unauthorized] = StatusCodes.Status401Unauthorized
    };

    private static readonly Dictionary<int, OperationStatus> _operationStatusMap = new()
    {
        [StatusCodes.Status200OK] = OperationStatus.Success,
        [StatusCodes.Status500InternalServerError] = OperationStatus.Failure,
        [StatusCodes.Status400BadRequest] = OperationStatus.InvalidInput,
        [StatusCodes.Status201Created] = OperationStatus.Created,
        [StatusCodes.Status404NotFound] = OperationStatus.Missing,
        [StatusCodes.Status202Accepted] = OperationStatus.Received,
        [StatusCodes.Status422UnprocessableEntity] = OperationStatus.Blocked,
        [StatusCodes.Status403Forbidden] = OperationStatus.Prohibited,
        [StatusCodes.Status401Unauthorized] = OperationStatus.Unauthorized
    };
    #endregion

    #region Public methods
    /// <summary>
    /// Gets the corresponding status code for the given <paramref name="status"/>.
    /// </summary>
    /// <param name="status">
    /// The <see cref="OperationStatus"/> to convert.
    /// </param>
    /// <returns>
    /// The corresponding status code.
    /// </returns>
    public static int GetStatusCode(this OperationStatus status)
        => _statusCodeMap[status];

    /// <summary>
    /// Gets the corresponding <see cref="OperationStatus"/> for the given <paramref name="statusCode"/>.
    /// </summary>
    /// <param name="statusCode">
    /// The status code to convert.
    /// </param>
    /// <returns>
    /// The corresponding <see cref="OperationStatus"/>.
    /// </returns>
    public static OperationStatus GetOperationStatus(this int statusCode)
        => _operationStatusMap[statusCode];
    #endregion
}