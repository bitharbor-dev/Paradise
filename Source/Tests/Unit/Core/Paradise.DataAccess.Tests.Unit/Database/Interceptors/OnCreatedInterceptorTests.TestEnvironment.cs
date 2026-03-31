using Paradise.DataAccess.Database.Interceptors;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.System;

namespace Paradise.DataAccess.Tests.Unit.Database.Interceptors;

public sealed partial class OnCreatedInterceptorTests
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="OnCreatedInterceptorTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Fields
        private readonly FakeTimeProvider _timeProvider;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _timeProvider = new();

            Target = new(_timeProvider);
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public OnCreatedInterceptor Target { get; }

        /// <summary>
        /// Gets or sets the current UTC time.
        /// </summary>
        public DateTimeOffset UtcNow
        {
            get => _timeProvider.GetUtcNow();
            set => _timeProvider.SetUtcNow(value);
        }
        #endregion
    }
    #endregion
}