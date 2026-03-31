using Paradise.DataAccess.Seed.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using System.Text.Json.Serialization;

namespace Paradise.DataAccess.Seed.Models.ApplicationLogic;

/// <summary>
/// Application data seeding schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ApplicationDataSeedModel"/> class.
/// </remarks>
/// <param name="emailTemplates">
/// Seed email templates.
/// </param>
[method: JsonConstructor]
public sealed class ApplicationDataSeedModel(IEnumerable<SeedEmailTemplateModel> emailTemplates)
{
    #region Properties
    /// <summary>
    /// Seed email templates.
    /// </summary>
    public IEnumerable<SeedEmailTemplateModel> EmailTemplates { get; } = emailTemplates;
    #endregion
}