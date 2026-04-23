using Paradise.ApplicationLogic.Infrastructure.Communication.Implementation;
using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Communication.Email;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.System;
using Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.ApplicationLogic.Infrastructure.Communication.Email;
using System.Globalization;
using OptionsBuilder = Microsoft.Extensions.Options.Options;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.Communication.Implementation;

public sealed partial class CommunicationClientTests : IDisposable
{
    #region Constants
    /// <summary>
    /// Test email address.
    /// </summary>
    public const string TestEmail = "test@test.com";

    /// <summary>
    /// Invalid test email address.
    /// </summary>
    public const string InvalidEmailAddress = "Invalid email address.";
    #endregion

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

    /// <summary>
    /// Default template formatting arguments.
    /// </summary>
    public static IEnumerable<string> DefaultArgs { get; } = ["Test parameter"];
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Dispose()
        => Test.Dispose();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="CommunicationClientTests"/> class.
    /// </summary>
    private sealed class TestEnvironment : IDisposable
    {
        #region Fields
        private readonly FakeTimeProvider _timeProvider;

        private readonly SpyEmailSender _emailSender;

        private readonly FakeDataSource _dataSource;
        private readonly FakeEmailTemplatesRepository _emailTemplatesRepository;

        private readonly IList<EmailModel> _sentEmails = [];
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _timeProvider = new();

            _emailSender = new SpyEmailSender();
            _emailSender.MailSent += OnMailSent;

            _dataSource = new(_timeProvider);
            _emailTemplatesRepository = new(_dataSource);

            Options = new()
            {
                Credentials = new("test@test.com", "Test"),
                EnableSecureSocketsLayer = true,
                Host = "Test",
                Port = 123
            };
        }
        #endregion

        #region Properties
        /// <summary>
        /// An accessor to the <see cref="SmtpOptions"/> instance
        /// used by the test target or it's dependencies.
        /// </summary>
        public SmtpOptions Options { get; }

        /// <summary>
        /// Contains emails which were sent during test runs
        /// instead of actually sending them.
        /// </summary>
        public IEnumerable<EmailModel> SentEmails
            => _sentEmails.AsReadOnly();

        /// <summary>
        /// Gets or sets the current UTC time.
        /// </summary>
        public DateTimeOffset UtcNow
        {
            get => _timeProvider.GetUtcNow();
            set => _timeProvider.SetUtcNow(value);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationClient"/> class.
        /// </summary>
        /// <returns>
        /// System under test.
        /// </returns>
        public CommunicationClient CreateClient()
            => new(OptionsBuilder.Create(Options), _timeProvider, _emailSender, _emailTemplatesRepository);

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
        /// <param name="placeholderName">
        /// Placeholder name to be replaced with values during a message formatting.
        /// </param>
        /// <param name="placeholdersNumber">
        /// The number of placeholders to be replaced with values during the message formatting.
        /// </param>
        /// <param name="subjectPlaceholderName">
        /// Subject placeholder name to be replaced with values during a message formatting.
        /// </param>
        /// <param name="subjectPlaceholdersNumber">
        /// The number of subject placeholders to be replaced with values during the message formatting.
        /// </param>
        public void AddEmailTemplate(string templateName = "TemplateName",
                                     CultureInfo? culture = null,
                                     string templateText = "TemplateText",
                                     string subject = "Subject",
                                     string? placeholderName = null,
                                     ushort placeholdersNumber = 0,
                                     string? subjectPlaceholderName = null,
                                     ushort subjectPlaceholdersNumber = 0)
        {
            var template = new EmailTemplate(templateName, culture, templateText, subject)
            {
                PlaceholderName = placeholderName,
                PlaceholdersNumber = placeholdersNumber,
                SubjectPlaceholderName = subjectPlaceholderName,
                SubjectPlaceholdersNumber = subjectPlaceholdersNumber
            };

            _dataSource.Add(template);
            _dataSource.SaveChanges();
        }
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public void Dispose()
            => _emailSender.MailSent -= OnMailSent;
        #endregion

        #region Private methods
        /// <summary>
        /// <see cref="SpyEmailSender.MailSent"/> event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="MailMessageSentEventArgs"/> instance containing the event data.
        /// </param>
        private void OnMailSent(object? sender, MailMessageSentEventArgs e)
            => _sentEmails.Add(e.EmailModel);
        #endregion
    }
    #endregion
}