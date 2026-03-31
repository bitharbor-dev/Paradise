using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Paradise.ApplicationLogic.Extensions;
using Paradise.Models;
using Paradise.WebApi.Infrastructure.Extensions;

namespace Paradise.WebApi.Infrastructure.Filters.ExceptionHandling;

/// <summary>
/// Catches any unhandled exceptions.
/// </summary>
public sealed class ExceptionFilter : IExceptionFilter, IOrderedFilter
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionFilter"/> class.
    /// </summary>
    private ExceptionFilter() { }
    #endregion

    #region Properties
    /// <summary>
    /// Singleton <see cref="ExceptionFilter"/> instance.
    /// </summary>
    public static ExceptionFilter Instance { get; } = new();

    /// <inheritdoc/>
    public int Order { get; } = int.MaxValue - 10;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void OnException(ExceptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var logger = context
            .HttpContext
            .RequestServices
            .GetRequiredService<ILogger<ExceptionFilter>>();

        var result = new Result(OperationStatus.Failure);
        result.AddException(context.Exception);

        logger.LogResultErrors(result.Errors, result.CapturedException, true);

        context.Result = result.AsActionResult();
        context.ExceptionHandled = true;
    }
    #endregion
}