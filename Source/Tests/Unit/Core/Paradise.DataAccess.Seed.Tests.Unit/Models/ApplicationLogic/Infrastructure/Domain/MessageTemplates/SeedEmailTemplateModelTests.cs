using Paradise.DataAccess.Seed.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.Localization.ExceptionHandling;

namespace Paradise.DataAccess.Seed.Tests.Unit.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;

/// <summary>
/// <see cref="SeedEmailTemplateModel"/> test class.
/// </summary>
public sealed partial class SeedEmailTemplateModelTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="SeedEmailTemplateModel"/> constructor should
    /// successfully instantiate the <see cref="SeedEmailTemplateModel"/> object
    /// and read the <see cref="SeedEmailTemplateModel.TemplateText"/> value
    /// from the file by the specified path.
    /// </summary>
    [Fact]
    public void Constructor_ReadsTextSource()
    {
        // Arrange
        var content = "<html>Test</html>";
        var path = Test.WriteTemporaryFile(content);

        // Act
        var model = new SeedEmailTemplateModel(string.Empty, null, string.Empty, false, null, 0, null, 0, null, path);

        // Assert
        Assert.Equal(content, model.TemplateText);
    }

    /// <summary>
    /// The <see cref="SeedEmailTemplateModel"/> constructor should
    /// throw the <see cref="ArgumentException"/> if the
    /// template text or template text source path values were not provided.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsOnTemplateTextOrSourcePathNull()
    {
        // Arrange
        var message = ExceptionMessages.GetMessageMessageTemplateTemplateTextOrSourcePathIsRequired();

        // Act
        var exception = Assert.Throws<ArgumentException>(()
            => new SeedEmailTemplateModel(string.Empty, null, string.Empty, false, null, 0, null, 0, null, null));

        // Assert
        Assert.Equal(message, exception.Message);
    }
    #endregion
}