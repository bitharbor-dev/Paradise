using Paradise.ApplicationLogic.Options.Models.DataAccess.Seed.Providers;

namespace Paradise.ApplicationLogic.Options.Tests.Unit.Models.DataAccess.Seed.Providers;

/// <summary>
/// <see cref="JsonSeedDataProviderOptions"/> test class.
/// </summary>
public sealed class JsonSeedDataProviderOptionsTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="ResolveSeedDirectoryPath"/> method.
    /// </summary>
    public static TheoryData<string, string> ResolveSeedDirectoryPath_MemberData { get; } = new()
    {
        { "SeedData", "SeedData"                                                                                            },
        { "{ApplicationRoot}", AppContext.BaseDirectory                                                                     },
        { $"{{ApplicationRoot}}{Path.DirectorySeparatorChar}SeedData", Path.Combine(AppContext.BaseDirectory, "SeedData")   },
        { "Config/{ApplicationRoot}SeedData", "Config/{ApplicationRoot}SeedData"                                            },
        { "{applicationroot}SeedData", "{applicationroot}SeedData"                                                          }
    };

    /// <summary>
    /// Provides member data for <see cref="ResolveSeedDirectoryPath"/> method.
    /// </summary>
    public static TheoryData<string?> ResolveSeedDirectoryPath_ReturnsNull_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="JsonSeedDataProviderOptions.ResolveSeedDirectoryPath"/> method should
    /// return the resolved seed data directory path, by replacing the '{ApplicationRoot}' placeholder
    /// if it is present in the beginning of the <paramref name="input"/> value and matches the letter casing.
    /// </summary>
    /// <param name="input">
    /// Input value.
    /// </param>
    /// <param name="expectedResult">
    /// Expected output.
    /// </param>
    [Theory, MemberData(nameof(ResolveSeedDirectoryPath_MemberData))]
    public void ResolveSeedDirectoryPath(string input, string expectedResult)
    {
        // Arrange
        var options = new JsonSeedDataProviderOptions
        {
            SeedDirectoryPath = input
        };

        // Act
        var result = options.ResolveSeedDirectoryPath();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    /// <summary>
    /// The <see cref="JsonSeedDataProviderOptions.ResolveSeedDirectoryPath"/> method should
    /// return <see langword="null"/> if the <paramref name="input"/>
    /// is equal to <see langword="null"/>, <see cref="string.Empty"/> or whitespace-only.
    /// </summary>
    /// <param name="input">
    /// Input value.
    /// </param>
    [Theory, MemberData(nameof(ResolveSeedDirectoryPath_ReturnsNull_MemberData))]
    public void ResolveSeedDirectoryPath_ReturnsNull(string? input)
    {
        // Arrange
        var options = new JsonSeedDataProviderOptions
        {
            SeedDirectoryPath = input
        };

        // Act
        var result = options.ResolveSeedDirectoryPath();

        // Assert
        Assert.Null(result);
    }
    #endregion
}