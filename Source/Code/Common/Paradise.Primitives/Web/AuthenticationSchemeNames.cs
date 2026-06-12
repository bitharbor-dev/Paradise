namespace Paradise.Primitives.Web;

/// <summary>
/// Contains the authentication scheme names used across the application.
/// </summary>
public static class AuthenticationSchemeNames
{
    #region Constants
    /// <summary>
    /// The default authentication scheme name.
    /// </summary>
    public const string DefaultScheme = "Bearer";

    /// <summary>
    /// The name of the authentication scheme, which ignores the token lifetime value.
    /// </summary>
    public const string LifetimelessScheme = "DisableTokenLifetimeValidation";
    #endregion
}