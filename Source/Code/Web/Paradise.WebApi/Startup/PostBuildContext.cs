namespace Paradise.WebApi.Startup;

/// <summary>
/// Represents shared state used during post-build bootstrapping.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PostBuildContext"/> class.
/// </remarks>
/// <param name="app">
/// The <see cref="WebApplication"/> representing
/// the built application instance.
/// </param>
internal sealed class PostBuildContext(WebApplication app)
{
    #region Properties
    /// <summary>
    /// The <see cref="WebApplication"/> representing
    /// the built application instance.
    /// </summary>
    public WebApplication App { get; } = app;
    #endregion
}