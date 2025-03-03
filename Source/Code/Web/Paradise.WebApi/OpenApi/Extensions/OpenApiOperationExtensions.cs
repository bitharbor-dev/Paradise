using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Paradise.WebApi.OpenApi.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="OpenApiOperation"/> <see langword="class"/>.
/// </summary>
internal static class OpenApiOperationExtensions
{
    #region Public methods
    /// <summary>
    /// Adds the given <paramref name="scheme"/> to the input <paramref name="operation"/>
    /// using the data from <paramref name="operationDescriptor"/>.
    /// </summary>
    /// <param name="operation">
    /// The <see cref="OpenApiOperation"/> to which to add the given <paramref name="scheme"/>.
    /// </param>
    /// <param name="operationDescriptor">
    /// The <see cref="ActionDescriptor"/> instance containing the <paramref name="operation"/>
    /// information.
    /// </param>
    /// <param name="scheme">
    /// The <see cref="OpenApiSecurityScheme"/> to be added.
    /// </param>
    public static void AddSecurityScheme(this OpenApiOperation operation, ActionDescriptor operationDescriptor, OpenApiSecurityScheme scheme)
    {
        MethodInfo? methodInfo = null;

        if (operationDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            methodInfo = controllerActionDescriptor.MethodInfo;

        methodInfo ??= operationDescriptor.EndpointMetadata
            .OfType<MethodInfo>()
            .FirstOrDefault();

        if (methodInfo is null)
            return;

        var actionAttributes = methodInfo.GetCustomAttributes(true);
        var controllerAttributes = methodInfo.DeclaringType?.GetCustomAttributes(true) ?? [];

        var attributes = actionAttributes.Union(controllerAttributes);

        var authRequired = !attributes
            .OfType<AllowAnonymousAttribute>()
            .Any();

        if (authRequired)
        {
            var authRequirement = new OpenApiSecurityRequirement
            {
                [scheme] = []
            };

            operation.Security.Add(authRequirement);
        }
    }
    #endregion
}