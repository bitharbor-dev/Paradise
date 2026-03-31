using Microsoft.AspNetCore.Localization;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using static Paradise.Localization.ExceptionHandling.ExceptionMessages;

namespace Paradise.WebApi.Infrastructure.TypeConverters;

/// <summary>
/// Converts a string representation of a request culture into a <see cref="RequestCulture"/> instance.
/// </summary>
/// <remarks>
/// The expected format for the string is either:
/// <list type="bullet">
/// <item>
/// <c>"en-US"</c> – same culture and UI culture.
/// </item>
/// <item>
/// <c>"en-US|fr-FR"</c> – separate culture and UI culture.
/// </item>
/// </list>
/// The delimiter between culture and UI culture is the pipe '<c>|</c>' character.
/// </remarks>
public sealed class RequestCultureConverter : TypeConverter
{
    #region Constants
    private const char Splitter = '|';
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    /// <inheritdoc/>
    public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType)
        => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

    /// <inheritdoc/>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is null)
            return null;

        if (value is not string stringValue)
            throw CreateConvertFromException(value);

        var parts = stringValue.Split(Splitter, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return parts.Length switch
        {
            1 => new RequestCulture(parts[0]),
            2 => new RequestCulture(parts[0], parts[1]),
            _ => throw new FormatException(GetMessageFailedToCreateInstanceOfType(typeof(RequestCulture)))
        };
    }

    /// <inheritdoc/>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is not RequestCulture requestCulture)
            throw CreateConvertToException(value, destinationType);

        var cultureName = requestCulture.Culture.Name;
        var uiCultureName = requestCulture.UICulture.Name;

        return string.Equals(cultureName, uiCultureName, StringComparison.OrdinalIgnoreCase)
            ? cultureName
            : $"{cultureName}{Splitter}{uiCultureName}";
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Returns the <see cref="NotSupportedException"/> produced by
    /// <see cref="TypeConverter.GetConvertFromException"/> without
    /// leaking the internal throw behavior.
    /// </summary>
    /// <remarks>
    /// This helper exists solely due to <see href="https://github.com/dotnet/runtime/issues/122790">dotnet/runtime#122790</see>.
    /// Remove when <see cref="TypeConverter.GetConvertFromException"/> is fixed or annotated with <see cref="DoesNotReturnAttribute"/>.
    /// </remarks>
    /// <param name="value">
    /// The value that failed conversion.
    /// </param>
    /// <returns>
    /// An <see cref="Exception"/> that represents the exception to throw when a conversion cannot be performed.
    /// </returns>
    [ExcludeFromCodeCoverage(Justification = "https://github.com/dotnet/runtime/issues/122790")]
    private NotSupportedException CreateConvertFromException(object value)
    {
        NotSupportedException? realException = null;

        try
        {
            GetConvertFromException(value);
        }
        catch (NotSupportedException exception)
        {
            realException = new(exception.Message);
        }

        return realException!;
    }

    /// <summary>
    /// Returns the <see cref="NotSupportedException"/> produced by
    /// <see cref="TypeConverter.GetConvertToException"/> without
    /// leaking the internal throw behavior.
    /// </summary>
    /// <remarks>
    /// This helper exists solely due to <see href="https://github.com/dotnet/runtime/issues/122790">dotnet/runtime#122790</see>.
    /// Remove when <see cref="TypeConverter.GetConvertToException"/> is fixed or annotated with <see cref="DoesNotReturnAttribute"/>.
    /// </remarks>
    /// <param name="value">
    /// The value that failed conversion.
    /// </param>
    /// <param name="destinationType">
    /// The destination type.
    /// </param>
    /// <returns>
    /// An <see cref="Exception"/> that represents the exception to throw when a conversion cannot be performed.
    /// </returns>
    [ExcludeFromCodeCoverage(Justification = "https://github.com/dotnet/runtime/issues/122790")]
    private NotSupportedException CreateConvertToException(object? value, Type destinationType)
    {
        NotSupportedException? realException = null;

        try
        {
            GetConvertToException(value, destinationType);
        }
        catch (NotSupportedException exception)
        {
            realException = new(exception.Message);
        }

        return realException!;
    }
    #endregion
}