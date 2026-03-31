using Paradise.ApplicationLogic.EventListeners.Domain.Identity.Users;
using Paradise.Domain.Events.Identity.Users;
using Paradise.Models;
using System.Globalization;

namespace Paradise.ApplicationLogic.Tests.Unit.EventListeners.Domain.Identity.Users;

/// <summary>
/// <see cref="SendEmailAddressConfirmationLink"/> test class.
/// </summary>
public sealed partial class SendEmailAddressConfirmationLinkTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="SendEmailAddressConfirmationLink.ProcessAsync"/> method should
    /// send an email containing the link to confirm the email address.
    /// </summary>
    [Fact]
    public async Task ProcessAsync()
    {
        // Arrange
        var emailAddress = "test@email.com";

        var emailConfirmationToken = "emailConfirmationToken";
        var timeout = Test.ApplicationOptions.Timeout.EmailConfirmationTimeout;
        var expiryDate = Test.UtcNow.Add(timeout);

        var identityTokenModel = new IdentityToken(emailAddress, emailConfirmationToken, expiryDate: expiryDate);
        var identityToken = Test.Protector.Protect(identityTokenModel);

        var domainEvent = new UserRegisteredEvent(
            occurredOn: Test.UtcNow,
            emailAddress: emailAddress,
            emailConfirmationToken: emailConfirmationToken,
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