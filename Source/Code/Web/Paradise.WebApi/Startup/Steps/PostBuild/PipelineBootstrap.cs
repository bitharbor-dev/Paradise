using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Paradise.WebApi.Infrastructure;

namespace Paradise.WebApi.Startup.Steps.PostBuild;

/// <summary>
/// Configures the HTTP request processing pipeline.
/// </summary>
internal sealed class PipelineBootstrap : IPostBuildStep
{
    #region Fields
    private static readonly ExceptionHandlerOptions _defaultHandlerOptions = new()
    {
        ExceptionHandler = ExceptionHandler.HandleFallbackAsync
    };
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task ExecuteAsync(PostBuildContext context)
    {
        var app = context.App;

        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseExceptionHandler(_defaultHandlerOptions);

        var requestLocalizationOptions = app
            .Services
            .GetRequiredService<IOptions<RequestLocalizationOptions>>();

        app.UseMiddleware<RequestLocalizationMiddleware>(requestLocalizationOptions);

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        return Task.CompletedTask;
    }
    #endregion
}