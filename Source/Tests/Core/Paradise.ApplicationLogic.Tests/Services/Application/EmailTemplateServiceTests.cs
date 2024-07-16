using Paradise.ApplicationLogic.Domain.MessageTemplates;
using Paradise.DataAccess.Repositories.Application.Implementation;
using Paradise.DataAccess.Repositories.Base;
using Paradise.Models.Application.EmailTemplateModels;
using System.Globalization;

namespace Paradise.ApplicationLogic.Tests.Services.Application;

/// <summary>
/// Test class for the <see cref="EmailTemplateService"/> methods.
/// </summary>
public sealed class EmailTemplateServiceTests
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailTemplateServiceTests"/> class.
    /// </summary>
    public EmailTemplateServiceTests()
    {
        var applicationDataSource = GetApplicationDataSource();
        var repository = new EmailTemplatesRepository(applicationDataSource);

        Source = applicationDataSource;
        Service = new(repository);
    }
    #endregion

    #region Properties
    /// <summary>
    /// A <see cref="IDataSource"/> instance used to
    /// arrange data and validate test results.
    /// </summary>
    public IDataSource Source { get; }

    /// <summary>
    /// A <see cref="EmailTemplateService"/> instance to be tested.
    /// </summary>
    public EmailTemplateService Service { get; }
    #endregion

    #region Public methods
    /// <summary>
    /// <see cref="EmailTemplateService.CreateAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Creates a new email template.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var creationModel = new EmailTemplateCreationModel("Test", "Test", "Test")
        {
            CultureId = 1,
            IsBodyHtml = true,
            PlaceholderName = "{parameter}",
            PlaceholdersNumber = 0
        };

        // Act
        var result = await Service.CreateAsync(creationModel);

        // Assert
        result.AssertSuccess(Created);
        Assert.NotNull(result.Value);
        Assert.NotNull(Source.GetQueryable<EmailTemplate>().ToList().Find(t => t.Id == result.Value.Id));
    }

    /// <summary>
    /// <see cref="EmailTemplateService.CreateAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="EmailTemplateCreationModel"/> provided
    /// does not have a <see cref="EmailTemplateCreationModel.TemplateText"/> specified.
    /// </para>
    /// </summary>
    [Fact]
    public async Task CreateAsync_ThrowsOnEmptyText()
    {
        // Arrange
        var creationModel = new EmailTemplateCreationModel("Test", "Test", string.Empty)
        {
            CultureId = 1,
            IsBodyHtml = true,
            PlaceholderName = "{parameter}",
            PlaceholdersNumber = 0
        };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.CreateAsync(creationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, MessageTemplateEmptyText);
    }

    /// <summary>
    /// <see cref="EmailTemplateService.CreateAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="EmailTemplateCreationModel"/> provided
    /// does not have a <see cref="EmailTemplateCreationModel.TemplateText"/> with the necessary number of placeholders.
    /// </para>
    /// </summary>
    [Fact]
    public async Task CreateAsync_ThrowsOnMissingPlaceholder()
    {
        // Arrange
        var creationModel = new EmailTemplateCreationModel("Test", "Test", "Test")
        {
            CultureId = 1,
            IsBodyHtml = true,
            PlaceholderName = "{parameter}",
            PlaceholdersNumber = 1
        };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.CreateAsync(creationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, MessageTemplateMissingPlaceholder);
    }

    /// <summary>
    /// <see cref="EmailTemplateService.CreateAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="EmailTemplateCreationModel"/> provided
    /// have a <see cref="EmailTemplateCreationModel.CultureId"/>
    /// and <see cref="EmailTemplateCreationModel.TemplateName"/> which are already in use.
    /// </para>
    /// </summary>
    [Fact]
    public async Task CreateAsync_ThrowsOnTakenNameAndCulture()
    {
        // Arrange
        var cultureId = 1;
        var templateName = "Test";

        var template = new EmailTemplate(templateName, "Test", "Test")
        {
            Culture = CultureInfo.GetCultureInfo(cultureId)
        };

        Source.Add(template);
        await Source.SaveChangesAsync();

        var creationModel = new EmailTemplateCreationModel("Test", templateName, "Test")
        {
            CultureId = cultureId,
            IsBodyHtml = true,
            PlaceholderName = "{parameter}",
            PlaceholdersNumber = 0
        };

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.CreateAsync(creationModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(UnprocessableEntity, MessageTemplateAlreadyExists);
    }

    /// <summary>
    /// <see cref="EmailTemplateService.DeleteAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Completely removes all data of an email template
    /// with the specified Id.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task DeleteAsync()
    {
        // Arrange
        var template = new EmailTemplate("Test", "Test", "Test")
        {
            Culture = CultureInfo.InvariantCulture,
            PlaceholderName = "{parameter}",
            PlaceholdersNumber = 1
        };

        Source.Add(template);
        await Source.SaveChangesAsync();

        // Act
        var result = await Service.DeleteAsync(template.Id);

        // Assert
        result.AssertSuccess(OK);
    }

    /// <summary>
    /// <see cref="EmailTemplateService.GetAllAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns a complete list of email templates.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task GetAllAsync()
    {
        // Arrange
        var template = new EmailTemplate("Test", "Test", "Test")
        {
            Culture = CultureInfo.InvariantCulture,
            PlaceholderName = "{parameter}",
            PlaceholdersNumber = 1
        };

        Source.Add(template);
        await Source.SaveChangesAsync();

        // Act
        var result = await Service.GetAllAsync();

        // Assert
        result.AssertSuccess(OK);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
    }

    /// <summary>
    /// <see cref="EmailTemplateService.GetByIdAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns the email template found.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task GetByIdAsync()
    {
        // Arrange
        var template = new EmailTemplate("Test", "Test", "Test")
        {
            Culture = CultureInfo.InvariantCulture,
            PlaceholderName = "{parameter}",
            PlaceholdersNumber = 1
        };

        Source.Add(template);
        await Source.SaveChangesAsync();

        // Act
        var result = await Service.GetByIdAsync(template.Id);

        // Assert
        result.AssertSuccess(OK);
        Assert.NotNull(result.Value);
        Assert.Equal(template.Id, result.Value.Id);
        Assert.Equal(template.TemplateName, result.Value.TemplateName);
        Assert.Equal(template.Culture?.LCID, result.Value.CultureId);
    }

    /// <summary>
    /// <see cref="EmailTemplateService.GetByIdAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="EmailTemplate"/>
    /// with the specified Id does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ThrowsOnNonExistingEmailTemplate()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.GetByIdAsync(Guid.Empty));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, MessageTemplateIdNotFound);
    }
    /// <summary>
    /// <see cref="EmailTemplateService.UpdateAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Updates the email template with the new values.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task UpdateTemplateAsync()
    {
        // Arrange
        const ushort PlaceholdersNumber = 1;
        const ushort PlaceholdersNumberToChange = PlaceholdersNumber + PlaceholdersNumber;

        var updateModel = new EmailTemplateUpdateModel
        {
            IsBodyHtml = true,
            PlaceholderName = "{parameter}",
            PlaceholdersNumber = PlaceholdersNumber,
            Subject = "UpdateTest {parameter}0",
            SubjectPlaceholderName = "{parameter}",
            SubjectPlaceholdersNumber = PlaceholdersNumber,
            TemplateText = "UpdateTest {parameter}0"
        };

        var template = new EmailTemplate("Test", "Original" + updateModel.Subject, "Original" + updateModel.TemplateText)
        {
            Culture = CultureInfo.InvariantCulture,
            IsBodyHtml = !updateModel.IsBodyHtml.Value,
            PlaceholderName = "Original" + updateModel.PlaceholderName,
            PlaceholdersNumber = PlaceholdersNumberToChange,
            SubjectPlaceholderName = "Original" + updateModel.SubjectPlaceholderName,
            SubjectPlaceholdersNumber = PlaceholdersNumberToChange
        };

        Source.Add(template);
        await Source.SaveChangesAsync();

        // Act
        var result = await Service.UpdateAsync(template.Id, updateModel);

        // Assert
        result.AssertSuccess(OK);
        Assert.NotNull(result.Value);
        Assert.Equal(updateModel.IsBodyHtml, result.Value.IsBodyHtml);
        Assert.Equal(updateModel.PlaceholderName, result.Value.PlaceholderName);
        Assert.Equal(updateModel.PlaceholdersNumber, result.Value.PlaceholdersNumber);
        Assert.Equal(updateModel.Subject, result.Value.Subject);
        Assert.Equal(updateModel.SubjectPlaceholderName, result.Value.SubjectPlaceholderName);
        Assert.Equal(updateModel.SubjectPlaceholdersNumber, result.Value.SubjectPlaceholdersNumber);
        Assert.Equal(updateModel.TemplateText, result.Value.TemplateText);
    }

    /// <summary>
    /// <see cref="EmailTemplateService.UpdateAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="EmailTemplateUpdateModel"/> provided
    /// have a <see cref="EmailTemplateUpdateModel.TemplateText"/> without required placeholders inside.
    /// </para>
    /// </summary>
    [Fact]
    public async Task UpdateTemplateAsync_ThrowsOnBodyMissingPlaceholder()
    {
        // Arrange
        var updateModel = new EmailTemplateUpdateModel
        {
            IsBodyHtml = true,
            PlaceholderName = "{parameter}",
            PlaceholdersNumber = 1,
            Subject = "UpdateTest {parameter}0",
            SubjectPlaceholderName = "{parameter}",
            SubjectPlaceholdersNumber = 0,
            TemplateText = "UpdateTest"
        };

        var template = new EmailTemplate("Test", "Original" + updateModel.Subject, "Original" + updateModel.TemplateText)
        {
            Culture = CultureInfo.InvariantCulture,
            IsBodyHtml = !updateModel.IsBodyHtml.Value,
            PlaceholderName = "Original" + updateModel.PlaceholderName,
            PlaceholdersNumber = 0,
            SubjectPlaceholderName = "Original" + updateModel.SubjectPlaceholderName,
            SubjectPlaceholdersNumber = 0,
            TemplateName = "Test"
        };

        Source.Add(template);
        await Source.SaveChangesAsync();

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.UpdateAsync(template.Id, updateModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, MessageTemplateMissingPlaceholder);
    }

    /// <summary>
    /// <see cref="EmailTemplateService.UpdateAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="EmailTemplate"/>
    /// with the specified Id does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async Task UpdateTemplateAsync_ThrowsOnNonExistingEmailTemplate()
    {
        // Arrange

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.UpdateAsync(Guid.Empty, new()));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, MessageTemplateIdNotFound);
    }

    /// <summary>
    /// <see cref="EmailTemplateService.UpdateAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="EmailTemplateUpdateModel"/> provided
    /// have a <see cref="EmailTemplateUpdateModel.Subject"/> without required placeholders inside.
    /// </para>
    /// </summary>
    [Fact]
    public async Task UpdateTemplateAsync_ThrowsOnSubjectMissingPlaceholder()
    {
        // Arrange
        var updateModel = new EmailTemplateUpdateModel
        {
            IsBodyHtml = true,
            PlaceholderName = "{parameter}",
            PlaceholdersNumber = 0,
            Subject = "UpdateTest",
            SubjectPlaceholderName = "{parameter}",
            SubjectPlaceholdersNumber = 1,
            TemplateText = "UpdateTest {parameter}0"
        };

        var template = new EmailTemplate("Test", "Original" + updateModel.Subject, "Original" + updateModel.TemplateText)
        {
            Culture = CultureInfo.InvariantCulture,
            IsBodyHtml = !updateModel.IsBodyHtml.Value,
            PlaceholderName = "Original" + updateModel.PlaceholderName,
            PlaceholdersNumber = 0,
            SubjectPlaceholderName = "Original" + updateModel.SubjectPlaceholderName,
            SubjectPlaceholdersNumber = 0,
            TemplateName = "Test"
        };

        Source.Add(template);
        await Source.SaveChangesAsync();

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.UpdateAsync(template.Id, updateModel));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, MessageTemplateMissingPlaceholder);
    }
    #endregion
}