using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Paradise.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.Domain.Base.Events;
using Paradise.Tests.Doubles.Spies.Core.Domain.Base.Events;
using Paradise.Tests.Extensibility.Web.Hosting.Configuration;
using Paradise.Tests.Extensibility.Web.Hosting.Configuration.Base;

namespace Paradise.Tests.Extensibility.Web.Hosting;

/// <summary>
/// Default <see cref="WebApplicationFactory{TEntryPoint}"/> implementation which
/// provides standardized way configure in-memory application instance.
/// </summary>
public sealed class DefaultWebApplicationFactory : WebApplicationFactory<Program>
{
    #region Fields
    private bool _disposed;
    private bool _hostStarted;

    private IWebApplicationServicesConfiguration[]? _configurations;

    private SqliteConnection? _connection;
    private DomainEventsConfiguration? _domainEventsConfiguration;

    private readonly List<IDomainEvent> _receivedEvents = [];
    #endregion

    #region Properties
    /// <summary>
    /// The list of received domain events.
    /// </summary>
    /// <remarks>
    /// Populated only if no configuration overrides were specified.
    /// </remarks>
    public IReadOnlyList<IDomainEvent> ReceivedEvents
        => _receivedEvents;
    #endregion

    #region Public methods
    /// <summary>
    /// Configures the <see cref="DefaultWebApplicationFactory"/>
    /// using the specified service overrides.
    /// </summary>
    /// <remarks>
    /// Should be invoked before host is started.
    /// </remarks>
    /// <param name="configurations">
    /// The service configurations applied to the application factory.
    /// </param>
    public void SetConfigurations(params IEnumerable<IWebApplicationServicesConfiguration>? configurations)
    {
        ThrowIfDisposed();

        if (_hostStarted)
            throw new InvalidOperationException();

        _configurations = configurations?.ToArray();
    }

    /// <summary>
    /// Creates a new <see cref="Role"/> and persists it
    /// using the configured <see cref="IRoleManager{TRole}"/>
    /// bypassing (partially or completely) the business rules.
    /// </summary>
    /// <param name="name">
    /// Role name.
    /// </param>
    /// <param name="isDefault">
    /// Indicates whether the role is default and should be
    /// assigned automatically when a user has been created.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public async Task AddRoleAsync(string name, bool isDefault)
    {
        ThrowIfDisposed();

        var scope = Services.CreateAsyncScope();

        await using (scope.ConfigureAwait(false))
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<IRoleManager<Role>>();

            var role = new Role(name, isDefault);

            await roleManager.CreateAsync(role)
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creates a new <see cref="User"/> and persists it
    /// using the configured <see cref="IUserManager{TUser}"/>
    /// bypassing (partially or completely) the business rules.
    /// </summary>
    /// <param name="userName">
    /// User's user-name.
    /// </param>
    /// <param name="emailAddress">
    /// User's email address.
    /// </param>
    /// <param name="password">
    /// User's password.
    /// </param>
    /// <param name="phoneNumber">
    /// User's phone number.
    /// </param>
    /// <param name="isEmailAddressConfirmed">
    /// Indicates whether the user's email address has been confirmed.
    /// </param>
    /// <param name="twoFactorEnabled">
    /// Indicates whether two-factor authentication is enabled for the user.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public async Task AddUserAsync(string userName,
                                   string emailAddress,
                                   string password,
                                   string? phoneNumber = null,
                                   bool isEmailAddressConfirmed = true,
                                   bool twoFactorEnabled = false)
    {
        ThrowIfDisposed();

        var scope = Services.CreateAsyncScope();

        await using (scope.ConfigureAwait(false))
        {
            var userManager = scope.ServiceProvider.GetRequiredService<IUserManager<User>>();

            var user = new User(emailAddress, userName)
            {
                EmailConfirmed = isEmailAddressConfirmed,
                PhoneNumber = phoneNumber,
                TwoFactorEnabled = twoFactorEnabled
            };

            await userManager.CreateAsync(user, password)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public override async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (_connection is not null)
            {
                await _connection.DisposeAsync()
                    .ConfigureAwait(false);
            }

            _domainEventsConfiguration?.DomainEventPulled -= OnDomainEventPulled;

            _disposed = true;
        }

        await base.DisposeAsync()
            .ConfigureAwait(false);
    }
    #endregion

    #region Protected methods
    /// <inheritdoc/>
    protected override IHost CreateHost(IHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var host = base.CreateHost(builder);

        _hostStarted = true;

        return host;
    }

    /// <inheritdoc/>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.ConfigureWebHost(builder);

        var configurations = _configurations ?? CreateDefaultConfigurations();

        builder.ConfigureServices((context, services) =>
        {
            foreach (var configuration in configurations)
                configuration.ConfigureServices(context, services);
        });
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _connection?.Dispose();
                _domainEventsConfiguration?.DomainEventPulled -= OnDomainEventPulled;
            }

            _disposed = true;
        }

        base.Dispose(disposing);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Returns the set of default configuration overrides, which enables end-to-end
    /// integration testing, by replacing external dependencies with an in-memory alternatives.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="IWebApplicationServicesConfiguration"/>
    /// containing end-to-end integration testing services overrides.
    /// </returns>
    private IEnumerable<IWebApplicationServicesConfiguration> CreateDefaultConfigurations()
    {
        _connection ??= SqliteConnection.InitializeInMemoryConnection(Guid.NewGuid().ToString());

        _domainEventsConfiguration = new();
        _domainEventsConfiguration.DomainEventPulled += OnDomainEventPulled;

        yield return new DataSourceConfiguration(_connection);
        yield return new OptionsConfiguration();
        yield return new SeedingConfiguration();
        yield return new CommunicationConfiguration();
        yield return _domainEventsConfiguration;
    }

    /// <summary>
    /// <see cref="DomainEventsConfiguration.DomainEventPulled"/> event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="DomainEventPulledEventArgs"/> instance containing the event data.
    /// </param>
    private void OnDomainEventPulled(object? sender, DomainEventPulledEventArgs e)
        => _receivedEvents.Add(e.DomainEvent);

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> if this instance has been disposed.
    /// </summary>
    private void ThrowIfDisposed()
        => ObjectDisposedException.ThrowIf(_disposed, this);
    #endregion
}