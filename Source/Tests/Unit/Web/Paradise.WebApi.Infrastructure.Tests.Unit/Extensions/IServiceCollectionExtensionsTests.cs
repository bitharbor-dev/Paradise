using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.Common;
using Paradise.Common.Web;
using Paradise.WebApi.Infrastructure.Authentication.Caching;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys;
using Paradise.WebApi.Infrastructure.Authentication.JwtBearer.Keys.Implementation;
using Paradise.WebApi.Infrastructure.Extensions;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Extensions;

/// <summary>
/// <see cref="IServiceCollectionExtensions"/> test class.
/// </summary>
public sealed partial class IServiceCollectionExtensionsTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="AddJwtBearerAuthentication_InDevelopmentEnvironment"/> method.
    /// </summary>
    public static TheoryData<string> AddJwtBearerAuthentication_InDevelopmentEnvironment_MemberData { get; } = new()
    {
        { EnvironmentNames.Development          },
        { EnvironmentNames.DevelopmentDocker    }
    };

    /// <summary>
    /// Provides member data for <see cref="AddJwtBearerAuthentication_InNonDevelopmentEnvironment"/> method.
    /// </summary>
    public static TheoryData<string> AddJwtBearerAuthentication_InNonDevelopmentEnvironment_MemberData { get; } = new()
    {
        { EnvironmentNames.Staging          },
        { EnvironmentNames.StagingDocker    },
        { EnvironmentNames.Production       },
        { EnvironmentNames.ProductionDocker }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddJwtBearerAuthentication"/> method should
    /// configure the DI container to resolve web infrastructure services for development environment.
    /// </summary>
    /// <param name="environmentName">
    /// Current environment name.
    /// </param>
    [Theory, MemberData(nameof(AddJwtBearerAuthentication_InDevelopmentEnvironment_MemberData))]
    public async Task AddJwtBearerAuthentication_InDevelopmentEnvironment(string environmentName)
    {
        // Arrange
        var provider = Test.BuildWebInfrastructureServiceProvider(environmentName);

        // Act
        var signingKeyProvider = provider.GetService<IJwtSigningKeyProvider>();

        var optionsFactory = provider.GetRequiredService<IOptionsFactory<JwtBearerOptions>>();
        var defaultJwtBearerOptions = optionsFactory.Create(AuthenticationSchemeNames.Default);
        var secondaryJwtBearerOptions = optionsFactory.Create(AuthenticationSchemeNames.DisableTokenLifetimeValidation);

        var schemeProvider = provider.GetRequiredService<IAuthenticationSchemeProvider>();
        var defaultScheme = await schemeProvider.GetSchemeAsync(AuthenticationSchemeNames.Default);
        var secondaryScheme = await schemeProvider.GetSchemeAsync(AuthenticationSchemeNames.DisableTokenLifetimeValidation);

        var jwtManager = provider.GetService<IJwtManager>();
        var refreshTokenCache = provider.GetService<IRefreshTokenCache>();

        // Assert
        Assert.IsType<SymmetricSigningKeyProvider>(signingKeyProvider);

        Assert.NotNull(defaultJwtBearerOptions.TokenValidationParameters.IssuerSigningKey);
        Assert.True(defaultJwtBearerOptions.TokenValidationParameters.ValidateLifetime);

        Assert.NotNull(secondaryJwtBearerOptions.TokenValidationParameters.IssuerSigningKey);
        Assert.False(secondaryJwtBearerOptions.TokenValidationParameters.ValidateLifetime);

        Assert.NotNull(defaultScheme);
        Assert.NotNull(secondaryScheme);

        Assert.NotNull(jwtManager);
        Assert.NotNull(refreshTokenCache);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddJwtBearerAuthentication"/> method should
    /// configure the DI container to resolve web infrastructure services for non-development environment.
    /// </summary>
    /// <param name="environmentName">
    /// Current environment name.
    /// </param>
    [Theory, MemberData(nameof(AddJwtBearerAuthentication_InNonDevelopmentEnvironment_MemberData))]
    public async Task AddJwtBearerAuthentication_InNonDevelopmentEnvironment(string environmentName)
    {
        // Arrange
        var provider = Test.BuildWebInfrastructureServiceProvider(environmentName);

        // Act
        var signingKeyProvider = provider.GetService<IJwtSigningKeyProvider>();

        var optionsFactory = provider.GetRequiredService<IOptionsFactory<JwtBearerOptions>>();
        var defaultJwtBearerOptions = optionsFactory.Create(AuthenticationSchemeNames.Default);
        var secondaryJwtBearerOptions = optionsFactory.Create(AuthenticationSchemeNames.DisableTokenLifetimeValidation);

        var schemeProvider = provider.GetRequiredService<IAuthenticationSchemeProvider>();
        var defaultScheme = await schemeProvider.GetSchemeAsync(AuthenticationSchemeNames.Default);
        var secondaryScheme = await schemeProvider.GetSchemeAsync(AuthenticationSchemeNames.DisableTokenLifetimeValidation);

        var jwtManager = provider.GetService<IJwtManager>();
        var refreshTokenCache = provider.GetService<IRefreshTokenCache>();

        // Assert
        Assert.IsType<AsymmetricSigningKeyProvider>(signingKeyProvider);

        Assert.NotNull(defaultJwtBearerOptions.TokenValidationParameters.IssuerSigningKey);
        Assert.True(defaultJwtBearerOptions.TokenValidationParameters.ValidateLifetime);

        Assert.NotNull(secondaryJwtBearerOptions.TokenValidationParameters.IssuerSigningKey);
        Assert.False(secondaryJwtBearerOptions.TokenValidationParameters.ValidateLifetime);

        Assert.NotNull(defaultScheme);
        Assert.NotNull(secondaryScheme);

        Assert.NotNull(jwtManager);
        Assert.NotNull(refreshTokenCache);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddJwtBearerAuthentication"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// environment name is invalid.
    /// </summary>
    [Fact]
    public void AddJwtBearerAuthentication_ThrowsOnInvalidEnvironmentName()
    {
        // Arrange
        var environmentName = "UnknownEnvironment";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(()
            => Test.BuildWebInfrastructureServiceProvider(environmentName));
    }
    #endregion
}