using Paradise.ApplicationLogic.EventListeners.Domain.Identity.Users;
using Paradise.Domain.Events.Identity.Users;
using System.Globalization;

namespace Paradise.ApplicationLogic.Tests.Unit.EventListeners.Domain.Identity.Users;

/// <summary>
/// <see cref="SendPasswordChangedNotification"/> test class.
/// </summary>
public sealed partial class SendPasswordChangedNotificationTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="SendPasswordChangedNotification.ProcessAsync"/> method should
    /// send a notification email, informing that the user's password has been changed.
    /// </summary>
    [Fact]
    public async Task ProcessAsync()
    {
        // Arrange
        var emailAddress = "test@email.com";

        var domainEvent = new PasswordResetCompletedEvent(
            occurredOn: Test.UtcNow,
            emailAddress: emailAddress,
            userCulture: CultureInfo.GetCultureInfo("en-US"));

        // Act
        await Test.Target.ProcessAsync(domainEvent, Token);

        // Assert
        var email = Assert.Single(Test.SentEmails);

        Assert.Contains(emailAddress, email.To);
    }
    #endregion
}