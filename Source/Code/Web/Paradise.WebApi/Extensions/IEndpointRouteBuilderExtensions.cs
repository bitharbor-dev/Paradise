using Paradise.Models;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.Models.WebApi.Services.Authentication;
using Paradise.Primitives;
using Paradise.Primitives.Web;
using Paradise.WebApi.Endpoints.Handlers.Infrastructure;
using Paradise.WebApi.Endpoints.Handlers.Infrastructure.Domain.Identity;
using Paradise.WebApi.Endpoints.Handlers.Infrastructure.Domain.MessageTemplates;
using static Paradise.Primitives.Web.AuthenticationSchemeNames;

namespace Paradise.WebApi.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IEndpointRouteBuilder"/> <see langword="interface"/>.
/// </summary>
internal static class IEndpointRouteBuilderExtensions
{
    #region Public methods
    /// <summary>
    /// Maps the application endpoints.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IEndpointRouteBuilder"/>.
    /// </param>
    public static void MapEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapAuthentication();
        builder.MapEmailTemplates();
        builder.MapRoles();
        builder.MapUsers();
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Maps the authentication endpoints.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IEndpointRouteBuilder"/>.
    /// </param>
    private static void MapAuthentication(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(AuthenticationRoutes.Prefix);

        group.MapPost(AuthenticationRoutes.Login, AuthenticationHandlers.LoginAsync)
            .Produces<AccessTokenModel>(OperationStatus.Success)
            .Produces<AccessTokenModel>(OperationStatus.Received)
            .ProducesProblem(OperationStatus.InvalidInput)
            .ProducesProblem(OperationStatus.Unauthorized)
            .AllowAnonymous();

        group.MapPut(AuthenticationRoutes.ConfirmLogin, AuthenticationHandlers.ConfirmLoginAsync)
            .Produces<AccessTokenModel>(OperationStatus.Success)
            .ProducesProblem(OperationStatus.InvalidInput)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Missing)
            .ProducesProblem(OperationStatus.Blocked)
            .AllowAnonymous();

        group.MapGet(AuthenticationRoutes.RenewToken, AuthenticationHandlers.RenewTokenAsync)
            .Produces<AccessTokenModel>(OperationStatus.Success)
            .ProducesProblem(OperationStatus.InvalidInput)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Missing)
            .RequireAuthorization(policy =>
            {
                policy.AddAuthenticationSchemes(LifetimelessScheme);
                policy.RequireAuthenticatedUser();
            });

        group.MapDelete(AuthenticationRoutes.Logout, AuthenticationHandlers.LogoutAsync)
            .Produces(OperationStatus.Success)
            .ProducesProblem(OperationStatus.InvalidInput)
            .ProducesProblem(OperationStatus.Unauthorized);

        group.MapDelete(AuthenticationRoutes.TerminateSessions, AuthenticationHandlers.TerminateSessionsAsync)
            .Produces(OperationStatus.Success)
            .ProducesProblem(OperationStatus.InvalidInput)
            .ProducesProblem(OperationStatus.Unauthorized);

        group.WithTags(AuthenticationRoutes.Prefix)
            .RequireAuthorization(policy =>
            {
                policy.AddAuthenticationSchemes(DefaultScheme);
                policy.RequireAuthenticatedUser();
            });
    }

    /// <summary>
    /// Maps the email template endpoints.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IEndpointRouteBuilder"/>.
    /// </param>
    private static void MapEmailTemplates(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(EmailTemplateRoutes.Prefix);

        group.MapGet(EmailTemplateRoutes.GetAll, EmailTemplateHandlers.GetAllAsync)
            .Produces<IEnumerable<EmailTemplateModel>>(OperationStatus.Success)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Prohibited);

        group.MapGet(EmailTemplateRoutes.GetById, EmailTemplateHandlers.GetByIdAsync)
            .Produces<EmailTemplateModel>(OperationStatus.Success)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Prohibited)
            .ProducesProblem(OperationStatus.Missing);

        group.MapPost(EmailTemplateRoutes.Create, EmailTemplateHandlers.CreateAsync)
            .Produces<EmailTemplateModel>(OperationStatus.Success)
            .ProducesProblem(OperationStatus.InvalidInput)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Prohibited)
            .ProducesProblem(OperationStatus.Blocked);

        group.MapPatch(EmailTemplateRoutes.Update, EmailTemplateHandlers.UpdateAsync)
            .Produces<EmailTemplateModel>(OperationStatus.Success)
            .Produces<EmailTemplateModel>(OperationStatus.Received)
            .ProducesProblem(OperationStatus.InvalidInput)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Prohibited)
            .ProducesProblem(OperationStatus.Missing);

        group.MapDelete(EmailTemplateRoutes.Delete, EmailTemplateHandlers.DeleteAsync)
            .Produces(OperationStatus.Success)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Prohibited);

        group.WithTags(EmailTemplateRoutes.Prefix)
            .RequireAuthorization(policy =>
            {
                policy.AddAuthenticationSchemes(DefaultScheme);
                policy.RequireAuthenticatedUser();
                policy.RequireRole(RoleNames.Administrator);
            });
    }

    /// <summary>
    /// Maps the role endpoints.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IEndpointRouteBuilder"/>.
    /// </param>
    private static void MapRoles(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(RoleRoutes.Prefix);

        group.MapGet(RoleRoutes.GetAll, RoleHandlers.GetAllAsync)
            .Produces<IEnumerable<RoleModel>>(OperationStatus.Success)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Prohibited);

        group.MapGet(RoleRoutes.GetById, RoleHandlers.GetByIdAsync)
            .Produces<RoleModel>(OperationStatus.Success)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Prohibited)
            .ProducesProblem(OperationStatus.Missing);

        group.MapGet(RoleRoutes.GetUserRoles, RoleHandlers.GetUserRolesAsync)
            .Produces<IEnumerable<RoleModel>>(OperationStatus.Success)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Prohibited)
            .ProducesProblem(OperationStatus.Missing);

        group.MapPost(RoleRoutes.Create, RoleHandlers.CreateAsync)
            .Produces<RoleModel>(OperationStatus.Success)
            .ProducesProblem(OperationStatus.InvalidInput)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Prohibited)
            .ProducesProblem(OperationStatus.Blocked);

        group.MapPatch(RoleRoutes.Update, RoleHandlers.UpdateAsync)
            .Produces<RoleModel>(OperationStatus.Success)
            .ProducesProblem(OperationStatus.InvalidInput)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Prohibited)
            .ProducesProblem(OperationStatus.Missing);

        group.MapDelete(RoleRoutes.Delete, RoleHandlers.DeleteAsync)
            .Produces<IEnumerable<RoleModel>>(OperationStatus.Success)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Prohibited)
            .ProducesProblem(OperationStatus.Missing);

        group.MapPatch(RoleRoutes.Assign, RoleHandlers.AssignAsync)
            .Produces<IEnumerable<RoleModel>>(OperationStatus.Success)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Prohibited)
            .ProducesProblem(OperationStatus.Missing);

        group.MapDelete(RoleRoutes.Unassign, RoleHandlers.UnassignAsync)
            .Produces<IEnumerable<RoleModel>>(OperationStatus.Success)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Prohibited)
            .ProducesProblem(OperationStatus.Missing);

        group.WithTags(RoleRoutes.Prefix)
            .RequireAuthorization(policy =>
            {
                policy.AddAuthenticationSchemes(DefaultScheme);
                policy.RequireAuthenticatedUser();
                policy.RequireRole(RoleNames.Administrator);
            });
    }

    /// <summary>
    /// Maps the user endpoints.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IEndpointRouteBuilder"/>.
    /// </param>
    private static void MapUsers(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(UserRoutes.Prefix);

        group.MapGet(UserRoutes.GetAll, UserHandlers.GetAllAsync)
            .Produces<IEnumerable<UserModel>>(OperationStatus.Success)
            .ProducesProblem(OperationStatus.Unauthorized);

        group.MapGet(UserRoutes.GetById, UserHandlers.GetByIdAsync)
            .Produces<UserModel>(OperationStatus.Success)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Missing);

        group.MapPost(UserRoutes.Register, UserHandlers.RegisterAsync)
            .Produces<UserModel>(OperationStatus.Created)
            .ProducesProblem(OperationStatus.InvalidInput)
            .ProducesProblem(OperationStatus.Blocked)
            .AllowAnonymous();

        group.MapGet(UserRoutes.ConfirmEmailAddress, UserHandlers.ConfirmEmailAddressAsync)
            .Produces<UserModel>(OperationStatus.Success)
            .ProducesProblem(OperationStatus.InvalidInput)
            .ProducesProblem(OperationStatus.Missing)
            .AllowAnonymous();

        group.MapPost(UserRoutes.CreatePasswordResetRequest, UserHandlers.CreatePasswordResetRequestAsync)
            .Produces(OperationStatus.Success)
            .ProducesProblem(OperationStatus.InvalidInput)
            .ProducesProblem(OperationStatus.Missing)
            .AllowAnonymous();

        group.MapPatch(UserRoutes.ResetPassword, UserHandlers.ResetPasswordAsync)
            .Produces(OperationStatus.Success)
            .ProducesProblem(OperationStatus.InvalidInput)
            .ProducesProblem(OperationStatus.Missing)
            .ProducesProblem(OperationStatus.Blocked)
            .AllowAnonymous();

        group.MapPost(UserRoutes.CreateEmailAddressResetRequest, UserHandlers.CreateEmailAddressResetRequestAsync)
            .Produces(OperationStatus.Success)
            .ProducesProblem(OperationStatus.InvalidInput)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Missing)
            .ProducesProblem(OperationStatus.Blocked);

        group.MapPatch(UserRoutes.ResetEmailAddress, UserHandlers.ResetEmailAddressAsync)
            .Produces(OperationStatus.Success)
            .ProducesProblem(OperationStatus.InvalidInput)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Missing)
            .ProducesProblem(OperationStatus.Blocked)
            .AllowAnonymous();

        group.MapPatch(UserRoutes.Update, UserHandlers.UpdateAsync)
            .Produces<UserModel>(OperationStatus.Success)
            .ProducesProblem(OperationStatus.InvalidInput)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Missing)
            .ProducesProblem(OperationStatus.Blocked);

        group.MapDelete(UserRoutes.Delete, UserHandlers.DeleteAsync)
            .Produces(OperationStatus.Success)
            .ProducesProblem(OperationStatus.Unauthorized)
            .ProducesProblem(OperationStatus.Missing)
            .ProducesProblem(OperationStatus.Blocked);

        group.WithTags(UserRoutes.Prefix)
            .RequireAuthorization(policy =>
            {
                policy.AddAuthenticationSchemes(DefaultScheme);
                policy.RequireAuthenticatedUser();
            });
    }
    #endregion
}