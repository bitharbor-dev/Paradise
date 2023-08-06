using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using static Paradise.Common.RegExContainer;

namespace Paradise.Models.Domain.RoleModels;

/// <summary>
/// Represents an application role.
/// </summary>
public sealed class RoleModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleModel"/> class.
    /// </summary>
    /// <param name="name">
    /// Role name.
    /// </param>
    [JsonConstructor]
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructors not working with constructor attributes.")]
    public RoleModel(string name)
        => Name = name;
    #endregion

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
    public string Name { get; set; }

    /// <summary>
    /// Indicates whether the role is default and should be
    /// assigned automatically when a user has been created.
    /// </summary>
    public bool IsDefault { get; set; }
    #endregion
}