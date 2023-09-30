using Paradise.Common;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Paradise.DataAccess.Seed.Models.Domain.Roles;

/// <summary>
/// Role seed data schema.
/// </summary>
public sealed class SeedRoleModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="SeedRoleModel"/> class.
    /// </summary>
    /// <param name="name">
    /// Role name.
    /// </param>
    [JsonConstructor]
    [SuppressMessage(SuppressionOfIDE0290.Category, SuppressionOfIDE0290.CheckId, Justification = SuppressionOfIDE0290.Justification)]
    public SeedRoleModel(string name)
        => Name = name;
    #endregion

    #region Properties
    /// <summary>
    /// Role name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Indicates whether the role is default and should be
    /// assigned automatically when a user has been created.
    /// </summary>
    public bool IsDefault { get; set; }
    #endregion
}