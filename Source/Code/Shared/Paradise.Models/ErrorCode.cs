using Paradise.Localization.ErrorsHandling;
using Paradise.Models.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Paradise.Models;

/// <summary>
/// Contains localized descriptions for all kinds of errors within the application.
/// </summary>
public enum ErrorCode
{
    #region IdentityResult errors
    /// <summary>
    /// Optimistic concurrency failure, the object has been modified.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityConcurrencyFailure))]
    ConcurrencyFailure,
    /// <summary>
    /// An unknown failure has occurred.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityDefaultError))]
    DefaultError,
    /// <summary>
    /// Email is already taken.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityDuplicateEmail))]
    DuplicateEmail,
    /// <summary>
    /// Role name is already taken.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityDuplicateRoleName))]
    DuplicateRoleName,
    /// <summary>
    /// User-name is already taken.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityDuplicateUserName))]
    DuplicateUserName,
    /// <summary>
    /// Email is invalid.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityInvalidEmail))]
    InvalidEmail,
    /// <summary>
    /// Invalid manager type.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityInvalidManagerType))]
    InvalidManagerType,
    /// <summary>
    /// The provided PasswordHasherCompatibilityMode is invalid.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityInvalidPasswordHasherCompatibilityMode))]
    InvalidPasswordHasherCompatibilityMode,
    /// <summary>
    /// The iteration count must be a positive integer.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityInvalidPasswordHasherIterationCount))]
    InvalidPasswordHasherIterationCount,
    /// <summary>
    /// Role name is invalid.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityInvalidRoleName))]
    InvalidRoleName,
    /// <summary>
    /// Invalid token.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityInvalidToken))]
    InvalidToken,
    /// <summary>
    /// User-name is invalid, can only contain letters or digits.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityInvalidUserName))]
    InvalidUserName,
    /// <summary>
    /// A user with this login already exists.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityLoginAlreadyAssociated))]
    LoginAlreadyAssociated,
    /// <summary>
    /// AddIdentity must be called on the service collection.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityMustCallAddIdentity))]
    MustCallAddIdentity,
    /// <summary>
    /// No IUserTwoFactorTokenProvider with this name is registered.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityNoTokenProvider))]
    NoTokenProvider,
    /// <summary>
    /// User security stamp cannot be null.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityNullSecurityStamp))]
    NullSecurityStamp,
    /// <summary>
    /// Incorrect password.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityPasswordMismatch))]
    PasswordMismatch,
    /// <summary>
    /// Passwords must have at least one digit ('0'-'9').
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityPasswordRequiresDigit))]
    PasswordRequiresDigit,
    /// <summary>
    /// Passwords must have at least one lowercase ('a'-'z').
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityPasswordRequiresLower))]
    PasswordRequiresLower,
    /// <summary>
    /// Passwords must have at least one non-alphanumeric character.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityPasswordRequiresNonAlphanumeric))]
    PasswordRequiresNonAlphanumeric,
    /// <summary>
    /// Passwords must have at least one uppercase ('A'-'Z').
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityPasswordRequiresUpper))]
    PasswordRequiresUpper,
    /// <summary>
    /// Password is too short.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityPasswordTooShort))]
    PasswordTooShort,
    /// <summary>
    /// Role does not exist.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityRoleNotFound))]
    RoleNotFound,
    /// <summary>
    /// Store does not implement IQueryableRoleStore&lt;TRole&gt;.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityStoreNotIQueryableRoleStore))]
    StoreNotIQueryableRoleStore,
    /// <summary>
    /// Store does not implement IQueryableUserStore&lt;TUser&gt;.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityStoreNotIQueryableUserStore))]
    StoreNotIQueryableUserStore,
    /// <summary>
    /// Store does not implement IRoleClaimStore&lt;TRole&gt;.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityStoreNotIRoleClaimStore))]
    StoreNotIRoleClaimStore,
    /// <summary>
    /// Store does not implement IUserAuthenticationTokenStore&lt;TUser&gt;.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityStoreNotIUserAuthenticationTokenStore))]
    StoreNotIUserAuthenticationTokenStore,
    /// <summary>
    /// Store does not implement IUserClaimStore&lt;TUser&gt;.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityStoreNotIUserClaimStore))]
    StoreNotIUserClaimStore,
    /// <summary>
    /// Store does not implement IUserConfirmationStore&lt;TUser&gt;.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityStoreNotIUserConfirmationStore))]
    StoreNotIUserConfirmationStore,
    /// <summary>
    /// Store does not implement IUserEmailStore&lt;TUser&gt;.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityStoreNotIUserEmailStore))]
    StoreNotIUserEmailStore,
    /// <summary>
    /// Store does not implement IUserLockoutStore&lt;TUser&gt;.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityStoreNotIUserLockoutStore))]
    StoreNotIUserLockoutStore,
    /// <summary>
    /// Store does not implement IUserLoginStore&lt;TUser&gt;.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityStoreNotIUserLoginStore))]
    StoreNotIUserLoginStore,
    /// <summary>
    /// Store does not implement IUserPasswordStore&lt;TUser&gt;.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityStoreNotIUserPasswordStore))]
    StoreNotIUserPasswordStore,
    /// <summary>
    /// Store does not implement IUserPhoneNumberStore&lt;TUser&gt;.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityStoreNotIUserPhoneNumberStore))]
    StoreNotIUserPhoneNumberStore,
    /// <summary>
    /// Store does not implement IUserRoleStore&lt;TUser&gt;.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityStoreNotIUserRoleStore))]
    StoreNotIUserRoleStore,
    /// <summary>
    /// Store does not implement IUserSecurityStampStore&lt;TUser&gt;.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityStoreNotIUserSecurityStampStore))]
    StoreNotIUserSecurityStampStore,
    /// <summary>
    /// Store does not implement IUserAuthenticatorKeyStore&lt;TUser&gt;.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityStoreNotIUserAuthenticatorKeyStore))]
    StoreNotIUserAuthenticatorKeyStore,
    /// <summary>
    /// Store does not implement IUserTwoFactorStore&lt;TUser&gt;.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityStoreNotIUserTwoFactorStore))]
    StoreNotIUserTwoFactorStore,
    /// <summary>
    /// Recovery code redemption failed.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityRecoveryCodeRedemptionFailed))]
    RecoveryCodeRedemptionFailed,
    /// <summary>
    /// User already has a password set.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityUserAlreadyHasPassword))]
    UserAlreadyHasPassword,
    /// <summary>
    /// User already in the role.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityUserAlreadyInRole))]
    UserAlreadyInRole,
    /// <summary>
    /// User is locked out.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityUserLockedOut))]
    UserLockedOut,
    /// <summary>
    /// Lockout is not enabled for this user.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityUserLockoutNotEnabled))]
    UserLockoutNotEnabled,
    /// <summary>
    /// User does not exist.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityUserNameNotFound))]
    UserNameNotFound,
    /// <summary>
    /// User is not in role.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityUserNotInRole))]
    UserNotInRole,
    /// <summary>
    /// Store does not implement IUserTwoFactorRecoveryCodeStore&lt;TUser&gt;.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityStoreNotIUserTwoFactorRecoveryCodeStore))]
    StoreNotIUserTwoFactorRecoveryCodeStore,
    /// <summary>
    /// Passwords must use different characters.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityPasswordRequiresUniqueChars))]
    PasswordRequiresUniqueChars,
    /// <summary>
    /// No RoleType was specified, try AddRoles&lt;TRole&gt;().
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityNoRoleType))]
    NoRoleType,
    /// <summary>
    /// Store does not implement IProtectedUserStore&lt;TUser&gt; which is required when ProtectPersonalData = true.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityStoreNotIProtectedUserStore))]
    StoreNotIProtectedUserStore,
    /// <summary>
    /// No IPersonalDataProtector service was registered, this is required when ProtectPersonalData = true.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.IdentityNoPersonalDataProtector))]
    NoPersonalDataProtector,
    #endregion

    #region Custom errors
    /// <summary>
    /// Access to the resource is forbidden.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.AccessForbidden))]
    AccessForbidden,
    /// <summary>
    /// Phone number is already taken.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.DuplicatePhoneNumber))]
    DuplicatePhoneNumber,
    /// <summary>
    /// The message recipients list is empty.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.EmptyRecipientsList))]
    EmptyRecipientsList,
    /// <summary>
    /// The model did not pass the validation process.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.InvalidModel))]
    InvalidModel,
    /// <summary>
    /// Phone number is invalid.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.InvalidPhoneNumber))]
    InvalidPhoneNumber,
    /// <summary>
    /// SMTP configuration is not valid.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.InvalidSmtpConfiguration))]
    InvalidSmtpConfiguration,
    /// <summary>
    /// Message template with the specified name and culture already exists.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.MessageTemplateAlreadyExists))]
    MessageTemplateAlreadyExists,
    /// <summary>
    /// Empty template text.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.MessageTemplateEmptyText))]
    MessageTemplateEmptyText,
    /// <summary>
    /// Message template with the specified Id does not exist.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.MessageTemplateIdNotFound))]
    MessageTemplateIdNotFound,
    /// <summary>
    /// Invalid placeholders number.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.MessageTemplateInvalidPlaceholdersNumber))]
    MessageTemplateInvalidPlaceholdersNumber,
    /// <summary>
    /// Message template name is a null or empty string.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.MessageTemplateMissingName))]
    MessageTemplateMissingName,
    /// <summary>
    /// Missing placeholder.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.MessageTemplateMissingPlaceholder))]
    MessageTemplateMissingPlaceholder,
    /// <summary>
    /// Message template with the specified name and culture does not exist.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.MessageTemplateNotFound))]
    MessageTemplateNotFound,
    /// <summary>
    /// The token is outdated. Generate a new one.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.OutdatedToken))]
    OutdatedToken,
    /// <summary>
    /// Missing password.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.PasswordMissing))]
    PasswordMissing,
    /// <summary>
    /// Password does not match its confirmation value.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.PasswordNotMatchConfirmation))]
    PasswordNotMatchConfirmation,
    /// <summary>
    /// Role with the specified Id does not exist.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.RoleIdNotFound))]
    RoleIdNotFound,
    /// <summary>
    /// No email address was provided to send messages from.
    /// </summary>
    [IsCritical(true), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.SenderEmailIsMissing))]
    SenderEmailIsMissing,
    /// <summary>
    /// The token owner does not exist.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.TokenOwnerNotExists))]
    TokenOwnerNotExists,
    /// <summary>
    /// Unauthorized.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.UnauthorizedUser))]
    UnauthorizedUser,
    /// <summary>
    /// The deletion request has exceeded its lifetime.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.UserDeletionRequestExpired))]
    UserDeletionRequestExpired,
    /// <summary>
    /// Email is already confirmed.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.UserEmailAlreadyConfirmed))]
    UserEmailAlreadyConfirmed,
    /// <summary>
    /// Email is not confirmed.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.UserEmailNotConfirmed))]
    UserEmailNotConfirmed,
    /// <summary>
    /// The user with the specified email does not exist.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.UserEmailNotFound))]
    UserEmailNotFound,
    /// <summary>
    /// The user with the specified Id does not exist.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.UserIdNotFound))]
    UserIdNotFound,
    /// <summary>
    /// The user is not pending deletion.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.UserNotPendingDeletion))]
    UserNotPendingDeletion,
    /// <summary>
    /// The user with the specified phone number does not exist.
    /// </summary>
    [IsCritical(false), Display(ResourceType = typeof(ErrorMessages), Name = nameof(ErrorMessages.UserPhoneNumberNotFound))]
    UserPhoneNumberNotFound,
    #endregion
}