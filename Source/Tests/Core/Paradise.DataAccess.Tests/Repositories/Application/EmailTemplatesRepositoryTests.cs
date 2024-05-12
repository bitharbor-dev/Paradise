using Paradise.ApplicationLogic.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Domain.MessageTemplates.Base;
using Paradise.DataAccess.Repositories.Application.Implementation;
using Paradise.DataAccess.Tests.Repositories.Base;
using System.Globalization;

namespace Paradise.DataAccess.Tests.Repositories.Application;

/// <summary>
/// Test class for the <see cref="EmailTemplatesRepository"/>.
/// </summary>
public sealed class EmailTemplatesRepositoryTests : RepositoryTests<EmailTemplatesRepository, EmailTemplate>
{
    #region Public methods
    /// <summary>
    /// <see cref="EmailTemplatesRepository.GetByNameAndCultureAsync(string, CultureInfo?, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns an <see cref="EmailTemplate"/>
    /// with the specified <see cref="MessageTemplate.TemplateName"/>
    /// and <see cref="MessageTemplate.Culture"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task GetByNameAndCultureAsync()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo(1);

        var template = new EmailTemplate("Test", "Test", "Test")
        {
            Culture = culture
        };

        Source.Add(template);
        Source.SaveChanges();

        // Act
        var result = await Repository.GetByNameAndCultureAsync(template.TemplateName, template.Culture);

        // Assert
        Assert.Equal(template, result, Comparer);
    }

    /// <summary>
    /// <see cref="EmailTemplatesRepository.GetByNameAndCultureAsync(string, CultureInfo?, CancellationToken)"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="null"/> since the repository contains no entities
    /// with the specified <see cref="MessageTemplate.TemplateName"/>
    /// and <see cref="MessageTemplate.Culture"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public async Task GetByNameAndCultureAsync_ReturnsNullOnNonExistingTemplateNameAndCulture()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo(1);

        var template = new EmailTemplate("Test", "Test", "Test")
        {
            Culture = culture
        };

        Source.Add(template);
        Source.SaveChanges();

        // Act
        var result = await Repository.GetByNameAndCultureAsync(string.Empty, null);

        // Assert
        Assert.Null(result);
    }
    #endregion

    #region Protected methods
    /// <inheritdoc/>
    protected override EmailTemplate GetTestEntity(Guid? id = null, DateTime? created = null, DateTime? modified = null)
    {
        var template = new EmailTemplate("Test", "Test", "Test");

        if (id.HasValue)
            template.Id = id.Value;

        if (created.HasValue)
            template.Created = created.Value;

        if (modified.HasValue)
            template.Modified = modified.Value;

        return template;
    }
    #endregion
}