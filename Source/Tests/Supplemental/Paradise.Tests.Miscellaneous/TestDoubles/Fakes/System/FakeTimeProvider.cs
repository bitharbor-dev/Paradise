namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.System;

/// <summary>
/// Fake <see cref="TimeProvider"/> implementation.
/// </summary>
public sealed class FakeTimeProvider : TimeProvider
{
    #region Fields
    private DateTimeOffset _utcNow = DateTimeOffset.UnixEpoch;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeTimeProvider"/> class.
    /// </summary>
    public FakeTimeProvider() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeTimeProvider"/> class.
    /// </summary>
    /// <param name="initialUtcNow">
    /// Initial return value for the <see cref="GetUtcNow"/> method.
    /// </param>
    public FakeTimeProvider(DateTimeOffset initialUtcNow)
        => _utcNow = initialUtcNow;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public sealed override DateTimeOffset GetUtcNow()
        => _utcNow;

    /// <summary>
    /// Sets the return value for the <see cref="GetUtcNow"/> method.
    /// </summary>
    /// <param name="utcNow">
    /// The <see cref="DateTimeOffset"/> value to set.
    /// </param>
    public void SetUtcNow(DateTimeOffset utcNow)
        => _utcNow = utcNow;
    #endregion
}