using Paradise.Tests.Miscellaneous.TestDoubles.Spies.Web.WebApi.Startup;
using Paradise.WebApi.Startup;

namespace Paradise.WebApi.Tests.Unit.Startup;

/// <summary>
/// <see cref="ApplicationBootstrapper"/> test class.
/// </summary>
public sealed class ApplicationBootstrapperTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="ApplicationBootstrapper.BootstrapAsync"/> method should
    /// execute all pre-build steps in the order they were provided.
    /// </summary>
    [Fact]
    public async Task BootstrapAsync()
    {
        // Arrange
        var actualPreSteps = new List<SpyPreBuildStep>();
        var actualPostSteps = new List<SpyPostBuildStep>();

        var expectedPreSteps = new[]
        {
            new SpyPreBuildStep() { ExecutedCallback = actualPreSteps.Add },
            new SpyPreBuildStep() { ExecutedCallback = actualPreSteps.Add },
            new SpyPreBuildStep() { ExecutedCallback = actualPreSteps.Add }
        };

        var expectedPostSteps = new[]
        {
            new SpyPostBuildStep() { ExecutedCallback = actualPostSteps.Add },
            new SpyPostBuildStep() { ExecutedCallback = actualPostSteps.Add },
            new SpyPostBuildStep() { ExecutedCallback = actualPostSteps.Add }
        };

        var bootstrapper = new ApplicationBootstrapper(expectedPreSteps, expectedPostSteps);

        // Act
        await bootstrapper.BootstrapAsync([]);

        // Assert
        Assert.Equivalent(expectedPreSteps, actualPreSteps, true);
        Assert.Equivalent(expectedPostSteps, actualPostSteps, true);

        Assert.All(actualPreSteps, step => Assert.True(step.Executed));
        Assert.All(actualPostSteps, step => Assert.True(step.Executed));
    }
    #endregion
}