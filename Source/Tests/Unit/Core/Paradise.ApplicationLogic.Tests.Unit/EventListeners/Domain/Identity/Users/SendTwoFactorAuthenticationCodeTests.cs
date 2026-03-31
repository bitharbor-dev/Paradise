using Paradise.ApplicationLogic.EventListeners.Domain.Identity.Users;
using Paradise.Domain.Events.Identity.Users;
using System.Globalization;

namespace Paradise.ApplicationLogic.Tests.Unit.EventListeners.Domain.Identity.Users;

/// <summary>
/// <see cref="SendTwoFactorAuthenticationCode"/> test class.
/// </summary>
public sealed partial class SendTwoFactorAuthenticationCodeTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="SendPasswordChangeLink.ProcessAsync"/> method should
    /// send an email containing the two-factor authentication code.
    /// </summary>
    [Fact]
    public async Task ProcessAsync()
    {
        // Arrange
        var emailAddress = "test@email.com";

        var verificationCode = "123456";

        var domainEvent = new TwoFactorAuthenticationOccurringEvent(
            occurredOn: Test.UtcNow,
            emailAddress: emailAddress,
            verificationCode: verificationCode,
            userCulture: CultureInfo.GetCultureInfo("en-US"));

        // Act
        await Test.Target.ProcessAsync(domainEvent, Token);

        // Assert
        var email = Assert.Single(Test.SentEmails);

        Assert.Contains(emailAddress, email.To);
        Assert.Contains(verificationCode, email.Body, StringComparison.Ordinal);
    }
    #endregion
}