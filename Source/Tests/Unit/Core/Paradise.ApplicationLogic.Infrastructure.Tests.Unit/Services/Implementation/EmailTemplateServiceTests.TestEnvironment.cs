using Microsoft.Extensions.Time.Testing;
using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Infrastructure.Services.Implementation;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess.Repositories;
using System.Globalization;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.Services.Implementation;

public sealed partial class EmailTemplateServiceTests
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();

    /// <summary>
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </summary>
    public CancellationToken Token { get; } = TestContext.Current.CancellationToken;
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="EmailTemplateServiceTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Fields
        private readonly FakeDataSource _dataSource;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _dataSource = new(new FakeTimeProvider());

            Target = new(new FakeInfrastructureUnitOfWork(_dataSource));
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        internal EmailTemplateService Target { get; }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates an <see cref="EmailTemplate"/> instance
        /// and saves its data into the test persistence storage.
        /// </summary>
        /// <param name="templateName">
        /// Template name.
        /// </param>
        /// <param name="culture">
        /// Template culture.
        /// </param>
        /// <param name="templateText">
        /// Template text.
        /// </param>
        /// <param name="subject">
        /// Email subject.
        /// </param>
        /// <returns>
        /// The newly created and saved <see cref="EmailTemplate"/> instance.
        /// </returns>
        public EmailTemplate AddEmailTemplate(string templateName = "TemplateName",
                                              CultureInfo? culture = null,
                                              string templateText = "TemplateText",
                                              string subject = "Subject")
        {
            var template = new EmailTemplate(templateName, culture, templateText, subject);

            _dataSource.Add(template);
            _dataSource.SaveChanges();

            return template;
        }

        /// <summary>
        /// Checks if the <see cref="EmailTemplate"/> with the given
        /// <paramref name="id"/> value
        /// exists in the persistence storage.
        /// </summary>
        /// <param name="id">
        /// Unique identifier.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <see cref="EmailTemplate"/> with the given
        /// <paramref name="id"/> value
        /// exists in the persistence storage,
        /// otherwise - <see langword="false"/>.
        /// </returns>
        public bool EmailTemplateExists(Guid id)
        {
            return _dataSource
                .GetQueryable<EmailTemplate>()
                .Any(template => template.Id == id);
        }
        #endregion
    }
    #endregion
}