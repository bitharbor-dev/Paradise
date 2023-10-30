using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Paradise.Common.Extensions;
using Paradise.Localization.ExceptionsHandling;
using Paradise.Options.Models;
using System.Text;

namespace Paradise.ApplicationLogic.Authorization.JwtBearer.Keys.Implementation;

/// <summary>
/// Provides JWT signing keys based on a secret string from configuration.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SecretBasedSigningKeyProvider"/> class.
/// </remarks>
/// <param name="configuration">
/// The <see cref="IConfiguration"/> instance to get
/// the secret from.
/// </param>
public sealed class SecretBasedSigningKeyProvider(IConfiguration configuration) : IJwtSigningKeyProvider
{
    #region Public methods
    /// <inheritdoc/>
    public SecurityKey GetSigningKey()
    {
        var secret = configuration
            .GetRequiredSection(nameof(ApplicationOptions))
            .GetValue<string>(nameof(ApplicationOptions.Secret));

        if (secret.IsNullOrWhiteSpace())
        {
            var message = ExceptionMessagesProvider.GetApplicationSecretMissingMessage();

            throw new InvalidOperationException(message);
        }

        var bytes = Encoding.UTF8.GetBytes(secret);
        var key = new SymmetricSecurityKey(bytes);

        return key;
    }
    #endregion
}