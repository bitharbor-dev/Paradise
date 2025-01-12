using Microsoft.AspNetCore.Mvc.Filters;
using Paradise.Models;

namespace Paradise.WebApi.Filters.Validation;

/// <summary>
/// Validates the model's state.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
internal sealed class ValidateModelAttribute : ActionFilterAttribute
{
    #region Public methods
    /// <inheritdoc/>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
            context.Result = Result.FromModelState(context.ModelState);
    }
    #endregion
}