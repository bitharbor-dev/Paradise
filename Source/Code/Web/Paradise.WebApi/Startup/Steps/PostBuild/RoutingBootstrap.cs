using Paradise.Common.Extensions;
using Scalar.AspNetCore;

namespace Paradise.WebApi.Startup.Steps.PostBuild;

/// <summary>
/// Maps application endpoints and routing configuration.
/// </summary>
internal sealed class RoutingBootstrap : IPostBuildStep
{
    #region Public methods
    /// <inheritdoc/>
    public void Execute(PostBuildContext context)
    {
        var app = context.App;

        app.MapStaticAssets();
        app.MapRazorPages()
           .WithStaticAssets();
        app.MapControllers();

        app.MapOpenApi();
        app.MapScalarApiReference("/reference", app.Configuration.BindSection);
    }
    #endregion
}