namespace Paradise.WebApi.Startup;

/// <summary>
/// Represents a bootstrap step executed after <see cref="WebApplicationBuilder.Build"/>.
/// </summary>
internal interface IPostBuildStep
{
    #region Methods
    /// <summary>
    /// Executes the step using the specified <paramref name="context"/>.
    /// </summary>
    /// <param name="context">
    /// The <see cref="PostBuildContext"/> containing shared state required for execution.
    /// </param>
    void Execute(PostBuildContext context);
    #endregion
}