using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace Paradise.WebApi.Startup.Steps.PostBuild;

/// <summary>
/// Configures the HTTP request processing pipeline.
/// </summary>
internal sealed class PipelineBootstrap : IPostBuildStep
{
    #region Public methods
    /// <inheritdoc/>
    public ValueTask ExecuteAsync(PostBuildContext context)
    {
        var app = context.App;

        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseExceptionHandler();

        var requestLocalizationOptions = app
            .Services
            .GetRequiredService<IOptions<RequestLocalizationOptions>>();

        app.UseMiddleware<RequestLocalizationMiddleware>(requestLocalizationOptions);

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        return ValueTask.CompletedTask;
    }
    #endregion
}