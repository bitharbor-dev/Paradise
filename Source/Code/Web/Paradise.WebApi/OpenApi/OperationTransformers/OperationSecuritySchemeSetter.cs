using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Paradise.Common.Extensions;

namespace Paradise.WebApi.OpenApi.OperationTransformers;

/// <summary>
/// Default <see cref="IOpenApiOperationTransformer"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OperationSecuritySchemeSetter"/> class.
/// </remarks>
/// <param name="configuration">
/// The <see cref="IConfiguration"/> containing the operation data.
/// </param>
internal sealed class OperationSecuritySchemeSetter(IConfiguration configuration) : IOpenApiOperationTransformer
{
    #region Public methods
    /// <inheritdoc/>
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(context);

        var scheme = configuration.GetRequiredInstance<OpenApiSecurityScheme>();

        var descriptor = (ControllerActionDescriptor)context.Description.ActionDescriptor;
        var schemeReference = new OpenApiSecuritySchemeReference(scheme.Scheme!, context.Document);

        AddSecurityScheme(operation, descriptor, schemeReference);

        return Task.CompletedTask;
    }
    #endregion

    #region Public methods
    /// <summary>
    /// Adds the given scheme <paramref name="reference"/> to the input <paramref name="operation"/>
    /// using the data from <paramref name="operationDescriptor"/>.
    /// </summary>
    /// <param name="operation">
    /// The <see cref="OpenApiOperation"/> to which to add the given <paramref name="reference"/>.
    /// </param>
    /// <param name="operationDescriptor">
    /// The <see cref="ControllerActionDescriptor"/> instance containing the <paramref name="operation"/>
    /// information.
    /// </param>
    /// <param name="reference">
    /// The <see cref="OpenApiSecuritySchemeReference"/> to add.
    /// </param>
    /// <param name="scopes">
    /// The list of scope names required for scheme execution.
    /// <para>
    /// If referenced security scheme is not "<c>oauth2</c>" or "<c>openIdConnect</c>" - the array MUST be empty.
    /// </para>
    /// </param>
    public static void AddSecurityScheme(OpenApiOperation operation, ControllerActionDescriptor operationDescriptor,
                                         OpenApiSecuritySchemeReference reference, params List<string> scopes)
    {
        var methodInfo = operationDescriptor.MethodInfo;

        var actionAttributes = methodInfo.GetCustomAttributes(true);
        var controllerAttributes = methodInfo.DeclaringType!.GetCustomAttributes(true);

        var attributes = actionAttributes.Union(controllerAttributes);

        var authRequired = !attributes
            .OfType<AllowAnonymousAttribute>()
            .Any();

        if (authRequired)
        {
            operation.Security ??= [];
            operation.Security.Add(new()
            {
                [reference] = scopes
            });
        }
    }
    #endregion
}