using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.Options.Origins.Base;

namespace Paradise.DependencyInjection.Base;

/// <summary>
/// Base class for configuring application services.
/// </summary>
public abstract class ServiceCollectionBuilderBase
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceCollectionBuilderBase"/> class.
    /// </summary>
    /// <param name="services">
    /// The service collection the builder will operate over.
    /// </param>
    /// <param name="configurationOrigin">
    /// Configuration origin.
    /// </param>
    protected ServiceCollectionBuilderBase(IServiceCollection services, IConfigurationOrigin configurationOrigin)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configurationOrigin);

        Services = services;
        Configuration = configurationOrigin.GetConfiguration();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Application services.
    /// </summary>
    protected IServiceCollection Services { get; }

    /// <summary>
    /// Application configuration.
    /// </summary>
    protected IConfiguration Configuration { get; }
    #endregion

    #region Public methods
    /// <summary>
    /// Adds the default required service descriptors.
    /// </summary>
    public void ConfigureRequiredServices()
    {
        AddRepositories();
        AddServices();
        AddMiscellaneous();
    }
    #endregion

    #region Protected methods
    /// <summary>
    /// Adds repositories to the service collection.
    /// </summary>
    protected virtual void AddRepositories() { }

    /// <summary>
    /// Adds services to the service collection.
    /// </summary>
    protected virtual void AddServices() { }

    /// <summary>
    /// Adds miscellaneous items to the service collection.
    /// </summary>
    protected virtual void AddMiscellaneous() { }

    /// <summary>
    /// Adds the <see cref="IOptions{TOptions}"/> of <typeparamref name="TOptions"/>.
    /// </summary>
    /// <typeparam name="TOptions">
    /// Options type.
    /// </typeparam>
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
    protected void AddOptions<TOptions>(Action<TOptions>? postConfigure = null, bool validateOnStartup = false, bool validateDataAnnotations = false)
        where TOptions : class
    {
        static void WithDefaultBinderOptions(BinderOptions options)
            => options.ErrorOnUnknownConfiguration = true;

        var configurationSection = Configuration.GetRequiredSection(typeof(TOptions).Name);

        var optionsBuilder = Services.AddOptions<TOptions>()
            .Bind(configurationSection, WithDefaultBinderOptions);

        if (validateDataAnnotations)
            optionsBuilder.ValidateDataAnnotations();

        if (validateOnStartup)
            optionsBuilder.ValidateOnStart();

        if (postConfigure is not null)
            optionsBuilder.PostConfigure(postConfigure);
    }
    #endregion
}