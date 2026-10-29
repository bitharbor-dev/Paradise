using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Paradise.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.Domain.Base.Events;
using Paradise.Models.WebApi.Services.Authentication;
using Paradise.Tests.Doubles.Spies.Core.Domain.Base.Events;
using Paradise.Tests.Extensibility.Web.Hosting.Configuration;
using Paradise.Tests.Extensibility.Web.Hosting.Configuration.Base;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer;
using Paradise.WebApi.Infrastructure.Options;
using Paradise.WebApi.Services.Authentication;

namespace Paradise.Tests.Extensibility.Web.Hosting;

/// <summary>
/// Default <see cref="WebApplicationFactory{TEntryPoint}"/> implementation which
/// provides a standardized way to configure an in-memory application instance.
/// </summary>
public sealed class DefaultWebApplicationFactory : WebApplicationFactory<Program>
{
    #region Fields
    private static readonly TimeSpan _eventTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan _eventPollingInterval = TimeSpan.FromMilliseconds(10);

    private bool _disposed;
    private bool _hostStarted;

    private IWebApplicationServicesConfiguration[]? _configurations;

    private SqliteConnection? _connection;
    private DomainEventsConfiguration? _domainEventsConfiguration;

    private readonly Lock _receivedEventsLock = new();
    private readonly List<IDomainEvent> _receivedEvents = [];

    private readonly FakeTimeProvider _timeProvider = new(DateTimeOffset.UnixEpoch);
    #endregion

    #region Properties
    /// <summary>
    /// The snapshot of received domain events.
    /// </summary>
    /// <remarks>
    /// Populated only if no configuration overrides were specified.
    /// </remarks>
    public IReadOnlyList<IDomainEvent> ReceivedEvents
    {
        get
        {
            lock (_receivedEventsLock)
                return [.. _receivedEvents];
        }
    }

    /// <summary>
    /// An accessor to the <see cref="WebApi.Infrastructure.Options.AuthenticationOptions"/> instance
    /// used by the application.
    /// </summary>
    public AuthenticationOptions AuthenticationOptions
        => Services.GetRequiredService<IOptions<AuthenticationOptions>>().Value;

    /// <summary>
    /// Gets or sets the current UTC time.
    /// </summary>
    /// <remarks>
    /// Used by the application only if no configuration overrides were specified.
    /// </remarks>
    public DateTimeOffset UtcNow
    {
        get => _timeProvider.GetUtcNow();
        set => _timeProvider.SetUtcNow(value);
    }
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

            var creationResult = await roleManager.CreateAsync(role)
                .ConfigureAwait(false);

            ThrowIfFailed(creationResult, $"Failed to create role '{name}'");
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

            var creationResult = await userManager.CreateAsync(user, password)
                .ConfigureAwait(false);

            ThrowIfFailed(creationResult, $"Failed to create user '{userName}'");
        }
    }

    /// <summary>
    /// Logs in the <see cref="User"/> with the specified <paramref name="userName"/>
    /// using the configured <see cref="IAuthenticationService"/>, bypassing the HTTP pipeline.
    /// </summary>
    /// <param name="userName">
    /// User's user-name.
    /// </param>
    /// <param name="password">
    /// User's password.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the access token, or the two-factor
    /// identity token when two-factor authentication is enabled for the user.
    /// </returns>
    public async Task<AccessTokenModel> AuthenticateUserAsync(string userName,
                                                              string password,
                                                              CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        var scope = Services.CreateAsyncScope();

        await using (scope.ConfigureAwait(false))
        {
            var authenticationService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

            var model = new LoginModel(password)
            {
                UserName = userName
            };

            var loginResult = await authenticationService.LoginAsync(model, cancellationToken)
                .ConfigureAwait(false);

            if (!loginResult.IsSuccess || loginResult.Value is null)
            {
                var errors = string.Join(", ", loginResult.Errors);

                throw new InvalidOperationException($"Failed to log in '{userName}': {loginResult.Status} ({errors}).");
            }

            return loginResult.Value;
        }
    }

    /// <summary>
    /// Deletes the <see cref="User"/> with the specified <paramref name="userName"/>
    /// using the configured <see cref="IUserManager{TUser}"/>
    /// bypassing (partially or completely) the business rules.
    /// </summary>
    /// <param name="userName">
    /// User's user-name.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public async Task DeleteUserAsync(string userName)
    {
        ThrowIfDisposed();

        var scope = Services.CreateAsyncScope();

        await using (scope.ConfigureAwait(false))
        {
            var userManager = scope.ServiceProvider.GetRequiredService<IUserManager<User>>();

            var user = await userManager.FindByNameAsync(userName)
                .ConfigureAwait(false)
                ?? throw new InvalidOperationException($"User '{userName}' does not exist.");

            var deletionResult = await userManager.DeleteAsync(user)
                .ConfigureAwait(false);

            ThrowIfFailed(deletionResult, $"Failed to delete user '{userName}'");
        }
    }

    /// <summary>
    /// Issues a validly signed access token bound to the given <paramref name="refreshTokenId"/>
    /// using the configured <see cref="IJwtManager"/>
    /// bypassing (partially or completely) the business rules.
    /// </summary>
    /// <param name="refreshTokenId">
    /// The Id of the refresh token to bind the access token with.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the issued access token.
    /// </returns>
    public async Task<string> IssueAccessTokenAsync(Guid refreshTokenId)
    {
        ThrowIfDisposed();

        var scope = Services.CreateAsyncScope();

        await using (scope.ConfigureAwait(false))
        {
            var jwtManager = scope.ServiceProvider.GetRequiredService<IJwtManager>();

            return jwtManager.IssueToken([], refreshTokenId, out _);
        }
    }

    /// <summary>
    /// Waits until a domain event of type <typeparamref name="T"/> is received.
    /// </summary>
    /// <typeparam name="T">
    /// The domain event type.
    /// </typeparam>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the first received domain event of type <typeparamref name="T"/>.
    /// </returns>
    /// <exception cref="TimeoutException">
    /// No domain event of type <typeparamref name="T"/> was received in time.
    /// </exception>
    public Task<T> WaitForEventAsync<T>(CancellationToken cancellationToken = default)
        where T : IDomainEvent
        => WaitForEventAsyncCore<T>(predicate: null, cancellationToken);

    /// <summary>
    /// Waits until a domain event of type <typeparamref name="T"/>
    /// matching the given <paramref name="predicate"/> is received.
    /// </summary>
    /// <typeparam name="T">
    /// The domain event type.
    /// </typeparam>
    /// <param name="predicate">
    /// A function to test each received domain event for a condition.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the first received domain event of type <typeparamref name="T"/>
    /// matching the given <paramref name="predicate"/>.
    /// </returns>
    /// <exception cref="TimeoutException">
    /// No matching domain event of type <typeparamref name="T"/> was received in time.
    /// </exception>
    public Task<T> WaitForEventAsync<T>(Func<T, bool> predicate, CancellationToken cancellationToken = default)
        where T : IDomainEvent
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return WaitForEventAsyncCore(predicate, cancellationToken);
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
    /// integration testing, by replacing external dependencies with in-memory alternatives.
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

        yield return new TimeConfiguration(_timeProvider);
        yield return new CachingConfiguration(_timeProvider);
        yield return new DataSourceConfiguration(_connection);
        yield return new OptionsConfiguration(out var emailTemplateOptions);
        yield return new SeedingConfiguration(emailTemplateOptions);
        yield return new CommunicationConfiguration();
        yield return _domainEventsConfiguration;
    }

    /// <summary>
    /// Waits until a domain event of type <typeparamref name="T"/>
    /// matching the given <paramref name="predicate"/> is received.
    /// </summary>
    /// <typeparam name="T">
    /// The domain event type.
    /// </typeparam>
    /// <param name="predicate">
    /// A function to test each received domain event for a condition,
    /// or <see langword="null"/> to accept any domain event of type <typeparamref name="T"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the first received matching domain event of type <typeparamref name="T"/>.
    /// </returns>
    /// <exception cref="TimeoutException">
    /// No matching domain event of type <typeparamref name="T"/> was received in time.
    /// </exception>
    private async Task<T> WaitForEventAsyncCore<T>(Func<T, bool>? predicate, CancellationToken cancellationToken)
        where T : IDomainEvent
    {
        ThrowIfDisposed();

        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(_eventTimeout);

        try
        {
            while (true)
            {
                lock (_receivedEventsLock)
                {
                    // Events are received from the dispatcher's thread,
                    // so the list is re-checked on each iteration.
                    foreach (var domainEvent in _receivedEvents.OfType<T>())
                    {
                        if (predicate is null || predicate(domainEvent))
                            return domainEvent;
                    }
                }

                await Task.Delay(_eventPollingInterval, timeoutSource.Token)
                    .ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"No matching '{typeof(T).Name}' was received within {_eventTimeout}.");
        }
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
    {
        lock (_receivedEventsLock)
            _receivedEvents.Add(e.DomainEvent);
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if the given <paramref name="result"/>
    /// indicates an unsuccessful operation.
    /// </summary>
    /// <param name="result">
    /// The <see cref="IdentityResult"/> to be checked.
    /// </param>
    /// <param name="message">
    /// The exception message, to be followed by the operation errors.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The given <paramref name="result"/> indicates an unsuccessful operation.
    /// </exception>
    private static void ThrowIfFailed(IdentityResult result, string message)
    {
        if (result.Succeeded)
            return;

        var errors = string.Join(", ", result.Errors.Select(error => error.Description));

        throw new InvalidOperationException($"{message}: {errors}.");
    }

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> if this instance has been disposed.
    /// </summary>
    private void ThrowIfDisposed()
        => ObjectDisposedException.ThrowIf(_disposed, this);
    #endregion
}