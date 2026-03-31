using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Implementation;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Options;
using OptionsBuilder = Microsoft.Extensions.Options.Options;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Authentication.JwtBearer.Keys.Implementation;

public sealed partial class SymmetricSigningKeyProviderTests
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="SymmetricSigningKeyProviderTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Properties
        /// <summary>
        /// An accessor to the <see cref="SymmetricSigningKeyProviderOptions"/> instance
        /// used by the test target or it's dependencies.
        /// </summary>
        public SymmetricSigningKeyProviderOptions Options { get; } = new()
        {
            Secret = "Ug5xCXJaNaxfx78KdQxQZDAmniAbZw6V"
        };
        #endregion

        #region Public methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SymmetricSigningKeyProvider"/> class.
        /// </summary>
        /// <returns>
        /// System under test.
        /// </returns>
        public SymmetricSigningKeyProvider CreateProvider()
        {
            var options = OptionsBuilder.Create(Options);

            return new(options);
        }
        #endregion
    }
    #endregion
}