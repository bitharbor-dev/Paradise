using Microsoft.OpenApi;
using Paradise.WebApi.OpenApi.DocumentTransformers;

namespace Paradise.WebApi.Tests.Unit.OpenApi.DocumentTransformers;

/// <summary>
/// <see cref="DocumentInfoSetter"/> test class.
/// </summary>
public sealed partial class DocumentInfoSetterTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="DocumentInfoSetter.TransformAsync"/> method should
    /// add the specified API information to the target <see cref="OpenApiDocument"/>.
    /// </summary>
    [Fact]
    public async Task TransformAsync()
    {
        // Arrange
        var transformer = Test.CreateTransformer();

        var document = new OpenApiDocument();

        // Act
        await transformer.TransformAsync(document, null!, Token);

        // Assert
        Assert.Equivalent(Test.Info, document.Info);
    }

    /// <summary>
    /// The <see cref="DocumentInfoSetter.TransformAsync"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="OpenApiDocument"/> is equal to null.
    /// </summary>
    [Fact]
    public async Task TransformAsync_ThrowsOnNullDocument()
    {
        // Arrange
        var transformer = Test.CreateTransformer();

        var document = null as OpenApiDocument;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(()
            => transformer.TransformAsync(document!, null!, Token));
    }
    #endregion
}