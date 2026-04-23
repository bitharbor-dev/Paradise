using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Time.Testing;
using Microsoft.IdentityModel.Tokens;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Implementation;
using Paradise.WebApi.Infrastructure.Options;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using OptionsBuilder = Microsoft.Extensions.Options.Options;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Authentication.JwtBearer.Implementation;

public sealed partial class JwtManagerTests
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="JwtManagerTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Fields
        private readonly FakeTimeProvider _timeProvider;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            // 'JwtSecurityTokenHandler' internally normalizes DateTimeOffset.MinValue
            // to DateTimeOffset.UnixEpoch, when creating JWTs. To comply this behavior
            // the initial 'UtcNow' value must be also set to DateTimeOffset.UnixEpoch.
            _timeProvider = new(DateTimeOffset.UnixEpoch);

            JwtHandler = new();

            AuthOptions = new()
            {
                AccessTokenLifetime = TimeSpan.FromMinutes(5)
            };

            JwtOptions = new()
            {
                TokenValidationParameters = new()
                {
                    AuthenticationType = "Bearer",
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Ug5xCXJaNaxfx78KdQxQZDAmniAbZw6V")),
                    LifetimeValidator = ValidateLifetime,
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidAudience = nameof(JwtManagerTests),
                    ValidIssuer = nameof(JwtManagerTests)
                }
            };

            Target = new(OptionsBuilder.Create(AuthOptions), OptionsBuilder.Create(JwtOptions), _timeProvider);
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public JwtManager Target { get; }

        /// <summary>
        /// JSON web token handler.
        /// </summary>
        public JwtSecurityTokenHandler JwtHandler { get; }

        /// <summary>
        /// An accessor to the <see cref="AuthenticationOptions"/> instance
        /// used by the test target or it's dependencies.
        /// </summary>
        public AuthenticationOptions AuthOptions { get; }

        /// <summary>
        /// An accessor to the <see cref="JwtBearerOptions"/> instance
        /// used by the test target or it's dependencies.
        /// </summary>
        public JwtBearerOptions JwtOptions { get; }

        /// <summary>
        /// Gets or sets the current UTC time.
        /// </summary>
        public DateTimeOffset UtcNow
        {
            get => _timeProvider.GetUtcNow();
            set => _timeProvider.SetUtcNow(value);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates a valid JSON web token based on the current <see cref="JwtOptions"/>,
        /// <see cref="UtcNow"/> and given <paramref name="claims"/>.
        /// </summary>
        /// <param name="claims">
        /// The list of claims representing the token subject.
        /// </param>
        /// <returns>
        /// A valid JWT.
        /// </returns>
        public string CreateJwt(IEnumerable<Claim> claims)
        {
            var creationTime = UtcNow;
            var expiryDate = creationTime.Add(AuthOptions.AccessTokenLifetime);

            var tokenParameters = JwtOptions.TokenValidationParameters;
            var algorithm = tokenParameters.ValidAlgorithms.Single();

            return JwtHandler.WriteToken(JwtHandler.CreateJwtSecurityToken(new()
            {
                Audience = tokenParameters.ValidAudience,
                Subject = new(claims),
                IssuedAt = creationTime.UtcDateTime,
                NotBefore = creationTime.UtcDateTime,
                Expires = expiryDate.UtcDateTime,
                Issuer = tokenParameters.ValidIssuer,
                SigningCredentials = new(tokenParameters.IssuerSigningKey, algorithm)
            }));
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
        /// This exists to ensure deterministic, time-controlled unit tests.
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

            var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

            if (notBefore.HasValue && (notBefore.Value > utcNow.Add(validationParameters.ClockSkew)))
                return false;

            if (expires.HasValue && (expires.Value < utcNow.Add(validationParameters.ClockSkew.Negate())))
                return false;

            return true;
        }
        #endregion
    }
    #endregion
}