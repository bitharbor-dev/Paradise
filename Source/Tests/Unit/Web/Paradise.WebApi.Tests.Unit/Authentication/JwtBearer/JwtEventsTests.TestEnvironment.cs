using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Paradise.Models;
using Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Web.WebApi.Services.Authentication;
using Paradise.WebApi.Authentication.JwtBearer;
using Paradise.WebApi.Services.Authentication;

namespace Paradise.WebApi.Tests.Unit.Authentication.JwtBearer;

public sealed partial class JwtEventsTests
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="JwtEventsTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Fields
        private readonly StubAuthenticationService _authenticationService = new();
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public JwtEvents Target { get; } = new();
        #endregion

        #region Public methods
        /// <summary>
        /// Creates a fully initialized <see cref="TokenValidatedContext"/> instance
        /// suitable for unit testing <see cref="JwtEvents"/> behavior.
        /// </summary>
        /// <remarks>
        /// The returned context is backed by a minimal <see cref="HttpContext"/> that
        /// provides an <see cref="IAuthenticationService"/> via
        /// <see cref="HttpContext.RequestServices"/>, allowing the
        /// <see cref="JwtEvents.TokenValidated"/> method to execute without relying on
        /// the ASP.NET Core authentication pipeline.
        /// </remarks>
        /// <returns>
        /// A configured <see cref="TokenValidatedContext"/> instance for testing purposes.
        /// </returns>
        public TokenValidatedContext GetTokenValidatedContext()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IAuthenticationService>(_ => _authenticationService)
                .BuildServiceProvider();

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };

            var scheme = new Microsoft.AspNetCore.Authentication.AuthenticationScheme(
                JwtBearerDefaults.AuthenticationScheme, null, typeof(JwtBearerHandler));

            return new TokenValidatedContext(httpContext, scheme, new())
            {
                Principal = new()
            };
        }

        /// <summary>
        /// Intercepts the internal <see cref="IAuthenticationService.CheckSessionAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetAuthenticationServiceCheckSessionAsyncResult(Func<Task<Result>> resultingDelegate)
            => _authenticationService.CheckSessionAsyncResult = resultingDelegate;
        #endregion
    }
    #endregion
}