namespace Paradise.WebApi.Startup;

/// <summary>
/// Represents a bootstrap step executed before <see cref="WebApplicationBuilder.Build"/>.
/// </summary>
internal interface IPreBuildStep
{
    #region Methods
    /// <summary>
    /// Executes the step using the specified <paramref name="context"/>.
    /// </summary>
    /// <param name="context">
    /// The <see cref="PreBuildContext"/> containing shared state required for execution.
    /// </param>
    void Execute(PreBuildContext context);
    #endregion
}