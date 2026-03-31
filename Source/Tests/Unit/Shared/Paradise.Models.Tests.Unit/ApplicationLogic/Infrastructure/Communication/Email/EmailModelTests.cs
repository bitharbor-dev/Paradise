using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;

namespace Paradise.Models.Tests.Unit.ApplicationLogic.Infrastructure.Communication.Email;

/// <summary>
/// <see cref="EmailModel"/> test class.
/// </summary>
public sealed class EmailModelTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="EmailModel(string, string, string, BaseEmailModel)"/> constructor should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="BaseEmailModel"/> is equal to null.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsOnNull()
    {
        // Arrange
        var subject = "Subject";
        var body = "Body";
        var from = "From";
        var baseModel = null as BaseEmailModel;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => new EmailModel(subject, body, from, baseModel!));
    }
    #endregion
}