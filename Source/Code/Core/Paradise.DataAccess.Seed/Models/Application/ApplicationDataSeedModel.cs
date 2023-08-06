using Paradise.DataAccess.Seed.Models.Application.MessageTemplates;

namespace Paradise.DataAccess.Seed.Models.Application;

/// <summary>
/// Application data seeding schema.
/// </summary>
public sealed class ApplicationDataSeedModel
{
    #region Properties
    /// <summary>
    /// Seed email templates.
    /// </summary>
    public IEnumerable<SeedEmailTemplateModel>? EmailTemplates { get; set; }
    #endregion
}