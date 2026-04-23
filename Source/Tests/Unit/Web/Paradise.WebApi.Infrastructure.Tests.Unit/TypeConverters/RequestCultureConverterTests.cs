using Microsoft.AspNetCore.Localization;
using Paradise.WebApi.Infrastructure.TypeConverters;
using System.Globalization;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.TypeConverters;

/// <summary>
/// <see cref="RequestCultureConverter"/> test class.
/// </summary>
public sealed class RequestCultureConverterTests
{
    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    public RequestCultureConverter Converter { get; } = new();

    /// <summary>
    /// Provides member data for <see cref="ConvertFrom"/> method.
    /// </summary>
    public static TheoryData<string, CultureInfo, CultureInfo> ConvertFrom_MemberData { get; } = new()
    {
        { "en-US|fr-FR",    CultureInfo.GetCultureInfo("en-US"),    CultureInfo.GetCultureInfo("fr-FR") },
        { "de-DE|en-US",    CultureInfo.GetCultureInfo("de-DE"),    CultureInfo.GetCultureInfo("en-US") },
        { "en-US",          CultureInfo.GetCultureInfo("en-US"),    CultureInfo.GetCultureInfo("en-US") }
    };

    /// <summary>
    /// Provides member data for <see cref="ConvertTo"/> method.
    /// </summary>
    public static TheoryData<CultureInfo, CultureInfo?, string> ConvertTo_MemberData { get; } = new()
    {
        { CultureInfo.GetCultureInfo("en-US"),  CultureInfo.GetCultureInfo("en-US"),    "en-US"         },
        { CultureInfo.GetCultureInfo("en-US"),  null,                                   "en-US"         },
        { CultureInfo.GetCultureInfo("en-US"),  CultureInfo.GetCultureInfo("de-DE"),    "en-US|de-DE"   },
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="RequestCultureConverter.CanConvertFrom"/> method should
    /// return <see langword="true"/> if the input
    /// target type is <see langword="string"/>.
    /// </summary>
    [Fact]
    public void CanConvertFrom_ReturnsTrueOnString()
    {
        // Arrange
        var sourceType = typeof(string);

        // Act
        var result = Converter.CanConvertFrom(null, sourceType);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="RequestCultureConverter.CanConvertFrom"/> method should
    /// return <see langword="false"/> if the input
    /// target type is not <see langword="string"/>.
    /// </summary>
    [Fact]
    public void CanConvertFrom_ReturnsFalseOnNonString()
    {
        // Arrange
        var sourceType = typeof(int);

        // Act
        var result = Converter.CanConvertFrom(null, sourceType);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="RequestCultureConverter.CanConvertTo"/> method should
    /// return <see langword="true"/> if the input
    /// target type is <see cref="string"/>.
    /// </summary>
    [Fact]
    public void CanConvertTo_ReturnsTrueOnString()
    {
        // Arrange
        var sourceType = typeof(string);

        // Act
        var result = Converter.CanConvertTo(null, sourceType);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="RequestCultureConverter.CanConvertTo"/> method should
    /// return <see langword="false"/> if the input
    /// target type is not <see cref="string"/>.
    /// </summary>
    [Fact]
    public void CanConvertTo_ReturnsFalseOnNonString()
    {
        // Arrange
        var sourceType = typeof(int);

        // Act
        var result = Converter.CanConvertTo(null, sourceType);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="RequestCultureConverter.ConvertFrom"/> method should
    /// return <see langword="null"/> if the input
    /// <see cref="object"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void ConvertFrom_ReturnsNullOnNull()
    {
        // Arrange
        var value = null as object;

        // Act
        var result = Converter.ConvertFrom(null, null, value!);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// The <see cref="RequestCultureConverter.ConvertFrom"/> method should
    /// convert the input string into a <see cref="RequestCulture"/> instance.
    /// </summary>
    /// <param name="value">
    /// The <see cref="object"/> to convert.
    /// </param>
    /// <param name="expectedCulture">
    /// Expected culture.
    /// </param>
    /// <param name="expectedUiCulture">
    /// Expected UI culture.
    /// </param>
    [Theory, MemberData(nameof(ConvertFrom_MemberData))]
    public void ConvertFrom(string value, CultureInfo expectedCulture, CultureInfo expectedUiCulture)
    {
        // Arrange

        // Act
        var result = Converter.ConvertFrom(null, null, value);

        // Assert
        var requestCulture = Assert.IsType<RequestCulture>(result);
        Assert.Equal(expectedCulture, requestCulture.Culture);
        Assert.Equal(expectedUiCulture, requestCulture.UICulture);
    }

    /// <summary>
    /// The <see cref="RequestCultureConverter.ConvertFrom"/> method should
    /// throw the <see cref="FormatException"/> if the input
    /// string value has invalid format.
    /// </summary>
    [Fact]
    public void ConvertFrom_ThrowsOnInvalidValue()
    {
        // Arrange
        var value = "en-US|fr-FR|de-DE";

        // Act & Assert
        Assert.Throws<FormatException>(()
            => Converter.ConvertFrom(context: null, culture: null, value: value));
    }

    /// <summary>
    /// The <see cref="RequestCultureConverter.ConvertFrom"/> method should
    /// throw the <see cref="NotSupportedException"/> if the input
    /// value has invalid type.
    /// </summary>
    [Fact]
    public void ConvertFrom_ThrowsOnInvalidValueType()
    {
        // Arrange
        var value = new object();

        // Act & Assert
        Assert.Throws<NotSupportedException>(()
            => Converter.ConvertFrom(context: null, culture: null, value: value));
    }

    /// <summary>
    /// The <see cref="RequestCultureConverter.ConvertTo"/> method should
    /// convert the input <see cref="RequestCulture"/> instance into a string.
    /// </summary>
    /// <param name="culture">
    /// The <see cref="CultureInfo"/> for the request to be used for formatting.
    /// </param>
    /// <param name="uiCulture">
    /// The <see cref="CultureInfo"/> for the request to be used for text, i.e. language.
    /// </param>
    /// <param name="expectedResult">
    /// Expected result.
    /// </param>
    [Theory, MemberData(nameof(ConvertTo_MemberData))]
    public void ConvertTo(CultureInfo culture, CultureInfo? uiCulture, string expectedResult)
    {
        // Arrange
        var requestCulture = uiCulture is not null
            ? new RequestCulture(culture, uiCulture)
            : new RequestCulture(culture);

        // Act
        var result = Converter.ConvertTo(null, null, requestCulture, typeof(string));

        // Assert
        Assert.Equal(expectedResult, result);
    }

    /// <summary>
    /// The <see cref="RequestCultureConverter.ConvertTo"/> method should
    /// throw the <see cref="NotSupportedException"/> if the input
    /// value has invalid type.
    /// </summary>
    [Fact]
    public void ConvertTo_ThrowsOnInvalidValueType()
    {
        // Arrange
        var value = new object();

        // Act & Assert
        Assert.Throws<NotSupportedException>(()
            => Converter.ConvertTo(null, null, value, typeof(object)));
    }
    #endregion
}