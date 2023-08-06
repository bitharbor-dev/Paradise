using Microsoft.OpenApi.Models;
using Paradise.Common.Extensions;
using Paradise.WebApi.Swagger.DocumentFilters;
using Paradise.WebApi.Swagger.OperationFilters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using static Paradise.Localization.ExceptionsHandling.ExceptionMessages;

namespace Paradise.WebApi.Swagger;

/// <summary>
/// Provides extension methods for Swagger configuring.
/// </summary>
internal static class SwaggerConfigurator
{
    #region Constants
    /// <summary>
    /// Swagger endpoint name configuration value key.
    /// </summary>
    public const string SwaggerEndpointName = nameof(SwaggerEndpointName);

    /// <summary>
    /// Swagger endpoint URL configuration value key.
    /// </summary>
    public const string SwaggerEndpointUrl = nameof(SwaggerEndpointUrl);

    /// <summary>
    /// XML files extension.
    /// </summary>
    private const string XmlFileExtension = ".xml";
    #endregion

    #region Public methods
    /// <summary>
    /// Configures the given <paramref name="options"/> with the values
    /// contained in the <paramref name="configuration"/>.
    /// </summary>
    /// <param name="options">
    /// <see cref="SwaggerGenOptions"/> to be configured.
    /// </param>
    /// <param name="configuration">
    /// Configuration values container.
    /// </param>
    public static void Configure(this SwaggerGenOptions options, IConfiguration configuration)
    {
        var apiInfo = configuration.GetRequiredInstance<OpenApiInfo>();
        var scheme = configuration.GetRequiredInstance<OpenApiSecurityScheme>();

        options.SwaggerDoc(apiInfo.Version, apiInfo);

        options.AddSecurityDefinition(scheme.Reference.Id, scheme);
        options.DocumentFilter<EndpointPathToLowercaseDocumentFilter>();
        options.DocumentFilter<SummaryDocumentFilter>();
        options.OperationFilter<OpenApiSecuritySchemeOperationFilter>(scheme);

        options.AddXmlComments();
    }

    /// <summary>
    /// Configures the given <paramref name="options"/> with the values
    /// contained in the <paramref name="configuration"/>.
    /// </summary>
    /// <param name="options">
    /// <see cref="SwaggerUIOptions"/> to be configured.
    /// </param>
    /// <param name="configuration">
    /// Configuration values container.
    /// </param>
    public static void Configure(this SwaggerUIOptions options, IConfiguration configuration)
    {
        configuration.GetRequiredSection(nameof(SwaggerUIOptions)).Bind(options);

        var name = configuration.GetValue<string>(SwaggerEndpointName);
        var url = configuration.GetValue<string>(SwaggerEndpointUrl);

        if (url.IsNullOrWhiteSpace())
            throw new InvalidOperationException(InvalidSwaggerConfiguration);

        if (name.IsNullOrWhiteSpace())
            throw new InvalidOperationException(InvalidSwaggerConfiguration);

        options.SwaggerEndpoint(url, name);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Adds XML comments to the given <paramref name="options"/> instance.
    /// </summary>
    /// <param name="options">
    /// <see cref="SwaggerGenOptions"/> to which to add XML comments.
    /// </param>
    private static void AddXmlComments(this SwaggerGenOptions options)
    {
        var appFiles = Directory.EnumerateFiles(AppContext.BaseDirectory)
            .Select(path => new FileInfo(path));

        var xmlFiles = appFiles.Where(file => file.Extension is XmlFileExtension);

        foreach (var file in xmlFiles)
            options.IncludeXmlComments(file.FullName, true);
    }

    /// <summary>
    /// Gets the value of type <typeparamref name="T"/>
    /// from the input <paramref name="configuration"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Value type.
    /// </typeparam>
    /// <param name="configuration">
    /// The configuration in which to look for a value.
    /// </param>
    /// <returns>
    /// The value of type <typeparamref name="T"/>,
    /// retrieved from the input <paramref name="configuration"/>.
    /// </returns>
    private static T GetRequiredInstance<T>(this IConfiguration configuration)
        => configuration.GetRequiredSection(typeof(T).Name).Get<T>()
        ?? throw new InvalidOperationException(InvalidSwaggerConfiguration);
    #endregion
}