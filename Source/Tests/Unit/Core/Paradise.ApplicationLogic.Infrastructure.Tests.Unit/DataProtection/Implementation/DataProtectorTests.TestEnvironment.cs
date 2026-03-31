using Microsoft.AspNetCore.DataProtection;
using Paradise.ApplicationLogic.Infrastructure.DataProtection.Implementation;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.AspNetCore.DataProtection;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.DataProtection.Implementation;

public sealed partial class DataProtectorTests
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="DataProtectorTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            InternalProtector = new FakeDataProtector();
            Target = new(InternalProtector);
        }
        #endregion

        #region Properties
        /// <summary>
        /// An <see cref="IDataProtector"/> instance used to
        /// arrange data and validate test results.
        /// </summary>
        public IDataProtector InternalProtector { get; }

        /// <summary>
        /// System under test.
        /// </summary>
        public DataProtector Target { get; }
        #endregion
    }
    #endregion
}