using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;

namespace Paradise.ApplicationLogic.Infrastructure.Domain.Tests.Unit.MessageTemplates;

/// <summary>
/// <see cref="EmailTemplate"/> test class.
/// </summary>
public sealed class EmailTemplateTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="GetFormattedSubject"/> method.
    /// </summary>
    public static TheoryData<string, string?, ushort, string?[]> GetFormattedSubject_MemberData { get; } = new()
    {
        { "Test",                           null,       0,  []                              },
        { "Test with {arg}0",               "{arg}",    1,  ["Value"]                       },
        { "Test with {arg}0",               "{arg}",    1,  [null]                          },
        { "Test with {arg}0 and {arg}1",    "{arg}",    2,  ["First value", "Second value"] }
    };

    /// <summary>
    /// Provides member data for <see cref="ValidateState"/> method.
    /// </summary>
    public static TheoryData<string, string?, ushort> ValidateState_MemberData { get; } = new()
    {
        { "Test",                           null,       0   },
        { "Test with {arg}0",               "{arg}",    1   },
        { "Test with {arg}0 and {arg}1",    "{arg}",    2   }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="EmailTemplate.GetFormattedSubject"/> method should
    /// return a well formatted email subject if the
    /// number of placeholders in <see cref="EmailTemplate.Subject"/> is
    /// equal to <see cref="EmailTemplate.SubjectPlaceholdersNumber"/> and
    /// the number of input values matches it as well.
    /// </summary>
    /// <param name="subject">
    /// Email subject.
    /// </param>
    /// <param name="subjectPlaceholderName">
    /// Subject placeholder name to be replaced with values during the message formatting.
    /// </param>
    /// <param name="subjectPlaceholdersNumber">
    /// The number subject of placeholders to be replaced with values during the message formatting.
    /// </param>
    /// <param name="parameters">
    /// <see cref="EmailTemplate.Subject"/> formatting values.
    /// </param>
    [Theory, MemberData(nameof(GetFormattedSubject_MemberData))]
    public void GetFormattedSubject(string subject, string? subjectPlaceholderName, ushort subjectPlaceholdersNumber, string?[] parameters)
    {
        // Arrange
        var entity = new EmailTemplate("TemplateName", null, "TemplateText", subject)
        {
            SubjectPlaceholderName = subjectPlaceholderName,
            SubjectPlaceholdersNumber = subjectPlaceholdersNumber
        };

        // Act
        var result = entity.GetFormattedSubject(parameters);

        // Assert
        Assert.All(parameters, parameter =>
        {
            if (parameter is not null)
                Assert.Contains(parameter, result, StringComparison.Ordinal);
        });
    }

    /// <summary>
    /// The <see cref="EmailTemplate.GetFormattedSubject"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// parameters number is not equal to the <see cref="EmailTemplate.SubjectPlaceholdersNumber"/>.
    /// </summary>
    [Fact]
    public void GetFormattedSubject_ThrowsOnInvalidParametersNumber()
    {
        // Arrange
        var parameters = new[] { "{arg}", "{arg}" };
        var invalidPlaceholdersNumber = (ushort)(parameters.Length + 1);

        var entity = new EmailTemplate("TemplateName", null, "TemplateText", "Subject")
        {
            SubjectPlaceholderName = "Test",
            SubjectPlaceholdersNumber = invalidPlaceholdersNumber
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(()
            => entity.GetFormattedSubject(parameters));
    }

    /// <summary>
    /// The <see cref="EmailTemplate.GetFormattedSubject"/> method should
    /// throw the <see cref="FormatException"/> if the
    /// <see cref="EmailTemplate.Subject"/> is missing a placeholder.
    /// </summary>
    [Fact]
    public void GetFormattedSubject_ThrowsOnMissingPlaceholder()
    {
        // Arrange
        var parameters = new[] { "{arg}" };

        var entity = new EmailTemplate("TemplateName", null, "TemplateText", "Subject")
        {
            SubjectPlaceholderName = "Test",
            SubjectPlaceholdersNumber = (ushort)parameters.Length
        };

        // Act & Assert
        Assert.Throws<FormatException>(()
            => entity.GetFormattedSubject(parameters));
    }

    /// <summary>
    /// The <see cref="EmailTemplate.GetFormattedSubject"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the
    /// <see cref="EmailTemplate.SubjectPlaceholderName"/> is <see langword="null"/>,
    /// <see cref="EmailTemplate.SubjectPlaceholdersNumber"/> equals to 0,
    /// but input parameters number is not equal to 0.
    /// </summary>
    [Fact]
    public void GetFormattedSubject_ThrowsOnNullSubjectPlaceholderNameWithNonZeroParametersNumber()
    {
        // Arrange
        var parameters = new[] { "{arg}" };

        var entity = new EmailTemplate("TemplateName", null, "TemplateText", "Subject")
        {
            SubjectPlaceholderName = null,
            SubjectPlaceholdersNumber = 0
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(()
            => entity.GetFormattedSubject(parameters));
    }

    /// <summary>
    /// The <see cref="EmailTemplate.GetFormattedSubject"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the
    /// <see cref="EmailTemplate.SubjectPlaceholderName"/> is <see langword="null"/> and
    /// <see cref="EmailTemplate.SubjectPlaceholdersNumber"/> does not equal to 0.
    /// </summary>
    [Fact]
    public void GetFormattedSubject_ThrowsOnNullSubjectPlaceholderNameWithNonZeroPlaceholdersNumber()
    {
        // Arrange
        var parameters = new[] { "{arg}" };

        var entity = new EmailTemplate("TemplateName", null, "TemplateText", "Subject")
        {
            SubjectPlaceholderName = null,
            SubjectPlaceholdersNumber = (ushort)parameters.Length
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(()
            => entity.GetFormattedSubject(parameters));
    }

    /// <summary>
    /// The <see cref="EmailTemplate.Update"/> method should
    /// return <see langword="false"/> if the input
    /// parameters were the same as the current object's
    /// corresponding property values.
    /// </summary>
    [Fact]
    public void Update_ReturnsFalse()
    {
        // Arrange
        var entity = new EmailTemplate("TemplateName", null, "TemplateText", "Subject")
        {
            IsBodyHtml = true,
            PlaceholderName = "{arg}",
            PlaceholdersNumber = 1,
            SubjectPlaceholderName = "{arg}",
            SubjectPlaceholdersNumber = 1
        };

        // Act
        var result = entity.Update(isBodyHtml: null,
                                   placeholderName: entity.PlaceholderName,
                                   placeholdersNumber: null,
                                   subject: null,
                                   subjectPlaceholderName: entity.SubjectPlaceholderName,
                                   subjectPlaceholdersNumber: null,
                                   templateText: null);

        // Need to pass placeholder names properties into update method,
        // since 'null' value is also suitable for these properties.

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="EmailTemplate.Update"/> method should
    /// return <see langword="true"/> the input
    /// parameters were different from current object's
    /// corresponding property values and they were updated.
    /// </summary>
    [Fact]
    public void Update_ReturnsTrue()
    {
        // Arrange
        var entity = new EmailTemplate("TemplateName", null, "TemplateText", "Subject")
        {
            IsBodyHtml = true,
            PlaceholderName = "{arg}",
            PlaceholdersNumber = 1,
            SubjectPlaceholderName = "{arg}",
            SubjectPlaceholdersNumber = 1
        };

        var isBodyHtmlNew = !entity.IsBodyHtml;
        var placeholderNameNew = entity.PlaceholderName + "Modified";
        var placeholdersNumberNew = (ushort)(entity.PlaceholdersNumber + 1);
        var subjectNew = entity.Subject + "Modified";
        var subjectPlaceholderNameNew = entity.SubjectPlaceholderName + "Modified";
        var subjectPlaceholdersNumberNew = (ushort)(entity.SubjectPlaceholdersNumber + 1);
        var templateTextNew = entity.TemplateText + "Modified";

        // Act
        var result = entity.Update(isBodyHtml: isBodyHtmlNew,
                                   placeholderName: placeholderNameNew,
                                   placeholdersNumber: placeholdersNumberNew,
                                   subject: subjectNew,
                                   subjectPlaceholderName: subjectPlaceholderNameNew,
                                   subjectPlaceholdersNumber: subjectPlaceholdersNumberNew,
                                   templateText: templateTextNew);

        // Assert
        Assert.True(result);

        Assert.Equal(isBodyHtmlNew, entity.IsBodyHtml);
        Assert.Equal(placeholderNameNew, entity.PlaceholderName);
        Assert.Equal(placeholdersNumberNew, entity.PlaceholdersNumber);
        Assert.Equal(subjectNew, entity.Subject);
        Assert.Equal(subjectPlaceholderNameNew, entity.SubjectPlaceholderName);
        Assert.Equal(subjectPlaceholdersNumberNew, entity.SubjectPlaceholdersNumber);
        Assert.Equal(templateTextNew, entity.TemplateText);
    }

    /// <summary>
    /// The <see cref="EmailTemplate.ValidateState"/> method should
    /// not throw any exception for the <see cref="EmailTemplate"/> instance
    /// which <see cref="EmailTemplate.SubjectPlaceholderName"/> is not <see langword="null"/>
    /// and <see cref="EmailTemplate.SubjectPlaceholdersNumber"/> does not equal to 0.
    /// </summary>
    /// <param name="subject">
    /// Email subject.
    /// </param>
    /// <param name="subjectPlaceholderName">
    /// Subject placeholder name to be replaced with values during the message formatting.
    /// </param>
    /// <param name="subjectPlaceholdersNumber">
    /// The number of subject placeholders to be replaced with values during the message formatting.
    /// </param>
    [Theory, MemberData(nameof(ValidateState_MemberData))]
    public void ValidateState(string subject, string? subjectPlaceholderName, ushort subjectPlaceholdersNumber)
    {
        // Arrange
        var entity = new EmailTemplate("TemplateName", null, "TemplateText", subject)
        {
            SubjectPlaceholderName = subjectPlaceholderName,
            SubjectPlaceholdersNumber = subjectPlaceholdersNumber
        };

        // Act & Assert
        entity.ValidateState();
    }

    /// <summary>
    /// The <see cref="EmailTemplate.ValidateState"/> method should
    /// throw the <see cref="InvalidOperationException"/> for the
    /// <see cref="EmailTemplate"/> instance
    /// which <see cref="EmailTemplate.SubjectPlaceholderName"/> is <see langword="null"/>
    /// but <see cref="EmailTemplate.SubjectPlaceholdersNumber"/> does not equal to 0.
    /// </summary>
    [Fact]
    public void ValidateState_ThrowsOnInvalidSubjectPlaceholders()
    {
        // Arrange
        var entity = new EmailTemplate("TemplateName", null, "TemplateText", "Subject")
        {
            SubjectPlaceholderName = null,
            SubjectPlaceholdersNumber = 1
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(entity.ValidateState);
    }

    /// <summary>
    /// The <see cref="EmailTemplate.ValidateState"/> method should
    /// throw the <see cref="InvalidOperationException"/> for the
    /// <see cref="EmailTemplate"/> instance
    /// which <see cref="EmailTemplate.Subject"/> does not contain the specified number of placeholders.
    /// </summary>
    [Fact]
    public void ValidateState_ThrowsOnInvalidSubjectPlaceholdersNumber()
    {
        // Arrange
        var entity = new EmailTemplate("TemplateName", null, "TemplateText", "Subject")
        {
            SubjectPlaceholderName = "{placeholder}",
            SubjectPlaceholdersNumber = 1
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(entity.ValidateState);
    }
    #endregion
}