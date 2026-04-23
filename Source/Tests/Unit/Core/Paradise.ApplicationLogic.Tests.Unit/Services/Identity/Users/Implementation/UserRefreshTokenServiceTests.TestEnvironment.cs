using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Time.Testing;
using Paradise.ApplicationLogic.Services.Identity.Users.Implementation;
using Paradise.Domain.Identity.Users;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.ApplicationLogic.Infrastructure.Identity;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess.Repositories;

namespace Paradise.ApplicationLogic.Tests.Unit.Services.Identity.Users;

public sealed partial class UserRefreshTokenServiceTests
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

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="UserRefreshTokenServiceTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Fields
        private readonly FakeTimeProvider _timeProvider;
        private readonly FakeUserManager _userManager;
        private readonly FakeDomainUnitOfWork _unitOfWork;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _timeProvider = new();
            var dataSource = new FakeDataSource(_timeProvider);
            var identityOptions = new IdentityOptions();
            _unitOfWork = new FakeDomainUnitOfWork(dataSource);

            _userManager = new FakeUserManager(_timeProvider, dataSource, identityOptions);

            Target = new(_timeProvider, _userManager, _unitOfWork);
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public UserRefreshTokenService Target { get; }

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
        /// <param name="password">
        /// Password.
        /// </param>
        /// <returns>
        /// The newly created and saved <see cref="User"/> instance.
        /// </returns>
        public async Task<User> AddUserAsync(string email = "Email", string userName = "UserName", string password = "Password")
        {
            var user = new User(email, userName);

            await _userManager.CreateAsync(user, password)
                .ConfigureAwait(false);

            return user;
        }

        /// <summary>
        /// Creates an <see cref="UserRefreshToken"/> instance
        /// and saves its data into the test persistence storage.
        /// </summary>
        /// <param name="owner">
        /// Refresh token owner.
        /// </param>
        /// <param name="expiryDateUtc">
        /// Refresh token expiry date.
        /// </param>
        /// <param name="isActive">
        /// Indicates whether the refresh token is active (was not revoked)
        /// and can be used during authentication processes.
        /// </param>
        /// <returns>
        /// The newly created and saved <see cref="UserRefreshToken"/> instance.
        /// </returns>
        public async Task<UserRefreshToken> AddRefreshTokenAsync(User owner, DateTimeOffset? expiryDateUtc = null, bool isActive = false)
        {
            expiryDateUtc ??= DateTimeOffset.UnixEpoch;

            var refreshToken = new UserRefreshToken(owner.Id, expiryDateUtc.Value)
            {
                IsActive = isActive
            };

            _unitOfWork.UserRefreshTokensRepository.Add(refreshToken);

            await _unitOfWork.CommitAsync()
                .ConfigureAwait(false);

            return refreshToken;
        }
        #endregion
    }
    #endregion
}