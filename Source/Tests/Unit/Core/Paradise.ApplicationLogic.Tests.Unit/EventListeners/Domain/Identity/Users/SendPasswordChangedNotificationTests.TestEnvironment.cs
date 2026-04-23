using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Paradise.ApplicationLogic.EventListeners.Domain.Identity.Users;
using Paradise.ApplicationLogic.Infrastructure.Communication;
using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Services;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.ApplicationLogic.Infrastructure.Communication;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess.Repositories;
using Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.ApplicationLogic.Infrastructure.Communication.Email;
using System.Globalization;
using OptionsBuilder = Microsoft.Extensions.Options.Options;

namespace Paradise.ApplicationLogic.Tests.Unit.EventListeners.Domain.Identity.Users;

public sealed partial class SendPasswordChangedNotificationTests
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
    /// Provides setup and behavior check methods for the <see cref="SendPasswordChangedNotificationTests"/> class.
    /// </summary>
    private sealed class TestEnvironment : IDisposable
    {
        #region Fields
        private readonly IList<EmailModel> _sentEmails = [];

        private readonly EmailTemplateOptions _emailTemplateOptions;

        private readonly FakeTimeProvider _timeProvider;
        private readonly SpyEmailSender _emailSender;
        private readonly FakeInfrastructureUnitOfWork _unitOfWork;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _emailTemplateOptions = new()
            {
                PasswordChangedNotificationTemplateName = "PasswordChangedNotification"
            };

            _timeProvider = new();
            _emailSender = new();
            _emailSender.MailSent += OnMailSent;

            _unitOfWork = new(new FakeDataSource(_timeProvider));

            CreateEmailTemplate();

            var services = new ServiceCollection()
                .AddScoped(_ => OptionsBuilder.Create(_emailTemplateOptions))
                .AddScoped<ICommunicationClient>(_ =>
                {
                    return new FakeCommunicationClient(_timeProvider, _emailSender,
                                                       _unitOfWork.EmailTemplatesRepository);
                });

            Target = new(services.BuildServiceProvider());
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public SendPasswordChangedNotification Target { get; }

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

        /// <summary>
        /// Creates an email template used within the <see cref="SendPasswordChangedNotification"/> class.
        /// </summary>
        private void CreateEmailTemplate()
        {
            var templateName = _emailTemplateOptions.PasswordChangedNotificationTemplateName;
            var culture = null as CultureInfo;
            var templateText = "Test";
            var subject = "Test";

            var emailTemplate = new EmailTemplate(templateName, culture, templateText, subject);

            _unitOfWork.EmailTemplatesRepository.Add(emailTemplate);
            _unitOfWork.CommitAsync().Wait();
        }
        #endregion
    }
    #endregion
}