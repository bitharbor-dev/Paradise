using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.EventListeners.Domain.Identity.Users;
using Paradise.ApplicationLogic.Services.Identity.Roles;
using Paradise.Domain.Identity.Roles;
using Paradise.Domain.Identity.Users;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.ApplicationLogic.Infrastructure.Identity;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.ApplicationLogic.Services.Identity.Roles;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.System;

namespace Paradise.ApplicationLogic.Tests.Unit.EventListeners.Domain.Identity.Users;

public sealed partial class AssignDefaultUserRolesTests
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
    /// Provides setup and behavior check methods for the <see cref="AssignDefaultUserRolesTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Fields
        private readonly FakeTimeProvider _timeProvider;
        private readonly FakeDataSource _source;
        private readonly FakeRoleManager _roleManager;
        private readonly FakeUserManager _userManager;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _timeProvider = new();
            _source = new(_timeProvider);
            _roleManager = new(_source);
            _userManager = new(_timeProvider, _source, new IdentityOptions());

            var services = new ServiceCollection()
                .AddScoped<IRoleService>(_ => new FakeRoleService(_roleManager, _userManager));

            Target = new(services.BuildServiceProvider());
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public AssignDefaultUserRoles Target { get; }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates an <see cref="Role"/> instance
        /// and saves its data into the test persistence storage.
        /// </summary>
        /// <param name="name">
        /// Role name.
        /// </param>
        /// <param name="isDefault">
        /// Indicates whether the role is default and should be
        /// assigned automatically when a user has been created.
        /// </param>
        /// <returns>
        /// The newly created and saved <see cref="Role"/> instance.
        /// </returns>
        public async Task<Role> AddRoleAsync(string name, bool isDefault)
        {
            var role = new Role(name, isDefault);

            await _roleManager.CreateAsync(role)
                .ConfigureAwait(false);

            return role;
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
        /// Returns a flag indicating whether the specified <paramref name="user"/>
        /// is a member of the given <paramref name="role"/>.
        /// </summary>
        /// <param name="user">
        /// The <see cref="User"/> whose role membership should be checked.
        /// </param>
        /// <param name="role">
        /// The <see cref="Role"/> to be checked.
        /// </param>
        /// <returns>
        /// A flag indicating whether the specified <paramref name="user"/>
        /// is a member of the given <paramref name="role"/>.
        /// </returns>
        public async Task<bool> UserIsInRoleAsync(User user, Role role)
        {
            var roles = await _userManager.GetRolesAsync(user)
                .ConfigureAwait(false);

            return roles.Contains(role.Name);
        }
        #endregion
    }
    #endregion
}