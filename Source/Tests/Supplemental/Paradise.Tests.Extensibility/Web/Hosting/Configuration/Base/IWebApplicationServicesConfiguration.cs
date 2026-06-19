using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Paradise.Tests.Extensibility.Web.Hosting.Configuration.Base;

/// <summary>
/// Defines a service configuration that can modify an application's
/// dependency injection container during test host initialization.
/// </summary>
public interface IWebApplicationServicesConfiguration
{
    #region Methods
    /// <summary>
    /// Configures services in the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="context">
    /// The <see cref="WebHostBuilderContext"/>.
    /// </param>
    /// <param name="services">
    /// The service collection to configure.
    /// </param>
    void ConfigureServices(WebHostBuilderContext context, IServiceCollection services);
    #endregion
}