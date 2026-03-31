using Microsoft.AspNetCore.Identity;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.ApplicationLogic.Services.Identity.Roles.Implementation;
using Paradise.Domain.Identity.Roles;
using Paradise.Domain.Identity.Users;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.ApplicationLogic.Infrastructure.Identity;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.System;

namespace Paradise.ApplicationLogic.Tests.Unit.Services.Identity.Roles.Implementation;

public sealed partial class RoleServiceTests
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
    /// Provides setup and behavior check methods for the <see cref="RoleServiceTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Fields
        private readonly FakeTimeProvider _timeProvider;
        private readonly IdentityOptions _identityOptions;

        private readonly FakeRoleManager _roleManager;
        private readonly FakeUserManager _userManager;

        private readonly FakeDataSource _dataSource;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _timeProvider = new();
            _identityOptions = new();

            _dataSource = new(_timeProvider);

            _roleManager = new FakeRoleManager(_dataSource);
            _userManager = new FakeUserManager(_timeProvider, _dataSource, _identityOptions);

            Target = new(_roleManager, _userManager);
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public RoleService Target { get; }
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
        public async Task<Role> AddRoleAsync(string name = "Test", bool isDefault = false)
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
        /// Assigns the <paramref name="role"/> to the <paramref name="user"/>.
        /// </summary>
        /// <param name="user">
        /// The <see cref="User"/> to whom to assign the <paramref name="role"/>.
        /// </param>
        /// <param name="role">
        /// The <see cref="Role"/> to be assigned to the <paramref name="user"/>.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public async Task AssignRoleAsync(User user, Role role)
        {
            await _userManager.AddToRoleAsync(user, role.Name)
                .ConfigureAwait(false);
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

        /// <summary>
        /// Checks if the <see cref="Role"/> with the given
        /// <paramref name="name"/> value
        /// exists in the persistence storage.
        /// </summary>
        /// <param name="name">
        /// The name for this role.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <see cref="Role"/> with the given
        /// <paramref name="name"/> value
        /// exists in the persistence storage,
        /// otherwise - <see langword="false"/>.
        /// </returns>
        public bool RoleExists(string? name)
            => _roleManager.Roles.Any(role => role.Name == name);

        /// <summary>
        /// Intercepts the internal <see cref="IRoleManager{TRole}.CreateAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetRoleManagerCreateAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _roleManager.CreateAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IRoleManager{TRole}.UpdateAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetRoleManagerUpdateAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _roleManager.UpdateAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IRoleManager{TRole}.DeleteAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetRoleManagerDeleteAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _roleManager.DeleteAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IUserManager{TUser}.AddToRoleAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetUserManagerAddToRoleAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _userManager.AddToRoleAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IUserManager{TUser}.RemoveFromRoleAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetUserManagerRemoveFromRoleAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _userManager.RemoveFromRoleAsyncResult = resultingDelegate;
        #endregion
    }
    #endregion
}