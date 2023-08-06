using Paradise.ApplicationLogic.Communication;
using Paradise.Models.Application.CommunicationModels;
using Paradise.Options.Models.Communication;

namespace Paradise.Tests.Miscellaneous.Fakes.Core.ApplicationLogic.Communication;

/// <summary>
/// Fake <see cref="ISmtpClient"/> implementation.
/// </summary>
public sealed class FakeSmtpClient : ISmtpClient
{
    #region Properties
    /// <inheritdoc/>
    public SmtpOptions Options { get; } = new();
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task SendAsync(EmailModel model, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
    #endregion
}