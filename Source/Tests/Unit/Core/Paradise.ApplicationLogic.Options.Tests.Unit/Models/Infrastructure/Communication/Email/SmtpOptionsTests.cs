using Paradise.ApplicationLogic.Options.Models.Infrastructure.Communication.Email;

namespace Paradise.ApplicationLogic.Options.Tests.Unit.Models.Infrastructure.Communication.Email;

/// <summary>
/// <see cref="SmtpOptions"/> test class.
/// </summary>
public sealed class SmtpOptionsTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="SmtpOptions.InitializeLocalEmailStorage"/>.
    /// </summary>
    public static TheoryData<string, string> InitializeLocalEmailStorage_MemberData { get; } = new()
    {
        { "LocalEmails", Path.GetFullPath("LocalEmails")                                                                },
        { $"{{ApplicationRoot}}{Path.DirectorySeparatorChar}Emails", Path.Combine(AppContext.BaseDirectory, "Emails")   },
        { "Config/{ApplicationRoot}Emails", Path.GetFullPath("Config/{ApplicationRoot}Emails")                          },
        { "{applicationroot}Emails", Path.GetFullPath("{applicationroot}Emails")                                        }
    };

    /// <summary>
    /// Provides member data for <see cref="SmtpOptions.InitializeLocalEmailStorage"/>
    /// returning <see langword="null"/>.
    /// </summary>
    public static TheoryData<string?> InitializeLocalEmailStorage_ReturnsNull_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="SmtpOptions.InitializeLocalEmailStorage"/> method should
    /// resolve the directory path, create the directory if it does not exist,
    /// and return the fully qualified path.
    /// </summary>
    /// <param name="input">
    /// Input value.
    /// </param>
    /// <param name="expectedResult">
    /// Expected output.
    /// </param>
    [Theory, MemberData(nameof(InitializeLocalEmailStorage_MemberData))]
    public void InitializeLocalEmailStorage(string input, string expectedResult)
    {
        // Arrange
        var options = new SmtpOptions
        {
            LocalEmailStorage = input
        };

        // Act
        var result = options.InitializeLocalEmailStorage();

        // Assert
        Assert.True(Directory.Exists(result));
        Assert.Equal(expectedResult, result);

        Directory.Delete(result, true);
    }

    /// <summary>
    /// The <see cref="SmtpOptions.InitializeLocalEmailStorage"/> method should
    /// return <see langword="null"/> if the <paramref name="input"/>
    /// is equal to <see langword="null"/>, <see cref="string.Empty"/> or whitespace-only.
    /// </summary>
    /// <param name="input">
    /// Input value.
    /// </param>
    [Theory, MemberData(nameof(InitializeLocalEmailStorage_ReturnsNull_MemberData))]
    public void InitializeLocalEmailStorage_ReturnsNull(string? input)
    {
        // Arrange
        var options = new SmtpOptions
        {
            LocalEmailStorage = input
        };

        // Act
        var result = options.InitializeLocalEmailStorage();

        // Assert
        Assert.Null(result);
    }
    #endregion
}