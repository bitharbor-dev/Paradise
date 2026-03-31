namespace Paradise.Models.Domain.Identity.Roles;

/// <summary>
/// Role update schema.
/// </summary>
public sealed class RoleUpdateModel
{
    #region Properties
    /// <summary>
    /// Indicates whether the role is default and should be
    /// assigned automatically when a user has been created.
    /// </summary>
    public bool IsDefault { get; set; }
    #endregion
}