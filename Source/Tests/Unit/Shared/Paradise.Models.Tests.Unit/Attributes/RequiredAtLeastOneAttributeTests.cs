using Paradise.Models.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Paradise.Models.Tests.Unit.Attributes;

/// <summary>
/// <see cref="RequiredAtLeastOneAttribute"/> test class.
/// </summary>
public sealed partial class RequiredAtLeastOneAttributeTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="IsValid"/> method.
    /// </summary>
    public static TheoryData<bool, int?, string?, bool> IsValid_MemberData { get; } = new()
    {
        { false,    null,   "value",    true    },
        { false,    null,   " ",        true    },
        { false,    null,   null,       false   },
        { false,    1,      null,       true    },
        { true,     null,   "value",    true    },
        { true,     null,   " ",        false   },
        { true,     null,   null,       false   },
        { true,     1,      null,       true    }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="RequiredAtLeastOneAttribute.IsValid"/> method should
    /// return the <paramref name="expectedResult"/>, depending on the input parameters.
    /// </summary>
    /// <remarks>
    /// The <see cref="RequiredAtLeastOneAttribute.IsValid"/> method is
    /// indirectly called via the base <see cref="ValidationAttribute.IsValid(object?)"/> method.
    /// </remarks>
    /// <param name="restrictEmptyOrWhitespaceStrings">
    /// Indicates whether the empty or whitespace strings would be treated as an invalid value.
    /// </param>
    /// <param name="stringValue">
    /// Test string property value.
    /// </param>
    /// <param name="integerValue">
    /// Test integer property value.
    /// </param>
    /// <param name="expectedResult">
    /// Expected result.
    /// </param>
    [Theory, MemberData(nameof(IsValid_MemberData))]
    public void IsValid(bool restrictEmptyOrWhitespaceStrings, int? integerValue, string? stringValue, bool expectedResult)
    {
        // Arrange
        Test.RestrictEmptyOrWhitespaceStrings = restrictEmptyOrWhitespaceStrings;
        Test.Model.IntegerValue = integerValue;
        Test.Model.StringValue = stringValue;

        var attribute = Test.CreateAttribute();

        // Act
        var result = attribute.IsValid(Test.Model);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    /// <summary>
    /// The <see cref="RequiredAtLeastOneAttribute.IsValid"/> method should
    /// throw the <see cref="MemberAccessException"/> if the input
    /// property name does not match any property defined on the input object.
    /// </summary>
    /// <remarks>
    /// The <see cref="RequiredAtLeastOneAttribute.IsValid"/> method is
    /// indirectly called via the base <see cref="ValidationAttribute.IsValid(object?)"/> method.
    /// </remarks>
    [Fact]
    public void IsValid_ThrowsOnNonExistingProperty()
    {
        // Arrange
        var attribute = Test.CreateAttribute("Non-ExistingProperty");

        // Act & Assert
        Assert.Throws<MemberAccessException>(()
            => attribute.IsValid(Test.Model));
    }

    /// <summary>
    /// The <see cref="RequiredAtLeastOneAttribute.IsValid"/> method should
    /// return <see langword="false"/> if the input
    /// <see langword="object"/> is equal to <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="RequiredAtLeastOneAttribute.IsValid"/> method is
    /// indirectly called via the base <see cref="ValidationAttribute.IsValid(object?)"/> method.
    /// </remarks>
    [Fact]
    public void IsValid_FailsOnNull()
    {
        // Arrange
        var attribute = Test.CreateAttribute();

        // Act
        var result = attribute.IsValid(null);

        // Assert
        Assert.False(result);
    }
    #endregion
}