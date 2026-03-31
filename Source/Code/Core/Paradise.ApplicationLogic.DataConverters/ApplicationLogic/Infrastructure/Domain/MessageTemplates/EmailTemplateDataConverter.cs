using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess.Seed.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using System.Globalization;

namespace Paradise.ApplicationLogic.DataConverters.ApplicationLogic.Infrastructure.Domain.MessageTemplates;

/// <summary>
/// Contains extension methods for <see cref="EmailTemplate"/> conversion operations.
/// </summary>
public static class EmailTemplateDataConverter
{
    #region Public methods
    /// <summary>
    /// Converts the <see cref="EmailTemplateCreationModel"/> into the <see cref="EmailTemplate"/>.
    /// </summary>
    /// <param name="creationModel">
    /// The input <see cref="EmailTemplateCreationModel"/> to be converted.
    /// </param>
    /// <returns>
    /// A new <see cref="EmailTemplate"/> instance
    /// converted from the input <paramref name="creationModel"/>.
    /// </returns>
    public static EmailTemplate ToEntity(this EmailTemplateCreationModel creationModel)
    {
        ArgumentNullException.ThrowIfNull(creationModel);

        var templateName = creationModel.TemplateName;
        var culture = creationModel.CultureId.HasValue ? CultureInfo.GetCultureInfo(creationModel.CultureId.Value) : null;
        var templateText = creationModel.TemplateText;
        var subject = creationModel.Subject;
        var isBodyHtml = creationModel.IsBodyHtml;
        var placeholderName = creationModel.PlaceholderName;
        var placeholdersNumber = creationModel.PlaceholdersNumber;
        var subjectPlaceholderName = creationModel.SubjectPlaceholderName;
        var subjectPlaceholdersNumber = creationModel.SubjectPlaceholdersNumber;

        return new(templateName, culture, templateText, subject)
        {
            IsBodyHtml = isBodyHtml,
            PlaceholderName = placeholderName,
            PlaceholdersNumber = placeholdersNumber,
            SubjectPlaceholderName = subjectPlaceholderName,
            SubjectPlaceholdersNumber = subjectPlaceholdersNumber
        };
    }

    /// <summary>
    /// Converts the <see cref="EmailTemplate"/> into the <see cref="EmailTemplateModel"/>.
    /// </summary>
    /// <param name="template">
    /// The input <see cref="EmailTemplate"/> to be converted.
    /// </param>
    /// <returns>
    /// A new <see cref="EmailTemplateModel"/> instance
    /// converted from the input <paramref name="template"/>.
    /// </returns>
    public static EmailTemplateModel ToModel(this EmailTemplate template)
    {
        ArgumentNullException.ThrowIfNull(template);

        var id = template.Id;
        var created = template.Created;
        var modified = template.Modified;
        var templateName = template.TemplateName;
        var culture = template.Culture?.LCID;
        var templateText = template.TemplateText;
        var subject = template.Subject;
        var placeholderName = template.PlaceholderName;
        var placeholdersNumber = template.PlaceholdersNumber;
        var subjectPlaceholderName = template.SubjectPlaceholderName;
        var subjectPlaceholdersNumber = template.SubjectPlaceholdersNumber;
        var isBodyHtml = template.IsBodyHtml;

        return new(id, created, modified, templateName, culture,
                   templateText, subject, placeholderName, placeholdersNumber,
                   subjectPlaceholderName, subjectPlaceholdersNumber, isBodyHtml);
    }

    /// <summary>
    /// Converts the <see cref="SeedEmailTemplateModel"/> into the <see cref="EmailTemplateCreationModel"/>.
    /// </summary>
    /// <param name="model">
    /// The input <see cref="SeedEmailTemplateModel"/> to be converted.
    /// </param>
    /// <returns>
    /// A new <see cref="EmailTemplateCreationModel"/> instance
    /// converted from the input <paramref name="model"/>.
    /// </returns>
    public static EmailTemplateCreationModel ToCreationModel(this SeedEmailTemplateModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var templateName = model.TemplateName;
        var cultureId = model.CultureId;
        var templateText = model.TemplateText;
        var subject = model.Subject;
        var placeholderName = model.PlaceholderName;
        var placeholdersNumber = model.PlaceholdersNumber;
        var subjectPlaceholderName = model.SubjectPlaceholderName;
        var subjectPlaceholdersNumber = model.SubjectPlaceholdersNumber;
        var isBodyHtml = model.IsBodyHtml;

        return new(templateName, cultureId,
                   templateText, subject, placeholderName, placeholdersNumber,
                   subjectPlaceholderName, subjectPlaceholdersNumber, isBodyHtml);
    }

    /// <summary>
    /// Converts the <see cref="SeedEmailTemplateModel"/> into the <see cref="EmailTemplateUpdateModel"/>.
    /// </summary>
    /// <param name="model">
    /// The input <see cref="SeedEmailTemplateModel"/> to be converted.
    /// </param>
    /// <returns>
    /// A new <see cref="EmailTemplateUpdateModel"/> instance
    /// converted from the input <paramref name="model"/>.
    /// </returns>
    public static EmailTemplateUpdateModel ToUpdateModel(this SeedEmailTemplateModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var isBodyHtml = model.IsBodyHtml;
        var placeholderName = model.PlaceholderName;
        var placeholdersNumber = model.PlaceholdersNumber;
        var subject = model.Subject;
        var subjectPlaceholderName = model.SubjectPlaceholderName;
        var subjectPlaceholdersNumber = model.SubjectPlaceholdersNumber;
        var templateText = model.TemplateText;

        return new()
        {
            IsBodyHtml = isBodyHtml,
            PlaceholderName = placeholderName,
            PlaceholdersNumber = placeholdersNumber,
            Subject = subject,
            SubjectPlaceholderName = subjectPlaceholderName,
            SubjectPlaceholdersNumber = subjectPlaceholdersNumber,
            TemplateText = templateText
        };
    }
    #endregion
}