using Paradise.ApplicationLogic.Infrastructure.Communication.Email.Implementation;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;
using Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.ApplicationLogic.Infrastructure.Communication.Email;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.Communication.Email.Implementation;

public sealed partial class DefaultEmailSenderTests : IDisposable
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
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Dispose()
        => Test.Dispose();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="DefaultEmailSenderTests"/> class.
    /// </summary>
    private sealed class TestEnvironment : IDisposable
    {
        #region Fields
        private readonly SpySmtpClient _smtpClient;
        private readonly IList<EmailModel> _sentEmails = [];
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _smtpClient = new SpySmtpClient();
            _smtpClient.MailSent += OnMailSent;

            Target = new(_smtpClient);
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public DefaultEmailSender Target { get; }

        /// <summary>
        /// Contains emails which were sent during test runs
        /// instead of actually sending them.
        /// </summary>
        public IEnumerable<EmailModel> SentEmails
            => _sentEmails.AsReadOnly();
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public void Dispose()
        {
            _smtpClient.MailSent -= OnMailSent;
            _smtpClient.Dispose();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// <see cref="SpySmtpClient.MailSent"/> event handler.
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