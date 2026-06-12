using Paradise.Localization.ErrorHandling;
using System.Globalization;
using System.Text;
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
    /// The <see cref="ApplicationError.ToString"/> method format string.
    /// </summary>
    public CompositeFormat ToStringFormat { get; } = Parse(ErrorMessages.ApplicationErrorToStringFormat);
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
    /// return the same value as <see cref="ApplicationError.ToString"/> method.
    /// </summary>
    [Fact]
    public void OperatorImplicitString()
    {
        // Arrange
        var error = new ApplicationError(DefaultError, "Error");

        // Act
        var result = (string)error;

        // Assert
        Assert.Equal(error.ToString(), result);
    }
    #endregion
}