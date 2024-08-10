using System.Globalization;
using System.Text;
using static System.Text.CompositeFormat;
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

    #region Fields
    private static readonly CompositeFormat _applicationSecretMissing                   = Parse(ApplicationSecretMissing);
    private static readonly CompositeFormat _changeTimerFailed                          = Parse(ChangeTimerFailed);
    private static readonly CompositeFormat _emailTemplateSubjectInInvalidState         = Parse(EmailTemplateSubjectInInvalidState);
    private static readonly CompositeFormat _failedToCast                               = Parse(FailedToCast);
    private static readonly CompositeFormat _failedToCreateInstanceOfType               = Parse(FailedToCreateInstanceOfType);
    private static readonly CompositeFormat _failedToDeserialize                        = Parse(FailedToDeserialize);
    private static readonly CompositeFormat _invalidClockWorkerInterval                 = Parse(InvalidClockWorkerInterval);
    private static readonly CompositeFormat _invalidEntityStateAdditionalInformation    = Parse(InvalidEntityStateAdditionalInformation);
    private static readonly CompositeFormat _invalidEmailAddress                        = Parse(InvalidEmailAddress);
    private static readonly CompositeFormat _invalidEntityState                         = Parse(InvalidEntityState);
    private static readonly CompositeFormat _invalidEnvironmentName                     = Parse(InvalidEnvironmentName);
    private static readonly CompositeFormat _invalidEqualityComparerConstructor         = Parse(InvalidEqualityComparerConstructor);
    private static readonly CompositeFormat _invalidEqualityComparerType                = Parse(InvalidEqualityComparerType);
    private static readonly CompositeFormat _invalidParametersNumber                    = Parse(InvalidParametersNumber);
    private static readonly CompositeFormat _invalidPropertyType                        = Parse(InvalidPropertyType);
    private static readonly CompositeFormat _ivalidSeedData                             = Parse(IvalidSeedData);
    private static readonly CompositeFormat _invalidSwaggerConfiguration                = Parse(InvalidSwaggerConfiguration);
    private static readonly CompositeFormat _messageTemplateTextInInvalidState          = Parse(MessageTemplateTextInInvalidState);
    private static readonly CompositeFormat _placeholderNotExists                       = Parse(PlaceholderNotExists);
    private static readonly CompositeFormat _propertyNotDeclared                        = Parse(PropertyNotDeclared);
    private static readonly CompositeFormat _templateTextOrSourcePathIsRequired         = Parse(TemplateTextOrSourcePathIsRequired);
    private static readonly CompositeFormat _valueCanNotBeLessOrEqualToZero             = Parse(ValueCanNotBeLessOrEqualToZero);
    #endregion

    #region Public methods
    /// <summary>
    /// Gets the <see cref="ApplicationSecretMissing"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="ApplicationSecretMissing"/> format string.
    /// </returns>
    public static string GetApplicationSecretMissingMessage()
        => _applicationSecretMissing.Format;

    /// <summary>
    /// Gets the <see cref="ChangeTimerFailed"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="ChangeTimerFailed"/> format string.
    /// </returns>
    public static string GetChangeTimerFailedMessage()
        => _changeTimerFailed.Format;

    /// <summary>
    /// Gets the <see cref="EmailTemplateSubjectInInvalidState"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="EmailTemplateSubjectInInvalidState"/> format string.
    /// </returns>
    public static string GetEmailTemplateSubjectInInvalidStateMessage()
        => _emailTemplateSubjectInInvalidState.Format;

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

        var messageFormat = _failedToCast;
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
        var messageFormat = _failedToCreateInstanceOfType;
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
        var messageFormat = _failedToDeserialize;
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
        => _invalidClockWorkerInterval.Format;

    /// <summary>
    /// Gets the <see cref="InvalidEntityStateAdditionalInformation"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="InvalidEntityStateAdditionalInformation"/> format string.
    /// </returns>
    public static string GetInvalidEntityStateAdditionalInformationMessage()
        => _invalidEntityStateAdditionalInformation.Format;

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
        var messageFromat = _invalidEmailAddress;

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

        var messageFormat = _invalidEntityState;
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
        var messageFormat = _invalidEnvironmentName;
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

        var messageFormat = _invalidEqualityComparerConstructor;
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

        var messageFormat = _invalidEqualityComparerType;
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
        => _invalidParametersNumber.Format;

    /// <summary>
    /// Gets the <see cref="InvalidPropertyType"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="InvalidPropertyType"/> format string.
    /// </returns>
    public static string GetInvalidPropertyTypeMessage()
        => _invalidPropertyType.Format;

    /// <summary>
    /// Gets the <see cref="IvalidSeedData"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="IvalidSeedData"/> format string.
    /// </returns>
    public static string GetIvalidSeedDataMessage()
        => _ivalidSeedData.Format;

    /// <summary>
    /// Gets the <see cref="InvalidSwaggerConfiguration"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="InvalidSwaggerConfiguration"/> format string.
    /// </returns>
    public static string GetInvalidSwaggerConfigurationMessage()
        => _invalidSwaggerConfiguration.Format;

    /// <summary>
    /// Gets the <see cref="MessageTemplateTextInInvalidState"/> formatted message.
    /// </summary>
    /// <returns>
    /// A formatted message using the <see cref="MessageTemplateTextInInvalidState"/> format string.
    /// </returns>
    public static string GetMessageTemplateTextInInvalidStateMessage()
        => _messageTemplateTextInInvalidState.Format;

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
        var messageFormat = _placeholderNotExists;
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

        var messageFormat = _propertyNotDeclared;
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
        => _templateTextOrSourcePathIsRequired.Format;

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
        var messageFormat = _valueCanNotBeLessOrEqualToZero;

        return string.Format(CultureInfo.CurrentCulture,
                             messageFormat,
                             propertyName);
    }
    #endregion
}