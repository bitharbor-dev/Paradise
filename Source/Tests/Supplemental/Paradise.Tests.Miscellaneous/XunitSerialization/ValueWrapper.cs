using Xunit;

namespace Paradise.Tests.Miscellaneous.XunitSerialization;

/// <summary>
/// A wrapper structure used to add generic support of
/// <see cref="MemberDataAttribute"/> arguments serialization
/// and tests enumeration.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ValueWrapper"/> structure.
/// </remarks>
/// <param name="Value">
/// The value to serialize.
/// </param>
public readonly record struct ValueWrapper(object? Value);