using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.Tests.Extensibility;
using Paradise.Tests.Extensibility.Web.Hosting;
using Paradise.Tests.Extensibility.Web.Hosting.Configuration;
using Paradise.Tests.Extensibility.Web.Hosting.Configuration.Base;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.WebApi.Tests.Integration.Endpoints.Handlers;

/// <summary>
/// Base endpoints handlers test class.
/// </summary>
public abstract class EndpointHandlersTests : IAsyncDisposable
{
    #region Fields
    private bool _disposed;

    private SqliteConnection? _connection;
    private DefaultWebApplicationFactory? _application;
    private HttpClient? _client;

    private IEnumerable<IWebApplicationServicesConfiguration>? _configurations;
    #endregion

    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    protected HttpClient Client
        => GetOrCreateClient();

    /// <summary>
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </summary>
    protected CancellationToken Token { get; } = TestContext.Current.CancellationToken;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        await DisposeAsyncCore()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);

        _disposed = true;
    }
    #endregion

    #region Protected methods
    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        _client?.Dispose();

        if (_application is not null)
        {
            await _application.DisposeAsync()
                .ConfigureAwait(false);
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync()
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creates a new <see cref="Role"/> and persists it
    /// using the configured <see cref="IRoleManager{TRole}"/>.
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
    protected async Task AddRoleAsync(string name, bool isDefault)
    {
        EnsureApplicationReady();

        var scope = _application.Services.CreateAsyncScope();

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
    /// using the configured <see cref="IUserManager{TUser}"/>.
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
    protected async Task AddUserAsync(string userName,
                                      string emailAddress,
                                      string password,
                                      string? phoneNumber = null,
                                      bool isEmailAddressConfirmed = true,
                                      bool twoFactorEnabled = false)
    {
        EnsureApplicationReady();

        var scope = _application.Services.CreateAsyncScope();

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

    /// <summary>
    /// Configures the <see cref="DefaultWebApplicationFactory"/>
    /// using the specified service configurations.
    /// </summary>
    /// <remarks>
    /// Should be invoked before the <see cref="Client"/>
    /// property is accessed or seeding methods called.
    /// Otherwise has no effect because
    /// the application factory has already been initialized.
    /// </remarks>
    /// <param name="configurations">
    /// The service configurations applied to the application factory.
    /// </param>
    protected void ConfigureApplication(params IEnumerable<IWebApplicationServicesConfiguration> configurations)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_application is not null)
            throw new InvalidOperationException();

        _configurations = configurations;
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Initializes the <see cref="_client"/> once and returns it to the caller.
    /// </summary>
    /// <returns>
    /// Initialized <see cref="_client"/> instance.
    /// </returns>
    private HttpClient GetOrCreateClient()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_client is null)
        {
            EnsureApplicationReady();

            _client = _application.CreateClient();
        }

        return _client;
    }

    /// <summary>
    /// Initializes the <see cref="DefaultWebApplicationFactory"/>
    /// when an application has not already been configured.
    /// </summary>
    /// <remarks>
    /// Creates and stores an in-memory <see cref="SqliteConnection"/>
    /// together with the default application configurations.
    /// </remarks>
    [MemberNotNull(nameof(_application))]
    private void EnsureApplicationReady()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_application is null)
        {
            if (_configurations is null)
            {
                var databaseName = TestContext.Current.Test!.UniqueID;
                _connection ??= SqliteConnection.InitializeInMemoryConnection(databaseName);

                _configurations =
                [
                    new DataSourceConfiguration(_connection),
                    new OptionsConfiguration(),
                    new SeedingConfiguration()
                ];
            }

            _application = new(_configurations);
        }
    }
    #endregion
}