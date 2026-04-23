using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates.Base;
using Paradise.Tests.Miscellaneous.TestImplementations.Core.ApplicationLogic.Infrastructure.Domain.MessageTemplates.Base;

namespace Paradise.ApplicationLogic.Infrastructure.Domain.Tests.Unit.MessageTemplates.Base;

/// <summary>
/// <see cref="MessageTemplate"/> test class.
/// </summary>
public sealed class MessageTemplateTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="GetFormattedText"/> method.
    /// </summary>
    public static TheoryData<string, string?, ushort, string?[]> GetFormattedText_MemberData { get; } = new()
    {
        { "Test",                           null,       0,  []                              },
        { "Test with {arg}0",               "{arg}",    1,  ["Value"]                       },
        { "Test with {arg}0",               "{arg}",    1,  [null]                          },
        { "Test with {arg}0 and {arg}1",    "{arg}",    2,  ["First value", "Second value"] }
    };

    /// <summary>
    /// Provides member data for <see cref="ValidateState"/> method.
    /// </summary>
    public static TheoryData<string, string?, ushort> ValidateState_MemberData { get; } = new()
    {
        { "Test",                           null,       0   },
        { "Test with {arg}0",               "{arg}",    1   },
        { "Test with {arg}0 and {arg}1",    "{arg}",    2   }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="MessageTemplate.GetEqualityComponents"/> method should
    /// return <see cref="MessageTemplate.TemplateName"/>
    /// and <see cref="MessageTemplate.Culture"/> values.
    /// </summary>
    [Fact]
    public void GetEqualityComponents()
    {
        // Arrange
        var entity = new TestMessageTemplate("TemplateName", null, "TemplateText");

        // Act
        var result = entity.GetEqualityComponents();

        // Assert
        Assert.Contains(entity.TemplateName, result);
        Assert.Contains(entity.Culture, result);

        Assert.Equal(2, result.Count());
    }

    /// <summary>
    /// The <see cref="MessageTemplate.GetFormattedText"/> method should
    /// return a well formatted email subject if the
    /// number of placeholders in <see cref="MessageTemplate.TemplateText"/> is
    /// equal to <see cref="MessageTemplate.PlaceholdersNumber"/> and
    /// the number of input values matches it as well.
    /// </summary>
    /// <param name="templateText">
    /// Template text.
    /// </param>
    /// <param name="placeholderName">
    /// Placeholder name to be replaced with values during the message formatting.
    /// </param>
    /// <param name="placeholdersNumber">
    /// The number of placeholders to be replaced with values during the message formatting.
    /// </param>
    /// <param name="parameters">
    /// <see cref="MessageTemplate.TemplateText"/> formatting values.
    /// </param>
    [Theory, MemberData(nameof(GetFormattedText_MemberData))]
    public void GetFormattedText(string templateText, string? placeholderName, ushort placeholdersNumber, string?[] parameters)
    {
        // Arrange
        var entity = new TestMessageTemplate("TemplateName", null, templateText)
        {
            PlaceholderName = placeholderName,
            PlaceholdersNumber = placeholdersNumber
        };

        // Act
        var result = entity.GetFormattedText(parameters);

        // Assert
        Assert.All(parameters, parameter =>
        {
            if (parameter is not null)
                Assert.Contains(parameter, result, StringComparison.Ordinal);
        });
    }

    /// <summary>
    /// The <see cref="MessageTemplate.GetFormattedText"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// parameters number is not equal to the <see cref="MessageTemplate.PlaceholdersNumber"/>.
    /// </summary>
    [Fact]
    public void GetFormattedText_ThrowsOnInvalidParametersNumber()
    {
        // Arrange
        var parameters = new[] { "{arg}", "{arg}" };
        var invalidPlaceholdersNumber = (ushort)(parameters.Length + 1);

        var entity = new TestMessageTemplate("TemplateName", null, "TemplateText")
        {
            PlaceholderName = "Test",
            PlaceholdersNumber = invalidPlaceholdersNumber
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(()
            => entity.GetFormattedText(parameters));
    }

    /// <summary>
    /// The <see cref="MessageTemplate.GetFormattedText"/> method should
    /// throw the <see cref="FormatException"/> if the
    /// <see cref="MessageTemplate.TemplateText"/> is missing a placeholder.
    /// </summary>
    [Fact]
    public void GetFormattedText_ThrowsOnMissingPlaceholder()
    {
        // Arrange
        var parameters = new[] { "{arg}" };

        var entity = new TestMessageTemplate("TemplateName", null, "TemplateText")
        {
            PlaceholderName = "Test",
            PlaceholdersNumber = (ushort)parameters.Length
        };

        // Act & Assert
        Assert.Throws<FormatException>(()
            => entity.GetFormattedText(parameters));
    }

    /// <summary>
    /// The <see cref="MessageTemplate.GetFormattedText"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the
    /// <see cref="MessageTemplate.PlaceholderName"/> is <see langword="null"/>,
    /// <see cref="MessageTemplate.PlaceholdersNumber"/> equals to 0,
    /// but input parameters number is not equal to 0.
    /// </summary>
    [Fact]
    public void GetFormattedText_ThrowsOnNullPlaceholderNameWithNonZeroParametersNumber()
    {
        // Arrange
        var parameters = new[] { "{arg}" };

        var entity = new TestMessageTemplate("TemplateName", null, "TemplateText")
        {
            PlaceholderName = null,
            PlaceholdersNumber = 0
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(()
            => entity.GetFormattedText(parameters));
    }

    /// <summary>
    /// The <see cref="MessageTemplate.GetFormattedText"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the
    /// <see cref="MessageTemplate.PlaceholderName"/> is <see langword="null"/> and
    /// <see cref="MessageTemplate.PlaceholdersNumber"/> does not equal to 0.
    /// </summary>
    [Fact]
    public void GetFormattedText_ThrowsOnNullPlaceholderNameWithNonZeroPlaceholdersNumber()
    {
        // Arrange
        var parameters = new[] { "{arg}" };

        var entity = new TestMessageTemplate("TemplateName", null, "TemplateText")
        {
            PlaceholderName = null,
            PlaceholdersNumber = (ushort)parameters.Length
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(()
            => entity.GetFormattedText(parameters));
    }

    /// <summary>
    /// The <see cref="MessageTemplate.ValidateState"/> method should
    /// not throw any exception for the <see cref="MessageTemplate"/> instance
    /// which <see cref="MessageTemplate.PlaceholderName"/> is not <see langword="null"/>
    /// and <see cref="MessageTemplate.PlaceholdersNumber"/> does not equal to 0.
    /// </summary>
    /// <param name="templateText">
    /// Template text.
    /// </param>
    /// <param name="placeholderName">
    /// Placeholder name to be replaced with values during the message formatting.
    /// </param>
    /// <param name="placeholdersNumber">
    /// The number of placeholders to be replaced with values during the message formatting.
    /// </param>
    [Theory, MemberData(nameof(ValidateState_MemberData))]
    public void ValidateState(string templateText, string? placeholderName, ushort placeholdersNumber)
    {
        // Arrange
        var entity = new TestMessageTemplate("TemplateName", null, templateText)
        {
            PlaceholderName = placeholderName,
            PlaceholdersNumber = placeholdersNumber
        };

        // Act & Assert
        entity.ValidateState();
    }

    /// <summary>
    /// The <see cref="MessageTemplate.ValidateState"/> method should
    /// throw the <see cref="InvalidOperationException"/> for the
    /// <see cref="MessageTemplate"/> instance
    /// which <see cref="MessageTemplate.PlaceholderName"/> is <see langword="null"/>
    /// but <see cref="MessageTemplate.PlaceholdersNumber"/> does not equal to 0.
    /// </summary>
    [Fact]
    public void ValidateState_ThrowsOnInvalidTemplateTextPlaceholders()
    {
        // Arrange
        var entity = new TestMessageTemplate("TemplateName", null, "TemplateText")
        {
            PlaceholderName = null,
            PlaceholdersNumber = 1
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(entity.ValidateState);
    }

    /// <summary>
    /// The <see cref="MessageTemplate.ValidateState"/> method should
    /// throw the <see cref="InvalidOperationException"/> for the
    /// <see cref="MessageTemplate"/> instance
    /// which <see cref="MessageTemplate.TemplateText"/> does not contain the specified number of placeholders.
    /// </summary>
    [Fact]
    public void ValidateState_ThrowsOnInvalidTemplateTextPlaceholdersNumber()
    {
        // Arrange
        var entity = new TestMessageTemplate("TemplateName", null, "TemplateText")
        {
            PlaceholderName = "{placeholder}",
            PlaceholdersNumber = 1
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(entity.ValidateState);
    }
    #endregion
}