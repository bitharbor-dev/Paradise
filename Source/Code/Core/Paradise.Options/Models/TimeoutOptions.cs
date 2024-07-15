namespace Paradise.Options.Models;

/// <summary>
/// Application tokens options.
/// </summary>
public sealed class TimeoutOptions
{
    #region Properties
    /// <summary>
    /// Default options value.
    /// </summary>
    public static TimeoutOptions Default { get; } = new()
    {
        EmailConfirmationTimeout = TimeSpan.FromDays(1),
        ResetEmailAddressTimeout = TimeSpan.FromDays(1),
        ResetPasswordTimeout = TimeSpan.FromDays(1),
        UserDeletionRequestTimeout = TimeSpan.FromDays(1)
    };

    /// <summary>
    /// Email address confirmation timeout.
    /// </summary>
    public TimeSpan EmailConfirmationTimeout { get; set; }

    /// <summary>
    /// Email address reset timeout.
    /// </summary>
    public TimeSpan ResetEmailAddressTimeout { get; set; }

    /// <summary>
    /// Password reset timeout.
    /// </summary>
    public TimeSpan ResetPasswordTimeout { get; set; }

    /// <summary>
    /// User deletion request timeout.
    /// </summary>
    public TimeSpan UserDeletionRequestTimeout { get; set; }
    #endregion
}