using Paradise.ApplicationLogic.Infrastructure.Services.Implementation;
using Paradise.Models;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using System.Globalization;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.Services.Implementation;

/// <summary>
/// <see cref="EmailTemplateService"/> test class.
/// </summary>
public sealed partial class EmailTemplateServiceTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="CreateAsync_FailsOnMissingSubject"/> method.
    /// </summary>
    public static TheoryData<string?> CreateAsync_FailsOnMissingSubject_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };

    /// <summary>
    /// Provides member data for <see cref="CreateAsync_FailsOnMissingTemplateName"/> method.
    /// </summary>
    public static TheoryData<string?> CreateAsync_FailsOnMissingTemplateName_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };

    /// <summary>
    /// Provides member data for <see cref="CreateAsync_FailsOnMissingTemplateText"/> method.
    /// </summary>
    public static TheoryData<string?> CreateAsync_FailsOnMissingTemplateText_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };

    /// <summary>
    /// Provides member data for <see cref="CreateAsync_FailsOnDuplicateNameAndCulture"/> method.
    /// </summary>
    public static TheoryData<CultureInfo?> CreateAsync_FailsOnDuplicateNameAndCulture_MemberData { get; } = new()
    {
        { null as CultureInfo           },
        { CultureInfo.InvariantCulture  }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="EmailTemplateService.CreateAsync"/> method should
    /// create a new email template.
    /// </summary>
    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var model = GetCreationModel();

        // Act
        var result = await Test.Target.CreateAsync(model, Token);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(Test.EmailTemplateExists(result.Value.Id));
    }

    /// <summary>
    /// The <see cref="EmailTemplateService.CreateAsync"/> method should
    /// fail to create a new email template if the input
    /// <see cref="EmailTemplateCreationModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task CreateAsync_FailsOnNull()
    {
        // Arrange
        var model = null as EmailTemplateCreationModel;

        // Act
        var result = await Test.Target.CreateAsync(model!, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.False(result.IsSuccess);
        Assert.Equal(OperationStatus.InvalidInput, result.Status);

        var error = Assert.Single(result.Errors);
        Assert.Equal(ErrorCode.InvalidModel, error.Code);
    }

    /// <summary>
    /// The <see cref="EmailTemplateService.CreateAsync"/> method should
    /// fail to create a new email template if the
    /// input <see cref="EmailTemplateCreationModel"/> does not
    /// contain a valid subject.
    /// </summary>
    /// <param name="subject">
    /// Email subject.
    /// </param>
    [Theory, MemberData(nameof(CreateAsync_FailsOnMissingSubject_MemberData))]
    public async Task CreateAsync_FailsOnMissingSubject(string? subject)
    {
        // Arrange
        var model = GetCreationModel(subject: subject!);

        // Act
        var result = await Test.Target.CreateAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.False(result.IsSuccess);
        Assert.Equal(OperationStatus.InvalidInput, result.Status);

        var error = Assert.Single(result.Errors);
        Assert.Equal(ErrorCode.MessageTemplateEmptyText, error.Code);
    }

    /// <summary>
    /// The <see cref="EmailTemplateService.CreateAsync"/> method should
    /// fail to create a new email template if the
    /// input <see cref="EmailTemplateCreationModel"/> does not
    /// contain a valid template name.
    /// </summary>
    /// <param name="templateName">
    /// Template name.
    /// </param>
    [Theory, MemberData(nameof(CreateAsync_FailsOnMissingTemplateName_MemberData))]
    public async Task CreateAsync_FailsOnMissingTemplateName(string? templateName)
    {
        // Arrange
        var model = GetCreationModel(templateName: templateName!);

        // Act
        var result = await Test.Target.CreateAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.False(result.IsSuccess);
        Assert.Equal(OperationStatus.InvalidInput, result.Status);

        var error = Assert.Single(result.Errors);
        Assert.Equal(ErrorCode.MessageTemplateMissingName, error.Code);
    }

    /// <summary>
    /// The <see cref="EmailTemplateService.CreateAsync"/> method should
    /// fail to create a new email template if the
    /// input <see cref="EmailTemplateCreationModel"/> does not
    /// contain a valid template text.
    /// </summary>
    /// <param name="templateText">
    /// Template text.
    /// </param>
    [Theory, MemberData(nameof(CreateAsync_FailsOnMissingTemplateText_MemberData))]
    public async Task CreateAsync_FailsOnMissingTemplateText(string? templateText)
    {
        // Arrange
        var model = GetCreationModel(templateText: templateText!);

        // Act
        var result = await Test.Target.CreateAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.False(result.IsSuccess);
        Assert.Equal(OperationStatus.InvalidInput, result.Status);

        var error = Assert.Single(result.Errors);
        Assert.Equal(ErrorCode.MessageTemplateEmptyText, error.Code);
    }

    /// <summary>
    /// The <see cref="EmailTemplateService.CreateAsync"/> method should
    /// fail to create a new email template if the
    /// input <see cref="EmailTemplateCreationModel"/> specifies the
    /// template name and culture which are already in use by existing template.
    /// </summary>
    /// <param name="culture">
    /// Template culture.
    /// </param>
    [Theory, MemberData(nameof(CreateAsync_FailsOnDuplicateNameAndCulture_MemberData))]
    public async Task CreateAsync_FailsOnDuplicateNameAndCulture(CultureInfo? culture)
    {
        // Arrange
        var templateName = "TemplateName";

        Test.AddEmailTemplate(templateName, culture);

        var model = GetCreationModel(templateName: templateName, cultureId: culture?.LCID);

        // Act
        var result = await Test.Target.CreateAsync(model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.False(result.IsSuccess);
        Assert.Equal(OperationStatus.InvalidInput, result.Status);

        var error = Assert.Single(result.Errors);
        Assert.Equal(ErrorCode.MessageTemplateAlreadyExists, error.Code);
    }

    /// <summary>
    /// The <see cref="EmailTemplateService.DeleteAsync"/> method should
    /// delete an email template.
    /// </summary>
    [Fact]
    public async Task DeleteAsync()
    {
        // Arrange
        var template = Test.AddEmailTemplate();

        var id = template.Id;

        // Act
        var result = await Test.Target.DeleteAsync(id, Token);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.Empty(result.Errors);

        Assert.False(Test.EmailTemplateExists(id));
    }

    /// <summary>
    /// The <see cref="EmailTemplateService.DeleteAsync"/> method should
    /// delete an email template and return successful result upon repetitive
    /// deletion attempts, effectively behaving as idempotent.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_Idempotent()
    {
        // Arrange
        var template = Test.AddEmailTemplate();

        var id = template.Id;

        // Act
        var firstAttempt = await Test.Target.DeleteAsync(id, Token);
        var secondAttempt = await Test.Target.DeleteAsync(id, Token);

        // Assert
        Assert.True(firstAttempt.IsSuccess);
        Assert.Equal(OperationStatus.Success, firstAttempt.Status);
        Assert.Empty(firstAttempt.Errors);

        Assert.True(secondAttempt.IsSuccess);
        Assert.Equal(OperationStatus.Success, secondAttempt.Status);
        Assert.Empty(secondAttempt.Errors);

        Assert.False(Test.EmailTemplateExists(id));
    }

    /// <summary>
    /// The <see cref="EmailTemplateService.GetAllAsync"/> method should
    /// return the list of email templates.
    /// </summary>
    [Fact]
    public async Task GetAllAsync()
    {
        // Arrange
        Test.AddEmailTemplate();

        // Act
        var result = await Test.Target.GetAllAsync(Token);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.Empty(result.Errors);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
    }

    /// <summary>
    /// The <see cref="EmailTemplateService.GetByIdAsync"/> method should
    /// return an email template with the specified Id.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync()
    {
        // Arrange
        var template = Test.AddEmailTemplate();

        // Act
        var result = await Test.Target.GetByIdAsync(template.Id, Token);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.Empty(result.Errors);
        Assert.NotNull(result.Value);

        Assert.Equal(template.Id, result.Value.Id);
        Assert.Equal(template.TemplateName, result.Value.TemplateName);
        Assert.Equal(template.Culture?.LCID, result.Value.CultureId);
    }

    /// <summary>
    /// The <see cref="EmailTemplateService.GetByIdAsync"/> method should
    /// fail to retrieve an email template
    /// when no template with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_FailsOnNonExistingEmailTemplate()
    {
        // Arrange
        var id = Guid.Empty;

        // Act
        var result = await Test.Target.GetByIdAsync(id, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.False(result.IsSuccess);
        Assert.Equal(OperationStatus.Missing, result.Status);

        var error = Assert.Single(result.Errors);
        Assert.Equal(ErrorCode.MessageTemplateIdNotFound, error.Code);
    }

    /// <summary>
    /// The <see cref="EmailTemplateService.GetByNameAndCultureAsync"/> method should
    /// return an email template with the specified name and culture.
    /// </summary>
    [Fact]
    public async Task GetByNameAndCultureAsync()
    {
        // Arrange
        var templateName = "TemplateName";
        var culture = null as CultureInfo;

        var template = Test.AddEmailTemplate(templateName, culture);

        // Act
        var result = await Test.Target.GetByNameAndCultureAsync(templateName, culture, Token);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.Empty(result.Errors);
        Assert.NotNull(result.Value);

        Assert.Equal(template.Id, result.Value.Id);
        Assert.Equal(template.TemplateName, result.Value.TemplateName);
        Assert.Equal(template.Culture?.LCID, result.Value.CultureId);
    }

    /// <summary>
    /// The <see cref="EmailTemplateService.GetByNameAndCultureAsync"/> method should
    /// fail to retrieve an email template
    /// when no template with the specified name and culture exists.
    /// </summary>
    [Fact]
    public async Task GetByNameAndCultureAsync_FailsOnNonExistingEmailTemplate()
    {
        // Arrange
        var templateName = "TemplateName";
        var culture = null as CultureInfo;

        // Act
        var result = await Test.Target.GetByNameAndCultureAsync(templateName, culture, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.False(result.IsSuccess);
        Assert.Equal(OperationStatus.Missing, result.Status);

        var error = Assert.Single(result.Errors);
        Assert.Equal(ErrorCode.MessageTemplateNotFound, error.Code);
    }

    /// <summary>
    /// The <see cref="EmailTemplateService.UpdateAsync"/> method should
    /// update an email template with the input <see cref="EmailTemplateUpdateModel"/>
    /// and return <see cref="OperationStatus.Success"/> if the
    /// changes were made and persisted.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ReturnsSuccessOnChanged()
    {
        // Arrange
        var template = Test.AddEmailTemplate();

        var model = new EmailTemplateUpdateModel
        {
            IsBodyHtml = !template.IsBodyHtml,
            PlaceholderName = template.PlaceholderName,
            PlaceholdersNumber = template.PlaceholdersNumber,
            Subject = template.Subject,
            SubjectPlaceholderName = template.SubjectPlaceholderName,
            SubjectPlaceholdersNumber = template.SubjectPlaceholdersNumber,
            TemplateText = template.TemplateText
        };

        // Act
        var result = await Test.Target.UpdateAsync(template.Id, model, Token);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.Empty(result.Errors);
        Assert.NotNull(result.Value);

        Assert.Equal(template.Id, result.Value.Id);
        Assert.Equal(model.IsBodyHtml, result.Value.IsBodyHtml);
    }

    /// <summary>
    /// The <see cref="EmailTemplateService.UpdateAsync"/> method should
    /// update an email template with the input <see cref="EmailTemplateUpdateModel"/>
    /// and return <see cref="OperationStatus.Received"/> if no
    /// changes were made and persisted.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ReturnsReceivedOnUnchanged()
    {
        // Arrange
        var template = Test.AddEmailTemplate();

        var model = new EmailTemplateUpdateModel
        {
            IsBodyHtml = template.IsBodyHtml,
            PlaceholderName = template.PlaceholderName,
            PlaceholdersNumber = template.PlaceholdersNumber,
            Subject = template.Subject,
            SubjectPlaceholderName = template.SubjectPlaceholderName,
            SubjectPlaceholdersNumber = template.SubjectPlaceholdersNumber,
            TemplateText = template.TemplateText
        };

        // Act
        var result = await Test.Target.UpdateAsync(template.Id, model, Token);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(OperationStatus.Received, result.Status);
        Assert.Empty(result.Errors);
        Assert.NotNull(result.Value);

        Assert.Equal(template.Id, result.Value.Id);
    }

    /// <summary>
    /// The <see cref="EmailTemplateService.UpdateAsync"/> method should
    /// fail to update an email template if the input
    /// <see cref="EmailTemplateUpdateModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_FailsOnNull()
    {
        // Arrange
        var id = Guid.Empty;
        var model = null as EmailTemplateUpdateModel;

        // Act
        var result = await Test.Target.UpdateAsync(id, model!, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.False(result.IsSuccess);
        Assert.Equal(OperationStatus.InvalidInput, result.Status);

        var error = Assert.Single(result.Errors);
        Assert.Equal(ErrorCode.InvalidModel, error.Code);
    }

    /// <summary>
    /// The <see cref="EmailTemplateService.UpdateAsync"/> method should
    /// fail to update an email template
    /// when no template with the specified Id exists.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_FailsOnNonExistingEmailTemplate()
    {
        // Arrange
        var id = Guid.Empty;
        var model = new EmailTemplateUpdateModel();

        // Act
        var result = await Test.Target.UpdateAsync(id, model, Token);

        // Assert
        Assert.Null(result.Value);
        Assert.False(result.IsSuccess);
        Assert.Equal(OperationStatus.Missing, result.Status);

        var error = Assert.Single(result.Errors);
        Assert.Equal(ErrorCode.MessageTemplateIdNotFound, error.Code);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailTemplateCreationModel"/> class.
    /// </summary>
    /// <param name="templateName">
    /// Template name.
    /// </param>
    /// <param name="cultureId">
    /// Template culture language code identifier.
    /// </param>
    /// <param name="templateText">
    /// Template text.
    /// </param>
    /// <param name="subject">
    /// Email subject.
    /// </param>
    /// <returns>
    /// A new instance of the <see cref="EmailTemplateCreationModel"/> class.
    /// </returns>
    private static EmailTemplateCreationModel GetCreationModel(string templateName = "TemplateName",
                                                              int? cultureId = null,
                                                              string templateText = "TemplateText",
                                                              string subject = "Subject")
    {
        return new EmailTemplateCreationModel(templateName: templateName,
                                              cultureId: cultureId,
                                              templateText: templateText,
                                              subject: subject,
                                              placeholderName: null,
                                              placeholdersNumber: 0,
                                              subjectPlaceholderName: null,
                                              subjectPlaceholdersNumber: 0,
                                              isBodyHtml: false);
    }
    #endregion
}