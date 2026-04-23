using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Paradise.Localization.ExceptionHandling;

[SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "Omitted for code style consistency.")]
public sealed partial class ExceptionMessages
{
    #region Constants
    /// <summary>
    /// The separator value to be used during environment names concatenation.
    /// </summary>
    private const string EnvironmentNamesSeparator = ", ";
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
    /// Gets the <see cref="FailedToCast"/> formatted message.
    /// </summary>
    /// <param name="actualType">
    /// Input type.
    /// </param>
    /// <param name="expectedType">
    /// Expected type.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="FailedToCast"/> format string.
    /// </returns>
    public static string GetMessageFailedToCast(Type? actualType, Type expectedType)
    {
        ArgumentNullException.ThrowIfNull(expectedType);

        var messageFormat = FailedToCast;
        var actualTypeName = actualType?.Name;
        var expectedTypeName = expectedType.Name;

        return string.Format(Culture,
                             messageFormat,
                             actualTypeName,
                             expectedTypeName);
    }

    /// <summary>
    /// Gets the <see cref="FailedToCreateInstanceOfType"/> formatted message.
    /// </summary>
    /// <param name="type">
    /// Input type.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="FailedToCreateInstanceOfType"/> format string.
    /// </returns>
    public static string GetMessageFailedToCreateInstanceOfType(Type type)
    {
        var messageFormat = FailedToCreateInstanceOfType;
        var typeName = type?.Name;

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
    /// Gets the <see cref="MessageTemplateInvalidParametersNumber"/> formatted message.
    /// </summary>
    /// <param name="expectedNumber">
    /// Expected number of placeholders.
    /// </param>
    /// <param name="actualNumber">
    /// Actual number of placeholders.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="MessageTemplateInvalidParametersNumber"/> format string.
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
    /// Gets the <see cref="ResultExceptionMessage"/> formatted message.
    /// </summary>
    /// <param name="details">
    /// Exception message details.
    /// </param>
    /// <param name="status">
    /// Result status.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="ResultExceptionMessage"/> format string.
    /// </returns>
    public static string GetMessageResultExceptionMessage(string details, string status)
    {
        var messageFormat = ResultExceptionMessage;

        return string.Format(Culture,
                             messageFormat,
                             Environment.NewLine,
                             details,
                             status);
    }
    #endregion
}