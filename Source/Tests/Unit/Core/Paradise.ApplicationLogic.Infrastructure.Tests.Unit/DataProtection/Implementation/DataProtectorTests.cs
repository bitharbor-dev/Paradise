using Microsoft.AspNetCore.DataProtection;
using Paradise.ApplicationLogic.Infrastructure.DataProtection.Implementation;
using Paradise.Tests.Miscellaneous.XunitSerialization;
using System.Text.Json;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.DataProtection.Implementation;

/// <summary>
/// <see cref="DataProtector"/> test class.
/// </summary>
public sealed partial class DataProtectorTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="Protect"/> method.
    /// </summary>
    public static TheoryData<ValueWrapper> Protect_MemberData { get; } = new()
    {
        { new ValueWrapper("Test")                      },
        { new ValueWrapper(new { Message = "Test" })    },
        { new ValueWrapper(1)                           },
        { new ValueWrapper(1.5)                         },
        { new ValueWrapper('A')                         },
        { new ValueWrapper(null)                        },
        { new ValueWrapper(new object())                }
    };

    /// <summary>
    /// Provides member data for <see cref="TryUnprotect_ReturnsFalse"/> method.
    /// </summary>
    public static TheoryData<string?> TryUnprotect_ReturnsFalse_MemberData { get; } = new()
    {
        { null as string        },
        { string.Empty          },
        { " "                   },
        { "invalid json string" }
    };

    /// <summary>
    /// Provides member data for <see cref="GenerateRandomDigitCode"/> method.
    /// </summary>
    public static TheoryData<ushort> GenerateRandomDigitCode_MemberData { get; } = new()
    {
        { 0                 },
        { 5                 },
        { 10                },
        { ushort.MaxValue   }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="DataProtector.Protect"/> method should
    /// transform the input of any type by serializing it into json and
    /// running the string protection algorithm.
    /// </summary>
    /// <param name="wrapper">
    /// Input value.
    /// </param>
    [Theory, MemberData(nameof(Protect_MemberData))]
    public void Protect(ValueWrapper wrapper)
    {
        // Arrange
        var input = wrapper.Value;

        // Act
        var protectedInput = Test.Target.Protect(input);

        // Assert
        var json = JsonSerializer.Serialize(input);

        var expectedProtectedInput = Test.InternalProtector.Protect(json);

        Assert.Equal(expectedProtectedInput, protectedInput);
    }

    /// <summary>
    /// The <see cref="DataProtector.TryUnprotect"/> method should
    /// return <see langword="false"/> if the input
    /// argument is an improperly protected value.
    /// </summary>
    /// <param name="token">
    /// String value to parse.
    /// </param>
    [Theory, MemberData(nameof(TryUnprotect_ReturnsFalse_MemberData))]
    public void TryUnprotect_ReturnsFalse(string? token)
    {
        // Arrange

        // Act
        var result = Test.Target.TryUnprotect(token, out string? value);

        // Assert
        Assert.False(result);
        Assert.Null(value);
    }

    /// <summary>
    /// The <see cref="DataProtector.TryUnprotect"/> method should
    /// return <see langword="true"/> if the input
    /// argument is a properly protected value.
    /// </summary>
    [Fact]
    public void TryUnprotect_ReturnsTrue()
    {
        // Arrange
        var initialValue = "Test";
        var json = JsonSerializer.Serialize(initialValue);

        var protectedValue = Test.InternalProtector.Protect(json);

        // Act
        var result = Test.Target.TryUnprotect(protectedValue, out string? value);

        // Assert
        Assert.True(result);
        Assert.Equal(initialValue, value);
    }

    /// <summary>
    /// The <see cref="DataProtector.GenerateRandomDigitCode"/> method should
    /// transform the input of any type by serializing it into json and
    /// running the string protection algorithm.
    /// </summary>
    /// <param name="length">
    /// Code length.
    /// </param>
    [Theory, MemberData(nameof(GenerateRandomDigitCode_MemberData))]
    public void GenerateRandomDigitCode(ushort length)
    {
        // Arrange

        // Act
        var result = Test.Target.GenerateRandomDigitCode(length);

        // Assert
        Assert.Equal(length, result.Length);
        Assert.Matches("^[0-9]+$|^$", result);
    }
    #endregion
}