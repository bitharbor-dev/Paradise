using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Paradise.Tests.Doubles.Fakes.Microsoft.Extensions.Caching.Distributed;
using Paradise.Tests.Extensibility.Web.Hosting.Configuration.Base;

namespace Paradise.Tests.Extensibility.Web.Hosting.Configuration;

/// <summary>
/// Overrides application caching services to use in-memory alternatives.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CachingConfiguration"/> class.
/// </remarks>
/// <param name="timeProvider">
/// Time provider used for expiration checks.
/// </param>
public sealed class CachingConfiguration(TimeProvider timeProvider) : IWebApplicationServicesConfiguration
{
    #region Public methods
    /// <inheritdoc/>
    public void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        services
            .RemoveAll<IDistributedCache>()
            .AddSingleton<IDistributedCache>(new FakeDistributedCache(timeProvider));
    }
    #endregion
}