using System.Globalization;
using static Paradise.Localization.ExceptionsHandling.ExceptionMessages;

namespace Paradise.Localization.ExceptionsHandling;

/// <summary>
/// Contains methods providing well-formatted exception messages.
/// </summary>
public static class ExceptionMessagesProvider
{
    #region Constants
    /// <summary>
    /// The separator value to be used during environment names concatenation.
    /// </summary>
    private const string EnvironmentNamesSeparator = ", ";
    #endregion

    #region Public methods
    /// <summary>
    /// Gets the <see cref="ApplicationSecretMissing"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="ApplicationSecretMissing"/> format string.
    /// </returns>
    public static string GetApplicationSecretMissingMessage()
        => ApplicationSecretMissing;

    /// <summary>
    /// Gets the <see cref="EmailTemplateSubjectInInvalidState"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="EmailTemplateSubjectInInvalidState"/> format string.
    /// </returns>
    public static string GetEmailTemplateSubjectInInvalidStateMessage()
        => EmailTemplateSubjectInInvalidState;

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
    public static string GetFailedToCastMessage(Type? actualType, Type expectedType)
    {
        ArgumentNullException.ThrowIfNull(expectedType);

        var messageFormat = FailedToCast;
        var actualTypeName = actualType?.Name;
        var expectedTypeName = expectedType.Name;

        return string.Format(CultureInfo.CurrentCulture,
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
    public static string GetFailedToCreateInstanceOfTypeMessage(Type type)
    {
        var messageFormat = FailedToCreateInstanceOfType;
        var typeName = type?.Name;

        return string.Format(CultureInfo.CurrentCulture,
                             messageFormat,
                             typeName);
    }

    /// <summary>
    /// Gets the <see cref="FailedToDeserialize"/> formatted message.
    /// </summary>
    /// <param name="type">
    /// Input type.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="FailedToDeserialize"/> format string.
    /// </returns>
    public static string GetFailedToDeserializeMessage(Type type)
    {
        var messageFormat = FailedToDeserialize;
        var typeName = type?.Name;

        return string.Format(CultureInfo.CurrentCulture,
                             messageFormat,
                             typeName);
    }

    /// <summary>
    /// Gets the <see cref="InvalidClockWorkerInterval"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="InvalidClockWorkerInterval"/> format string.
    /// </returns>
    public static string GetInvalidClockWorkerIntervalMessage()
        => InvalidClockWorkerInterval;

    /// <summary>
    /// Gets the <see cref="InvalidEntityStateAdditionalInformation"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="InvalidEntityStateAdditionalInformation"/> format string.
    /// </returns>
    public static string GetInvalidEntityStateAdditionalInformationMessage()
        => InvalidEntityStateAdditionalInformation;

    /// <summary>
    /// Gets the <see cref="InvalidEmailAddress"/> formatted message.
    /// </summary>
    /// <param name="email">
    /// Invalid email address.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="InvalidEmailAddress"/> format string.
    /// </returns>
    public static string GetInvalidEmailAddressMessage(string email)
    {
        var messageFromat = InvalidEmailAddress;

        return string.Format(CultureInfo.CurrentCulture,
                             messageFromat,
                             email);
    }

    /// <summary>
    /// Gets the <see cref="InvalidEntityState"/> formatted message.
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
    /// A formatted message using the <see cref="InvalidEntityState"/> format string.
    /// </returns>
    public static string GetInvalidEntityStateMessage(Type entityType, string? propertyName, object? value)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        var messageFormat = InvalidEntityState;
        var entityTypeName = entityType.Name;

        return string.Format(CultureInfo.CurrentCulture,
                             messageFormat,
                             entityTypeName,
                             propertyName,
                             value);
    }

    /// <summary>
    /// Gets the <see cref="InvalidEnvironmentName"/> formatted message.
    /// </summary>
    /// <param name="allowedEnvironments">
    /// The list of allowed environment names.
    /// </param>
    /// <param name="currentEnvironment">
    /// Current environment name.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="InvalidEnvironmentName"/> format string.
    /// </returns>
    public static string GetInvalidEnvironmentNameMessage(IEnumerable<string> allowedEnvironments, string? currentEnvironment)
    {
        var messageFormat = InvalidEnvironmentName;
        var environments = string.Join(EnvironmentNamesSeparator, allowedEnvironments);

        return string.Format(CultureInfo.CurrentCulture,
                             messageFormat,
                             currentEnvironment,
                             environments);
    }

    /// <summary>
    /// Gets the <see cref="InvalidEqualityComparerConstructor"/> formatted message.
    /// </summary>
    /// <param name="type">
    /// Equality comparer type.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="InvalidEqualityComparerConstructor"/> format string.
    /// </returns>
    public static string GetInvalidEqualityComparerConstructorMessage(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var messageFormat = InvalidEqualityComparerConstructor;
        var typeName = type.Name;

        return string.Format(CultureInfo.CurrentCulture,
                             messageFormat,
                             typeName);
    }

    /// <summary>
    /// Gets the <see cref="InvalidEqualityComparerType"/> formatted message.
    /// </summary>
    /// <param name="type">
    /// Equality comparer type.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="InvalidEqualityComparerType"/> format string.
    /// </returns>
    public static string GetInvalidEqualityComparerTypeMessage(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var messageFormat = InvalidEqualityComparerType;
        var typeName = type.Name;

        return string.Format(CultureInfo.CurrentCulture,
                             messageFormat,
                             typeName);
    }

    /// <summary>
    /// Gets the <see cref="InvalidParametersNumber"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="InvalidParametersNumber"/> format string.
    /// </returns>
    public static string GetInvalidParametersNumberMessage()
        => InvalidParametersNumber;

    /// <summary>
    /// Gets the <see cref="InvalidPropertyType"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="InvalidPropertyType"/> format string.
    /// </returns>
    public static string GetInvalidPropertyTypeMessage()
        => InvalidPropertyType;

    /// <summary>
    /// Gets the <see cref="IvalidSeedData"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="IvalidSeedData"/> format string.
    /// </returns>
    public static string GetIvalidSeedDataMessage()
        => IvalidSeedData;

    /// <summary>
    /// Gets the <see cref="InvalidSwaggerConfiguration"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="InvalidSwaggerConfiguration"/> format string.
    /// </returns>
    public static string GetInvalidSwaggerConfigurationMessage()
        => InvalidSwaggerConfiguration;

    /// <summary>
    /// Gets the <see cref="MessageTemplateTextInInvalidState"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="MessageTemplateTextInInvalidState"/> format string.
    /// </returns>
    public static string GetMessageTemplateTextInInvalidStateMessage()
        => MessageTemplateTextInInvalidState;

    /// <summary>
    /// Gets the <see cref="PlaceholderNotExists"/> formatted message.
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
    /// A formatted message using the <see cref="PlaceholderNotExists"/> format string.
    /// </returns>
    public static string GetPlaceholderNotExistsMessage(string templateName, CultureInfo? culture, string? placeholder)
    {
        var messageFormat = PlaceholderNotExists;
        var cultureName = culture?.Name;

        return string.Format(CultureInfo.CurrentCulture,
                             messageFormat,
                             templateName,
                             cultureName,
                             placeholder);
    }

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
    public static string GetPropertyNotDeclaredMessage(string propertyName, Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        var messageFormat = PropertyNotDeclared;
        var entityTypeName = entityType.Name;

        return string.Format(CultureInfo.CurrentCulture,
                             messageFormat,
                             propertyName,
                             entityTypeName);
    }

    /// <summary>
    /// Gets the <see cref="TemplateTextOrSourcePathIsRequired"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="TemplateTextOrSourcePathIsRequired"/> format string.
    /// </returns>
    public static string GetTemplateTextOrSourcePathIsRequiredMessage()
        => TemplateTextOrSourcePathIsRequired;

    /// <summary>
    /// Gets the <see cref="ValueCanNotBeLessOrEqualToZero"/> formatted message.
    /// </summary>
    /// <param name="propertyName">
    /// Property name, which value is less or equal to zero.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="ValueCanNotBeLessOrEqualToZero"/> format string.
    /// </returns>
    public static string GetValueCanNotBeLessOrEqualToZeroMessage(string? propertyName)
    {
        var messageFormat = ValueCanNotBeLessOrEqualToZero;

        return string.Format(CultureInfo.CurrentCulture,
                             messageFormat,
                             propertyName);
    }

    /// <summary>
    /// Gets the <see cref="ChangeTimerFailed"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="ChangeTimerFailed"/> format string.
    /// </returns>
    public static string GetChangeTimerFailedMessage()
        => ChangeTimerFailed;
    #endregion
}