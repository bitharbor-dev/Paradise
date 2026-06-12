using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Extensions;
using Paradise.ApplicationLogic.Options.Models;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Services.MessageTemplates;
using Paradise.Primitives;
using Paradise.Tests.Extensibility;
using System.Text.Json;

namespace Paradise.ApplicationLogic.Tests.Unit.Extensions;

/// <summary>
/// <see cref="IServiceCollectionExtensions"/> test class.
/// </summary>
public sealed partial class IServiceCollectionExtensionsTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="AddApplicationLogic"/> method.
    /// </summary>
    public static TheoryData<string> AddApplicationLogic_MemberData { get; } = [.. EnvironmentNames.AllowedEnvironments];
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddApplicationLogic"/> method should
    /// configure the DI container to resolve core application logic services for the specified environment.
    /// </summary>
    /// <param name="environmentName">
    /// Current environment name.
    /// </param>
    [Theory, MemberData(nameof(AddApplicationLogic_MemberData))]
    public void AddApplicationLogic(string environmentName)
    {
        // Arrange
        var provider = Test.BuildApplicationLogicServiceProvider(environmentName);

        // Act & Assert
        Assert.ServiceLifetime<IOptions<ApplicationOptions>>(provider, ServiceLifetime.Singleton,
            options => Assert.Equivalent(Test.Options.ApplicationOptions, options.Value));

        Assert.ServiceLifetime<IOptions<EmailTemplateOptions>>(provider, ServiceLifetime.Singleton,
            options => Assert.Equivalent(Test.Options.EmailTemplateOptions, options.Value));

        Assert.ServiceLifetime<IOptions<JsonSerializerOptions>>(provider, ServiceLifetime.Singleton,
            options => Assert.Equivalent(Test.Options.JsonSerializerOptions, options.Value));
    }
    #endregion
}