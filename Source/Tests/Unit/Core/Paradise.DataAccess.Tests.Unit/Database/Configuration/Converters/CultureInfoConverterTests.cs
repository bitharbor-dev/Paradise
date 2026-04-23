using Paradise.DataAccess.Database.Configuration.Converters;
using System.Globalization;

namespace Paradise.DataAccess.Tests.Unit.Database.Configuration.Converters;

/// <summary>
/// <see cref="CultureInfoConverter"/> test class.
/// </summary>
public sealed class CultureInfoConverterTests
{
    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    internal CultureInfoConverter Converter { get; } = new();

    /// <summary>
    /// Provides member data for <see cref="ConvertTo"/> method.
    /// </summary>
    public static TheoryData<CultureInfo?> ConvertTo_MemberData { get; } = new()
    {
        { null as CultureInfo           },
        { CultureInfo.InvariantCulture  },
        { new CultureInfo("en-US")      }
    };

    /// <summary>
    /// Provides member data for <see cref="ConvertFrom"/> method.
    /// </summary>
    public static TheoryData<int?> ConvertFrom_MemberData { get; } = new()
    {
        { null as int?                      },
        { CultureInfo.InvariantCulture.LCID },
        { new CultureInfo("en-US").LCID     }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="CultureInfoConverter"/> 'ConvertTo' delegate should
    /// convert the input nullable <see cref="CultureInfo"/> instances
    /// into the nullable <see cref="int"/> values.
    /// </summary>
    /// <param name="culture">
    /// The <see cref="CultureInfo"/> to convert.
    /// </param>
    [Theory, MemberData(nameof(ConvertTo_MemberData))]
    public void ConvertTo(CultureInfo? culture)
    {
        // Arrange

        // Act
        var result = Converter.ConvertToProvider(culture);

        // Assert
        if (culture is null)
        {
            Assert.Null(result);
        }
        else
        {
            var languageCodeId = Assert.IsType<int>(result);
            Assert.Equal(culture.LCID, languageCodeId);
        }
    }

    /// <summary>
    /// The <see cref="CultureInfoConverter"/> 'ConvertFrom' delegate should
    /// convert the input nullable <see cref="int"/> values
    /// into the nullable <see cref="CultureInfo"/> instances.
    /// </summary>
    /// <param name="languageCodeId">
    /// The <see cref="int"/> to convert.
    /// </param>
    [Theory, MemberData(nameof(ConvertFrom_MemberData))]
    public void ConvertFrom(int? languageCodeId)
    {
        // Arrange

        // Act
        var result = Converter.ConvertFromProvider(languageCodeId);

        // Assert
        if (languageCodeId is null)
        {
            Assert.Null(result);
        }
        else
        {
            var culture = Assert.IsType<CultureInfo>(result);
            Assert.Equal(languageCodeId, culture.LCID);
        }
    }
    #endregion
}