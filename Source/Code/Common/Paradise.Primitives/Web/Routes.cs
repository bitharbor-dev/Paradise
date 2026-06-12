using static Paradise.Primitives.Web.ParameterNames;

namespace Paradise.Primitives.Web;

/// <summary>
/// Authentication API endpoints routs.
/// </summary>
public static class AuthenticationRoutes
{
    #region Constants
    /// <summary>
    /// "Authentication" route prefix.
    /// </summary>
    public const string Prefix = $"Authentication/";

    /// <summary>
    /// Logging in.
    /// </summary>
    public const string Login = "";

    /// <summary>
    /// Confirming user's login.
    /// </summary>
    public const string ConfirmLogin = "";

    /// <summary>
    /// Renewing authorization token.
    /// </summary>
    public const string RenewToken = "";

    /// <summary>
    /// Logging out.
    /// </summary>
    public const string Logout = "";

    /// <summary>
    /// Terminating all sessions.
    /// </summary>
    public const string TerminateSessions = "All";
    #endregion
}

/// <summary>
/// Email templates API endpoints routes.
/// </summary>
public static class EmailTemplateRoutes
{
    #region Constants
    /// <summary>
    /// "Email templates" route prefix.
    /// </summary>
    public const string Prefix = $"EmailTemplates/";

    /// <summary>
    /// Getting email templates list.
    /// </summary>
    public const string GetAll = "";

    /// <summary>
    /// Getting an email template by Id.
    /// </summary>
    public const string GetById = $"/{{{IdParameter}}}";

    /// <summary>
    /// Creating a new email template.
    /// </summary>
    public const string Create = "";

    /// <summary>
    /// Updating an email template.
    /// </summary>
    public const string Update = $"/{{{IdParameter}}}";

    /// <summary>
    /// Deleting an email template.
    /// </summary>
    public const string Delete = $"/{{{IdParameter}}}";
    #endregion
}

/// <summary>
/// Roles API endpoints routes.
/// </summary>
public static class RoleRoutes
{
    #region Constants
    /// <summary>
    /// "User" route prefix.
    /// </summary>
    private const string UserPrefix = $"User/";

    /// <summary>
    /// "Roles" route prefix.
    /// </summary>
    public const string Prefix = $"Roles/";

    /// <summary>
    /// Getting roles list.
    /// </summary>
    public const string GetAll = "";

    /// <summary>
    /// Getting a role by Id.
    /// </summary>
    public const string GetById = $"/{{{IdParameter}}}";

    /// <summary>
    /// Getting user roles.
    /// </summary>
    public const string GetUserRoles = $"/{UserPrefix}{{{UserIdParameter}}}";

    /// <summary>
    /// Creating a new role.
    /// </summary>
    public const string Create = "";

    /// <summary>
    /// Updating a role.
    /// </summary>
    public const string Update = $"/{{{IdParameter}}}";

    /// <summary>
    /// Deleting a role.
    /// </summary>
    public const string Delete = $"/{{{IdParameter}}}";

    /// <summary>
    /// Assigning role to a user.
    /// </summary>
    public const string Assign = $"/{{{RoleIdParameter}}}/{{{UserIdParameter}}}";

    /// <summary>
    /// Unassigning role from a user.
    /// </summary>
    public const string Unassign = $"/{{{RoleIdParameter}}}/{{{UserIdParameter}}}";
    #endregion
}

/// <summary>
/// Users API endpoints routes.
/// </summary>
public static class UserRoutes
{
    #region Constants
    /// <summary>
    /// "Users" route prefix.
    /// </summary>
    public const string Prefix = $"Users/";

    /// <summary>
    /// Getting users list.
    /// </summary>
    public const string GetAll = "";

    /// <summary>
    /// Getting a user by Id.
    /// </summary>
    public const string GetById = $"/{{{IdParameter}}}";

    /// <summary>
    /// Creating a new user.
    /// </summary>
    public const string Register = "";

    /// <summary>
    /// Confirming user's email address.
    /// </summary>
    public const string ConfirmEmailAddress = $"/Email/{{{IdentityTokenParameter}}}";

    /// <summary>
    /// Requesting password reset.
    /// </summary>
    public const string CreatePasswordResetRequest = "/Password";

    /// <summary>
    /// Resetting password.
    /// </summary>
    public const string ResetPassword = "/Password";

    /// <summary>
    /// Requesting email address reset.
    /// </summary>
    public const string CreateEmailAddressResetRequest = "/Email";

    /// <summary>
    /// Resetting email address.
    /// </summary>
    public const string ResetEmailAddress = $"/Email/{{{IdentityTokenParameter}}}";

    /// <summary>
    /// Updating a user.
    /// </summary>
    public const string Update = "";

    /// <summary>
    /// Deleting a user.
    /// </summary>
    public const string Delete = "";
    #endregion
}

/// <summary>
/// Static API endpoints routes.
/// </summary>
public static class StaticRoutes
{
    #region Constants
    /// <summary>
    /// Confirming user's email address.
    /// </summary>
    public const string ConfirmEmailAddress = $"confirm-email/{{{IdentityTokenParameter}}}";

    /// <summary>
    /// Resetting password.
    /// </summary>
    public const string ResetPassword = $"reset-password/{{{IdentityTokenParameter}}}";

    /// <summary>
    /// Resetting email address.
    /// </summary>
    public const string ResetEmailAddress = $"reset-email/{{{IdentityTokenParameter}}}";
    #endregion
}