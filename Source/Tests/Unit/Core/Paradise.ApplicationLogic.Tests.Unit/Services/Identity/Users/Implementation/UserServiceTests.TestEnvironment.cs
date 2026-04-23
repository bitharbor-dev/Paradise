using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.ApplicationLogic.Options.Models;
using Paradise.ApplicationLogic.Services.Identity.Users.Implementation;
using Paradise.Domain.Base.Events;
using Paradise.Domain.Identity.Users;
using Paradise.Models;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.ApplicationLogic.Infrastructure.DataProtection;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.ApplicationLogic.Infrastructure.Identity;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.Extensions.Logging;
using Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.Domain.Events;
using System.Security.Claims;
using OptionsBuilder = Microsoft.Extensions.Options.Options;

namespace Paradise.ApplicationLogic.Tests.Unit.Services.Identity.Users;

public sealed partial class UserServiceTests : IDisposable
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();

    /// <summary>
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </summary>
    public CancellationToken Token { get; } = TestContext.Current.CancellationToken;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Dispose()
        => Test.Dispose();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="UserServiceTests"/> class.
    /// </summary>
    private sealed class TestEnvironment : IDisposable
    {
        #region Fields
        private readonly IList<MessageLoggedEventArgs> _loggedMessages;
        private readonly IList<IDomainEvent> _domainEvents;

        private readonly FakeTimeProvider _timeProvider;
        private readonly FakeLogger<UserService> _logger;

        private readonly IOptions<ApplicationOptions> _applicationOptions;

        private readonly FakeUserManager _userManager;
        private readonly SpyDomainEventSink _domainEventSink;
        private readonly FakeDataProtector _dataProtector;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _loggedMessages = [];
            _domainEvents = [];

            _logger = new();
            _logger.MessageLogged += OnMessageLogged;

            _applicationOptions = OptionsBuilder.Create(new ApplicationOptions
            {
                ApiUrl = new("https://localhost:5001"),
                Timeout = new()
                {
                    ResetEmailAddressTimeout = TimeSpan.FromDays(1),
                    EmailConfirmationTimeout = TimeSpan.FromDays(1),
                    ResetPasswordTimeout = TimeSpan.FromDays(1),
                    UserDeletionRequestTimeout = TimeSpan.FromDays(1)
                }
            });

            _timeProvider = new();
            var dataSource = new FakeDataSource(_timeProvider);
            IdentityOptions = new();

            _userManager = new FakeUserManager(_timeProvider, dataSource, IdentityOptions);

            _domainEventSink = new();
            _domainEventSink.Pushed += OnPushed;

            _dataProtector = new FakeDataProtector();

            Target = new(_logger, _applicationOptions, _timeProvider, _userManager, _domainEventSink, _dataProtector);
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public UserService Target { get; }

        /// <summary>
        /// Contains messages which were logged during tests.
        /// </summary>
        public IEnumerable<MessageLoggedEventArgs> LoggedMessages
            => _loggedMessages.AsReadOnly();

        /// <summary>
        /// The list of produced domain events.
        /// </summary>
        public IEnumerable<IDomainEvent> DomainEvents
            => _domainEvents.AsReadOnly();

        /// <summary>
        /// An accessor to the <see cref="Microsoft.AspNetCore.Identity.IdentityOptions"/> instance
        /// used by the test target or it's dependencies.
        /// </summary>
        public IdentityOptions IdentityOptions { get; }

        /// <summary>
        /// An accessor to the <see cref="Options.Models.ApplicationOptions"/> instance
        /// used by the test target or it's dependencies.
        /// </summary>
        public ApplicationOptions ApplicationOptions
            => _applicationOptions.Value;

        /// <summary>
        /// Gets or sets the current UTC time.
        /// </summary>
        public DateTimeOffset UtcNow
        {
            get => _timeProvider.GetUtcNow();
            set => _timeProvider.SetUtcNow(value);
        }
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public void Dispose()
        {
            _domainEventSink.Pushed -= OnPushed;
            _logger.MessageLogged -= OnMessageLogged;
        }

        /// <summary>
        /// Creates an <see cref="User"/> instance
        /// and saves its data into the test persistence storage.
        /// </summary>
        /// <param name="email">
        /// Email address.
        /// </param>
        /// <param name="userName">
        /// User-name.
        /// </param>
        /// <param name="phoneNumber">
        /// A telephone number for the user.
        /// </param>
        /// <param name="password">
        /// Password.
        /// </param>
        /// <param name="emailConfirmed">
        /// A flag indicating if a user has confirmed their email address.
        /// </param>
        /// <param name="lockoutEnabled">
        /// A flag indicating if the user could be locked out.
        /// </param>
        /// <param name="lockoutEnd">
        /// The date and time, in UTC, when any user lockout ends.
        /// </param>
        /// <param name="accessFailedCount">
        /// The number of failed login attempts for the current user.
        /// </param>
        /// <param name="deletionRequestSubmitted">
        /// Deletion request submission date.
        /// </param>
        /// <returns>
        /// The newly created and saved <see cref="User"/> instance.
        /// </returns>
        public async Task<User> AddUserAsync(string email = "test@email.com",
                                             string userName = "UserName",
                                             string phoneNumber = "+1000000000",
                                             string password = "Password",
                                             bool emailConfirmed = false,
                                             bool lockoutEnabled = false,
                                             DateTimeOffset? lockoutEnd = null,
                                             int accessFailedCount = 0,
                                             DateTimeOffset? deletionRequestSubmitted = null)
        {
            var user = new User(email, userName)
            {
                PhoneNumber = phoneNumber,
                EmailConfirmed = emailConfirmed,
                LockoutEnabled = lockoutEnabled,
                LockoutEnd = lockoutEnd,
                AccessFailedCount = accessFailedCount,
                DeletionRequestSubmitted = deletionRequestSubmitted
            };

            await _userManager.CreateAsync(user, password)
                .ConfigureAwait(false);

            return user;
        }

        /// <summary>
        /// Deletes the given <paramref name="user"/>
        /// from the persistence storage.
        /// </summary>
        /// <param name="user">
        /// The <see cref="User"/> to be deleted.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public async Task DeleteUserAsync(User user)
        {
            await _userManager.DeleteAsync(user)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Checks if a user with the specified <paramref name="id"/>
        /// exists in the persistence storage.
        /// </summary>
        /// <param name="id">
        /// Unique identifier.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the user exists,
        /// otherwise - <see langword="false"/>.
        /// </returns>
        public bool IdExists(Guid id)
            => _userManager.Users.SingleOrDefault(user => user.Id == id) is not null;

        /// <summary>
        /// Checks if a user with the specified <paramref name="email"/>
        /// exists in the persistence storage.
        /// </summary>
        /// <param name="email">
        /// The email address for this user.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the user exists,
        /// otherwise - <see langword="false"/>.
        /// </returns>
        public bool EmailAddressExists(string? email)
            => _userManager.Users.SingleOrDefault(user => user.Email == email) is not null;

        /// <summary>
        /// Checks if a user with the specified <paramref name="userName"/>
        /// exists in the persistence storage.
        /// </summary>
        /// <param name="userName">
        /// The user name for this user.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the user exists,
        /// otherwise - <see langword="false"/>.
        /// </returns>
        public bool UserNameExists(string? userName)
            => _userManager.Users.SingleOrDefault(user => user.UserName == userName) is not null;

        /// <summary>
        /// Checks if a user with the specified <paramref name="phoneNumber"/>
        /// exists in the persistence storage.
        /// </summary>
        /// <param name="phoneNumber">
        /// A telephone number for the user.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the user exists,
        /// otherwise - <see langword="false"/>.
        /// </returns>
        public bool PhoneNumberExists(string? phoneNumber)
            => _userManager.Users.SingleOrDefault(user => user.PhoneNumber == phoneNumber) is not null;

        /// <summary>
        /// Gets the flag indicating if a user has confirmed their email address
        /// of the user with the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// Target user Id.
        /// </param>
        /// <returns>
        /// The flag indicating if a user has confirmed their email address
        /// of the user with the given <paramref name="id"/>.
        /// </returns>
        public bool GetEmailConfirmed(Guid id)
            => _userManager.Users.Single(user => user.Id == id).EmailConfirmed;

        /// <summary>
        /// Gets the number of failed login attempts
        /// of the user with the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// Target user Id.
        /// </param>
        /// <returns>
        /// The number of failed login attempts
        /// of the user with the given <paramref name="id"/>.
        /// </returns>
        public int GetAccessFailedCount(Guid id)
            => _userManager.Users.Single(user => user.Id == id).AccessFailedCount;

        /// <summary>
        /// Gets the password of the user with the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// Target user Id.
        /// </param>
        /// <returns>
        /// The password of the user with the given <paramref name="id"/>.
        /// </returns>
        public string? GetPassword(Guid id)
            => _userManager.Users.Single(user => user.Id == id).PasswordHash;

        /// <summary>
        /// Gets the email address of the user with the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// Target user Id.
        /// </param>
        /// <returns>
        /// The email address of the user with the given <paramref name="id"/>.
        /// </returns>
        public string GetEmailAddress(Guid id)
            => _userManager.Users.Single(user => user.Id == id).Email;

        /// <summary>
        /// Gets the deletion request submission date
        /// of the user with the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// Target user Id.
        /// </param>
        /// <returns>
        /// The deletion request submission date
        /// of the user with the given <paramref name="id"/>.
        /// </returns>
        public DateTimeOffset? GetDeletionRequestSubmitted(Guid id)
            => _userManager.Users.Single(user => user.Id == id).DeletionRequestSubmitted;

        /// <summary>
        /// Gets the flag indicating if two factor authentication is enabled
        /// for the user with the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// Target user Id.
        /// </param>
        /// <returns>
        /// The flag indicating if two factor authentication is enabled
        /// for the user with the given <paramref name="id"/>.
        /// </returns>
        public bool GetTwoFactorEnabled(Guid id)
            => _userManager.Users.Single(user => user.Id == id).TwoFactorEnabled;

        /// <summary>
        /// Gets the username of the user with the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// Target user Id.
        /// </param>
        /// <returns>
        /// The username of the user with the given <paramref name="id"/>.
        /// </returns>
        public string? GetUserName(Guid id)
            => _userManager.Users.Single(user => user.Id == id).UserName;

        /// <summary>
        /// Generates an identity token used to confirm the user's email address.
        /// </summary>
        /// <param name="user">
        /// Token target.
        /// </param>
        /// <param name="expiryDate">
        /// Identity token expiry date.
        /// </param>
        /// <returns>
        /// A generated identity token.
        /// </returns>
        public async Task<string> GenerateEmailAddressConfirmationIdentityTokenAsync(User user, DateTimeOffset? expiryDate = null)
        {
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user)
                .ConfigureAwait(false);

            var identityToken = new IdentityToken(user.Email, emailConfirmationToken, expiryDate: expiryDate);

            return _dataProtector.Protect(identityToken);
        }

        /// <summary>
        /// Generates an invalid identity token used to confirm the user's email address.
        /// </summary>
        /// <param name="user">
        /// Token target.
        /// </param>
        /// <param name="expiryDate">
        /// Identity token expiry date.
        /// </param>
        /// <returns>
        /// A generated identity token.
        /// </returns>
        public async Task<string> GenerateInvalidEmailAddressConfirmationIdentityTokenAsync(User user, DateTimeOffset? expiryDate = null)
        {
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user)
                .ConfigureAwait(false);

            return $"{emailConfirmationToken} {expiryDate}";
        }

        /// <summary>
        /// Generates an identity token used to reset the user's password.
        /// </summary>
        /// <param name="user">
        /// Token target.
        /// </param>
        /// <param name="expiryDate">
        /// Identity token expiry date.
        /// </param>
        /// <returns>
        /// A generated identity token.
        /// </returns>
        public async Task<string> GeneratePasswordResetIdentityTokenAsync(User user, DateTimeOffset? expiryDate = null)
        {
            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user)
                .ConfigureAwait(false);

            var identityToken = new IdentityToken(user.Email, passwordResetToken, expiryDate: expiryDate);

            return _dataProtector.Protect(identityToken);
        }

        /// <summary>
        /// Generates an invalid identity token used to reset the user's password.
        /// </summary>
        /// <param name="user">
        /// Token target.
        /// </param>
        /// <param name="expiryDate">
        /// Identity token expiry date.
        /// </param>
        /// <returns>
        /// A generated identity token.
        /// </returns>
        public async Task<string> GenerateInvalidPasswordResetIdentityTokenAsync(User user, DateTimeOffset? expiryDate = null)
        {
            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user)
                .ConfigureAwait(false);

            return $"{passwordResetToken} {expiryDate}";
        }

        /// <summary>
        /// Generates an identity token used to reset the user's email address.
        /// </summary>
        /// <param name="user">
        /// Token target.
        /// </param>
        /// <param name="newEmailAddress">
        /// The new email address.
        /// </param>
        /// <param name="expiryDate">
        /// Identity token expiry date.
        /// </param>
        /// <returns>
        /// A generated identity token.
        /// </returns>
        public async Task<string> GenerateEmailAddressResetIdentityTokenAsync(User user, string newEmailAddress,
                                                                              DateTimeOffset? expiryDate = null)
        {
            var emailResetToken = await _userManager.GenerateChangeEmailTokenAsync(user, newEmailAddress)
                .ConfigureAwait(false);

            var identityToken = new IdentityToken(user.Email, emailResetToken, newEmailAddress, expiryDate);

            return _dataProtector.Protect(identityToken);
        }

        /// <summary>
        /// Generates an invalid identity token used to reset the user's email address.
        /// </summary>
        /// <param name="user">
        /// Token target.
        /// </param>
        /// <param name="newEmailAddress">
        /// The new email address.
        /// </param>
        /// <param name="expiryDate">
        /// Identity token expiry date.
        /// </param>
        /// <returns>
        /// A generated identity token.
        /// </returns>
        public async Task<string> GenerateInvalidEmailAddressResetIdentityTokenAsync(User user, string newEmailAddress,
                                                                                     DateTimeOffset? expiryDate = null)
        {
            var emailResetToken = await _userManager.GenerateChangeEmailTokenAsync(user, newEmailAddress)
                .ConfigureAwait(false);

            return $"{emailResetToken} {newEmailAddress} {expiryDate}";
        }

        /// <summary>
        /// Intercepts the internal <see cref="IUserManager{TUser}.CreateAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetUserManagerCreateAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _userManager.CreateAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IUserManager{TUser}.UpdateAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetUserManagerUpdateAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _userManager.UpdateAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IUserManager{TUser}.DeleteAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetUserManagerDeleteAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _userManager.DeleteAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IUserManager{TUser}.GetClaimsAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetUserManagerGetClaimsAsyncResult(Func<Task<IList<Claim>>> resultingDelegate)
            => _userManager.GetClaimsAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IUserManager{TUser}.AccessFailedAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetUserManagerAccessFailedAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _userManager.AccessFailedAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IUserManager{TUser}.ConfirmEmailAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetUserManagerConfirmEmailAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _userManager.ConfirmEmailAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IUserManager{TUser}.ChangeEmailAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetUserManagerChangeEmailAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _userManager.ChangeEmailAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IUserManager{TUser}.ResetPasswordAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetUserManagerResetPasswordAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _userManager.ResetPasswordAsyncResult = resultingDelegate;
        #endregion

        #region Private methods
        /// <summary>
        /// The <see cref="FakeLogger{T}.MessageLogged"/> event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="MessageLoggedEventArgs"/> instance containing the event data.
        /// </param>
        private void OnMessageLogged(object? sender, MessageLoggedEventArgs e)
            => _loggedMessages.Add(e);

        /// <summary>
        /// <see cref="SpyDomainEventSink.Pushed"/> event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="DomainEventPushedEventArgs"/> instance containing the event data.
        /// </param>
        private void OnPushed(object? sender, DomainEventPushedEventArgs e)
            => _domainEvents.Add(e.DomainEvent);
        #endregion
    }
    #endregion
}