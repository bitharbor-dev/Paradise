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
public abstract class EndpointHandlersTests : IDisposable, IAsyncDisposable
{
    #region Fields
    private SqliteConnection? _connection;
    private DefaultWebApplicationFactory? _application;
    #endregion

    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    /// <remarks>
    /// Either uses the default application factory configuration,
    /// or a configuration provided through <see cref="ConfigureApplication"/>.
    /// </remarks>
    public HttpClient Client
    {
        get
        {
            if (field is null)
            {
                InitializeDefaultApplication();

                field = _application.CreateClient();
            }

            return field;
        }
    }

    /// <summary>
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </summary>
    public CancellationToken Token { get; } = TestContext.Current.CancellationToken;
    #endregion

    #region Public methods
    /// <summary>
    /// Configures the <see cref="DefaultWebApplicationFactory"/>
    /// using the specified service configurations.
    /// </summary>
    /// <remarks>
    /// Should be invoked before the <see cref="Client"/>
    /// property is accessed or seeding methods called.
    /// Otherwise has no effect because
    /// the application factory has already been initialized.
    /// <para>
    /// This method is idempotent in the scope of each test method.
    /// </para>
    /// </remarks>
    /// <param name="configurations">
    /// The service configurations applied to the application factory.
    /// </param>
    public void ConfigureApplication(params IWebApplicationServicesConfiguration[] configurations)
        => _application ??= new(configurations);

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
    public async Task AddRoleAsync(string name, bool isDefault)
    {
        InitializeDefaultApplication();

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
    public async Task AddUserAsync(string userName,
                                   string emailAddress,
                                   string password,
                                   string? phoneNumber = null,
                                   bool isEmailAddressConfirmed = true,
                                   bool twoFactorEnabled = false)
    {
        InitializeDefaultApplication();

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

    /// <inheritdoc/>
    public void Dispose()
    {
        _application?.Dispose();

        _connection?.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
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

        GC.SuppressFinalize(this);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Initializes the default <see cref="DefaultWebApplicationFactory"/>
    /// when an application has not already been configured.
    /// </summary>
    /// <remarks>
    /// Creates and stores an in-memory <see cref="SqliteConnection"/>
    /// together with the default application configurations.
    /// </remarks>
    [MemberNotNull(nameof(_application))]
    private void InitializeDefaultApplication()
    {
        if (_application is null)
        {
            _connection ??= SqliteConnection.InitializeInMemoryConnection(TestContext.Current.Test!.UniqueID);

            var dataSourceConfiguration = new DataSourceConfiguration(_connection);
            var optionsConfiguration = new OptionsConfiguration();
            var seedingConfiguration = new SeedingConfiguration();

            _application = new(dataSourceConfiguration, optionsConfiguration, seedingConfiguration);
        }
    }
    #endregion
}