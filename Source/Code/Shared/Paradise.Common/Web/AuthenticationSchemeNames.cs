namespace Paradise.Common.Web;

/// <summary>
/// Contains authentication scheme names used across the application.
/// </summary>
public static class AuthenticationSchemeNames
{
    #region Constants
    /// <summary>
    /// Default authentication scheme name.
    /// </summary>
    public const string Default = "Bearer";

    /// <summary>
    /// The name of the authentication scheme, which ignores token lifetime value.
    /// </summary>
    public const string DisableTokenLifetimeValidation = "DisableTokenLifetimeValidation";
    #endregion
}