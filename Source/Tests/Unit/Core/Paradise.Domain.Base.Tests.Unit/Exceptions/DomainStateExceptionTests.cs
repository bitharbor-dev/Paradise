using Paradise.Domain.Base.Exceptions;
using Paradise.Tests.Miscellaneous.TestDoubles.Dummies.Core.Domain.Base;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Base.Tests.Unit.Exceptions;

/// <summary>
/// <see cref="DomainStateException{TEntity}"/> test class.
/// </summary>
public sealed class DomainStateExceptionTests
{
    #region Constants
    [StringSyntax("Regex")]
    private const string DefaultPattern = "^The object of type '.*' is in invalid state\\. Property name: '.*', value: '.*'\\.";

    [StringSyntax("Regex")]
    private const string AdditionalInformationPattern = " Additional information:\\r?\\n.*$";

    [StringSyntax("Regex")]
    private const string NullValuePattern = "^The object of type '.*' is in invalid state\\. Property name: '.*', value: 'null'\\.$";
    #endregion

    #region Properties
    /// <summary>
    /// Provides member data for <see cref="Message_ReturnsProperlyFormattedValue"/> method.
    /// </summary>
    public static TheoryData<string?, string?, string> Message_ReturnsProperlyFormattedValue_MemberData { get; } = new()
    {
        { "Invalid data",   null,                           DefaultPattern },
        { "Invalid data",   "Invalid value in the test",    DefaultPattern + AdditionalInformationPattern },
        { null,             null,                           NullValuePattern }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="DomainStateException{TEntity}"/> message property should
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
        var type = nameof(DummyEntity);
        var property = nameof(input);

        // Act
        var exception = new DomainStateException<DummyEntity>(input, additionalInformation);

        // Assert
        Assert.Matches(pattern, exception.Message);
        Assert.Contains(type, exception.Message, StringComparison.Ordinal);
        Assert.Contains(input ?? "null", exception.Message, StringComparison.Ordinal);
        Assert.Contains(property, exception.Message, StringComparison.Ordinal);

        if (!string.IsNullOrWhiteSpace(additionalInformation))
            Assert.Contains(additionalInformation, exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// The <see cref="DomainStateException{TEntity}"/> message property should
    /// return the properly formatted value, containing name of the parameter
    /// which was passed into exception constructor.
    /// </summary>
    [Fact]
    public void Message_CapturesParameterName()
    {
        // Arrange
        var test = "Invalid data";

        // Act
        var exception = new DomainStateException<DummyEntity>(test);

        // Assert
        Assert.Contains(nameof(test), exception.Message, StringComparison.Ordinal);
    }
    #endregion
}