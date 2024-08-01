namespace Paradise.Options.Models;

/// <summary>
/// Authentication options.
/// </summary>
public sealed class AuthenticationOptions
{
    #region Properties
    /// <summary>
    /// Default options value.
    /// </summary>
    public static AuthenticationOptions Default { get; } = new()
    {
        AccessTokenLifetime = TimeSpan.FromMinutes(5),
        RefreshTokenLifetime = TimeSpan.FromDays(90),
        TwoFactorTokenLifetime = TimeSpan.FromMinutes(30),
        TwoFactorVerificationCodeLength = 6
    };

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