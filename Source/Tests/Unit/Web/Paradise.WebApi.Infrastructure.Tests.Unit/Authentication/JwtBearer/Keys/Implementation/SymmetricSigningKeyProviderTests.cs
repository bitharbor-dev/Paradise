using Microsoft.IdentityModel.Tokens;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Implementation;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Options;
using System.Text;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Authentication.JwtBearer.Keys.Implementation;

/// <summary>
/// <see cref="SymmetricSigningKeyProvider"/> test class.
/// </summary>
public sealed partial class SymmetricSigningKeyProviderTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="SymmetricSigningKeyProvider"/> constructor should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// <see cref="SymmetricSigningKeyProviderOptions"/> instance
    /// has <see langword="null"/> secret value.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsOnInvalidCertificateName()
    {
        // Arrange
        Test.Options.Secret = null;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(Test.CreateProvider);
    }

    /// <summary>
    /// The <see cref="SymmetricSigningKeyProvider.GetSigningKey"/> method should
    /// return an <see cref="SymmetricSecurityKey"/> created from the configured secret.
    /// </summary>
    [Fact]
    public void GetSigningKey()
    {
        // Arrange
        var expectedKey = Encoding.UTF8.GetBytes(Test.Options.Secret);

        var provider = Test.CreateProvider();

        // Act
        var securityKey = provider.GetSigningKey();

        // Assert
        var symmetricSecurityKey = Assert.IsType<SymmetricSecurityKey>(securityKey);
        Assert.Equal(expectedKey, symmetricSecurityKey.Key);
    }

    /// <summary>
    /// The <see cref="SymmetricSigningKeyProvider.GetSigningKey"/> method should
    /// return the same <see cref="SymmetricSecurityKey"/> instance on repeated calls.
    /// </summary>
    [Fact]
    public void GetSigningKey_ReturnsSameKeyOnRepeatedCall()
    {
        // Arrange
        var provider = Test.CreateProvider();

        // Act
        var firstKey = provider.GetSigningKey();
        var secondKey = provider.GetSigningKey();

        // Assert
        Assert.Same(firstKey, secondKey);
    }

    /// <summary>
    /// The <see cref="SymmetricSigningKeyProvider.JwtAlgorithm"/> property should
    /// return the HMAC SHA-256 algorithm identifier.
    /// </summary>
    [Fact]
    public void JwtAlgorithm()
    {
        // Arrange
        var provider = Test.CreateProvider();

        // Act
        var algorithm = provider.JwtAlgorithm;

        // Assert
        Assert.Equal(SecurityAlgorithms.HmacSha256, algorithm);
    }
    #endregion
}