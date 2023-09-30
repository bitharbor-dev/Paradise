using Paradise.Common;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using static Paradise.Common.RegExContainer;

namespace Paradise.Models.Domain.RoleModels;

/// <summary>
/// Role creation schema.
/// </summary>
public sealed class RoleCreationModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleCreationModel"/> class.
    /// </summary>
    /// <param name="name">
    /// Role name.
    /// </param>
    [JsonConstructor]
    [SuppressMessage(SuppressionOfIDE0290.Category, SuppressionOfIDE0290.CheckId, Justification = SuppressionOfIDE0290.Justification)]
    public RoleCreationModel(string name)
        => Name = name;
    #endregion

    #region Properties
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