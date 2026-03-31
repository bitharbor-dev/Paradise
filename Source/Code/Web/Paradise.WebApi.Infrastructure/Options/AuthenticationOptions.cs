namespace Paradise.WebApi.Infrastructure.Options;

/// <summary>
/// Authentication options.
/// </summary>
public sealed class AuthenticationOptions
{
    #region Properties
    /// <summary>
    /// Authorization token lifetime.
    /// </summary>
    public TimeSpan AccessTokenLifetime { get; set; }

    /// <summary>
    /// User refresh token lifetime.
    /// </summary>
    public TimeSpan RefreshTokenLifetime { get; set; }

    /// <summary>
    /// Two-factor authentication token lifetime.
    /// </summary>
    public TimeSpan TwoFactorTokenLifetime { get; set; }

    /// <summary>
    /// The length of the verification code used within
    /// the two-factor authentication process.
    /// </summary>
    public ushort TwoFactorVerificationCodeLength { get; set; }
    #endregion
}