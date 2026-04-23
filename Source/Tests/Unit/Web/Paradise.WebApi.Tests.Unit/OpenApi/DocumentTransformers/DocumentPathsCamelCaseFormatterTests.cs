using Microsoft.OpenApi;
using Paradise.WebApi.OpenApi.DocumentTransformers;

namespace Paradise.WebApi.Tests.Unit.OpenApi.DocumentTransformers;

/// <summary>
/// <see cref="DocumentPathsCamelCaseFormatter"/> test class.
/// </summary>
public sealed partial class DocumentPathsCamelCaseFormatterTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="TransformAsync"/> method.
    /// </summary>
    public static TheoryData<string, string> TransformAsync_MemberData { get; } = new()
    {
        { "/FirstSegment", "/firstSegment" },
        { "/FirstSegment/SecondSegment", "/firstSegment/secondSegment" },
        { "/firstSegment/secondSegment", "/firstSegment/secondSegment" },
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="DocumentPathsCamelCaseFormatter.TransformAsync"/> method should
    /// format all paths in the target <see cref="OpenApiDocument"/> to the camel case.
    /// </summary>
    /// <param name="path">
    /// Initial path value.
    /// </param>
    /// <param name="expectedPath">
    /// Expected post-formatting path value.
    /// </param>
    [Theory, MemberData(nameof(TransformAsync_MemberData))]
    public async Task TransformAsync(string path, string expectedPath)
    {
        // Arrange
        var transformer = new DocumentPathsCamelCaseFormatter();

        var document = new OpenApiDocument
        {
            Paths = new()
            {
                [path] = new OpenApiPathItem()
                {
                    Operations = new() { [HttpMethod.Get] = new() }
                }
            }
        };

        // Act
        await transformer.TransformAsync(document, null!, TestContext.Current.CancellationToken);

        // Assert
        var formattedPath = Assert.Single(document.Paths);
        Assert.Equal(formattedPath.Key, expectedPath);
    }

    /// <summary>
    /// The <see cref="DocumentPathsCamelCaseFormatter.TransformAsync"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="OpenApiDocument"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task FormatPathsToCamelCase_ThrowsOnNull()
    {
        // Arrange
        var transformer = new DocumentPathsCamelCaseFormatter();

        var document = null as OpenApiDocument;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(()
            => transformer.TransformAsync(document!, null!, TestContext.Current.CancellationToken));
    }
    #endregion
}