using Paradise.WebApi.Infrastructure.Authentication.JwtBearer;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Web.WebApi.Infrastructure.Authentication.JwtBearer;

/// <summary>
/// Stub <see cref="IJwtManager"/> implementation.
/// </summary>
public sealed class StubJwtManager : IJwtManager
{
    #region Properties
    /// <summary>
    /// <see cref="IssueToken"/> result.
    /// </summary>
    public Tuple<string, DateTimeOffset>? IssueTokenResult { get; set; }

    /// <summary>
    /// <see cref="TryParseToken"/> result.
    /// </summary>
    public Tuple<bool, ClaimsPrincipal?>? TryParseTokenResult { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public string IssueToken(IEnumerable<Claim> claims, Guid refreshTokenId, out DateTimeOffset expiryDate)
    {
        // Force enumeration for proper code coverage results.
        _ = claims.ToList();

        expiryDate = IssueTokenResult!.Item2;

        return IssueTokenResult.Item1;
    }

    /// <inheritdoc/>
    public bool TryParseToken(string? token, [NotNullWhen(true)] out ClaimsPrincipal? claimsPrincipal, bool checkExpiry = true)
    {
        claimsPrincipal = TryParseTokenResult!.Item2;

        return TryParseTokenResult.Item1;
    }
    #endregion
}