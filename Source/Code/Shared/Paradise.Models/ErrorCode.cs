using Paradise.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using EM = Paradise.Localization.ErrorHandling.ErrorMessages;

namespace Paradise.Models;

/// <summary>
/// Contains error codes which describes all kinds of errors within the application
/// and provides access to the localized error descriptions.
/// </summary>
public enum ErrorCode
{
    /// <summary>
    /// An unknown failure has occurred.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(EM), Name = nameof(EM.DefaultError))]
    DefaultError,
    /// <summary>
    /// Email address '{0}' is in use.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.DuplicateEmailAddress))]
    DuplicateEmailAddress,
    /// <summary>
    /// Phone number '{0}' is in use.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.DuplicatePhoneNumber))]
    DuplicatePhoneNumber,
    /// <summary>
    /// Role name '{0}' is in use.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.DuplicateRoleName))]
    DuplicateRoleName,
    /// <summary>
    /// User-name '{0}' is in use.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.DuplicateUserName))]
    DuplicateUserName,
    /// <summary>
    /// Email address '{0}' is invalid.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.InvalidEmailAddress))]
    InvalidEmailAddress,
    /// <summary>
    /// {0}
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.InvalidModel))]
    InvalidModel,
    /// <summary>
    /// Phone number '{0}' is invalid.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.InvalidPhoneNumber))]
    InvalidPhoneNumber,
    /// <summary>
    /// Role name '{0}' is invalid.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.InvalidRoleName))]
    InvalidRoleName,
    /// <summary>
    /// Invalid token.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.InvalidToken))]
    InvalidToken,
    /// <summary>
    /// User-name '{0}' is invalid.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.InvalidUserName))]
    InvalidUserName,
    /// <summary>
    /// Message template with the name '{0}' and culture '{1}' already exists.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.MessageTemplateAlreadyExists))]
    MessageTemplateAlreadyExists,
    /// <summary>
    /// {0}
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.MessageTemplateEmptyText))]
    MessageTemplateEmptyText,
    /// <summary>
    /// Message template with Id '{0}' does not exist.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.MessageTemplateIdNotFound))]
    MessageTemplateIdNotFound,
    /// <summary>
    /// Message template name is a null or empty string.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.MessageTemplateMissingName))]
    MessageTemplateMissingName,
    /// <summary>
    /// Message template with the name '{0}' and culture '{1}' does not exist.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(EM), Name = nameof(EM.MessageTemplateNotFound))]
    MessageTemplateNotFound,
    /// <summary>
    /// The token is outdated. Generate a new one.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.OutdatedToken))]
    OutdatedToken,
    /// <summary>
    /// Missing password.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.PasswordMissing))]
    PasswordMissing,
    /// <summary>
    /// The password value does not match it's confirmation value.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.PasswordNotMatchConfirmation))]
    PasswordNotMatchConfirmation,
    /// <summary>
    /// The email address '{0}' does not match it's confirmation - '{1}'.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.EmailAddressNotMatchConfirmation))]
    EmailAddressNotMatchConfirmation,
    /// <summary>
    /// Role with Id '{0}' does not exist.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.RoleIdNotFound))]
    RoleIdNotFound,
    /// <summary>
    /// The deletion request has exceeded its lifetime ({0}).
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.UserDeletionRequestExpired))]
    UserDeletionRequestExpired,
    /// <summary>
    /// The email address '{0}' is already confirmed.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.UserEmailAddressAlreadyConfirmed))]
    UserEmailAddressAlreadyConfirmed,
    /// <summary>
    /// Email address '{0}' is not confirmed.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.UserEmailAddressNotConfirmed))]
    UserEmailAddressNotConfirmed,
    /// <summary>
    /// User with email address '{0}' does not exist.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.UserEmailAddressNotFound))]
    UserEmailAddressNotFound,
    /// <summary>
    /// User with Id '{0}' does not exist.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.UserIdNotFound))]
    UserIdNotFound,
    /// <summary>
    /// User is locked out.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.UserLockedOut))]
    UserLockedOut,
    /// <summary>
    /// The specified user does not exist or password is incorrect.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.UserNotFoundOrPasswordMismatch))]
    UserNotFoundOrPasswordMismatch,
    /// <summary>
    /// The user '{0}' is not pending deletion.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.UserNotPendingDeletion))]
    UserNotPendingDeletion,
    /// <summary>
    /// User with phone number '{0}' does not exist.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.UserPhoneNumberNotFound))]
    UserPhoneNumberNotFound,
    /// <summary>
    /// Refresh token with Id '{0}' does not exist.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.UserRefreshTokenIdNotFound))]
    UserRefreshTokenIdNotFound,
    /// <summary>
    /// Unauthorized.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.UserUnauthorized))]
    UserUnauthorized,
    /// <summary>
    /// User with user-name '{0}' does not exist.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.UserUserNameNotFound))]
    UserUserNameNotFound,
    /// <summary>
    /// Access forbidden.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(EM), Name = nameof(EM.AccessForbidden))]
    AccessForbidden,
}