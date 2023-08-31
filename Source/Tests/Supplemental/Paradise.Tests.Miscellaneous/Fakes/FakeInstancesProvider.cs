using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Paradise.ApplicationLogic.Identity;
using Paradise.DataAccess.Repositories;
using Paradise.Domain.Roles;
using Paradise.Domain.Users;
using Paradise.Options.Models;
using Paradise.Options.Models.Communication;
using Paradise.Tests.Miscellaneous.Fakes.Core.ApplicationLogic.Communication;
using Paradise.Tests.Miscellaneous.Fakes.Core.DataAccess.Repositories.Base;
using Paradise.Tests.Miscellaneous.Fakes.Microsoft.AspNetCore.Identity;
using Paradise.Tests.Miscellaneous.Fakes.Microsoft.Extensions.Logging.Xunit;
using System.Security.Claims;
using System.Text;
using Xunit.Abstractions;
using AspNetCoreTokenOptions = Microsoft.AspNetCore.Identity.TokenOptions;
using OptionsBuilder = Microsoft.Extensions.Options.Options;

namespace Paradise.Tests.Miscellaneous.Fakes;

/// <summary>
/// Provides pre-configured fake instances.
/// </summary>
public static class FakeInstancesProvider
{
    #region Constants
    /// <summary>
    /// Uppercase alphabet.
    /// </summary>
    private const string UppercaseAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// Lowercase alphabet.
    /// </summary>
    private const string LowercaseAlphabet = "abcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// Numbers.
    /// </summary>
    private const string Numbers = "0123456789";

    /// <summary>
    /// Dot.
    /// </summary>
    private const char Dot = '.';

    /// <summary>
    /// Underscore.
    /// </summary>
    private const char Underscore = '_';
    #endregion

    #region Public methods
    /// <summary>
    /// Gets a new <see cref="IApplicationDataSource"/> instance.
    /// </summary>
    /// <returns>
    /// <see cref="IApplicationDataSource"/> instance.
    /// </returns>
    public static IApplicationDataSource GetApplicationDataSource()
        => new FakeDataSource();

    /// <summary>
    /// Gets a new <see cref="IDomainDataSource"/> instance.
    /// </summary>
    /// <returns>
    /// <see cref="IDomainDataSource"/> instance.
    /// </returns>
    public static IDomainDataSource GetDomainDataSource()
        => new FakeDataSource();

    /// <summary>
    /// Gets the pre-configured <see cref="SmtpOptions"/> instance.
    /// </summary>
    /// <returns>
    /// <see cref="SmtpOptions"/> instance.
    /// </returns>
    public static IOptions<SmtpOptions> GetSmtpOptions()
    {
        var instance = new SmtpOptions
        {
            Credentials = new("test@test.com", "Test"),
            EnableSsl = true,
            Host = "Test",
            Port = 123,
            Timeout = 0
        };

        return OptionsBuilder.Create(instance);
    }

    /// <summary>
    /// Gets a new <see cref="FakeSmtpClient"/> instance.
    /// </summary>
    /// <returns>
    /// <see cref="FakeSmtpClient"/> instance.
    /// </returns>
    public static FakeSmtpClient GetSmtpClient()
        => new();

    /// <summary>
    /// Gets the pre-configured <see cref="ApplicationOptions"/> instance.
    /// </summary>
    /// <param name="secret">
    /// Value to be used during data encryption.
    /// </param>
    /// <returns>
    /// <see cref="ApplicationOptions"/> instance.
    /// </returns>
    public static IOptions<ApplicationOptions> GetApplicationOptions(string secret)
    {
        var instance = new ApplicationOptions

        {
            ApiUrl = new Uri("https://localhost:5001"),
            Authentication = new()
            {
                JsonWebTokenLifetime = TimeSpan.FromSeconds(0.5),
                RefreshTokenLifetime = TimeSpan.FromSeconds(0.5),
                TwoFactorTokenLifetime = TimeSpan.FromSeconds(0.5),
                TwoFactorVerificationCodeLength = 6
            },
            Secret = secret,
            Tokens = new()
            {
                EmailConfirmationTokenLifetime = TimeSpan.FromSeconds(0.5),
                ResetEmailAddressTokenLifetime = TimeSpan.FromSeconds(0.5),
                ResetPasswordTokenLifetime = TimeSpan.FromSeconds(0.5),
                UserDeletionRequestLifetime = TimeSpan.FromSeconds(0.5)
            }
        };

        return OptionsBuilder.Create(instance);
    }

    /// <summary>
    /// Gets the pre-configured <see cref="IdentityOptions"/> instance.
    /// </summary>
    /// <returns>
    /// <see cref="IdentityOptions"/> instance.
    /// </returns>
    public static IOptions<IdentityOptions> GetIdentityOptions()
    {
        var instance = new IdentityOptions
        {
            ClaimsIdentity = new()
            {
                EmailClaimType = ClaimTypes.Email,
                RoleClaimType = ClaimTypes.Role,
                UserIdClaimType = ClaimTypes.NameIdentifier,
                UserNameClaimType = ClaimTypes.Name
            },
            Lockout = new()
            {
                AllowedForNewUsers = true,
                MaxFailedAccessAttempts = 5,
                DefaultLockoutTimeSpan = TimeSpan.FromSeconds(0.5)
            },
            Password = new()
            {
                RequireDigit = true,
                RequiredLength = 8,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequiredUniqueChars = 5,
                RequireUppercase = true
            },
            SignIn = new()
            {
                RequireConfirmedAccount = true,
                RequireConfirmedEmail = true,
                RequireConfirmedPhoneNumber = false
            },
            Stores = new()
            {
                MaxLengthForKeys = -1,
                ProtectPersonalData = false
            },
            User = new()
            {
                AllowedUserNameCharacters = UppercaseAlphabet + LowercaseAlphabet + Numbers + Dot + Underscore,
                RequireUniqueEmail = true
            }
        };

        return OptionsBuilder.Create(instance);
    }

    /// <summary>
    /// Gets the pre-configured <see cref="JwtBearerOptions"/> instance.
    /// </summary>
    /// <param name="secret">
    /// Value to be used during data encryption.
    /// </param>
    /// <returns>
    /// <see cref="JwtBearerOptions"/> instance.
    /// </returns>
    public static IOptions<JwtBearerOptions> GetJwtBearerOptions(string secret)
    {
        var instance = new JwtBearerOptions
        {
            TokenValidationParameters = new()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidAudience = "ValidAudience",
                ValidIssuer = "ValidIssuer",
                NameClaimType = ClaimTypes.Name
            }
        };

        return OptionsBuilder.Create(instance);
    }

    /// <summary>
    /// Gets the pre-configured <see cref="EmailTemplateOptions"/> instance.
    /// </summary>
    /// <returns>
    /// <see cref="EmailTemplateOptions"/> instance.
    /// </returns>
    public static IOptions<EmailTemplateOptions> GetEmailTemplateOptions()
    {
        var instance = new EmailTemplateOptions
        {
            EmailAddressConfirmationTemplateName = "EmailConfirmationTemplate",
            EmailAddressResetCompletedTemplateName = "EmailAddressResetCompletedTemplate",
            EmailAddressResetNotificationTemplateName = "EmailResetNotificationTemplate",
            EmailAddressResetTemplateName = "EmailAddressResetTemplate",
            PasswordResetCompletedTemplateName = "PasswordResetCompletedTemplate",
            PasswordResetTemplateName = "PasswordResetTemplate",
            TwoFactorVerificationTemplateName = "TwoFactorVerificationTemplate"
        };

        return OptionsBuilder.Create(instance);
    }

    /// <summary>
    /// Gets a new <see cref="UserManager"/> instance.
    /// </summary>
    /// <param name="output">
    /// Xunit output helper.
    /// </param>
    /// <param name="domainDataSource">
    /// Domain data source.
    /// </param>
    /// <param name="identityOptions">
    /// The accessor used to access the <see cref="IdentityOptions"/>.
    /// </param>
    /// <returns>
    /// <see cref="UserManager"/> instance.
    /// </returns>
    public static UserManager GetUserManager(ITestOutputHelper output, IDomainDataSource? domainDataSource = null, IOptions<IdentityOptions>? identityOptions = null)
    {
        domainDataSource ??= GetDomainDataSource();
        identityOptions ??= GetIdentityOptions();

        var store = new FakeUserStore(domainDataSource);
        var hasher = new PasswordHasher<User>();
        var errorDescriber = new IdentityErrorDescriber();
        var userValidators = new[] { new UserValidator<User>(errorDescriber) };
        var passwordValidators = new[] { new PasswordValidator<User>(errorDescriber) };
        var lookupNormalizer = new UpperInvariantLookupNormalizer();

        var logger = GetLogger<UserManager>(output);

        var userManager = new UserManager(store, identityOptions, hasher, userValidators, passwordValidators,
                                          lookupNormalizer, errorDescriber, null!, logger);

        userManager.RegisterTokenProvider(AspNetCoreTokenOptions.DefaultProvider, new FakeUserTwoFactorTokenProvider());

        return userManager;
    }

    /// <summary>
    /// Gets a new <see cref="RoleManager{TRole}"/> instance.
    /// </summary>
    /// <param name="output">
    /// Xunit output helper.
    /// </param>
    /// <param name="domainDataSource">
    /// Domain data source.
    /// </param>
    /// <returns>
    /// <see cref="RoleManager{TRole}"/> instance.
    /// </returns>
    public static RoleManager<Role> GetRoleManager(ITestOutputHelper output, IDomainDataSource? domainDataSource = null)
    {
        domainDataSource ??= GetDomainDataSource();

        var store = new FakeRoleStore(domainDataSource);
        var errorDescriber = new IdentityErrorDescriber();
        var validators = new[] { new RoleValidator<Role>(errorDescriber) };
        var lookupNormalizer = new UpperInvariantLookupNormalizer();
        var logger = GetLogger<RoleManager<Role>>(output);

        return new(store, validators, lookupNormalizer, errorDescriber, logger);
    }

    /// <summary>
    /// Gets a new <see cref="ILogger{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">
    /// The type whose name is used for the logger category name.
    /// </typeparam>
    /// <param name="output">
    /// Xunit output helper.
    /// </param>
    /// <returns>
    /// <see cref="ILogger{T}"/> instance.
    /// </returns>
    public static ILogger<T> GetLogger<T>(ITestOutputHelper output)
        => new XunitTestOutputLogger<T>(output);
    #endregion
}