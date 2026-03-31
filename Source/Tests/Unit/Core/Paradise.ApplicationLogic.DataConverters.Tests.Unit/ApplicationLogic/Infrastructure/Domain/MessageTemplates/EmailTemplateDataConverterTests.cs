using Paradise.ApplicationLogic.DataConverters.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess.Seed.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using System.Globalization;

namespace Paradise.ApplicationLogic.DataConverters.Tests.Unit.ApplicationLogic.Infrastructure.Domain.MessageTemplates;

/// <summary>
/// <see cref="EmailTemplateDataConverter"/> test class.
/// </summary>
public sealed class EmailTemplateDataConverterTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="ToModel"/> method.
    /// </summary>
    public static TheoryData<CultureInfo?> ToModel_MemberData { get; } = new()
    {
        { null as CultureInfo           },
        { CultureInfo.InvariantCulture  }
    };

    /// <summary>
    /// Provides member data for <see cref="ToEntity"/> method.
    /// </summary>
    public static TheoryData<int?> ToEntity_MemberData { get; } = new()
    {
        { null as int?  },
        { 33            }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="EmailTemplateDataConverter.ToEntity"/> method should
    /// return a new <see cref="EmailTemplate"/> instance populated
    /// with the data from the input <see cref="EmailTemplateCreationModel"/> object.
    /// </summary>
    /// <param name="cultureId">
    /// Template culture LCID.
    /// </param>
    [Theory, MemberData(nameof(ToEntity_MemberData))]
    public void ToEntity(int? cultureId)
    {
        // Arrange
        var model = new EmailTemplateCreationModel(
            templateName: "TemplateName",
            cultureId: cultureId,
            templateText: "TemplateText",
            subject: "Subject",
            placeholderName: null,
            placeholdersNumber: 0,
            subjectPlaceholderName: null,
            subjectPlaceholdersNumber: 0,
            isBodyHtml: false
        );

        // Act
        var result = model.ToEntity();

        // Assert
        Assert.Equal(model.TemplateName, result.TemplateName);
        Assert.Equal(model.CultureId, result.Culture?.LCID);
        Assert.Equal(model.TemplateText, result.TemplateText);
        Assert.Equal(model.Subject, result.Subject);
        Assert.Equal(model.PlaceholderName, result.PlaceholderName);
        Assert.Equal(model.PlaceholdersNumber, result.PlaceholdersNumber);
        Assert.Equal(model.SubjectPlaceholderName, result.SubjectPlaceholderName);
        Assert.Equal(model.SubjectPlaceholdersNumber, result.SubjectPlaceholdersNumber);
        Assert.Equal(model.IsBodyHtml, result.IsBodyHtml);
    }

    /// <summary>
    /// The <see cref="EmailTemplateDataConverter.ToEntity"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="EmailTemplateCreationModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void ToEntity_ThrowsOnNull()
    {
        // Arrange
        var model = null as EmailTemplateCreationModel;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => model!.ToEntity());
    }

    /// <summary>
    /// The <see cref="EmailTemplateDataConverter.ToModel"/> method should
    /// return a new <see cref="EmailTemplateModel"/> instance populated
    /// with the data from the input <see cref="EmailTemplate"/> object.
    /// </summary>
    /// <param name="culture">
    /// Template culture.
    /// </param>
    [Theory, MemberData(nameof(ToModel_MemberData))]
    public void ToModel(CultureInfo? culture)
    {
        // Arrange
        var entity = new EmailTemplate(templateName: "TemplateName", culture: culture, templateText: "TemplateText", subject: "Subject")
        {
            IsBodyHtml = false,
            PlaceholderName = null,
            PlaceholdersNumber = 0,
            SubjectPlaceholderName = null,
            SubjectPlaceholdersNumber = 0
        };

        // Act
        var result = entity.ToModel();

        // Assert
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.Created, result.Created);
        Assert.Equal(entity.Modified, result.Modified);
        Assert.Equal(entity.TemplateName, result.TemplateName);
        Assert.Equal(entity.Culture?.LCID, result.CultureId);
        Assert.Equal(entity.TemplateText, result.TemplateText);
        Assert.Equal(entity.Subject, result.Subject);
        Assert.Equal(entity.PlaceholderName, result.PlaceholderName);
        Assert.Equal(entity.PlaceholdersNumber, result.PlaceholdersNumber);
        Assert.Equal(entity.SubjectPlaceholderName, result.SubjectPlaceholderName);
        Assert.Equal(entity.SubjectPlaceholdersNumber, result.SubjectPlaceholdersNumber);
        Assert.Equal(entity.IsBodyHtml, result.IsBodyHtml);
    }

    /// <summary>
    /// The <see cref="EmailTemplateDataConverter.ToModel"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="EmailTemplate"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void ToModel_ThrowsOnNull()
    {
        // Arrange
        var entity = null as EmailTemplate;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => entity!.ToModel());
    }

    /// <summary>
    /// The <see cref="EmailTemplateDataConverter.ToCreationModel"/> method should
    /// return a new <see cref="EmailTemplateCreationModel"/> instance populated
    /// with the data from the input <see cref="SeedEmailTemplateModel"/> object.
    /// </summary>
    [Fact]
    public void ToCreationModel()
    {
        // Arrange
        var model = new SeedEmailTemplateModel(
            templateName: "TemplateName",
            cultureId: null,
            subject: "Subject",
            isBodyHtml: false,
            placeholderName: null,
            placeholdersNumber: 0,
            subjectPlaceholderName: null,
            subjectPlaceholdersNumber: 0,
            templateText: "TemplateText");

        // Act
        var result = model.ToCreationModel();

        // Assert
        Assert.Equal(model.TemplateName, result.TemplateName);
        Assert.Equal(model.CultureId, result.CultureId);
        Assert.Equal(model.TemplateText, result.TemplateText);
        Assert.Equal(model.Subject, result.Subject);
        Assert.Equal(model.PlaceholderName, result.PlaceholderName);
        Assert.Equal(model.PlaceholdersNumber, result.PlaceholdersNumber);
        Assert.Equal(model.SubjectPlaceholderName, result.SubjectPlaceholderName);
        Assert.Equal(model.SubjectPlaceholdersNumber, result.SubjectPlaceholdersNumber);
        Assert.Equal(model.IsBodyHtml, result.IsBodyHtml);
    }

    /// <summary>
    /// The <see cref="EmailTemplateDataConverter.ToCreationModel"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="SeedEmailTemplateModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void ToCreationModel_ThrowsOnNull()
    {
        // Arrange
        var model = null as SeedEmailTemplateModel;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => model!.ToCreationModel());
    }

    /// <summary>
    /// The <see cref="EmailTemplateDataConverter.ToUpdateModel"/> method should
    /// return a new <see cref="EmailTemplateUpdateModel"/> instance populated
    /// with the data from the input <see cref="SeedEmailTemplateModel"/> object.
    /// </summary>
    [Fact]
    public void ToUpdateModel()
    {
        // Arrange
        var model = new SeedEmailTemplateModel(
            templateName: "TemplateName",
            cultureId: null,
            subject: "Subject",
            isBodyHtml: false,
            placeholderName: null,
            placeholdersNumber: 0,
            subjectPlaceholderName: null,
            subjectPlaceholdersNumber: 0,
            templateText: "TemplateText");

        // Act
        var result = model.ToUpdateModel();

        // Assert
        Assert.Equal(model.TemplateText, result.TemplateText);
        Assert.Equal(model.Subject, result.Subject);
        Assert.Equal(model.PlaceholderName, result.PlaceholderName);
        Assert.Equal(model.PlaceholdersNumber, result.PlaceholdersNumber);
        Assert.Equal(model.SubjectPlaceholderName, result.SubjectPlaceholderName);
        Assert.Equal(model.SubjectPlaceholdersNumber, result.SubjectPlaceholdersNumber);
        Assert.Equal(model.IsBodyHtml, result.IsBodyHtml);
    }

    /// <summary>
    /// The <see cref="EmailTemplateDataConverter.ToUpdateModel"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="SeedEmailTemplateModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void ToUpdateModel_ThrowsOnNull()
    {
        // Arrange
        var model = null as SeedEmailTemplateModel;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => model!.ToUpdateModel());
    }
    #endregion
}