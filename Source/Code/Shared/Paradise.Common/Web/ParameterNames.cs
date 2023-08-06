namespace Paradise.Common.Web;

/// <summary>
/// Contains parameters' names used in Web API controllers.
/// </summary>
public static class ParameterNames
{
    #region Constants
    /// <summary>
    /// Email template Id parameter name.
    /// </summary>
    public const string EmailTemplateIdParameter = "emailTemplateId";

    /// <summary>
    /// User Id parameter name.
    /// </summary>
    public const string UserIdParameter = "userId";

    /// <summary>
    /// Role Id parameter name.
    /// </summary>
    public const string RoleIdParameter = "roleId";

    /// <summary>
    /// Role "Is default" parameter name.
    /// </summary>
    public const string IsDefaultParameter = "isDefault";

    /// <summary>
    /// Optional user Id parameter name.
    /// </summary>
    public const string OptionalUserIdParameter = $"{UserIdParameter}?"; // TODO: Check if needed.

    /// <summary>
    /// Identity token parameter name.
    /// </summary>
    public const string IdentityTokenParameter = "identityToken";

    /// <summary>
    /// Access token parameter name.
    /// </summary>
    public const string AccessTokenParameter = "accessToken";
    #endregion
}