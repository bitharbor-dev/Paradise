using Paradise.ApplicationLogic.Domain.MessageTemplates;
using Paradise.DataAccess.Seed.Models.Application.MessageTemplates;
using Paradise.Models.Application.EmailTemplateModels;
using System.Globalization;

namespace Paradise.ApplicationLogic.DataConverters.Application;

/// <summary>
/// Contains extension methods for <see cref="EmailTemplate"/> conversion operations.
/// </summary>
internal static class EmailTemplateDataConverters
{
    #region Public methods
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
        => new(template.TemplateName, template.TemplateText, template.Subject)
        {
            Created = template.Created,
            CultureId = template.Culture?.LCID,
            Id = template.Id,
            IsBodyHtml = template.IsBodyHtml,
            Modified = template.Modified,
            PlaceholderName = template.PlaceholderName,
            PlaceholdersNumber = template.PlaceholdersNumber,
            SubjectPlaceholderName = template.SubjectPlaceholderName,
            SubjectPlaceholdersNumber = template.SubjectPlaceholdersNumber
        };

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
    public static EmailTemplate ToEntity(this EmailTemplateCreationModel creationModel) =>
        new(creationModel.TemplateName, creationModel.TemplateText, creationModel.Subject)
        {
            Culture = creationModel.CultureId.HasValue ? CultureInfo.GetCultureInfo(creationModel.CultureId.Value) : null,
            IsBodyHtml = creationModel.IsBodyHtml,
            PlaceholderName = creationModel.PlaceholderName,
            PlaceholdersNumber = creationModel.PlaceholdersNumber,
            SubjectPlaceholderName = creationModel.SubjectPlaceholderName,
            SubjectPlaceholdersNumber = creationModel.SubjectPlaceholdersNumber
        };

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
        => new(model.TemplateName, model.Subject, model.TemplateText)
        {
            CultureId = model.CultureId,
            IsBodyHtml = model.IsBodyHtml,
            PlaceholderName = model.PlaceholderName,
            PlaceholdersNumber = model.PlaceholdersNumber,
            SubjectPlaceholderName = model.SubjectPlaceholderName,
            SubjectPlaceholdersNumber = model.SubjectPlaceholdersNumber
        };

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
    public static EmailTemplateUpdateModel ToUpdateModel(this SeedEmailTemplateModel model) => new()
    {
        IsBodyHtml = model.IsBodyHtml,
        PlaceholderName = model.PlaceholderName,
        PlaceholdersNumber = model.PlaceholdersNumber,
        Subject = model.Subject,
        SubjectPlaceholderName = model.SubjectPlaceholderName,
        SubjectPlaceholdersNumber = model.SubjectPlaceholdersNumber,
        TemplateText = model.TemplateText
    };
    #endregion
}