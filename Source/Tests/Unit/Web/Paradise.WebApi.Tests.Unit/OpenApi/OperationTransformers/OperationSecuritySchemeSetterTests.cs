using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Paradise.WebApi.OpenApi.OperationTransformers;

namespace Paradise.WebApi.Tests.Unit.OpenApi.OperationTransformers;

/// <summary>
/// <see cref="OperationSecuritySchemeSetter"/> test class.
/// </summary>
public sealed partial class OperationSecuritySchemeSetterTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="OperationSecuritySchemeSetter.TransformAsync"/> method should
    /// add the security scheme reference to the target <see cref="OpenApiOperation"/>
    /// if the related controller or action are not decorated with
    /// <see cref="AllowAnonymousAttribute"/>.
    /// </summary>
    [Fact]
    public async Task TransformAsync()
    {
        // Arrange
        var operation = new OpenApiOperation();
        var context = Test.CreateContext(allowAnonymousOnMethod: false, allowAnonymousOnType: false);

        var sampleScheme = Test.GetConfiguredSecurityScheme();

        // Act
        await Test.Target.TransformAsync(operation, context, Token);

        // Assert
        Assert.NotNull(operation.Security);
        var requirement = Assert.Single(operation.Security);
        var reference = Assert.Single(requirement.Keys);

        Assert.Equal(sampleScheme.Scheme, reference.Reference.Id);
    }

    /// <summary>
    /// The <see cref="OperationSecuritySchemeSetter.TransformAsync"/> method should
    /// not add the security scheme reference to the target <see cref="OpenApiOperation"/>
    /// if the related controller is decorated with
    /// <see cref="AllowAnonymousAttribute"/>.
    /// </summary>
    [Fact]
    public async Task TransformAsync_SkipsOnAnonymousController()
    {
        // Arrange
        var operation = new OpenApiOperation();
        var context = Test.CreateContext(allowAnonymousOnMethod: false, allowAnonymousOnType: true);

        // Act
        await Test.Target.TransformAsync(operation, context, Token);

        // Assert
        Assert.Null(operation.Security);
    }

    /// <summary>
    /// The <see cref="OperationSecuritySchemeSetter.TransformAsync"/> method should
    /// not add the security scheme reference to the target <see cref="OpenApiOperation"/>
    /// if the related action is decorated with
    /// <see cref="AllowAnonymousAttribute"/>.
    /// </summary>
    [Fact]
    public async Task TransformAsync_SkipsOnAnonymousAction()
    {
        // Arrange
        var operation = new OpenApiOperation();
        var context = Test.CreateContext(allowAnonymousOnMethod: true, allowAnonymousOnType: false);

        // Act
        await Test.Target.TransformAsync(operation, context, Token);

        // Assert
        Assert.Null(operation.Security);
    }

    /// <summary>
    /// The <see cref="OperationSecuritySchemeSetter.TransformAsync"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="OpenApiOperation"/> is equal to null.
    /// </summary>
    [Fact]
    public async Task TransformAsync_ThrowsOnNullOperation()
    {
        // Arrange
        var operation = null as OpenApiOperation;
        var context = Test.CreateContext();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(()
            => Test.Target.TransformAsync(operation!, context, Token));
    }

    /// <summary>
    /// The <see cref="OperationSecuritySchemeSetter.TransformAsync"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="OpenApiOperationTransformerContext"/> is equal to null.
    /// </summary>
    [Fact]
    public async Task TransformAsync_ThrowsOnNullContext()
    {
        // Arrange
        var operation = new OpenApiOperation();
        var context = null as OpenApiOperationTransformerContext;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(()
            => Test.Target.TransformAsync(operation, context!, Token));
    }
    #endregion
}