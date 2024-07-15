using static Paradise.Common.Web.ParameterNames;

namespace Paradise.Common.Web;

/// <summary>
/// Web API endpoints routes container class.
/// </summary>
internal static class Routes
{
    #region Constants
    /// <summary>
    /// Default Uri separator.
    /// </summary>
    public const string Slash = "/";

    /// <summary>
    /// API version.
    /// </summary>
    public const string Version = $"v1{Slash}";

    /// <summary>
    /// Base API endpoint route (without domain).
    /// </summary>
    public const string Base = Version;
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
    public const string GetUserRoles = $"{Controller}{UserPrefix}{{{IdParameter}}}";

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
    /// "Confirm" route prefix.
    /// </summary>
    private const string ConfirmPrefix = $"Confirm{Routes.Slash}";

    /// <summary>
    /// "Auth" route prefix.
    /// </summary>
    private const string AuthPrefix = $"Auth{Routes.Slash}";

    /// <summary>
    /// "Reset" route prefix.
    /// </summary>
    private const string ResetPrefix = $"Reset{Routes.Slash}";

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
    /// Confirming user's email.
    /// </summary>
    public const string ConfirmEmail = $"{Controller}{ConfirmPrefix}Email{Routes.Slash}{{{IdentityTokenParameter}}}";

    /// <summary>
    /// Logging a user in.
    /// </summary>
    public const string Login = $"{Controller}{AuthPrefix}";

    /// <summary>
    /// Confirming user's login.
    /// </summary>
    public const string ConfirmLogin = $"{Controller}{AuthPrefix}";

    /// <summary>
    /// Renewing authorization token.
    /// </summary>
    public const string RenewToken = $"{Controller}{AuthPrefix}";

    /// <summary>
    /// Logging a user out.
    /// </summary>
    public const string Logout = $"{Controller}{AuthPrefix}";

    /// <summary>
    /// Logging a user out everywhere.
    /// </summary>
    public const string LogoutEverywhere = $"{Controller}{AuthPrefix}All";

    /// <summary>
    /// Requesting password reset.
    /// </summary>
    public const string CreatePasswordResetRequest = $"{Controller}{ResetPrefix}Password";

    /// <summary>
    /// Resetting password.
    /// </summary>
    public const string ResetPassword = $"{Controller}{ResetPrefix}Password";

    /// <summary>
    /// Requesting email reset.
    /// </summary>
    public const string CreateEmailResetRequest = $"{Controller}{ResetPrefix}Email";

    /// <summary>
    /// Resetting email.
    /// </summary>
    public const string ResetEmail = $"{Controller}{ResetPrefix}Email{Routes.Slash}{{{IdentityTokenParameter}}}";

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