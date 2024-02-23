namespace Paradise.ApplicationLogic.Domain.Tests.MessageTemplates.Base;

/// <summary>
/// Test class for the <see cref="MessageTemplate"/>.
/// </summary>
public sealed class MessageTemplateTests
{
    #region Public methods
    /// <summary>
    /// <see cref="MessageTemplate.GetFormattedText"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Message template text is returned in it's formatted state,
    /// which means all placeholders within are replaced with the
    /// values from the input parameters.
    /// </para>
    /// </para>
    /// </summary>
    [Theory, MemberData(nameof(GetFormattedText_MemberData))]
    public void GetFormattedText(string templateText, string? placeholderName, ushort placeholdersNumber, IList<object?> parameters)
    {
        // Arrange
        var messageTemplate = new FakeMessageTemplate("Template name", templateText)
        {
            PlaceholderName = placeholderName,
            PlaceholdersNumber = placeholdersNumber
        };

        // Act
        var formatterText = messageTemplate.GetFormattedText(parameters);

        // Assert
        Assert.All(parameters, parameter => Assert.Contains(parameter!.ToString()!, formatterText, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// <see cref="MessageTemplate.GetFormattedText"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Original text should be returned since the <see cref="MessageTemplate.PlaceholderName"/>
    /// is <see langword="null"/> and <see cref="MessageTemplate.PlaceholdersNumber"/> is equal to 0.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetFormattedText_ReturnsPlainTextOnEmptyParameters()
    {
        // Arrange
        var messageTemplate = new FakeMessageTemplate("Template name", "Template text")
        {
            PlaceholderName = null,
            PlaceholdersNumber = 0
        };

        // Act
        var formatterText = messageTemplate.GetFormattedText([]);

        // Assert
        Assert.Equal(messageTemplate.TemplateText, formatterText);
    }

    /// <summary>
    /// <see cref="MessageTemplate.GetFormattedText"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="IndexOutOfRangeException"/> since the parameters number
    /// is not equal to <see cref="MessageTemplate.PlaceholdersNumber"/>.
    /// </para>
    /// </summary>
    [Fact]
    public void GetFormattedText_ThrowsOnInvalidParametersNumber()
    {
        // Arrange
        var parameters = new[] { "{arg}", "{arg}" };

        var messageTemplate = new FakeMessageTemplate("Template name", "Template text")
        {
            PlaceholderName = "Test",
            PlaceholdersNumber = (ushort)(parameters.Length + 1)
        };

        // Act

        // Assert
        Assert.Throws<IndexOutOfRangeException>(()
            => messageTemplate.GetFormattedText(parameters));
    }

    /// <summary>
    /// <see cref="MessageTemplate.GetFormattedText"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="FormatException"/> since there is no such parameter
    /// inside the <see cref="MessageTemplate.TemplateText"/>.
    /// </para>
    /// </summary>
    [Fact]
    public void GetFormattedText_ThrowsOnMissingPlaceholder()
    {
        // Arrange
        var parameters = new[] { "{arg}" };

        var messageTemplate = new FakeMessageTemplate("Template name", "Template text")
        {
            PlaceholderName = "Test",
            PlaceholdersNumber = (ushort)parameters.Length
        };

        // Act

        // Assert
        Assert.Throws<FormatException>(()
            => messageTemplate.GetFormattedText(parameters));
    }

    /// <summary>
    /// <see cref="MessageTemplate.GetFormattedText"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ArgumentException"/> since the
    /// <see cref="MessageTemplate.PlaceholderName"/> is <see langword="null"/>,
    /// <see cref="MessageTemplate.PlaceholdersNumber"/> equals to 0,
    /// but input parameters number is not equal to 0.
    /// </para>
    /// </summary>
    [Fact]
    public void GetFormattedText_ThrowsOnNullPlaceholderNameWithNonZeroParametersNumber()
    {
        // Arrange
        var parameters = new[] { "{arg}" };

        var messageTemplate = new FakeMessageTemplate("Template name", "Template text")
        {
            PlaceholderName = null,
            PlaceholdersNumber = 0
        };

        // Act

        // Assert
        Assert.Throws<ArgumentException>(()
            => messageTemplate.GetFormattedText(parameters));
    }

    /// <summary>
    /// <see cref="MessageTemplate.GetFormattedText"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="InvalidEntityStateException"/> since the
    /// <see cref="MessageTemplate.PlaceholderName"/> is <see langword="null"/>,
    /// but <see cref="MessageTemplate.PlaceholdersNumber"/> is not equal to 0.
    /// </para>
    /// </summary>
    [Fact]
    public void GetFormattedText_ThrowsOnNullPlaceholderNameWithNonZeroPlaceholdersNumber()
    {
        // Arrange
        var parameters = new[] { "{arg}" };

        var messageTemplate = new FakeMessageTemplate("Template name", "Template text")
        {
            PlaceholderName = null,
            PlaceholdersNumber = (ushort)parameters.Length
        };

        // Act

        // Assert
        Assert.Throws<InvalidEntityStateException>(()
            => messageTemplate.GetFormattedText(parameters));
    }

    /// <summary>
    /// <see cref="MessageTemplate.ValidateState"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// No exception is thrown because message template
    /// is in it's valid state.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void ValidateState()
    {
        // Arrange
        var messageTemplate = new FakeMessageTemplate("Template name", "Template text")
        {
            PlaceholderName = "{arg}",
            PlaceholdersNumber = 1
        };

        // Act
        messageTemplate.ValidateState();

        // Assert
    }

    /// <summary>
    /// <see cref="MessageTemplate.ValidateState"/> test method.
    /// <para>
    /// throws a <see cref="InvalidEntityStateException"/> since the
    /// <see cref="MessageTemplate.PlaceholderName"/> is <see langword="null"/>,
    /// but <see cref="MessageTemplate.PlaceholdersNumber"/> is not equal to 0.
    /// </para>
    /// </summary>
    [Fact]
    public void ValidateState_ThrowsOnInvalidState()
    {
        // Arrange
        var messageTemplate = new FakeMessageTemplate("Template name", "Template text")
        {
            PlaceholderName = null,
            PlaceholdersNumber = 1
        };

        // Act

        // Assert
        Assert.Throws<InvalidEntityStateException>(
            messageTemplate.ValidateState);
    }
    #endregion

    #region Data generation
    /// <summary>
    /// Provides member data for <see cref="GetFormattedText"/> method.
    /// </summary>
    public static TheoryData<string, string?, ushort, List<object?>> GetFormattedText_MemberData => new()
    {
        { "Test",                           null,       0,  [] },
        { "Test with {arg}0",               "{arg}",    1,  ["Value"] },
        { "Test with {arg}0 and {arg}1",    "{arg}",    2,  ["First value", "Second value"] }
    };
    #endregion
}