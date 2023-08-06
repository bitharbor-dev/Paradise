using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Options.Models.Communication;

/// <summary>
/// Email templates options.
/// </summary>
public sealed class EmailTemplateOptions
{
    #region Properties
    /// <summary>
    /// Email address confirmation template name.
    /// </summary>
    [Required, NotNull]
    public string? EmailAddressConfirmationTemplateName { get; set; }

    /// <summary>
    /// Email address reset completed template name.
    /// </summary>
    [Required, NotNull]
    public string? EmailAddressResetCompletedTemplateName { get; set; }

    /// <summary>
    /// Email address reset notification template name.
    /// </summary>
    [Required, NotNull]
    public string? EmailAddressResetNotificationTemplateName { get; set; }

    /// <summary>
    /// Email address reset template name.
    /// </summary>
    [Required, NotNull]
    public string? EmailAddressResetTemplateName { get; set; }

    /// <summary>
    /// Password reset completed template name.
    /// </summary>
    [Required, NotNull]
    public string? PasswordResetCompletedTemplateName { get; set; }

    /// <summary>
    /// Password reset template name.
    /// </summary>
    [Required, NotNull]
    public string? PasswordResetTemplateName { get; set; }

    /// <summary>
    /// Two-factor verification template name.
    /// </summary>
    [Required, NotNull]
    public string? TwoFactorVerificationTemplateName { get; set; }
    #endregion
}