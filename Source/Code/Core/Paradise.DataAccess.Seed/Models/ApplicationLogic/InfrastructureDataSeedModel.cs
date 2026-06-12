using Paradise.DataAccess.Seed.Models.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.DataAccess.Seed.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using System.Text.Json.Serialization;

namespace Paradise.DataAccess.Seed.Models.ApplicationLogic;

/// <summary>
/// Infrastructure data seeding schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="InfrastructureDataSeedModel"/> class.
/// </remarks>
/// <param name="emailTemplates">
/// Seed email templates.
/// </param>
/// <param name="roles">
/// Seed roles.
/// </param>
/// <param name="users">
/// Seed users.
/// </param>
[method: JsonConstructor]
public sealed class InfrastructureDataSeedModel(IEnumerable<SeedEmailTemplateModel> emailTemplates,
                                                IEnumerable<SeedRoleModel> roles,
                                                IEnumerable<SeedUserModel> users)
{
    #region Properties
    /// <summary>
    /// Seed email templates.
    /// </summary>
    public IEnumerable<SeedEmailTemplateModel> EmailTemplates { get; } = emailTemplates;

    /// <summary>
    /// Seed roles.
    /// </summary>
    public IEnumerable<SeedRoleModel> Roles { get; } = roles;

    /// <summary>
    /// Seed users.
    /// </summary>
    public IEnumerable<SeedUserModel> Users { get; } = users;
    #endregion
}