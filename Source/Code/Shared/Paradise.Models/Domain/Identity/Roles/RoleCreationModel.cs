using Paradise.Common;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.Identity.Roles;

/// <summary>
/// Role creation schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RoleCreationModel"/> class.
/// </remarks>
/// <param name="name">
/// Role name.
/// </param>
/// <param name="isDefault">
/// Indicates whether the role is default and should be
/// assigned automatically when a user has been created.
/// </param>
[method: JsonConstructor]
public sealed class RoleCreationModel(string name, bool isDefault)
{
    #region Properties
    /// <summary>
    /// Role name.
    /// </summary>
    [StringLength(50, MinimumLength = 3)]
    [DataType(DataType.Text), RegularExpression(RegExContainer.OnlyAlphabetCharacters)]
    public string Name { get; } = name;

    /// <summary>
    /// Indicates whether the role is default and should be
    /// assigned automatically when a user has been created.
    /// </summary>
    public bool IsDefault { get; } = isDefault;
    #endregion
}