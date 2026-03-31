namespace Paradise.WebApi.Startup;

/// <summary>
/// Represents shared state used during pre-build bootstrapping.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PreBuildContext"/> class.
/// </remarks>
/// <param name="builder">
/// The <see cref="WebApplicationBuilder"/> used to
/// configure services and application settings.
/// </param>
internal sealed class PreBuildContext(WebApplicationBuilder builder)
{
    #region Properties
    /// <summary>
    /// The <see cref="WebApplicationBuilder"/> used to
    /// configure services and application settings.
    /// </summary>
    public WebApplicationBuilder Builder { get; } = builder;
    #endregion
}