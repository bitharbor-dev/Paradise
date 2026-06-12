using Microsoft.Extensions.Logging;

namespace Paradise.Localization.Logging;

/// <summary>
/// Represents a logging event identifier together with the
/// <see cref="LogLevel"/> derived from its conventional numbering.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LoggedEvent"/> structure.
/// </remarks>
/// <param name="Id">
/// The underlying event identifier.
/// </param>
/// <param name="Level">
/// Event log level.
/// </param>
public readonly record struct LoggedEvent(EventId Id, LogLevel Level);