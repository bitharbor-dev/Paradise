using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

namespace Paradise.ApplicationLogic.Tests.Services.Application;

/// <summary>
/// Test class for the <see cref="DataProtectionService"/> methods.
/// </summary>
/// <remarks>
/// <see cref="DataProtectionService.ProtectAsJson{T}(T)"/> test methods are not available
/// since the <see cref="IDataProtector"/> is providing different output strings each time
/// when protecting the same <see cref="string"/> values, yet being able to unprotect them all.
/// Unfortunately, this means that test data arrangement is not possible.
/// </remarks>
public sealed class DataProtectionServiceTests
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonWebTokenServiceTests"/> class.
    /// </summary>
    public DataProtectionServiceTests()
    {
        var dataProtectionProvider = DataProtectionProvider.Create(nameof(Paradise));

        Protector = dataProtectionProvider.CreateProtector(DataProtectionService.DataProtectionPurpose);

        Service = new(dataProtectionProvider);
    }
    #endregion

    #region Properties
    /// <summary>
    /// A <see cref="IDataProtector"/> instance used to
    /// arrange data and validate test results.
    /// </summary>
    public IDataProtector Protector { get; }

    /// <summary>
    /// A <see cref="DataProtectionService"/> instance to be tested.
    /// </summary>
    public DataProtectionService Service { get; }
    #endregion

    #region Public methods
    /// <summary>
    /// <see cref="DataProtectionService.GenerateRandomDigitCode"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// A <see cref="string"/> value containing the
    /// generated code of the given <paramref name="length"/>
    /// is returned.
    /// </para>
    /// </para>
    /// </summary>
    /// <param name="length">
    /// Code length.
    /// </param>
    [Theory, InlineData(0), InlineData(1), InlineData(6), InlineData(ushort.MaxValue)]
    public void GenerateRandomDigitCode(ushort length)
    {
        // Arrange

        // Act
        var result = Service.GenerateRandomDigitCode(length);

        // Assert
        Assert.Equal(length, result.Length);
        Assert.All(result, character => Assert.True(char.IsDigit(character)));
    }

    /// <summary>
    /// <see cref="DataProtectionService.TryUnprotectJson"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="true"/>
    /// and not-null value in the output parameter.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void TryUnprotectJson()
    {
        // Arrange
        var data = "Test";
        var json = JsonSerializer.Serialize(data);
        var token = Protector.Protect(json);

        // Act
        var result = Service.TryUnprotectJson(token, out string? value);

        // Assert
        Assert.True(result);
        Assert.Equal(data, value);
    }

    /// <summary>
    /// <see cref="DataProtectionService.TryUnprotectJson"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/>
    /// and null value in the output parameter
    /// due to an empty string was passed as the first argument.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void TryUnprotectJson_ReturnsFalseOnEmptyString()
    {
        // Arrange

        // Act
        var result = Service.TryUnprotectJson(string.Empty, out object? value);

        // Assert
        Assert.False(result);
        Assert.Null(value);
    }

    /// <summary>
    /// <see cref="DataProtectionService.TryUnprotectJson"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/>
    /// and null value in the output parameter
    /// due to a non-JSON string was passed as the first argument.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void TryUnprotectJson_ReturnsFalseOnNonJsonString()
    {
        // Arrange
        var data = "Not a JSON string";
        var token = Protector.Protect(data);

        // Act
        var result = Service.TryUnprotectJson(token, out object? value);

        // Assert
        Assert.False(result);
        Assert.Null(value);
    }

    /// <summary>
    /// <see cref="DataProtectionService.TryUnprotectJson"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/>
    /// and null value in the output parameter
    /// due to a <see langword="null"/> value was passed as the first argument.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void TryUnprotectJson_ReturnsFalseOnNullString()
    {
        // Arrange

        // Act
        var result = Service.TryUnprotectJson(null, out object? value);

        // Assert
        Assert.False(result);
        Assert.Null(value);
    }
    #endregion
}