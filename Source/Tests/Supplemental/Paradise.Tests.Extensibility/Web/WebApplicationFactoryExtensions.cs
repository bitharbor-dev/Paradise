using Microsoft.AspNetCore.Mvc.Testing;
using Paradise.Tests.Extensibility.Web.Hosting.Configuration.Base;

namespace Paradise.Tests.Extensibility.Web;

/// <summary>
/// Contains extension methods for the <see cref="WebApplicationFactory{TEntryPoint}"/> <see langword="class"/>.
/// </summary>
public static class WebApplicationFactoryExtensions
{
    #region Public methods
    /// <summary>
    /// Creates a derived <see cref="WebApplicationFactory{TEntryPoint}"/> and applies the specified
    /// service configurations to the application's service collection.
    /// </summary>
    /// <param name="rootFactory">
    /// The root <see cref="WebApplicationFactory{TEntryPoint}"/>.
    /// </param>
    /// <param name="configurations">
    /// The service configurations that modify the application's dependency injection container.
    /// </param>
    /// <returns>
    /// A configured <see cref="WebApplicationFactory{TEntryPoint}"/>.
    /// </returns>
    public static WebApplicationFactory<TEntryPoint> WithConfigurations<TEntryPoint>(
        this WebApplicationFactory<TEntryPoint> rootFactory, params IWebApplicationServicesConfiguration[] configurations)
        where TEntryPoint : class
    {
        ArgumentNullException.ThrowIfNull(rootFactory);

        return rootFactory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            foreach (var configuration in configurations)
                configuration.ConfigureServices(services);
        }));
    }
    #endregion
}