using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Paradise.Tests.Miscellaneous.TestDoubles.Spies.Microsoft.AspNetCore.Authorization;
using Paradise.WebApi.Authorization;

namespace Paradise.WebApi.Tests.Unit.Authorization;

public sealed partial class AuthorizationResultHandlerTests
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="AuthorizationResultHandlerTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Fields
        private readonly SpyAuthorizationMiddlewareResultHandler _frameworkHandler;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _frameworkHandler = new();

            Target = new(_frameworkHandler);
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public AuthorizationResultHandler Target { get; }

        /// <summary>
        /// Dummy request delegate.
        /// </summary>
        public RequestDelegate Next { get; } = _ => Task.CompletedTask;

        /// <summary>
        /// Dummy authorization policy.
        /// </summary>
        public AuthorizationPolicy Policy { get; } = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

        /// <summary>
        /// Indicates whether the <see cref="Target"/> has delegated result
        /// handling to the default authorization middleware result handler.
        /// </summary>
        public bool HandlingDelegated
            => _frameworkHandler.Handled;
        #endregion

        #region Public methods

        #endregion
    }
    #endregion
}