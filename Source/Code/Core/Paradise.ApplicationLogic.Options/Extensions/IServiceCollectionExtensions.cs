using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.Common.Extensions;

namespace Paradise.ApplicationLogic.Options.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IServiceCollection"/> <see langword="interface"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    #region Public methods
    /// <summary>
    /// Adds the <see cref="IOptions{TOptions}"/> of <typeparamref name="TOptions"/>
    /// via binding the instance to a configuration section with the name corresponding to the
    /// <typeparamref name="TOptions"/> type name.
    /// </summary>
    /// <typeparam name="TOptions">
    /// Options type being configured.
    /// </typeparam>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configuration">
    /// The <see cref="IConfiguration"/> instance containing the
    /// <typeparamref name="TOptions"/> configuration section.
    /// </param>
    /// <param name="configurationSectionPath">
    /// A semicolon ':' delimited string representing
    /// the sections path to the target options section.
    /// </param>
    /// <param name="postConfigure">
    /// Action used to configure the options.
    /// </param>
    /// <param name="validateOnStartup">
    /// Indicates whether the options would be
    /// validated on the application startup.
    /// </param>
    /// <param name="validateDataAnnotations">
    /// Indicates whether the options would be
    /// validated on the application startup.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    /// <remarks>
    /// TODO: Implement parametrized constructors usage during configuration-to-instance binding.
    /// </remarks>
    public static IServiceCollection AddOptions<TOptions>(this IServiceCollection services,
                                                          IConfiguration configuration,
                                                          string? configurationSectionPath = null,
                                                          Action<TOptions, IServiceProvider>? postConfigure = null,
                                                          bool validateOnStartup = false,
                                                          bool validateDataAnnotations = false)
        where TOptions : class
    {
        static void WithDefaultBinderOptions(BinderOptions options)
            => options.ErrorOnUnknownConfiguration = true;

        if (configurationSectionPath.IsNotNullOrWhiteSpace())
            configuration = configuration.GetRequiredSection(configurationSectionPath);

        var configurationSection = configuration.GetRequiredSection(typeof(TOptions).Name);

        var optionsBuilder = services.AddOptions<TOptions>()
            .Bind(configurationSection, WithDefaultBinderOptions);

        if (validateDataAnnotations)
            optionsBuilder.ValidateDataAnnotations();

        if (validateOnStartup)
            optionsBuilder.ValidateOnStart();

        if (postConfigure is not null)
            optionsBuilder.PostConfigure(postConfigure);

        return services;
    }

    /// <summary>
    /// Registers an action used to initialize a particular type of options.
    /// Note: These are run after all
    /// <see cref="OptionsServiceCollectionExtensions.Configure{TOptions}(IServiceCollection, Action{TOptions})"/>.
    /// </summary>
    /// <typeparam name="TOptions">
    /// Options type being configured.
    /// </typeparam>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="name">
    /// The name of the options.
    /// </param>
    /// <param name="postConfigure">
    /// Action used to configure the options.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection PostConfigure<TOptions>(this IServiceCollection services,
                                                             string name,
                                                             Action<TOptions, IServiceProvider> postConfigure)
        where TOptions : class
    {
        services.AddTransient<IPostConfigureOptions<TOptions>>(provider
            => new PostConfigureOptions<TOptions, IServiceProvider>(name, provider, postConfigure));

        return services;
    }
    #endregion
}