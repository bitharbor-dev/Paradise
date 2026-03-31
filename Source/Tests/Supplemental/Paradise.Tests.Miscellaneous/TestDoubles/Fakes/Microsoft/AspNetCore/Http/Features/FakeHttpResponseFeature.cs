using Microsoft.AspNetCore.Http.Features;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.AspNetCore.Http.Features;

/// <summary>
/// Fake <see cref="HttpResponseFeature"/> implementation.
/// </summary>
public sealed class FakeHttpResponseFeature : HttpResponseFeature
{
    #region Fields
    private bool _hasStarted;
    #endregion

    #region Properties
    /// <inheritdoc/>
    public override bool HasStarted
        => _hasStarted;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override void OnStarting(Func<object, Task> callback, object state)
    {
        _hasStarted = true;
        base.OnStarting(callback, state);
    }
    #endregion
}