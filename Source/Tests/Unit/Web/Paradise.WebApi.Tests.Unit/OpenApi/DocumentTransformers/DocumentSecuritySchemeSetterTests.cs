using Microsoft.OpenApi;
using Paradise.WebApi.OpenApi.DocumentTransformers;

namespace Paradise.WebApi.Tests.Unit.OpenApi.DocumentTransformers;

/// <summary>
/// <see cref="DocumentSecuritySchemeSetter"/> test class.
/// </summary>
public sealed partial class DocumentSecuritySchemeSetterTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="TransformAsync_ThrowsOnInvalidSchemeName"/> method.
    /// </summary>
    public static TheoryData<string?> TransformAsync_ThrowsOnInvalidSchemeName_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="DocumentSecuritySchemeSetter.TransformAsync"/> method should
    /// add the specified security scheme to the target <see cref="OpenApiDocument"/>.
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
        var schemes = document.Components?.SecuritySchemes;

        Assert.NotNull(schemes);
        Assert.True(schemes.TryGetValue(Test.SecurityScheme.Scheme!, out var actualScheme));
        Assert.Equivalent(Test.SecurityScheme, actualScheme);
    }

    /// <summary>
    /// The <see cref="DocumentSecuritySchemeSetter.TransformAsync"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="OpenApiDocument"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task TransformAsync_ThrowsOnNull()
    {
        // Arrange
        var transformer = Test.CreateTransformer();

        var document = null as OpenApiDocument;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(()
            => transformer.TransformAsync(document!, null!, Token));
    }

    /// <summary>
    /// The <see cref="DocumentSecuritySchemeSetter.TransformAsync"/> method should
    /// throw the <see cref="ArgumentException"/> if the input
    /// scheme name is invalid.
    /// </summary>
    /// <param name="schemeName">
    /// Scheme name.
    /// </param>
    [Theory, MemberData(nameof(TransformAsync_ThrowsOnInvalidSchemeName_MemberData))]
    public async Task TransformAsync_ThrowsOnInvalidSchemeName(string? schemeName)
    {
        // Arrange
        Test.SecurityScheme.Scheme = schemeName;

        var transformer = Test.CreateTransformer();

        var document = new OpenApiDocument();

        var expectedException = schemeName is null
            ? typeof(ArgumentNullException)
            : typeof(ArgumentException);

        // Act & Assert
        await Assert.ThrowsAsync(expectedException, ()
            => transformer.TransformAsync(document, null!, Token));
    }
    #endregion
}