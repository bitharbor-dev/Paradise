namespace Paradise.ApplicationLogic.Domain.Tests.MessageTemplates;

/// <summary>
/// Test class for the <see cref="EmailTemplate"/>.
/// </summary>
public sealed class EmailTemplateTests
{
    #region Public methods
    /// <summary>
    /// <see cref="EmailTemplate.GetEqualityComponents"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// All properties, which are necessary for email template comparison,
    /// are returned.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetEqualityComponents()
    {
        // Arrange
        var emailTemplate = new EmailTemplate("Test", "Test", "Test");

        // Act
        var equalityComponents = emailTemplate.GetEqualityComponents();

        // Assert
        Assert.Contains(emailTemplate.Culture, equalityComponents);
        Assert.Contains(emailTemplate.IsBodyHtml, equalityComponents);
        Assert.Contains(emailTemplate.PlaceholderName, equalityComponents);
        Assert.Contains(emailTemplate.PlaceholdersNumber, equalityComponents);
        Assert.Contains(emailTemplate.Subject, equalityComponents);
        Assert.Contains(emailTemplate.SubjectPlaceholderName, equalityComponents);
        Assert.Contains(emailTemplate.SubjectPlaceholdersNumber, equalityComponents);
        Assert.Contains(emailTemplate.TemplateName, equalityComponents);
        Assert.Contains(emailTemplate.TemplateText, equalityComponents);
    }

    /// <summary>
    /// <see cref="EmailTemplate.GetFormattedSubject"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Email template subject is returned in it's formatted state,
    /// which means all placeholders within are replaced with the
    /// values from the input parameters.
    /// </para>
    /// </para>
    /// </summary>
    [Theory, MemberData(nameof(GetFormattedSubject_MemberData))]
    public void GetFormattedSubject(string subject, string? subjectPlaceholderName, ushort subjectPlaceholdersNumber, IList<object?> parameters)
    {
        // Arrange
        var emailTemplate = new EmailTemplate("Template name", "Template text", subject)
        {
            SubjectPlaceholderName = subjectPlaceholderName,
            SubjectPlaceholdersNumber = subjectPlaceholdersNumber
        };

        // Act
        var formatterSubject = emailTemplate.GetFormattedSubject(parameters);

        // Assert
        Assert.All(parameters, parameter => Assert.Contains(parameter!.ToString()!, formatterSubject, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// <see cref="EmailTemplate.GetFormattedSubject"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Original subject should be returned since the <see cref="EmailTemplate.SubjectPlaceholderName"/>
    /// is <see langword="null"/> and <see cref="EmailTemplate.SubjectPlaceholdersNumber"/> is equal to 0.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetFormattedSubject_ReturnsPlainSubjectOnEmptyParameters()
    {
        // Arrange
        var emailTemplate = new EmailTemplate("Template name", "Template text", "Subject")
        {
            SubjectPlaceholderName = null,
            SubjectPlaceholdersNumber = 0
        };

        // Act
        var formatterSubject = emailTemplate.GetFormattedSubject(Array.Empty<object>());

        // Assert
        Assert.Equal(emailTemplate.Subject, formatterSubject);
    }

    /// <summary>
    /// <see cref="EmailTemplate.GetFormattedSubject"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws an <see cref="IndexOutOfRangeException"/> since the parameters number
    /// is not equal to <see cref="EmailTemplate.SubjectPlaceholdersNumber"/>.
    /// </para>
    /// </summary>
    [Fact]
    public void GetFormattedSubject_ThrowsOnInvalidParametersNumber()
    {
        // Arrange
        var parameters = new[] { "{arg}", "{arg}" };

        var emailTemplate = new EmailTemplate("Template name", "Template text", "Subject")
        {
            SubjectPlaceholderName = "Test",
            SubjectPlaceholdersNumber = (ushort)(parameters.Length + 1)
        };

        // Act

        // Assert
        Assert.Throws<IndexOutOfRangeException>(()
            => emailTemplate.GetFormattedSubject(parameters));
    }

    /// <summary>
    /// <see cref="EmailTemplate.GetFormattedSubject"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="FormatException"/> since there is no such parameter
    /// inside the <see cref="EmailTemplate.Subject"/>.
    /// </para>
    /// </summary>
    [Fact]
    public void GetFormattedSubject_ThrowsOnMissingPlaceholder()
    {
        // Arrange
        var parameters = new[] { "{arg}" };

        var emailTemplate = new EmailTemplate("Template name", "Template text", "Subject without necessary parameter")
        {
            SubjectPlaceholderName = "Test",
            SubjectPlaceholdersNumber = (ushort)parameters.Length
        };

        // Act

        // Assert
        Assert.Throws<FormatException>(()
            => emailTemplate.GetFormattedSubject(parameters));
    }

    /// <summary>
    /// <see cref="EmailTemplate.GetFormattedSubject"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ArgumentException"/> since the
    /// <see cref="EmailTemplate.SubjectPlaceholderName"/> is <see langword="null"/>,
    /// <see cref="EmailTemplate.SubjectPlaceholdersNumber"/> equals to 0,
    /// but input parameters number is not equal to 0.
    /// </para>
    /// </summary>
    [Fact]
    public void GetFormattedSubject_ThrowsOnNullSubjectPlaceholderNameWithNonZeroParametersNumber()
    {
        // Arrange
        var parameters = new[] { "{arg}" };

        var emailTemplate = new EmailTemplate("Template name", "Template text", "Subject without necessary parameter")
        {
            SubjectPlaceholderName = null,
            SubjectPlaceholdersNumber = 0
        };

        // Act

        // Assert
        Assert.Throws<ArgumentException>(()
            => emailTemplate.GetFormattedSubject(parameters));
    }

    /// <summary>
    /// <see cref="EmailTemplate.GetFormattedSubject"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="InvalidEntityStateException"/> since the
    /// <see cref="EmailTemplate.SubjectPlaceholderName"/> is <see langword="null"/>,
    /// but <see cref="EmailTemplate.SubjectPlaceholdersNumber"/> is not equal to 0.
    /// </para>
    /// </summary>
    [Fact]
    public void GetFormattedSubject_ThrowsOnNullSubjectPlaceholderNameWithNonZeroPlaceholdersNumber()
    {
        // Arrange
        var parameters = new[] { "{arg}" };

        var emailTemplate = new EmailTemplate("Template name", "Template text", "Subject Parameter1")
        {
            SubjectPlaceholderName = null,
            SubjectPlaceholdersNumber = (ushort)parameters.Length
        };

        // Act

        // Assert
        Assert.Throws<InvalidEntityStateException>(()
            => emailTemplate.GetFormattedSubject(parameters));
    }

    /// <summary>
    /// <see cref="EmailTemplate.Update"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/>
    /// due to no changes were applied to the <see cref="EmailTemplate"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Update_ReturnsFalseOnNonExistingChanges()
    {
        // Arrange
        var emailTemplate = new EmailTemplate("Template name", "Template text", "Subject")
        {
            IsBodyHtml = true,
            PlaceholderName = "{arg}",
            PlaceholdersNumber = 1,
            SubjectPlaceholderName = "{arg}",
            SubjectPlaceholdersNumber = 1
        };

        // Act
        var result = emailTemplate.Update(null,
                                          emailTemplate.PlaceholderName,
                                          null,
                                          null,
                                          emailTemplate.SubjectPlaceholderName,
                                          null,
                                          null);

        // Need to pass placeholder names properties into update method,
        // since 'null' value is also suitable for these properties.

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// <see cref="EmailTemplate.Update"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="true"/>
    /// due to any changes which were applied to the <see cref="EmailTemplate"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Update_ReturnsTrueOnExistingChanges()
    {
        // Arrange
        var emailTemplate = new EmailTemplate("Template name", "Template text", "Subject")
        {
            IsBodyHtml = true,
            PlaceholderName = "{arg}",
            PlaceholdersNumber = 1,
            SubjectPlaceholderName = "{arg}",
            SubjectPlaceholdersNumber = 1
        };

        var isBodyHtmlNew = emailTemplate.IsBodyHtml!;
        var placeholderNameNew = emailTemplate.PlaceholderName + "Modified";
        var placeholdersNumberNew = (ushort)(emailTemplate.PlaceholdersNumber + 1);
        var subjectNew = emailTemplate.Subject + "Modified";
        var subjectPlaceholderNameNew = emailTemplate.SubjectPlaceholderName + "Modified";
        var subjectPlaceholdersNumberNew = (ushort)(emailTemplate.SubjectPlaceholdersNumber + 1);
        var templateTextNew = emailTemplate.TemplateText + "Modified";

        // Act
        var result = emailTemplate.Update(isBodyHtmlNew,
                                          placeholderNameNew,
                                          placeholdersNumberNew,
                                          subjectNew,
                                          subjectPlaceholderNameNew,
                                          subjectPlaceholdersNumberNew,
                                          templateTextNew);

        // Assert
        Assert.True(result);

        Assert.Equal(isBodyHtmlNew, emailTemplate.IsBodyHtml);
        Assert.Equal(placeholderNameNew, emailTemplate.PlaceholderName);
        Assert.Equal(placeholdersNumberNew, emailTemplate.PlaceholdersNumber);
        Assert.Equal(subjectNew, emailTemplate.Subject);
        Assert.Equal(subjectPlaceholderNameNew, emailTemplate.SubjectPlaceholderName);
        Assert.Equal(subjectPlaceholdersNumberNew, emailTemplate.SubjectPlaceholdersNumber);
        Assert.Equal(templateTextNew, emailTemplate.TemplateText);
    }

    /// <summary>
    /// <see cref="EmailTemplate.ValidateState"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// No exception is thrown because message template
    /// is in it's valid state.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void ValidateState()
    {
        // Arrange
        var emailTemplate = new EmailTemplate("Template name", "Template text", "Subject")
        {
            PlaceholderName = "{arg}",
            PlaceholdersNumber = 1,
            SubjectPlaceholderName = "{arg}",
            SubjectPlaceholdersNumber = 1
        };

        // Act
        emailTemplate.ValidateState();

        // Assert
    }

    /// <summary>
    /// <see cref="EmailTemplate.ValidateState"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="InvalidEntityStateException"/> since the
    /// <see cref="MessageTemplate.PlaceholderName"/> is <see langword="null"/> and
    /// <see cref="EmailTemplate.SubjectPlaceholderName"/> is <see langword="null"/>,
    /// but <see cref="MessageTemplate.PlaceholdersNumber"/>
    /// and <see cref="EmailTemplate.SubjectPlaceholdersNumber"/> are not equal to 0.
    /// </para>
    /// </summary>
    [Fact]
    public void ValidateState_ThrowsOnInvalidState()
    {
        // Arrange
        var emailTemplate = new EmailTemplate("Template name", "Template text", "Subject")
        {
            PlaceholderName = null,
            PlaceholdersNumber = 1,
            SubjectPlaceholderName = null,
            SubjectPlaceholdersNumber = 1
        };

        // Act

        // Assert
        Assert.Throws<InvalidEntityStateException>(
            emailTemplate.ValidateState);
    }
    #endregion

    #region Data generation
    /// <summary>
    /// Provides member data for <see cref="GetFormattedSubject"/> method.
    /// </summary>
    public static IEnumerable<object?[]> GetFormattedSubject_MemberData => new object?[][]
    {
        ["Test",                        null,       (ushort)0,  new List<object?>()],
        ["Test with {arg}0",            "{arg}",    (ushort)1,  new List<object?> { "Value" }],
        ["Test with {arg}0 and {arg}1", "{arg}",    (ushort)2,  new List<object?> { "First value", "Second value" }]
    };
    #endregion
}