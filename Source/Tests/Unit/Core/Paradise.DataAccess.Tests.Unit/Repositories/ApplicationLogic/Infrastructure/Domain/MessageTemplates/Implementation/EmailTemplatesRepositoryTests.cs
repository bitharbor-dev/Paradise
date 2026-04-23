using Microsoft.Extensions.Time.Testing;
using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates.Base;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates.Implementation;
using Paradise.DataAccess.Tests.Unit.Repositories.Base.Implementation;
using System.Globalization;

namespace Paradise.DataAccess.Tests.Unit.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates.Implementation;

/// <summary>
/// <see cref="EmailTemplatesRepository"/> test class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailTemplatesRepositoryTests"/> class.
/// </remarks>
public sealed class EmailTemplatesRepositoryTests()
    : RepositoryTests<EmailTemplatesRepository, EmailTemplate>(new FakeTimeProvider())
{
    #region Public methods
    /// <summary>
    /// The <see cref="EmailTemplatesRepository.GetByNameAndCultureAsync"/> method should
    /// return an <see cref="EmailTemplate"/>
    /// with the specified <see cref="MessageTemplate.TemplateName"/>
    /// and <see cref="MessageTemplate.Culture"/>.
    /// </summary>
    [Fact]
    public async Task GetByNameAndCultureAsync()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        await Source.SaveChangesAsync(Token);

        // Act
        var result = await Repository.GetByNameAndCultureAsync(entity.TemplateName, entity.Culture, Token);

        // Assert
        Assert.Equal(entity, result, Comparer);
    }

    /// <summary>
    /// The <see cref="EmailTemplatesRepository.GetByNameAndCultureAsync"/> method should
    /// return <see langword="null"/> if the
    /// repository contains no entities
    /// with the specified <see cref="MessageTemplate.TemplateName"/>
    /// and <see cref="MessageTemplate.Culture"/>.
    /// </summary>
    [Fact]
    public async Task GetByNameAndCultureAsync_ReturnsNullOnNonExistingTemplateNameAndCulture()
    {
        // Arrange
        var entity = GetTestEntity();

        Source.Add(entity);
        await Source.SaveChangesAsync(Token);

        // Act
        var result = await Repository.GetByNameAndCultureAsync(string.Empty, null, Token);

        // Assert
        Assert.Null(result);
    }
    #endregion

    #region Protected methods
    /// <inheritdoc/>
    protected override EmailTemplate GetTestEntity(Guid? id = null, DateTimeOffset? created = null, DateTimeOffset? modified = null)
    {
        var templateName = "templateName";
        var culture = CultureInfo.GetCultureInfo(1);
        var templateText = "templateText";
        var subject = "subject";

        var template = new EmailTemplate(templateName, culture, templateText, subject);

        var targetType = typeof(EmailTemplate);

        if (id.HasValue)
        {
            var idProperty = targetType.GetProperty(nameof(EmailTemplate.Id));
            idProperty!.SetValue(template, id.Value);
        }

        if (created.HasValue)
        {
            var createdProperty = targetType.GetProperty(nameof(EmailTemplate.Created));
            createdProperty!.SetValue(template, created.Value);
        }

        if (modified.HasValue)
        {
            var modifiedProperty = targetType.GetProperty(nameof(EmailTemplate.Modified));
            modifiedProperty!.SetValue(template, modified.Value);
        }

        return template;
    }
    #endregion
}