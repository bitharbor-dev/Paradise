using Microsoft.IdentityModel.Tokens;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Implementation;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Options;
using System.Security.Cryptography;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Authentication.JwtBearer.Keys.Implementation;

/// <summary>
/// <see cref="AsymmetricSigningKeyProvider"/> test class.
/// </summary>
public sealed partial class AsymmetricSigningKeyProviderTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="AsymmetricSigningKeyProvider"/> constructor should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// <see cref="AsymmetricSigningKeyProviderOptions"/> instance
    /// has <see langword="null"/> private key value.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsOnInvalidCertificateName()
    {
        // Arrange
        Test.Options.PrivateKey = null;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(Test.CreateProvider);
    }

    /// <summary>
    /// The <see cref="AsymmetricSigningKeyProvider.GetSigningKey"/> method should
    /// return an <see cref="RsaSecurityKey"/> created from the configured private key.
    /// </summary>
    [Fact]
    public void GetSigningKey()
    {
        // Arrange
        var expectedKey = Convert.FromBase64String(Test.Options.PrivateKey);

        var provider = Test.CreateProvider();

        // Act
        var securityKey = provider.GetSigningKey();

        // Assert
        var rsaSecurityKey = Assert.IsType<RsaSecurityKey>(securityKey);
        Assert.Equal(expectedKey, GetRsaPrivateKey(rsaSecurityKey.Parameters));
    }

    /// <summary>
    /// The <see cref="AsymmetricSigningKeyProvider.GetSigningKey"/> method should
    /// return the same <see cref="RsaSecurityKey"/> instance on repeated calls.
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
    /// The <see cref="AsymmetricSigningKeyProvider.JwtAlgorithm"/> property should
    /// return the RSA SHA-256 algorithm identifier.
    /// </summary>
    [Fact]
    public void JwtAlgorithm()
    {
        // Arrange
        var provider = Test.CreateProvider();

        // Act
        var algorithm = provider.JwtAlgorithm;

        // Assert
        Assert.Equal(SecurityAlgorithms.RsaSha256, algorithm);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Gets the private key from an RSA instance
    /// initialized with the given <paramref name="parameters"/>.
    /// </summary>
    /// <param name="parameters">
    /// The <see cref="RSAParameters"/> to initialize the RSA.
    /// </param>
    /// <returns>
    /// A byte array containing the PKCS#1 RSAPrivateKey
    /// representation of the given <paramref name="parameters"/>.
    /// </returns>
    private static byte[] GetRsaPrivateKey(RSAParameters parameters)
    {
        using var rsa = RSA.Create(parameters);

        return rsa.ExportRSAPrivateKey();
    }
    #endregion
}