using static Paradise.Common.Web.ParameterNames;

namespace Paradise.Common.Web;

/// <summary>
/// Web API endpoints routes base class.
/// </summary>
internal static class Routes
{
    #region Constants
    /// <summary>
    /// Default Uri separator.
    /// </summary>
    public const string Slash = "/";

    /// <summary>
    /// Base API endpoint route (without domain).
    /// </summary>
    public const string Base = Slash;
    #endregion
}

/// <summary>
/// Authentication API endpoints routs.
/// </summary>
public static class AuthenticationRoutes
{
    #region Constants
    /// <summary>
    /// "Auth" route prefix.
    /// </summary>
    private const string AuthPrefix = $"Authentication{Routes.Slash}";

    /// <summary>
    /// Controller route.
    /// </summary>
    private const string Controller = $"{Routes.Base}{AuthPrefix}";

    /// <summary>
    /// Logging in.
    /// </summary>
    public const string Login = Controller;

    /// <summary>
    /// Confirming user's login.
    /// </summary>
    public const string ConfirmLogin = Controller;

    /// <summary>
    /// Renewing authorization token.
    /// </summary>
    public const string RenewToken = Controller;

    /// <summary>
    /// Logging out.
    /// </summary>
    public const string Logout = Controller;

    /// <summary>
    /// Terminating all sessions.
    /// </summary>
    public const string TerminateSessions = $"{Controller}All";
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
    private const string EmailTemplatesPrefix = $"EmailTemplates{Routes.Slash}";

    /// <summary>
    /// Controller route.
    /// </summary>
    private const string Controller = $"{Routes.Base}{EmailTemplatesPrefix}";

    /// <summary>
    /// Getting email templates list.
    /// </summary>
    public const string GetAll = Controller;

    /// <summary>
    /// Getting an email template by Id.
    /// </summary>
    public const string GetById = $"{Controller}{{{IdParameter}}}";

    /// <summary>
    /// Creating a new email template.
    /// </summary>
    public const string Create = Controller;

    /// <summary>
    /// Updating an email template.
    /// </summary>
    public const string Update = $"{Controller}{{{IdParameter}}}";

    /// <summary>
    /// Deleting an email template.
    /// </summary>
    public const string Delete = $"{Controller}{{{IdParameter}}}";
    #endregion
}

/// <summary>
/// Roles API endpoints routes.
/// </summary>
public static class RoleRoutes
{
    #region Constants
    /// <summary>
    /// "Roles" route prefix.
    /// </summary>
    private const string RolesPrefix = $"Roles{Routes.Slash}";

    /// <summary>
    /// "User" route prefix.
    /// </summary>
    private const string UserPrefix = $"User{Routes.Slash}";

    /// <summary>
    /// Controller route.
    /// </summary>
    private const string Controller = $"{Routes.Base}{RolesPrefix}";

    /// <summary>
    /// Getting roles list.
    /// </summary>
    public const string GetAll = Controller;

    /// <summary>
    /// Getting a role by Id.
    /// </summary>
    public const string GetById = $"{Controller}{{{IdParameter}}}";

    /// <summary>
    /// Getting user roles.
    /// </summary>
    public const string GetUserRoles = $"{Controller}{UserPrefix}{{{UserIdParameter}}}";

    /// <summary>
    /// Creating a new role.
    /// </summary>
    public const string Create = Controller;

    /// <summary>
    /// Updating a role.
    /// </summary>
    public const string Update = $"{Controller}{{{IdParameter}}}";

    /// <summary>
    /// Deleting a role.
    /// </summary>
    public const string Delete = $"{Controller}{{{IdParameter}}}";

    /// <summary>
    /// Assigning role to a user.
    /// </summary>
    public const string Assign = $"{Controller}{{{RoleIdParameter}}}{Routes.Slash}{{{UserIdParameter}}}";

    /// <summary>
    /// Unassigning role from a user.
    /// </summary>
    public const string Unassign = $"{Controller}{{{RoleIdParameter}}}{Routes.Slash}{{{UserIdParameter}}}";
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
    private const string UsersPrefix = $"Users{Routes.Slash}";

    /// <summary>
    /// Controller route.
    /// </summary>
    private const string Controller = $"{Routes.Base}{UsersPrefix}";

    /// <summary>
    /// Getting users list.
    /// </summary>
    public const string GetAll = Controller;

    /// <summary>
    /// Getting a user by Id.
    /// </summary>
    public const string GetById = $"{Controller}{{{IdParameter}}}";

    /// <summary>
    /// Creating a new user.
    /// </summary>
    public const string Register = Controller;

    /// <summary>
    /// Confirming user's email address.
    /// </summary>
    public const string ConfirmEmailAddress = $"{Controller}Email{Routes.Slash}{{{IdentityTokenParameter}}}";

    /// <summary>
    /// Requesting password reset.
    /// </summary>
    public const string CreatePasswordResetRequest = $"{Controller}Password";

    /// <summary>
    /// Resetting password.
    /// </summary>
    public const string ResetPassword = $"{Controller}Password";

    /// <summary>
    /// Requesting email address reset.
    /// </summary>
    public const string CreateEmailAddressResetRequest = $"{Controller}Email";

    /// <summary>
    /// Resetting email address.
    /// </summary>
    public const string ResetEmailAddress = $"{Controller}Email{Routes.Slash}{{{IdentityTokenParameter}}}";

    /// <summary>
    /// Updating a user.
    /// </summary>
    public const string Update = Controller;

    /// <summary>
    /// Deleting a user.
    /// </summary>
    public const string Delete = Controller;
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
    public const string ConfirmEmailAddress = $"confirm-email{Routes.Slash}{{{IdentityTokenParameter}}}";

    /// <summary>
    /// Resetting password.
    /// </summary>
    public const string ResetPassword = $"reset-password{Routes.Slash}{{{IdentityTokenParameter}}}";

    /// <summary>
    /// Resetting email address.
    /// </summary>
    public const string ResetEmailAddress = $"reset-email{Routes.Slash}{{{IdentityTokenParameter}}}";
    #endregion
}