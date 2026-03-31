using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.ApplicationLogic.Options.Models.Infrastructure.Services;

/// <summary>
/// Email templates options.
/// </summary>
public sealed class EmailTemplateOptions
{
    #region Properties
    /// <summary>
    /// Email address changed notification template name.
    /// </summary>
    [Required, NotNull]
    public string? EmailAddressChangedNotificationTemplateName { get; set; }

    /// <summary>
    /// Email address change link template name.
    /// </summary>
    [Required, NotNull]
    public string? EmailAddressChangeLinkTemplateName { get; set; }

    /// <summary>
    /// Email address changing notification template name.
    /// </summary>
    [Required, NotNull]
    public string? EmailAddressChangingNotificationTemplateName { get; set; }

    /// <summary>
    /// Email address confirmation link template name.
    /// </summary>
    [Required, NotNull]
    public string? EmailAddressConfirmationLinkTemplateName { get; set; }

    /// <summary>
    /// Password changed notification template name.
    /// </summary>
    [Required, NotNull]
    public string? PasswordChangedNotificationTemplateName { get; set; }

    /// <summary>
    /// Password change link template name.
    /// </summary>
    [Required, NotNull]
    public string? PasswordChangeLinkTemplateName { get; set; }

    /// <summary>
    /// Two-factor verification template name.
    /// </summary>
    [Required, NotNull]
    public string? TwoFactorVerificationTemplateName { get; set; }
    #endregion
}