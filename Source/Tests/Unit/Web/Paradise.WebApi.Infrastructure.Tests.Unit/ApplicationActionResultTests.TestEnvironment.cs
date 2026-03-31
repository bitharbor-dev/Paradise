using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Paradise.Models;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.AspNetCore.Http.Features;
using System.Text.Json;
using OptionsBuilder = Microsoft.Extensions.Options.Options;
using TestResult = Paradise.Tests.Miscellaneous.TestImplementations.Shared.Models.TestResult;

namespace Paradise.WebApi.Infrastructure.Tests.Unit;

public sealed partial class ApplicationActionResultTests
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
    /// Provides setup and behavior check methods for the <see cref="ApplicationActionResultTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            Result = new TestResult();
            Target = new(Result);
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public ApplicationActionResult Target { get; }

        /// <summary>
        /// An accessor to the <see cref="ResultBase"/> instance
        /// used by the test target or it's dependencies.
        /// </summary>
        public ResultBase Result { get; }

        /// <summary>
        /// An accessor to the <see cref="JsonSerializerOptions"/> instance
        /// used by the test target or it's dependencies.
        /// </summary>
        public JsonSerializerOptions? Options { get; set; } = new();
        #endregion

        #region Public methods
        /// <summary>
        /// Builds an <see cref="ActionContext"/> instance to be passed into
        /// <see cref="ApplicationActionResult.ExecuteResultAsync"/> method call.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionContext"/> with minimum configuration.
        /// </returns>
        public ActionContext CreateContext()
        {
            var services = new ServiceCollection();

            if (Options is not null)
                services.AddSingleton(OptionsBuilder.Create(Options));

            var httpContext = new DefaultHttpContext
            {
                RequestServices = services.BuildServiceProvider(),
                Response =
                {
                    Body = new MemoryStream()
                }
            };

            httpContext.Features.Set<IHttpResponseFeature>(new FakeHttpResponseFeature());

            return new(httpContext, new(), new());
        }
        #endregion
    }
    #endregion
}