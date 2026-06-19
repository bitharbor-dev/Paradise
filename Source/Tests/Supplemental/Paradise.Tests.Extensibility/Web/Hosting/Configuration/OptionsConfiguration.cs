using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.Options.Models;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Communication.Email;
using Paradise.Tests.Extensibility.Web.Hosting.Configuration.Base;

namespace Paradise.Tests.Extensibility.Web.Hosting.Configuration;

/// <summary>
/// Configures application options used by the test host.
/// </summary>
public sealed class OptionsConfiguration : IWebApplicationServicesConfiguration
{
    #region Properties
    /// <summary>
    /// Web API base URL.
    /// </summary>
    /// <remarks>
    /// Defaults to <see href="http://localhost"/>.
    /// </remarks>
    public Uri ApiUrl { get; set; } = new("http://localhost");
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        services.PostConfigureAll<ApplicationOptions>(options => options.ApiUrl = ApiUrl);
        services.PostConfigureAll<SmtpOptions>(options => options.LocalEmailStorage = "{ApplicationRoot}");
    }
    #endregion
}