using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Paradise.WebApi.Infrastructure.Filters.Validation;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Filters.Validation;

/// <summary>
/// <see cref="ValidateModelAttribute"/> test class.
/// </summary>
public sealed class ValidateModelAttributeTests
{
    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    public ValidateModelAttribute Attribute { get; } = new();
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="ValidateModelAttribute.OnActionExecuting"/> method should
    /// instantiate and assign the action result
    /// using the present model validation errors.
    /// </summary>
    [Fact]
    public void OnActionExecuting()
    {
        // Arrange
        var context = CreateContext(new()
        {
            ["Property1"] = ["Error1", "Error2"],
            ["Property2"] = ["Error3", "Error4"]
        });

        // Act
        Attribute.OnActionExecuting(context);

        // Assert
        Assert.NotNull(context.Result);
    }

    /// <summary>
    /// The <see cref="ValidateModelAttribute.OnActionExecuting"/> method should
    /// skip instantiating and assigning the action result
    /// when the model state is valid.
    /// </summary>
    [Fact]
    public void OnActionExecuting_SkipsValidModels()
    {
        // Arrange
        var context = CreateContext();

        // Act
        Attribute.OnActionExecuting(context);

        // Assert
        Assert.Null(context.Result);
    }

    /// <summary>
    /// The <see cref="ValidateModelAttribute.OnActionExecuting"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="ActionExecutingContext"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void OnActionExecuting_ThrowsOnNull()
    {
        // Arrange
        var context = null as ActionExecutingContext;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => Attribute.OnActionExecuting(context!));
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Initializes a new instance of the <see cref="ActionExecutingContext"/> class
    /// with the given <paramref name="errors"/>, if supplied.
    /// </summary>
    /// <param name="errors">
    /// The errors collection to populate the model state with.
    /// </param>
    /// <returns>
    /// A configured <see cref="ActionExecutingContext"/> instance.
    /// </returns>
    private static ActionExecutingContext CreateContext(Dictionary<string, IEnumerable<string>>? errors = null)
    {
        var actionContext = new ActionContext(new DefaultHttpContext(), new(), new());

        var context = new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), null!);

        if (errors is not null)
        {
            foreach (var entry in errors)
            {
                foreach (var error in entry.Value)
                    context.ModelState.AddModelError(entry.Key, error);
            }
        }

        return context;
    }
    #endregion
}