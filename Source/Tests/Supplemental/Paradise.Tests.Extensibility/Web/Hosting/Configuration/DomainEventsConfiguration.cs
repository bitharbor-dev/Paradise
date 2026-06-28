using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Paradise.Domain.Base.Events;
using Paradise.Tests.Doubles.Spies.Core.Domain.Base.Events;
using Paradise.Tests.Extensibility.Web.Hosting.Configuration.Base;

namespace Paradise.Tests.Extensibility.Web.Hosting.Configuration;

/// <summary>
/// Overrides domain events processing services to use in-memory alternatives.
/// </summary>
public sealed class DomainEventsConfiguration : IWebApplicationServicesConfiguration
{
    #region Fields
    private SpyDomainEventDispatcher? _eventDispatcher;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        services
            .RemoveAll<IDomainEventDispatcher>()
            .AddSingleton<IDomainEventDispatcher>(provider =>
            {
                if (_eventDispatcher is null)
                {
                    _eventDispatcher = new(provider.GetRequiredService<IDomainEventSource>());

                    _eventDispatcher.DomainEventPulled += OnDomainEventPulled;
                }

                return _eventDispatcher;
            });
    }
    #endregion

    #region Private methods
    /// <summary>
    /// <see cref="SpyDomainEventDispatcher.DomainEventPulled"/> event handler.
    /// </summary>
    /// <remarks>
    /// Event forwarding via <see langword="add"/> and <see langword="remove"/> are not
    /// available since subscriptions may occur when the internal
    /// <see cref="_eventDispatcher"/> is still equal to <see langword="null"/>.
    /// </remarks>
    /// <param name="sender">
    /// The sender of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="DomainEventPulledEventArgs"/> instance containing the event data.
    /// </param>
    private void OnDomainEventPulled(object? sender, DomainEventPulledEventArgs e)
        => DomainEventPulled?.Invoke(sender, e);
    #endregion

    #region Events
    /// <inheritdoc cref="SpyDomainEventDispatcher.DomainEventPulled"/>
    public event EventHandler<DomainEventPulledEventArgs>? DomainEventPulled;
    #endregion
}