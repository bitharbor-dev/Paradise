using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Paradise.Tests.Extensibility.Web.Hosting.Configuration.Base;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Tests.Extensibility.Web.Hosting.Configuration;

/// <summary>
/// Overrides application <see cref="TimeProvider"/> to use a controllable alternative
/// and makes the JWT lifetime validation rely on it.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TimeConfiguration"/> class.
/// </remarks>
/// <param name="timeProvider">
/// The <see cref="TimeProvider"/> to be used by the application.
/// </param>
public sealed class TimeConfiguration(TimeProvider timeProvider) : IWebApplicationServicesConfiguration
{
    #region Public methods
    /// <inheritdoc/>
    public void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        services
            .RemoveAll<TimeProvider>()
            .AddSingleton(timeProvider);

        services.PostConfigureAll<JwtBearerOptions>(options
            => options.TokenValidationParameters.LifetimeValidator = ValidateLifetime);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Validates the lifetime of a <see cref="SecurityToken"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// IMPORTANT:
    /// </para>
    /// This method intentionally mirrors the default JWT lifetime validation logic
    /// implemented by Microsoft.IdentityModel.Tokens, with the sole difference of
    /// using TimeProvider instead of DateTime.UtcNow.
    /// <para>
    /// This exists to ensure deterministic, time-controlled integration tests.
    /// Do NOT copy this logic into production code.
    /// </para>
    /// </remarks>
    /// <param name="notBefore">
    /// The 'notBefore' time found in the <see cref="SecurityToken"/>.
    /// </param>
    /// <param name="expires">
    /// The 'expiration' time found in the <see cref="SecurityToken"/>.
    /// </param>
    /// <param name="securityToken">
    /// The <see cref="SecurityToken"/> being validated.
    /// </param>
    /// <param name="validationParameters">
    /// <see cref="TokenValidationParameters"/> required for validation.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the token lifetime passes validation,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Omitted for readability.")]
    private bool ValidateLifetime(DateTime? notBefore, DateTime? expires,
                                  SecurityToken securityToken,
                                  TokenValidationParameters validationParameters)
    {
        if (!validationParameters.ValidateLifetime)
            return true;

        if (!expires.HasValue && validationParameters.RequireExpirationTime)
            return false;

        if (notBefore.HasValue && expires.HasValue && (notBefore.Value > expires.Value))
            return false;

        var utcNow = timeProvider.GetUtcNow().UtcDateTime;

        if (notBefore.HasValue && (notBefore.Value > utcNow.Add(validationParameters.ClockSkew)))
            return false;

        if (expires.HasValue && (expires.Value < utcNow.Add(validationParameters.ClockSkew.Negate())))
            return false;

        return true;
    }
    #endregion
}