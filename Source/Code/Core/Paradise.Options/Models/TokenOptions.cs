namespace Paradise.Options.Models;

/// <summary>
/// Application tokens options.
/// </summary>
public sealed class TokenOptions
{
    #region Properties
    /// <summary>
    /// Default options value.
    /// </summary>
    public static TokenOptions Default { get; } = new()
    {
        EmailConfirmationTokenLifetime = TimeSpan.FromDays(1),
        ResetEmailAddressTokenLifetime = TimeSpan.FromDays(1),
        ResetPasswordTokenLifetime = TimeSpan.FromDays(1),
        UserDeletionRequestLifetime = TimeSpan.FromDays(1)
    };

    /// <summary>
    /// Email address confirmation period.
    /// </summary>
    public TimeSpan EmailConfirmationTokenLifetime { get; set; }

    /// <summary>
    /// Email address reset token lifetime.
    /// </summary>
    public TimeSpan ResetEmailAddressTokenLifetime { get; set; }

    /// <summary>
    /// Password reset token lifetime.
    /// </summary>
    public TimeSpan ResetPasswordTokenLifetime { get; set; }

    /// <summary>
    /// User deletion request lifetime.
    /// </summary>
    public TimeSpan UserDeletionRequestLifetime { get; set; }
    #endregion
}