namespace Paradise.WebApi.Startup;

/// <summary>
/// Orchestrates application bootstrapping by executing pre-build and post-build steps.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="ApplicationBootstrapper"/>.
/// </remarks>
/// <param name="preBuildSteps">
/// Pre-build steps to execute.
/// </param>
/// <param name="postBuildSteps">
/// Post-build steps to execute.
/// </param>
internal sealed class ApplicationBootstrapper(IEnumerable<IPreBuildStep> preBuildSteps,
                                              IEnumerable<IPostBuildStep> postBuildSteps)
{
    #region Public Methods
    /// <summary>
    /// Builds and configures the <see cref="WebApplication"/> by executing all bootstrap steps.
    /// </summary>
    /// <param name="args">
    /// The command-line arguments passed to the application.
    /// </param>
    /// <returns>
    /// The fully configured <see cref="WebApplication"/> instance.
    /// </returns>
    public async Task<WebApplication> BootstrapAsync(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var preContext = new PreBuildContext(builder);

        foreach (var step in preBuildSteps)
            await step.ExecuteAsync(preContext);

        var app = builder.Build();

        var postContext = new PostBuildContext(app);

        foreach (var step in postBuildSteps)
            await step.ExecuteAsync(postContext);

        return app;
    }
    #endregion
}