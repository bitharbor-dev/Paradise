using Paradise.Primitives.Extensions;
using Paradise.WebApi.Extensions;
using Scalar.AspNetCore;

namespace Paradise.WebApi.Startup.Steps.PostBuild;

/// <summary>
/// Maps application endpoints and routing configuration.
/// </summary>
internal sealed class RoutingBootstrap : IPostBuildStep
{
    #region Public methods
    /// <inheritdoc/>
    public ValueTask ExecuteAsync(PostBuildContext context)
    {
        var app = context.App;

        app.MapStaticAssets();
        app.MapRazorPages()
           .WithStaticAssets();

        app.MapEndpoints();

        app.MapOpenApi();
        app.MapScalarApiReference("/reference", app.Configuration.BindSection);

        return ValueTask.CompletedTask;
    }
    #endregion
}