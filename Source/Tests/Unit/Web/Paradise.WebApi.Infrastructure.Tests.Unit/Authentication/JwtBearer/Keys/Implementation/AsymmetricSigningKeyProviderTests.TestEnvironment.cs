using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Implementation;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Options;
using OptionsBuilder = Microsoft.Extensions.Options.Options;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Authentication.JwtBearer.Keys.Implementation;

public sealed partial class AsymmetricSigningKeyProviderTests
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="AsymmetricSigningKeyProviderTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Properties
        /// <summary>
        /// An accessor to the <see cref="AsymmetricSigningKeyProviderOptions"/> instance
        /// used by the test target or it's dependencies.
        /// </summary>
        public AsymmetricSigningKeyProviderOptions Options { get; } = new()
        {
            PrivateKey = "MIICXQIBAAKBgQCNgomP9R3QYNDP35i5OyWs2eyXeRj4x+AtmjE/12XOitu0l5Od"
                       + "qwYvWNQogxNPxjNr65xaZMZbXJz7EGq++JjP52yq8BJgN/VnW6cJP7jhBljNaozs"
                       + "F5zvmEAT1IIo7sz+G/xYijkr3wwG6S8kN1EBIWpo7RkDEJG4EDtk17+4rwIDAQAB"
                       + "AoGAUknccKgbJDeIdbkSeHRanj9Dg3nZ+aFRTXNivDsnaon45PVX09HGEPZYuQ4v"
                       + "xq387P7ftvjvF+WtK5oKWO76/NaKP+TLPZHwJM3puJOrTaDrwhI6otLyA+fVWZgl"
                       + "8dDcgjeQ2T9WDNhHZMRu7yUIQwWtbFVl2jbJLyAw6ce+iEECQQD1cEIzGmEmP/3p"
                       + "LDNHba2eaoeLpl6H81gT8bCdLNOcseZSEiUfPEwFKTYgumwtSuW5QOteUVjFoJKQ"
                       + "s198EHO/AkEAk5lnvKphkK7MCATckgrW5Hm0vBYXdW6e7xBnPIyT0GCVeIGbAysd"
                       + "orG7Mclmt/RmTmeSk+JIOlcyoM8eOLY3EQJAPHTFaa8SxQg4NApWKz8B6CaXcret"
                       + "S1GOnYMIHP8gtNVBRXAAwtvoYdEP6yngYZu0UFiEYXwqIKv3zjrQx0+KIwJBAIPl"
                       + "HfJWPwFPcjvoPEK1NPrOV1eMVkI2LAhtnBNbe+tFo8wf5SmbqcvtDt6anxPbbmC5"
                       + "5R4Jo4meyjsxWkxLaEECQQDnIX/OVc7ckaYZJiHjxEosOF7D2io+qFWWWSMZMyEP"
                       + "mctLm7sOCsCn/Ik/q7H81rI4P4Lr/HWMywwdH7LMz1SC"
        };
        #endregion

        #region Public methods
        /// <summary>
        /// Initializes a new instance of the <see cref="AsymmetricSigningKeyProvider"/> class.
        /// </summary>
        /// <returns>
        /// System under test.
        /// </returns>
        public AsymmetricSigningKeyProvider CreateProvider()
        {
            var options = OptionsBuilder.Create(Options);

            return new(options);
        }
        #endregion
    }
    #endregion
}