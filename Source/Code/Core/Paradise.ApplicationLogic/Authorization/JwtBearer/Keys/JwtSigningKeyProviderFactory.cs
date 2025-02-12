using Microsoft.Extensions.Configuration;
using Paradise.ApplicationLogic.Authorization.JwtBearer.Keys.Implementation;
using Paradise.Common;

namespace Paradise.ApplicationLogic.Authorization.JwtBearer.Keys;

/// <summary>
/// Factory class, which is responsible for the
/// <see cref="IJwtSigningKeyProvider"/> instances creation.
/// </summary>
public static class JwtSigningKeyProviderFactory
{
    #region Fields
    private static readonly Dictionary<string, Func<IConfiguration, IJwtSigningKeyProvider>> _providerMap = new()
    {
        [EnvironmentNames.Development] = CreateSecretBasedProvider,
        [EnvironmentNames.DevelopmentDocker] = CreateSecretBasedProvider,
        [EnvironmentNames.Staging] = CreateKeyVaultProvider,
        [EnvironmentNames.StagingDocker] = CreateKeyVaultProvider,
        [EnvironmentNames.Production] = CreateKeyVaultProvider,
        [EnvironmentNames.ProductionDocker] = CreateKeyVaultProvider
    };
    #endregion

    #region Public methods
    /// <summary>
    /// Creates a new <see cref="IJwtSigningKeyProvider"/> instance
    /// based on the given <paramref name="configuration"/> and
    /// <see cref="EnvironmentNames.Current"/> value.
    /// </summary>
    /// <param name="configuration">
    /// Provider configuration.
    /// </param>
    /// <returns>
    /// A new <see cref="IJwtSigningKeyProvider"/> instance
    /// based on the given <paramref name="configuration"/> and
    /// <see cref="EnvironmentNames.Current"/> value.
    /// </returns>
    public static IJwtSigningKeyProvider CreateProvider(IConfiguration configuration)
        => _providerMap[EnvironmentNames.Current].Invoke(configuration);
    #endregion

    #region Private methods
    /// <summary>
    /// Creates a new <see cref="SecretBasedSigningKeyProvider"/> instance
    /// based on the given <paramref name="configuration"/>.
    /// </summary>
    /// <param name="configuration">
    /// The <see cref="IConfiguration"/> instance to get
    /// the secret from.
    /// </param>
    /// <returns>
    /// A new <see cref="SecretBasedSigningKeyProvider"/> instance
    /// based on the given <paramref name="configuration"/>.
    /// </returns>
    private static SecretBasedSigningKeyProvider CreateSecretBasedProvider(IConfiguration configuration)
        => new(configuration);

    /// <summary>
    /// Creates a new <see cref="KeyVaultSigningKeyProvider"/> instance
    /// based on the given <paramref name="configuration"/>.
    /// </summary>
    /// <param name="configuration">
    /// The <see cref="IConfiguration"/> instance to get
    /// the Azure Key Vault credentials from.
    /// </param>
    /// <returns>
    /// A new <see cref="KeyVaultSigningKeyProvider"/> instance
    /// based on the given <paramref name="configuration"/>.
    /// </returns>
    private static KeyVaultSigningKeyProvider CreateKeyVaultProvider(IConfiguration configuration)
        => new(configuration);
    #endregion
}