using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Paradise.ApplicationLogic.Infrastructure.Communication;
using Paradise.Tests.Doubles.Spies.Core.ApplicationLogic.Infrastructure.Communication;
using Paradise.Tests.Extensibility.Web.Hosting.Configuration.Base;

namespace Paradise.Tests.Extensibility.Web.Hosting.Configuration;

/// <summary>
/// Overrides application communication services to use in-memory alternatives.
/// </summary>
public sealed class CommunicationConfiguration : IWebApplicationServicesConfiguration
{
    #region Fields
    private readonly SpyCommunicationClient _communicatorClient = new();
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        services
            .RemoveAll<ICommunicationClient>()
            .AddSingleton<ICommunicationClient>(_communicatorClient);
    }
    #endregion

    #region Events
    /// <inheritdoc cref="SpyCommunicationClient.SendEmailRequested"/>
    public event EventHandler<SendEmailRequestSubmittedEventArgs> SendEmailRequested
    {
        add => _communicatorClient.SendEmailRequested += value;
        remove => _communicatorClient.SendEmailRequested -= value;
    }
    #endregion
}