using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static Paradise.Common.RegExContainer;

namespace Paradise.Models.Domain.RoleModels;

/// <summary>
/// Role creation schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RoleCreationModel"/> class.
/// </remarks>
/// <param name="name">
/// Role name.
/// </param>
[method: JsonConstructor]
public sealed class RoleCreationModel(string name)
{
    #region Properties
    /// <summary>
    /// Role name.
    /// </summary>
    [Required, StringLength(50, MinimumLength = 3)]
    [DataType(DataType.Text), RegularExpression(OnlyAZCharacters)]
    public string Name { get; set; } = name;

    /// <summary>
    /// Indicates whether the role is default and should be
    /// assigned automatically when a user has been created.
    /// </summary>
    public bool IsDefault { get; set; }
    #endregion
}