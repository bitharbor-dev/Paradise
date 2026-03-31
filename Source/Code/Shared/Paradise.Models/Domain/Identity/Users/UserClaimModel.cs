using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.Identity.Users;

/// <summary>
/// Represents a user's claim.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserClaimModel"/> class.
/// </remarks>
/// <param name="type">
/// Type.
/// </param>
/// <param name="value">
/// Value.
/// </param>
[method: JsonConstructor]
public sealed class UserClaimModel(string type, string value)
{
    #region Properties
    /// <summary>
    /// Type.
    /// </summary>
    public string Type { get; } = type;

    /// <summary>
    /// Value.
    /// </summary>
    public string Value { get; } = value;
    #endregion
}