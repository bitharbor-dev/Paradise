using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;

namespace Paradise.Domain.Base.Events.Implementation;

/// <summary>
/// Default in-memory implementation of <see cref="IDomainEventSink"/> and <see cref="IDomainEventSource"/>
/// using <see cref="Channel{T}"/> of <see cref="IDomainEvent"/>.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "No better naming is available.")]
internal sealed class InMemoryDomainEventQueue : IDomainEventSink, IDomainEventSource
{
    #region Fields
    private readonly Channel<IDomainEvent> _channel = Channel.CreateUnbounded<IDomainEvent>();
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public ValueTask PushAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        => _channel.Writer.WriteAsync(domainEvent, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IDomainEvent> PullAsync(CancellationToken cancellationToken = default)
        => _channel.Reader.ReadAllAsync(cancellationToken);
    #endregion
}