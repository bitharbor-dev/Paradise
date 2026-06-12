using Paradise.Tests.Doubles.Dummies.Core.Domain.Base;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Base.Tests.Unit;

/// <summary>
/// <see cref="DomainStateError"/> test class.
/// </summary>
public sealed class DomainStateErrorTests
{
    #region Constants
    [StringSyntax("Regex")]
    private const string DefaultPattern = "^The object of type '.*' is in an invalid state\\. Property name: '.*', value: '.*'\\.";

    [StringSyntax("Regex")]
    private const string AdditionalInformationPattern = " Additional information:\\r?\\n.*$";

    [StringSyntax("Regex")]
    private const string NullValuePattern = "^The object of type '.*' is in an invalid state\\. Property name: '.*', value: 'null'\\.$";
    #endregion

    #region Properties
    /// <summary>
    /// Provides member data for the <see cref="Message_ReturnsProperlyFormattedValue"/> method.
    /// </summary>
    public static TheoryData<string?, string?, string> Message_ReturnsProperlyFormattedValue_MemberData { get; } = new()
    {
        { "Invalid data",   null,                           DefaultPattern                                  },
        { "Invalid data",   "Invalid value in the test",    DefaultPattern + AdditionalInformationPattern   },
        { null,             null,                           NullValuePattern                                }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="DomainStateError.implicit operator string?"/> operator should
    /// return the same value as <see cref="DomainStateError.ToString"/> method.
    /// </summary>
    [Fact]
    public void OperatorImplicitString()
    {
        // Arrange
        var test = "Test";

        var error = new DomainStateError(typeof(DummyEntity), test);

        // Act
        var result = (string?)error;

        // Assert
        Assert.Equal(error.ToString(), result);
    }

    /// <summary>
    /// The <see cref="DomainStateError.implicit operator string?"/> operator should
    /// return <see langword="null"/> if the input
    /// <see cref="DomainStateError"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void OperatorImplicitString_ReturnsNull()
    {
        // Arrange
        var error = null as DomainStateError;

        // Act
        var result = (string?)error!;

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// The <see cref="DomainStateError"/> message property should
    /// return the properly formatted value, containing the type of the entity
    /// whish is in invalid state, the name of the entity's property which is invalid,
    /// value of that property and additional information (if provided).
    /// </summary>
    /// <param name="input">
    /// Property value.
    /// </param>
    /// <param name="additionalInformation">
    /// Additional information to be captured into exception message.
    /// </param>
    /// <param name="pattern">
    /// Expected exception message pattern.
    /// </param>
    [Theory, MemberData(nameof(Message_ReturnsProperlyFormattedValue_MemberData))]
    public void Message_ReturnsProperlyFormattedValue(string? input, string? additionalInformation, string pattern)
    {
        // Arrange
        var type = typeof(DummyEntity);
        var property = nameof(input);

        // Act
        var error = new DomainStateError(type, input, additionalInformation);

        // Assert
        Assert.Matches(pattern, error.Message);
        Assert.Contains(type.Name, error.Message, StringComparison.Ordinal);
        Assert.Contains(input ?? "null", error.Message, StringComparison.Ordinal);
        Assert.Contains(property, error.Message, StringComparison.Ordinal);

        if (!string.IsNullOrWhiteSpace(additionalInformation))
            Assert.Contains(additionalInformation, error.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// The <see cref="DomainStateError"/> message property should
    /// return the properly formatted value, containing name of the parameter
    /// which was passed into exception constructor.
    /// </summary>
    [Fact]
    public void Message_CapturesParameterName()
    {
        // Arrange
        var test = "Invalid data";

        // Act
        var error = new DomainStateError(typeof(DummyEntity), test, additionalInformation: null);

        // Assert
        Assert.Contains(nameof(test), error.Message, StringComparison.Ordinal);
    }
    #endregion
}