using Azure.Monitor.OpenTelemetry.AspNetCore;
using Paradise.ApplicationLogic.Extensions;
using Paradise.Primitives;
using Paradise.Primitives.Extensions;
using Paradise.WebApi.Extensions;
using Paradise.WebApi.Infrastructure;
using Paradise.WebApi.OpenApi.DocumentTransformers;
using Paradise.WebApi.OpenApi.OperationTransformers;

namespace Paradise.WebApi.Startup.Steps.PreBuild;

/// <summary>
/// Registers services and application dependencies.
/// </summary>
internal sealed class ServiceRegistrationBootstrap : IPreBuildStep
{
    #region Constants
    private const string PagesLocalizationResourcesPath = "Resources";
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public ValueTask ExecuteAsync(PreBuildContext context)
    {
        var services = context.Builder.Services;
        var configuration = context.Builder.Configuration;

        RegisterCore(services, configuration, context.Builder.Environment);
        RegisterErrorHandling(services);
        RegisterPages(services);
        RegisterOpenApi(services, configuration);

        return ValueTask.CompletedTask;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Registers core services including domain logic, authentication, authorization,
    /// localization, and environment-specific integrations.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configuration">
    /// The <see cref="IConfiguration"/> providing configuration values for registered services.
    /// </param>
    /// <param name="environment">
    /// The <see cref="IWebHostEnvironment"/> determining environment-specific behavior.
    /// </param>
    private static void RegisterCore(IServiceCollection services,
                                     IConfiguration configuration,
                                     IWebHostEnvironment environment)
    {
        services.AddDomainEventsDispatchingService();
        services.AddAuthenticationAndAuthorization(configuration, environment.EnvironmentName);
        services.AddApplicationLogic(configuration, environment.EnvironmentName);
        services.AddRequestLocalization(configuration);

        if (EnvironmentNames.IsProduction(environment.EnvironmentName))
            services.AddOpenTelemetry().UseAzureMonitor(configuration.BindOptionalSection);
    }

    /// <summary>
    /// Registers error handling services.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    private static void RegisterErrorHandling(IServiceCollection services)
    {
        services.AddExceptionHandler<ExceptionHandler>();
        services.AddProblemDetails();
        services.AddValidation();
    }

    /// <summary>
    /// Registers Razor Pages and view localization.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    private static void RegisterPages(IServiceCollection services)
    {
        services
            .AddRazorPages()
            .AddViewLocalization(options => options.ResourcesPath = PagesLocalizationResourcesPath)
            .AddDataAnnotationsLocalization();
    }

    /// <summary>
    /// Registers OpenAPI services and configures document and operation transformations.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configuration">
    /// The <see cref="IConfiguration"/> used to initialize OpenAPI transformers.
    /// </param>
    private static void RegisterOpenApi(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer(new DocumentInfoSetter(configuration));
            options.AddDocumentTransformer(new DocumentSecuritySchemeSetter(configuration));
            options.AddDocumentTransformer(new DocumentPathsCamelCaseFormatter());

            options.AddOperationTransformer(new OperationSecuritySchemeSetter(configuration));
        });
    }
    #endregion
}