using Paradise.ApplicationLogic.EventListeners.Domain.Identity.Users;
using Paradise.Domain.Events.Identity.Users;
using Paradise.Models;
using System.Globalization;

namespace Paradise.ApplicationLogic.Tests.Unit.EventListeners.Domain.Identity.Users;

/// <summary>
/// <see cref="SendPasswordChangeLink"/> test class.
/// </summary>
public sealed partial class SendPasswordChangeLinkTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="SendPasswordChangeLink.ProcessAsync"/> method should
    /// send an email containing the link to change the password.
    /// </summary>
    [Fact]
    public async Task ProcessAsync()
    {
        // Arrange
        var emailAddress = "test@email.com";

        var changePasswordToken = "changePasswordToken";
        var timeout = Test.ApplicationOptions.Timeout.ResetPasswordTimeout;
        var expiryDate = Test.UtcNow.Add(timeout);

        var identityTokenModel = new IdentityToken(emailAddress, changePasswordToken, expiryDate: expiryDate);
        var identityToken = Test.Protector.Protect(identityTokenModel);

        var domainEvent = new PasswordResetRequestedEvent(
            occurredOn: Test.UtcNow,
            emailAddress: emailAddress,
            changePasswordToken: changePasswordToken,
            userCulture: CultureInfo.GetCultureInfo("en-US"));

        // Act
        await Test.Target.ProcessAsync(domainEvent, Token);

        // Assert
        var email = Assert.Single(Test.SentEmails);

        Assert.Contains(emailAddress, email.To);
        Assert.Contains(identityToken, email.Body, StringComparison.Ordinal);
    }
    #endregion
}