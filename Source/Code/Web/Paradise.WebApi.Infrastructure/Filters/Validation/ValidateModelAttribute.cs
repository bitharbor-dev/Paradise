using Microsoft.AspNetCore.Mvc.Filters;
using Paradise.Models;
using Paradise.WebApi.Infrastructure.Extensions;

namespace Paradise.WebApi.Infrastructure.Filters.Validation;

/// <summary>
/// Validates the model's state.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class ValidateModelAttribute : ActionFilterAttribute
{
    #region Public methods
    /// <inheritdoc/>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!context.ModelState.IsValid)
        {
            var result = new Result(OperationStatus.InvalidInput);

            foreach (var entry in context.ModelState)
            {
                var errors = entry
                    .Value
                    .Errors
                    .Select(error => new ApplicationError(ErrorCode.InvalidModel, error.ErrorMessage));

                result.AddErrors(errors);
            }

            context.Result = result.AsActionResult();
        }
    }
    #endregion
}