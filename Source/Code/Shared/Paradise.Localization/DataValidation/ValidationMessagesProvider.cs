using System.Globalization;
using System.Text;
using static Paradise.Localization.DataValidation.ValidationMessages;
using static System.Text.CompositeFormat;

namespace Paradise.Localization.DataValidation;

/// <summary>
/// Contains methods providing well-formatted validation messages.
/// </summary>
public static class ValidationMessagesProvider
{
    #region Constants
    /// <summary>
    /// The separator value to be used during property names concatenation.
    /// </summary>
    private const string PropertyNamesSeparator = ", ";
    #endregion

    #region Fields
    private static readonly CompositeFormat _objectIsNull       = Parse(ObjectIsNull);
    private static readonly CompositeFormat _requiredAtLeastOne = Parse(RequiredAtLeastOne);
    #endregion

    #region Public methods
    /// <summary>
    /// Gets the <see cref="ObjectIsNull"/> formatted message.
    /// </summary>
    /// <param name="parameterName">
    /// Parameter name.
    /// </param>
    /// <returns>
    /// A formatted message using the <see cref="ObjectIsNull"/> format string.
    /// </returns>
    public static string GetObjectIsNullMessage(string parameterName)
    {
        var messageFormat = _objectIsNull;

        return string.Format(CultureInfo.CurrentCulture,
                             messageFormat,
                             parameterName);
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
    public static string GetRequiredAtLeastOneMessage(params string[] propertyNames)
    {
        var messageFormat = _requiredAtLeastOne;
        var properties = string.Join(PropertyNamesSeparator, propertyNames);

        return string.Format(CultureInfo.CurrentCulture,
                             messageFormat,
                             properties);
    }
    #endregion
}