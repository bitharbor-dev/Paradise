using Microsoft.AspNetCore.Mvc;
using Paradise.ApplicationLogic.Extensions;
using Paradise.Common;
using Paradise.WebApi.Extensions;
using Paradise.WebApi.Infrastructure.Binders.Providers;
using Paradise.WebApi.Infrastructure.Filters.ExceptionHandling;
using Paradise.WebApi.OpenApi.DocumentTransformers;
using Paradise.WebApi.OpenApi.OperationTransformers;

namespace Paradise.WebApi.Startup.Steps.PreBuild;

/// <summary>
/// Registers services and application dependencies.
/// </summary>
internal sealed class ServiceRegistrationBootstrap : IPreBuildStep
{
    #region Public methods
    /// <inheritdoc/>
    public void Execute(PreBuildContext context)
    {
        var services = context.Builder.Services;
        var configuration = context.Builder.Configuration;

        RegisterCore(services, configuration, context.Builder.Environment);
        RegisterPagesAndControllers(services);
        RegisterOpenApi(services, configuration);

        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
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
        services.AddStartupAndShutdownActivities();
        services.AddAuthenticationAndAuthorization(configuration, environment.EnvironmentName);
        services.AddAuthorizationResultHandler();
        services.AddApplicationLogic(configuration, environment.EnvironmentName);
        services.AddDomainEvents();
        services.AddRequestLocalization(configuration);

        if (EnvironmentNames.IsProduction(environment.EnvironmentName))
            services.AddApplicationInsightsTelemetry(configuration);
    }

    /// <summary>
    /// Registers Razor Pages and controller configuration.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <remarks>
    /// Configures controllers to use <see cref="ExceptionFilter"/> and prioritizes
    /// <see cref="CustomModelBinderProvider"/> within the model binder provider collection.
    /// </remarks>
    private static void RegisterPagesAndControllers(IServiceCollection services)
    {
        services
            .AddRazorPages()
            .AddViewLocalization(options => options.ResourcesPath = "Resources")
            .AddDataAnnotationsLocalization();

        services.AddControllers(options =>
        {
            options.Filters.Add(ExceptionFilter.Instance);
            options.ModelBinderProviders.Insert(0, CustomModelBinderProvider.Instance);
        });
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