using Microsoft.AspNetCore.Http;

namespace Paradise.Tests.Doubles.Fakes.Microsoft.AspNetCore.Http;

/// <summary>
/// Fake <see cref="IProblemDetailsService"/> implementation.
/// </summary>
public sealed class FakeProblemDetailsService : IProblemDetailsService
{
    #region Properties
    /// <summary>
    /// Context passed to <see cref="TryWriteAsync"/> or <see cref="WriteAsync"/>.
    /// </summary>
    public ProblemDetailsContext? DetailsContext { get; private set; }

    /// <summary>
    /// Result returned from <see cref="TryWriteAsync"/>.
    /// </summary>
    public bool Result { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public ValueTask WriteAsync(ProblemDetailsContext context)
    {
        DetailsContext = context;

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask<bool> TryWriteAsync(ProblemDetailsContext context)
    {
        DetailsContext = context;

        return ValueTask.FromResult(Result);
    }
    #endregion
}