using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Paradise.Common;
using Paradise.Tests.Miscellaneous;
using Paradise.WebApi.Authorization;
using Paradise.WebApi.Extensions;
using Paradise.WebApi.Infrastructure.TypeConverters;
using Paradise.WebApi.Services.Authentication;
using Paradise.WebApi.Services.Background;
using System.ComponentModel;

namespace Paradise.WebApi.Tests.Unit.Extensions;

/// <summary>
/// <see cref="IServiceCollectionExtensions"/> test class.
/// </summary>
public sealed partial class IServiceCollectionExtensionsTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="AddAuthenticationAndAuthorization"/> method.
    /// </summary>
    public static TheoryData<string> AddAuthenticationAndAuthorization_MemberData { get; } = new()
    {
        { EnvironmentNames.Development          },
        { EnvironmentNames.DevelopmentDocker    },
        { EnvironmentNames.Staging              },
        { EnvironmentNames.StagingDocker        },
        { EnvironmentNames.Production           },
        { EnvironmentNames.ProductionDocker     }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddAuthenticationAndAuthorization"/> method should
    /// register authentication, authorization and related services.
    /// </summary>
    /// <param name="environmentName">
    /// Current environment name.
    /// </param>
    [Theory, MemberData(nameof(AddAuthenticationAndAuthorization_MemberData))]
    public void AddAuthenticationAndAuthorization(string environmentName)
    {
        // Arrange
        var provider = Test.BuildAuthenticationAndAuthorizationServiceProvider(environmentName);

        // Act & Assert
        Assert.ServiceLifetime<IAuthenticationService>(provider, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddAuthenticationAndAuthorization"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// environment name is invalid.
    /// </summary>
    [Fact]
    public void AddAuthenticationAndAuthorization_ThrowsOnInvalidEnvironmentName()
    {
        // Arrange
        var environmentName = "UnknownEnvironment";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(()
            => Test.BuildAuthenticationAndAuthorizationServiceProvider(environmentName));
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddAuthorizationResultHandler"/> method should
    /// register a singleton <see cref="IAuthorizationMiddlewareResultHandler"/> implementation.
    /// </summary>
    [Fact]
    public void AddAuthorizationResultHandler()
    {
        // Arrange
        var provider = Test.BuilderAuthorizationResultHandlerServiceProvider();

        // Act & Assert
        Assert.ServiceLifetime<IAuthorizationMiddlewareResultHandler>(provider, ServiceLifetime.Singleton,
            handler => Assert.IsType<AuthorizationResultHandler>(handler));
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddStartupAndShutdownActivities"/> method should
    /// register the <see cref="LifecycleManagementService"/> as hosted service.
    /// </summary>
    [Fact]
    public void AddStartupAndShutdownActivities()
    {
        // Arrange
        var provider = Test.BuildStartupActivitiesServiceProvider();

        // Act
        var hostedServices = provider.GetServices<IHostedService>();

        // Assert
        Assert.Contains(hostedServices, hostedService => hostedService is LifecycleManagementService);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddDomainEventsDispatchingService"/> method should
    /// register the <see cref="DomainEventsDispatchingService"/> as hosted service.
    /// </summary>
    [Fact]
    public void AddDomainEventsDispatchingService()
    {
        // Arrange
        var provider = Test.BuildDomainEventsDispatchingServiceProvider();

        // Act
        var hostedServices = provider.GetServices<IHostedService>();

        // Assert
        Assert.Contains(hostedServices, hostedService => hostedService is DomainEventsDispatchingService);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddRequestLocalization"/> method should
    /// register localization services and apply
    /// <see cref="RequestCultureConverter"/> to <see cref="RequestCulture"/>.
    /// </summary>
    [Fact]
    public void AddRequestLocalization()
    {
        // Arrange
        var provider = Test.BuildRequestLocalizationServiceProvider();

        // Act & Assert
        Assert.ServiceLifetime<IOptions<RequestLocalizationOptions>>(provider, ServiceLifetime.Singleton);

        var typeConverters = TypeDescriptor.GetAttributes(typeof(RequestCulture)).OfType<TypeConverterAttribute>();
        var converter = Assert.Single(typeConverters);

        Assert.Equal(typeof(RequestCultureConverter).AssemblyQualifiedName, converter.ConverterTypeName);
    }
    #endregion
}