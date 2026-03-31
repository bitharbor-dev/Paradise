using Paradise.Common;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.Identity.Roles;

/// <summary>
/// Represents an application role.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RoleModel"/> class.
/// </remarks>
/// <param name="id">
/// Role Id.
/// </param>
/// <param name="created">
/// Creation date.
/// </param>
/// <param name="modified">
/// Last changed date.
/// </param>
/// <param name="name">
/// Role name.
/// </param>
/// <param name="isDefault">
/// Indicates whether the role is default and should be
/// assigned automatically when a user has been created.
/// </param>
[method: JsonConstructor]
public sealed class RoleModel(Guid id, DateTimeOffset created, DateTimeOffset modified, string name, bool isDefault)
{
    #region Properties
    /// <summary>
    /// Role Id.
    /// </summary>
    public Guid Id { get; } = id;

    /// <summary>
    /// Creation date.
    /// </summary>
    [DataType(DataType.DateTime)]
    public DateTimeOffset Created { get; } = created;

    /// <summary>
    /// Last changed date.
    /// </summary>
    [DataType(DataType.DateTime)]
    public DateTimeOffset Modified { get; } = modified;

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