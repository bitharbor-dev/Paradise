using System.Text.Json.Serialization;

namespace Paradise.DataAccess.Seed.Models.Domain.Roles;

/// <summary>
/// Role seed data schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SeedRoleModel"/> class.
/// </remarks>
/// <param name="name">
/// Role name.
/// </param>
[method: JsonConstructor]
public sealed class SeedRoleModel(string name)
{
    #region Properties
    /// <summary>
    /// Role name.
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Indicates whether the role is default and should be
    /// assigned automatically when a user has been created.
    /// </summary>
    public bool IsDefault { get; set; }
    #endregion
}