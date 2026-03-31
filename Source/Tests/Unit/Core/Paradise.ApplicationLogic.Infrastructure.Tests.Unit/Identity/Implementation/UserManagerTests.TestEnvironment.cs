using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Paradise.Domain.Identity.Roles;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.AspNetCore.Identity;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.Extensions.Logging;
using Paradise.Tests.Miscellaneous.TestImplementations.Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;
using OptionsBuilder = Microsoft.Extensions.Options.Options;
using UserManager = Paradise.ApplicationLogic.Infrastructure.Identity.Implementation.UserManager
    <Paradise.Tests.Miscellaneous.TestImplementations.Microsoft.AspNetCore.Identity.TestUser>;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.Identity.Implementation;

public sealed partial class UserManagerTests : IDisposable
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Dispose()
        => Test.Dispose();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="UserManagerTests"/> class.
    /// </summary>
    private sealed class TestEnvironment : IDisposable
    {
        #region Fields
        private readonly FakeIdentityDbContext _context;
        private readonly FakeUserStore _userStore;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _context = new();

            var errors = new IdentityErrorDescriber();
            _userStore = new FakeUserStore(_context, errors);
            var identityOptions = OptionsBuilder.Create(new IdentityOptions());
            var passwordHasher = new PasswordHasher<TestUser>();
            var userValidators = new List<UserValidator<TestUser>>();
            var passwordValidators = new List<PasswordValidator<TestUser>>();
            var keyNormalizer = new UpperInvariantLookupNormalizer();
            var services = new ServiceCollection();
            var logger = new FakeLogger<UserManager>();

            services
                .AddSingleton<ILookupProtector, FakeLookupProtector>()
                .AddSingleton<ILookupProtectorKeyRing, FakeLookupProtectorKeyRing>()
                .AddSingleton<IUserTwoFactorTokenProvider<TestUser>, FakeUserTwoFactorTokenProvider>();

            Target = new(_userStore, identityOptions, passwordHasher,
                         userValidators, passwordValidators, keyNormalizer,
                         errors, services.BuildServiceProvider(), logger);
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public UserManager Target { get; }
        #endregion

        #region Public methods
        /// <summary>
        /// Simulates the data protection mechanism implemented
        /// within <see cref="UserManager"/> instances.
        /// </summary>
        /// <param name="data">
        /// The data to protect.
        /// </param>
        /// <returns>
        /// The protected data.
        /// </returns>
        [return: NotNullIfNotNull(nameof(data))]
        public string? ConvertToProtectedData(string? data)
        {
            var protector = Target.ServiceProvider.GetRequiredService<ILookupProtector>();
            var ring = Target.ServiceProvider.GetRequiredService<ILookupProtectorKeyRing>();

            return protector.Protect(ring.CurrentKeyId, data);
        }

        /// <summary>
        /// Creates a <see cref="TestUser"/> instance
        /// and saves its data into the test persistence storage.
        /// </summary>
        /// <param name="emailAddress">
        /// Email address.
        /// </param>
        /// <param name="userName">
        /// User-name.
        /// </param>
        /// <param name="phoneNumber">
        /// Phone number.
        /// </param>
        /// <returns>
        /// The persisted <see cref="TestUser"/> instance.
        /// </returns>
        public TestUser AddUser(string emailAddress = "EmailAddress",
                                string userName = "UserName",
                                string phoneNumber = "PhoneNumber")
        {
            var user = new TestUser
            {
                NormalizedEmail = emailAddress,
                NormalizedUserName = userName,
                PhoneNumber = phoneNumber
            };

            _context.Add(user);
            _context.SaveChanges();

            return user;
        }

        /// <summary>
        /// Checks if the <see cref="TestUser"/> with the given
        /// <paramref name="userName"/> and <paramref name="emailAddress"/> values
        /// exists in the persistence storage.
        /// </summary>
        /// <param name="emailAddress">
        /// Email address.
        /// </param>
        /// <param name="userName">
        /// User-name.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <see cref="Role"/> with the given
        /// <paramref name="userName"/> and <paramref name="emailAddress"/> values
        /// exists in the persistence storage,
        /// otherwise - <see langword="false"/>.
        /// </returns>
        public bool UserExists(string emailAddress, string userName)
        {
            return _context
                .Set<TestUser>()
                .Any(user => user.Email == emailAddress
                          && user.UserName == userName);
        }

        /// <summary>
        /// Intercepts the internal <see cref="IUserStore{TUser}.CreateAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetUserStoreCreateAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _userStore.CreateAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IUserStore{TUser}.UpdateAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetUserStoreUpdateAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _userStore.UpdateAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IUserClaimStore{TUser}.AddClaimsAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetUserStoreAddClaimsAsyncResult(Func<Task> resultingDelegate)
            => _userStore.AddClaimsAsyncResult = resultingDelegate;

        /// <inheritdoc/>
        public void Dispose()
        {
            _userStore.Dispose();
            _context.Dispose();
            Target.Dispose();
        }
        #endregion
    }
    #endregion
}