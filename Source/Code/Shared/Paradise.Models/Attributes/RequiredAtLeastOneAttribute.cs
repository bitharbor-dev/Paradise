using Paradise.Common.Extensions;
using System.ComponentModel.DataAnnotations;
using static Paradise.Localization.DataValidation.ValidationMessages;

namespace Paradise.Models.Attributes;

/// <summary>
/// Provides "at least one required" functionality for the class properties.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RequiredAtLeastOneAttribute"/> class
/// with the specified names of the properties that are required.
/// </remarks>
/// <param name="restrictEmptyOrWhitespaceStrings">
/// Indicates whether the empty or whitespace strings would be treated as an invalid value.
/// </param>
/// <param name="propertyNames">
/// The names of the properties that are required.
/// </param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class RequiredAtLeastOneAttribute(bool restrictEmptyOrWhitespaceStrings, params string[] propertyNames) : ValidationAttribute
{
    #region Properties
    /// <summary>
    /// Indicates whether the empty or whitespace strings would be treated as an invalid value.
    /// </summary>
    public bool RestrictEmptyOrWhitespaceStrings { get; } = restrictEmptyOrWhitespaceStrings;

    /// <summary>
    /// The names of the properties that are required.
    /// </summary>
    public IEnumerable<string> PropertyNames { get; } = propertyNames;
    #endregion

    #region Protected methods
    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            var error = GetMessageObjectIsNull(nameof(value));

            return new ValidationResult(error);
        }

        var properties = GetPropertyValues(value, PropertyNames);

        if (properties.All(IsValueNullOrEmptyString))
        {
            var error = GetMessageRequiredAtLeastOne(PropertyNames);

            return new ValidationResult(error);
        }

        return ValidationResult.Success;
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Determines whether the given <paramref name="value"/>
    /// is an empty <see cref="string"/> or <see langword="null"/>.
    /// </summary>
    /// <param name="value">
    /// The value to be checked.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given equals <see langword="null"/>
    /// or <see cref="string.Empty"/>, otherwise - <see langword="false"/>.
    /// </returns>
    private bool IsValueNullOrEmptyString(object? value)
        => RestrictEmptyOrWhitespaceStrings
        ? value is null || (value is string str && str.IsNullOrWhiteSpace())
        : value is null;

    /// <summary>
    /// Gets the <paramref name="object"/> properties values
    /// by the given <paramref name="properties"/>.
    /// </summary>
    /// <param name="object">
    /// The object to get the values from.
    /// </param>
    /// <param name="properties">
    /// Property names array.
    /// </param>
    /// <returns>
    /// The values of the <paramref name="object"/> properties
    /// which names match the given <paramref name="properties"/>.
    /// </returns>
    private static IEnumerable<object?> GetPropertyValues(object @object, IEnumerable<string> properties)
    {
        var type = @object.GetType();

        return properties.Select(propertyName =>
        {
            var property = type.GetProperty(propertyName);

            return property is not null
                ? property.GetValue(@object)
                : throw new MemberAccessException();
        });
    }
    #endregion
}