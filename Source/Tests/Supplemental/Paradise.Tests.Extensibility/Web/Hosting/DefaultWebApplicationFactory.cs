using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Paradise.Tests.Extensibility.Web.Hosting.Configuration.Base;

namespace Paradise.Tests.Extensibility.Web.Hosting;

/// <summary>
/// Default <see cref="WebApplicationFactory{TEntryPoint}"/> implementation which
/// provides standardized way configure in-memory application instance.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DefaultWebApplicationFactory"/> class.
/// </remarks>
/// <param name="configurations">
/// The configurations used to modify application configuration.
/// </param>
public sealed class DefaultWebApplicationFactory(params IEnumerable<IWebApplicationServicesConfiguration> configurations)
    : WebApplicationFactory<Program>
{
    #region Protected methods
    /// <inheritdoc/>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.ConfigureWebHost(builder);

        builder.ConfigureServices((context, services) =>
        {
            foreach (var configuration in configurations)
                configuration.ConfigureServices(context, services);
        });
    }
    #endregion
}