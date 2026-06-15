using Microsoft.AspNetCore.Http;
using Paradise.Models;

namespace Paradise.WebApi.Infrastructure.Tests.Unit;

/// <summary>
/// <see cref="ApplicationProblemDetails"/> test class.
/// </summary>
public sealed class ApplicationProblemDetailsTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="ApplicationProblemDetails(HttpValidationProblemDetails)"/> constructor should
    /// populate the instance with the data from the input <see cref="HttpValidationProblemDetails"/>
    /// and convert <see cref="HttpValidationProblemDetails.Errors"/> into
    /// the <see cref="ApplicationProblemDetails.Errors"/>.
    /// </summary>
    [Fact]
    public void Constructor()
    {
        // Arrange
        var error1 = "error 1";
        var error2 = "error 2";

        var validationProblem = new HttpValidationProblemDetails
        {
            Detail = "Detail",
            Errors = new Dictionary<string, string[]>
            {
                ["error"] = [error1, error2]
            },
            Extensions = new Dictionary<string, object?>
            {
                ["extension"] = "Extension"
            },
            Instance = "Instance",
            Status = 400,
            Title = "Title",
            Type = "Type"
        };

        // Act
        var applicationProblem = new ApplicationProblemDetails(validationProblem);

        // Assert
        Assert.Equal(validationProblem.Detail, applicationProblem.Detail);
        Assert.Equivalent(validationProblem.Extensions, applicationProblem.Extensions);
        Assert.Equal(validationProblem.Instance, applicationProblem.Instance);
        Assert.Equal(validationProblem.Status, applicationProblem.Status);
        Assert.Equal(validationProblem.Title, applicationProblem.Title);
        Assert.Equal(validationProblem.Type, applicationProblem.Type);
        Assert.Collection(applicationProblem.Errors,
            error =>
            {
                Assert.Equal(ErrorCode.InvalidModel, error.Code);
                Assert.Equal(error1, error.Description);
            },
            error =>
            {
                Assert.Equal(ErrorCode.InvalidModel, error.Code);
                Assert.Equal(error2, error.Description);
            });

    }

    /// <summary>
    /// The <see cref="ApplicationProblemDetails(HttpValidationProblemDetails)"/> constructor should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="HttpValidationProblemDetails"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsOnNull()
    {
        // Arrange
        var validationProblem = null as HttpValidationProblemDetails;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => new ApplicationProblemDetails(validationProblem!));
    }
    #endregion
}