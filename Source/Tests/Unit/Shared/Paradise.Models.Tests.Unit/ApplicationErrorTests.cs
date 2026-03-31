using System.Globalization;
using System.Text;
using static Paradise.Localization.DataRepresentation.RepresentationMessages;
using static Paradise.Models.ErrorCode;
using static System.Text.CompositeFormat;

namespace Paradise.Models.Tests.Unit;

/// <summary>
/// <see cref="ApplicationError"/> test class.
/// </summary>
public sealed class ApplicationErrorTests
{
    #region Properties
    /// <summary>
    ///
    /// </summary>
    public CompositeFormat ToStringFormat { get; } = Parse(ApplicationErrorToStringFormat);
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="ApplicationError.ToString"/> method should
    /// return the properly formatted string representation
    /// of an <see cref="ApplicationError"/>.
    /// </summary>
    [Fact]
    public void ToString_Override()
    {
        // Arrange
        var errorCode = DefaultError;
        var description = "Description";
        var expectedResult = string.Format(CultureInfo.CurrentCulture,
                                           ToStringFormat,
                                           (int)errorCode,
                                           description);

        var error = new ApplicationError(errorCode, description);

        // Act
        var result = error.ToString();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    /// <summary>
    /// The <see cref="ApplicationError.implicit operator string"/> operator should
    /// return the save value as <see cref="ApplicationError.ToString"/> method.
    /// </summary>
    [Fact]
    public void OperatorImplicitString()
    {
        // Arrange
        var error = new ApplicationError(DefaultError, "Error");

        // Act
        var result = (string)error;

        // Assert
        Assert.Equal(result.ToString(), result);
    }

    /// <summary>
    /// The <see cref="ApplicationError.Equals(ApplicationError)"/> method should
    /// return <see langword="true"/> if both of the values compared
    /// have the same error code and description.
    /// </summary>
    [Fact]
    public void Equals_ReturnsTrueOnEqualCodeAndDescription()
    {
        // Arrange
        var left = new ApplicationError(DefaultError, "Error");
        var right = new ApplicationError(DefaultError, "Error");

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="ApplicationError.Equals(ApplicationError)"/> method should
    /// return <see langword="false"/> if both of the values compared
    /// have different error code.
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseOnNonEqualCodes()
    {
        // Arrange
        var left = new ApplicationError(DefaultError, "Error");
        var right = new ApplicationError(InvalidModel, "Error");

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="ApplicationError.Equals(ApplicationError)"/> method should
    /// return <see langword="false"/> if both of the values compared
    /// have different description.
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseOnNonEqualDescription()
    {
        // Arrange
        var left = new ApplicationError(DefaultError, "Error1");
        var right = new ApplicationError(DefaultError, "Error2");

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="ApplicationError.Equals(object?)"/> method should
    /// return <see langword="true"/> if both of the values compared
    /// have the same error code and description.
    /// </summary>
    [Fact]
    public void Equals_Overload_ReturnsTrueOnEqualCodeAndDescription()
    {
        // Arrange
        var left = new ApplicationError(DefaultError, "Error");
        var right = new ApplicationError(DefaultError, "Error") as object;

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="ApplicationError.Equals(object?)"/> method should
    /// return <see langword="false"/> if one of the values being compared
    /// is <see langword="null"/>.
    /// </summary>
    [Fact]
    public void Equals_Overload_ReturnsFalseOnNull()
    {
        // Arrange
        var left = new ApplicationError(DefaultError, "Error");
        var right = null as object;

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="ApplicationError.Equals(object?)"/> method should
    /// return <see langword="false"/> if both of the values being compared
    /// have different types.
    /// </summary>
    [Fact]
    public void Equals_Overload_ReturnsFalseOnNonEqualType()
    {
        // Arrange
        var left = new ApplicationError(DefaultError, "Error");
        var right = new object();

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="ApplicationError.operator =="/> operator should
    /// return <see langword="true"/> if both of the values compared
    /// have the same error code and description.
    /// </summary>
    [Fact]
    public void OperatorEquals_ReturnsTrueOnEqualCodeAndDescription()
    {
        // Arrange
        var left = new ApplicationError(DefaultError, "Error");
        var right = new ApplicationError(DefaultError, "Error");

        // Act
        var result = left == right;

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="ApplicationError.operator =="/> operator should
    /// return <see langword="false"/> if both of the values compared
    /// have different error code.
    /// </summary>
    [Fact]
    public void OperatorEquals_ReturnsFalseOnNonEqualCodes()
    {
        // Arrange
        var left = new ApplicationError(DefaultError, "Error");
        var right = new ApplicationError(InvalidModel, "Error");

        // Act
        var result = left == right;

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="ApplicationError.operator =="/> operator should
    /// return <see langword="false"/> if both of the values compared
    /// have different description.
    /// </summary>
    [Fact]
    public void OperatorEquals_ReturnsFalseOnNonEqualDescription()
    {
        // Arrange
        var left = new ApplicationError(DefaultError, "Error1");
        var right = new ApplicationError(DefaultError, "Error2");

        // Act
        var result = left == right;

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="ApplicationError.operator !="/> operator should
    /// return <see langword="false"/> if both of the values compared
    /// have the same error code and description.
    /// </summary>
    [Fact]
    public void OperatorNotEquals_ReturnsFalseOnEqualCodeAndDescription()
    {
        // Arrange
        var left = new ApplicationError(DefaultError, "Error");
        var right = new ApplicationError(DefaultError, "Error");

        // Act
        var result = left != right;

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="ApplicationError.operator !="/> operator should
    /// return <see langword="true"/> if both of the values compared
    /// have different error code.
    /// </summary>
    [Fact]
    public void OperatorNotEquals_ReturnsTrueOnNonEqualCodes()
    {
        // Arrange
        var left = new ApplicationError(DefaultError, "Error");
        var right = new ApplicationError(InvalidModel, "Error");

        // Act
        var result = left != right;

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="ApplicationError.operator !="/> operator should
    /// return <see langword="true"/> if both of the values compared
    /// have different description.
    /// </summary>
    [Fact]
    public void OperatorNotEquals_ReturnsTrueOnNonEqualDescription()
    {
        // Arrange
        var left = new ApplicationError(DefaultError, "Error1");
        var right = new ApplicationError(DefaultError, "Error2");

        // Act
        var result = left != right;

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="ApplicationError.GetHashCode"/> method should
    /// return the same values if both of the values being compared
    /// have the same error code and description.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsEqualValuesOnEqualCodeAndDescription()
    {
        // Arrange
        var left = new ApplicationError(DefaultError, "Error");
        var right = new ApplicationError(DefaultError, "Error");

        // Act
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // Assert
        Assert.Equal(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="ApplicationError.GetHashCode"/> method should
    /// return different values if both of the values being compared
    /// have different error code.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsNonEqualValuesOnDifferentCode()
    {
        // Arrange
        var left = new ApplicationError(DefaultError, "Error");
        var right = new ApplicationError(InvalidModel, "Error");

        // Act
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // Assert
        Assert.NotEqual(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="ApplicationError.GetHashCode"/> method should
    /// return different values if both of the values being compared
    /// have different description.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsNonEqualValuesOnDifferentDescription()
    {
        // Arrange
        var left = new ApplicationError(DefaultError, "Error1");
        var right = new ApplicationError(DefaultError, "Error2");

        // Act
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // Assert
        Assert.NotEqual(leftHash, rightHash);
    }
    #endregion
}