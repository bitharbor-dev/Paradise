using Microsoft.AspNetCore.Mvc.Filters;
using Paradise.ApplicationLogic.Exceptions;
using Paradise.ApplicationLogic.Extensions;
using Paradise.Models;

namespace Paradise.WebApi.Filters.ExceptionHandling;

/// <summary>
/// Catches any unhandled exceptions.
/// </summary>
internal sealed class ExceptionFilter : IActionFilter, IOrderedFilter
{
    #region Fields
    private static ExceptionFilter? _instance;
    #endregion

    #region Properties
    /// <summary>
    /// Exception filter singleton instance.
    /// </summary>
    public static ExceptionFilter Instance
        => _instance ??= new ExceptionFilter();

    /// <inheritdoc/>
    public int Order { get; } = int.MaxValue - 10;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void OnActionExecuting(ActionExecutingContext context) { }

    /// <inheritdoc/>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is not null)
        {
            Result? result;

            if (context.Exception is ResultException resultException)
            {
                result = resultException.GetResult();
            }
            else
            {
                result = new Result();
                result.AddException(context.Exception);
            }

            var logger = context
                .HttpContext
                .RequestServices
                .GetRequiredService<ILogger<ExceptionFilter>>();

            logger.LogResultCriticalErrors(result);

            context.Result = result;
            context.ExceptionHandled = true;
        }
    }
    #endregion
}