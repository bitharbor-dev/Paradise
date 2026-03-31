using Azure.Communication.Email;
using Paradise.ApplicationLogic.Infrastructure.Communication.Email.Implementation;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Communication.Email;
using Paradise.Tests.Miscellaneous.TestDoubles.Spies.Azure.Communication.Email;
using OptionsBuilder = Microsoft.Extensions.Options.Options;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.Communication.Email.Implementation;

public sealed partial class AzureEmailSenderTests : IDisposable
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
    /// Provides setup and behavior check methods for the <see cref="AzureEmailSenderTests"/> class.
    /// </summary>
    private sealed class TestEnvironment : IDisposable
    {
        #region Fields
        private readonly SpyEmailClient _emailClient;
        private readonly IList<EmailMessage> _sentEmails = [];
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            Options = new SmtpOptions
            {
                Credentials = new(TestEmail, "Test"),
                EnableSsl = false,
                Host = "SMTP",
                Port = 587
            };

            _emailClient = new SpyEmailClient();
            _emailClient.EmailSent += OnEmailSent;

            Target = new(OptionsBuilder.Create(Options), _emailClient);
        }
        #endregion

        #region Properties
        /// <summary>
        /// An accessor to the <see cref="SmtpOptions"/> instance
        /// used by the test target or it's dependencies.
        /// </summary>
        public SmtpOptions Options { get; }

        /// <summary>
        /// System under test.
        /// </summary>
        public AzureEmailSender Target { get; }

        /// <summary>
        /// Contains emails which were sent during test runs
        /// instead of actually sending them.
        /// </summary>
        public IEnumerable<EmailMessage> SentEmails
            => _sentEmails.AsReadOnly();
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public void Dispose()
            => _emailClient.EmailSent -= OnEmailSent;
        #endregion

        #region Private methods
        /// <summary>
        /// <see cref="SpyEmailClient.EmailSent"/> event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="EmailMessageSentEventArgs"/> instance containing the event data.
        /// </param>
        private void OnEmailSent(object? sender, EmailMessageSentEventArgs e)
            => _sentEmails.Add(e.EmailMessage);
        #endregion
    }
    #endregion
}