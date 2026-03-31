using Paradise.Common.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Paradise.Common.Tests.Unit.Extensions;

/// <summary>
/// <see cref="StringExtensions"/> test class.
/// </summary>
public sealed class StringExtensionsTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="IsNullOrWhiteSpace"/> method.
    /// </summary>
    public static TheoryData<string?, bool> IsNullOrWhiteSpace_MemberData { get; } = new()
    {
        { null,     true    },
        { "",       true    },
        { "    ",   true    },
        { "test",   false   }
    };

    /// <summary>
    /// Provides member data for <see cref="IsNullOrWhiteSpace"/> method.
    /// </summary>
    public static TheoryData<string?, bool> IsNotNullOrWhiteSpace_MemberData { get; } = new()
    {
        { null,     false   },
        { "",       false   },
        { "    ",   false   },
        { "test",   true    }
    };

    /// <summary>
    /// Provides member data for <see cref="IsValidEmailAddress"/> method.
    /// </summary>
    public static TheoryData<string, bool> IsValidEmailAddress_MemberData { get; } = new()
    {
        { "test@email.com",         true    },
        { "first.last@domain.co",   true    },
        { "invalid@",               false   },
        { "missing-at-symbol.com",  false   },
        { "",                       false   },
        { " ",                      false   }
    };

    /// <summary>
    /// Provides member data for <see cref="IsValidPhoneNumber"/> method.
    /// </summary>
    public static TheoryData<string, bool> IsValidPhoneNumber_MemberData { get; } = new()
    {
        { "123-456-7890",       true    },
        { "(123) 456-7890",     true    },
        { "+1 (123) 456-7890",  true    },
        { "123456",             true    },
        { "letters",            false   },
        { "",                   false   },
        { " ",                  false   }
    };

    /// <summary>
    /// Provides member data for <see cref="IsValidUserName"/> method.
    /// </summary>
    public static TheoryData<string?, string?, bool> IsValidUserName_MemberData { get; } = new()
    {
        { "test",       "abcdefghijklmnopqrstuvwxyz",                                       true    },
        { "Test123",    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",   true    },
        { "test!",      "abcdefghijklmnopqrstuvwxyz",                                       false   },
        { "",           "abcdefghijklmnopqrstuvwxyz",                                       false   },
        { null,         "abcdefghijklmnopqrstuvwxyz",                                       false   },
        { "test",       null,                                                               true    },
        { "test",       "",                                                                 true    },
        { "test",       "    ",                                                             true    }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="StringExtensions.IsNullOrWhiteSpace"/> method should
    /// work exactly like the <see cref="string.IsNullOrWhiteSpace"/> method.
    /// </summary>
    /// <param name="value">
    /// Input value.
    /// </param>
    /// <param name="expectedResult">
    /// Expected result.
    /// </param>
    [Theory, MemberData(nameof(IsNullOrWhiteSpace_MemberData))]
    public void IsNullOrWhiteSpace(string? value, bool expectedResult)
    {
        // Arrange

        // Act
        var result = value.IsNullOrWhiteSpace();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    /// <summary>
    /// The <see cref="StringExtensions.IsNullOrWhiteSpace"/> method should
    /// work exactly opposite to the <see cref="string.IsNullOrWhiteSpace"/> method.
    /// </summary>
    /// <param name="value">
    /// Input value.
    /// </param>
    /// <param name="expectedResult">
    /// Expected result.
    /// </param>
    [Theory, MemberData(nameof(IsNotNullOrWhiteSpace_MemberData))]
    public void IsNotNullOrWhiteSpace(string? value, bool expectedResult)
    {
        // Arrange

        // Act
        var result = value.IsNotNullOrWhiteSpace();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    /// <summary>
    /// The <see cref="StringExtensions.SanitizePathSeparators"/> method should
    /// replace all '/' and '\' symbols in the input string
    /// with the current <see cref="Path.DirectorySeparatorChar"/> value.
    /// </summary>
    [Fact]
    public void SanitizePathSeparators()
    {
        // Arrange
        var separator = Path.DirectorySeparatorChar;

        var input = "\\a/b\\c/d";
        var expected = $"{separator}a{separator}b{separator}c{separator}d";

        // Act
        var result = input.SanitizePathSeparators();

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// The <see cref="StringExtensions.SanitizePathSeparators"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="string"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void SanitizePathSeparators_ThrowsOnNull()
    {
        // Arrange
        var input = null as string;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => input!.SanitizePathSeparators());
    }

    /// <summary>
    /// The <see cref="StringExtensions.IsValidEmailAddress"/> method should
    /// work exactly like the <see cref="EmailAddressAttribute.IsValid"/> method.
    /// </summary>
    /// <param name="value">
    /// Input value.
    /// </param>
    /// <param name="expectedResult">
    /// Expected result.
    /// </param>
    [Theory, MemberData(nameof(IsValidEmailAddress_MemberData))]
    public void IsValidEmailAddress(string value, bool expectedResult)
    {
        // Arrange

        // Act
        var result = value.IsValidEmailAddress();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    /// <summary>
    /// The <see cref="StringExtensions.IsValidPhoneNumber"/> method should
    /// work exactly like the <see cref="PhoneAttribute.IsValid"/> method.
    /// </summary>
    /// <param name="value">
    /// Input value.
    /// </param>
    /// <param name="expectedResult">
    /// Expected result.
    /// </param>
    [Theory, MemberData(nameof(IsValidPhoneNumber_MemberData))]
    public void IsValidPhoneNumber(string value, bool expectedResult)
    {
        // Arrange

        // Act
        var result = value.IsValidPhoneNumber();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    /// <summary>
    /// The <see cref="StringExtensions.IsValidPhoneNumber"/> method should
    /// check if the <paramref name="value"/> contains only <paramref name="allowedCharacters"/>.
    /// </summary>
    /// <remarks>
    /// The method should return <see langword="true"/> if the <paramref name="allowedCharacters"/>
    /// is equal to <see langword="null"/>, <see cref="string.Empty"/> or whitespace-only.
    /// </remarks>
    /// <param name="value">
    /// Input value.
    /// </param>
    /// <param name="allowedCharacters">
    /// The list of allowed characters in the user-name used to
    /// validate <paramref name="value"/>.
    /// </param>
    /// <param name="expectedResult">
    /// Expected result.
    /// </param>
    [Theory, MemberData(nameof(IsValidUserName_MemberData))]
    public void IsValidUserName(string? value, string? allowedCharacters, bool expectedResult)
    {
        // Arrange

        // Act
        var result = value!.IsValidUserName(allowedCharacters!);

        // Assert
        Assert.Equal(expectedResult, result);
    }
    #endregion
}
