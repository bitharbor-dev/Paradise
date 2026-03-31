namespace Paradise.Localization.DataValidation;

/// <summary>
/// Provides localized and formatted validation messages.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ValidationMessages"/> class.
/// </remarks>
public sealed partial class ValidationMessages
{
    #region Constants
    /// <summary>
    /// The separator value to be used during property names concatenation.
    /// </summary>
    private const string PropertyNamesSeparator = ", ";
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
    public static string GetMessageObjectIsNull(string parameterName)
    {
        var messageFormat = ObjectIsNull;

        return string.Format(Culture,
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