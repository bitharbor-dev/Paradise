using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using static Paradise.Localization.ExceptionHandling.ExceptionMessages;

namespace Paradise.Localization.ExceptionHandling;

/// <summary>
/// Provides methods for accessing formatted and localized exception messages.
/// </summary>
[SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "Omitted for code style consistency.")]
public static class ExceptionMessagesProvider
{
    #region Constants
    /// <summary>
    /// The separator value to be used during environment names concatenation.
    /// </summary>
    private const string EnvironmentNamesSeparator = ", ";

    /// <summary>
    /// The separator value to be used during property names concatenation.
    /// </summary>
    private const string PropertyNamesSeparator = ", ";
    #endregion

    #region Public methods
    /// <summary>
    /// Gets the <see cref="EmptyRecipientsList"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="EmptyRecipientsList"/> format string.
    /// </returns>
    public static string GetMessageEmptyRecipientsList()
        => EmptyRecipientsList;

    /// <summary>
    /// Gets the <see cref="FailedToCreateInstanceOfType"/> formatted message.
    /// </summary>
    /// <typeparam name="T">
    /// Input type.
    /// </typeparam>
    /// <returns>
    /// A formatted message using the <see cref="FailedToCreateInstanceOfType"/> format string.
    /// </returns>
    public static string GetMessageFailedToCreateInstanceOfType<T>()
    {
        var messageFormat = FailedToCreateInstanceOfType;
        var typeName = typeof(T).Name;

        return string.Format(Culture,
                             messageFormat,
                             typeName);
    }

    /// <summary>
    /// Gets the <see cref="FailedToDeserialize"/> formatted message.
    /// </summary>
    /// <typeparam name="T">
    /// Input type.
    /// </typeparam>
    /// <returns>
    /// A formatted message using the <see cref="FailedToDeserialize"/> format string.
    /// </returns>
    public static string GetMessageFailedToDeserialize<T>()
    {
        var messageFormat = FailedToDeserialize;
        var typeName = typeof(T).Name;

        return string.Format(Culture,
                             messageFormat,
                             typeName);
    }

    /// <summary>
    /// Gets the <see cref="IdentityIsAlreadyAssigned"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="IdentityIsAlreadyAssigned"/> format string.
    /// </returns>
    public static string GetMessageIdentityIsAlreadyAssigned()
        => IdentityIsAlreadyAssigned;

    /// <summary>
    /// Gets the <see cref="InvalidDomainState"/> formatted message.
    /// </summary>
    /// <param name="entityType">
    /// Entity type.
    /// </param>
    /// <param name="propertyName">
    /// Entity's property name.
    /// </param>
    /// <param name="value">
    /// Entity's property value.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="InvalidDomainState"/> format string.
    /// </returns>
    public static string GetMessageInvalidDomainState(Type entityType, string? propertyName, object? value)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        var messageFormat = InvalidDomainState;
        var entityTypeName = entityType.Name;

        return string.Format(Culture,
                             messageFormat,
                             entityTypeName,
                             propertyName,
                             value);
    }

    /// <summary>
    /// Gets the <see cref="InvalidDomainStateAdditionalInformation"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="InvalidDomainStateAdditionalInformation"/> format string.
    /// </returns>
    public static string GetMessageInvalidDomainStateAdditionalInformation()
        => InvalidDomainStateAdditionalInformation;

    /// <summary>
    /// Gets the <see cref="InvalidEmailAddress"/> formatted message.
    /// </summary>
    /// <param name="email">
    /// Invalid email address.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="InvalidEmailAddress"/> format string.
    /// </returns>
    public static string GetMessageInvalidEmailAddress(string email)
    {
        var messageFromat = InvalidEmailAddress;

        return string.Format(Culture,
                             messageFromat,
                             email);
    }

    /// <summary>
    /// Gets the <see cref="InvalidEnvironmentName"/> formatted message.
    /// </summary>
    /// <param name="currentEnvironment">
    /// Current environment name.
    /// </param>
    /// <param name="allowedEnvironments">
    /// The list of allowed environment names.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="InvalidEnvironmentName"/> format string.
    /// </returns>
    public static string GetMessageInvalidEnvironmentName(string? currentEnvironment, IEnumerable<string> allowedEnvironments)
    {
        var messageFormat = InvalidEnvironmentName;
        var environments = string.Join(EnvironmentNamesSeparator, allowedEnvironments);

        return string.Format(Culture,
                             messageFormat,
                             currentEnvironment,
                             environments);
    }

    /// <summary>
    /// Gets the <see cref="InvalidSeedData"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="InvalidSeedData"/> format string.
    /// </returns>
    public static string GetMessageInvalidSeedData()
        => InvalidSeedData;

    /// <summary>
    /// Gets the <see cref="InvalidSmtpConfiguration"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="InvalidSmtpConfiguration"/> format string.
    /// </returns>
    public static string GetMessageInvalidSmtpConfiguration()
        => InvalidSmtpConfiguration;

    /// <summary>
    /// Gets the <see cref="MessageTemplateFormattableTextIsInvalid"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="MessageTemplateFormattableTextIsInvalid"/> format string.
    /// </returns>
    public static string GetMessageMessageTemplateFormattableTextInInvalidState()
        => MessageTemplateFormattableTextIsInvalid;

    /// <summary>
    /// Gets the <see cref="MessageTemplateInvalidParametersNumber"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="MessageTemplateInvalidParametersNumber"/> format string.
    /// </returns>
    public static string GetMessageMessageTemplateInvalidParametersNumber()
        => MessageTemplateInvalidParametersNumber;

    /// <summary>
    /// Gets the <see cref="MessageTemplateNotFound"/> formatted message.
    /// </summary>
    /// <param name="templateName">
    /// Template name.
    /// </param>
    /// <param name="culture">
    /// Template culture.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="MessageTemplateNotFound"/> format string.
    /// </returns>
    public static string GetMessageMessageTemplateNotFound(string templateName, CultureInfo? culture)
    {
        var messageFormat = MessageTemplateNotFound;
        var cultureName = culture?.Name;

        return string.Format(Culture,
                             messageFormat,
                             templateName,
                             cultureName);
    }

    /// <summary>
    /// Gets the <see cref="MessageTemplateInvalidPlaceholdersNumber"/> formatted message.
    /// </summary>
    /// <param name="expectedNumber">
    /// Expected number of placeholders.
    /// </param>
    /// <param name="actualNumber">
    /// Actual number of placeholders.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="MessageTemplateInvalidPlaceholdersNumber"/> format string.
    /// </returns>
    public static string GetMessageMessageTemplateInvalidPlaceholdersNumber(ushort expectedNumber, ushort actualNumber)
    {
        var messageFormat = MessageTemplateInvalidPlaceholdersNumber;

        return string.Format(Culture,
                             messageFormat,
                             expectedNumber,
                             actualNumber);
    }

    /// <summary>
    /// Gets the <see cref="MessageTemplatePlaceholderNotExists"/> formatted message.
    /// </summary>
    /// <param name="templateName">
    /// Template name.
    /// </param>
    /// <param name="culture">
    /// Template culture.
    /// </param>
    /// <param name="placeholder">
    /// Missing placeholder value.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="MessageTemplatePlaceholderNotExists"/> format string.
    /// </returns>
    public static string GetMessageMessageTemplatePlaceholderNotExists(string templateName, CultureInfo? culture, string? placeholder)
    {
        var messageFormat = MessageTemplatePlaceholderNotExists;
        var cultureName = culture?.Name;

        return string.Format(Culture,
                             messageFormat,
                             templateName,
                             cultureName,
                             placeholder);
    }

    /// <summary>
    /// Gets the <see cref="MessageTemplateTemplateTextOrSourcePathIsRequired"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="MessageTemplateTemplateTextOrSourcePathIsRequired"/> format string.
    /// </returns>
    public static string GetMessageMessageTemplateTemplateTextOrSourcePathIsRequired()
        => MessageTemplateTemplateTextOrSourcePathIsRequired;

    /// <summary>
    /// Gets the <see cref="MissingSubstring"/> formatted message.
    /// </summary>
    /// <param name="value">
    /// <see langword="string"/> value which was expected to contain the <paramref name="subString"/>.
    /// </param>
    /// <param name="subString">
    /// Expected sub-string.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="MissingSubstring"/> format string.
    /// </returns>
    public static string GetMessageMissingSubString(string value, string subString)
    {
        var messageFormat = MissingSubstring;

        return string.Format(Culture,
                             messageFormat,
                             subString,
                             value);
    }

    /// <summary>
    /// Gets the <see cref="PropertyHasInvalidType"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="PropertyHasInvalidType"/> format string.
    /// </returns>
    public static string GetMessagePropertyHasInvalidType()
        => PropertyHasInvalidType;

    /// <summary>
    /// Gets the <see cref="PropertyNotDeclared"/> formatted message.
    /// </summary>
    /// <param name="propertyName">
    /// Entity's property name.
    /// </param>
    /// <param name="entityType">
    /// Entity type.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="PropertyNotDeclared"/> format string.
    /// </returns>
    public static string GetMessagePropertyNotDeclared(string propertyName, Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        var messageFormat = PropertyNotDeclared;
        var entityTypeName = entityType.Name;

        return string.Format(Culture,
                             messageFormat,
                             propertyName,
                             entityTypeName);
    }

    /// <summary>
    /// Gets the <see cref="RequiredAtLeastOne"/> formatted message.
    /// </summary>
    /// <param name="propertyNames">
    /// Properties to be included in the message.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="RequiredAtLeastOne"/> format string.
    /// </returns>
    public static string GetMessageRequiredAtLeastOne(params IEnumerable<string> propertyNames)
    {
        var messageFormat = RequiredAtLeastOne;
        var properties = string.Join(PropertyNamesSeparator, propertyNames);

        return string.Format(Culture,
                             messageFormat,
                             properties);
    }
    #endregion
}