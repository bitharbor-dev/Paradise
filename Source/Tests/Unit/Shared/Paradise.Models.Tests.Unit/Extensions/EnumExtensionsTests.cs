using Paradise.Models.Extensions;
using Paradise.Tests.Miscellaneous.TestData.Shared.Models;

namespace Paradise.Models.Tests.Unit.Extensions;

/// <summary>
/// <see cref="EnumExtensions"/> test class.
/// </summary>
public sealed partial class EnumExtensionsTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="GetFormattedDisplayValue"/> method.
    /// </summary>
    public static TheoryData<TestErrorCode, string[], string> GetFormattedDisplayValue_MemberData { get; } = new()
    {
        { TestErrorCode.DefaultMember,                          [],             nameof(TestErrorCode.DefaultMember) },
        { TestErrorCode.DefaultMember,                          ["argument"],   nameof(TestErrorCode.DefaultMember) },
        { TestErrorCode.DisplayValueWithoutParametersMember,    [],             "Test"                              },
        { TestErrorCode.DisplayValueWithParametersMember,       [],             "Test {0}"                          },
        { TestErrorCode.DisplayValueWithParametersMember,       ["argument"],   "Test argument"                     }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="EnumExtensions.GetFormattedDisplayValue"/> method should
    /// return the formatted display value based on the input
    /// <paramref name="errorCode"/> value and formatting <paramref name="arguments"/>.
    /// </summary>
    /// <param name="errorCode">
    /// <see cref="TestErrorCode"/> to run tests onto.
    /// </param>
    /// <param name="arguments">
    /// An object array that contains zero or more objects to format.
    /// </param>
    /// <param name="expectedResult">
    /// Expected result.
    /// </param>
    [Theory, MemberData(nameof(GetFormattedDisplayValue_MemberData))]
    public void GetFormattedDisplayValue(TestErrorCode errorCode, string[] arguments, string expectedResult)
    {
        // Arrange

        // Act
        var result = errorCode.GetFormattedDisplayValue(arguments);

        // Assert
        Assert.Equal(expectedResult, result);
    }
    #endregion
}