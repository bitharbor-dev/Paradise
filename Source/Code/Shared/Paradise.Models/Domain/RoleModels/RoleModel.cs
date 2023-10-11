using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static Paradise.Common.RegExContainer;

namespace Paradise.Models.Domain.RoleModels;

/// <summary>
/// Represents an application role.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RoleModel"/> class.
/// </remarks>
/// <param name="name">
/// Role name.
/// </param>
[method: JsonConstructor]
public sealed class RoleModel(string name)
{
    #region Properties
    /// <summary>
    /// Role Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Creation date.
    /// </summary>
    [DataType(DataType.DateTime)]
    public DateTime Created { get; set; }

    /// <summary>
    /// Last changed date.
    /// </summary>
    [DataType(DataType.DateTime)]
    public DateTime Modified { get; set; }

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