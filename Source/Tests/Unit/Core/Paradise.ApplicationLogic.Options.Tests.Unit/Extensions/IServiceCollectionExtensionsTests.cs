using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Options.Extensions;
using Paradise.Tests.Miscellaneous;

namespace Paradise.ApplicationLogic.Options.Tests.Unit.Extensions;

/// <summary>
/// <see cref="IServiceCollectionExtensions"/> test class.
/// </summary>
public sealed partial class IServiceCollectionExtensionsTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="AddOptions"/> method.
    /// </summary>
    public static TheoryData<string?, string?, int, string?> AddOptions_MemberData { get; } = new()
    {
        { null,     "root",     1,  null    },
        { "Parent", "nested",   2,  null    },
        { null,     "parent",   3,  "child" }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddOptions"/> method should
    /// register the <see cref="IOptions{TOptions}"/>, of the type
    /// which was passed as generic parameter, optionally - specifying the path
    /// to the corresponding configuration section.
    /// </summary>
    /// <param name="configurationSectionPath">
    /// A semicolon ':' delimited string representing
    /// the sections path to the target options section.
    /// </param>
    /// <param name="stringValue">
    /// Test string property value.
    /// </param>
    /// <param name="integerValue">
    /// Test integer property value.
    /// </param>
    /// <param name="childStringValue">
    /// Test child string property value.
    /// </param>
    [Theory, MemberData(nameof(AddOptions_MemberData))]
    public void AddOptions(string? configurationSectionPath, string? stringValue, int integerValue, string? childStringValue)
    {
        // Arrange
        Test.Options.StringValue = stringValue;
        Test.Options.IntegerValue = integerValue;

        if (childStringValue is not null)
            Test.Options.Child = new() { StringValue = childStringValue };

        var configuration = Test.BuildConfiguration(configurationSectionPath);
        var provider = new ServiceCollection()
            .AddOptions<TestOptions>(configuration, configurationSectionPath)
            .BuildServiceProvider();

        // Act & Assert
        Assert.ServiceLifetime<IOptions<TestOptions>>(provider, ServiceLifetime.Singleton,
            options => Assert.Equivalent(Test.Options, options.Value));
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddOptions"/> method should
    /// register the <see cref="IOptions{TOptions}"/>, of the type
    /// which was passed as generic parameter, as well as an action to be invoked
    /// on the post-configuration stage.
    /// </summary>
    [Fact]
    public void AddOptions_WithPostConfigure()
    {
        // Arrange
        var postConfigureInvoked = false;

        void PostConfigure(TestOptions options, IServiceProvider provider)
        {
            options.StringValue = string.Empty;
            options.IntegerValue = int.MaxValue;

            postConfigureInvoked = true;
        }

        var configuration = Test.BuildConfiguration();
        var provider = new ServiceCollection()
            .AddOptions<TestOptions>(configuration, postConfigure: PostConfigure)
            .BuildServiceProvider();

        // Act & Assert
        Assert.ServiceLifetime<IOptions<TestOptions>>(provider, ServiceLifetime.Singleton, options =>
        {
            Assert.Equal(string.Empty, options.Value.StringValue);
            Assert.Equal(int.MaxValue, options.Value.IntegerValue);
        });

        Assert.True(postConfigureInvoked);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddOptions"/> method should
    /// cause the <see cref="IOptions{TOptions}.Value"/> property to
    /// throw the <see cref="OptionsValidationException"/> if the input
    /// configuration did not pass the data annotations validation.
    /// </summary>
    [Fact]
    public void AddOptions_ThrowsOnFailedDataAnnotationsValidation()
    {
        // Arrange
        Test.Options.StringValue = null;

        var configuration = Test.BuildConfiguration();
        var provider = new ServiceCollection()
            .AddOptions<TestOptions>(configuration, validateDataAnnotations: true)
            .BuildServiceProvider();

        // Act & Assert
        Assert.Throws<OptionsValidationException>(()
            => provider.GetRequiredService<IOptions<TestOptions>>().Value);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddOptions"/> method should
    /// cause the <see cref="IHost"/> instance to
    /// throw the <see cref="OptionsValidationException"/> if the input
    /// configuration did not pass the data annotations validation on application startup.
    /// </summary>
    [Fact]
    public void AddOptions_ThrowsOnFailedDataAnnotationsStartupValidation()
    {
        // Arrange
        Test.Options.StringValue = null;

        var configuration = Test.BuildConfiguration();

        var host = new HostBuilder()
            .ConfigureServices(services =>
            {
                services.AddOptions<TestOptions>(configuration,
                                                 validateOnStartup: true,
                                                 validateDataAnnotations: true);
            }).Build();

        // Act & Assert
        Assert.Throws<OptionsValidationException>(host.Start);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.PostConfigure{TOptions}"/> method should
    /// setup the post-configure action with the configured options instance and service provider.
    /// </summary>
    [Fact]
    public void PostConfigure()
    {
        // Arrange
        var optionsName = "OptionsName";
        var stringValue = "Test";

        var provider = new ServiceCollection()
            .AddOptions<TestOptions>(optionsName)
            .Services
            .PostConfigure<TestOptions>(optionsName, (options, _) => options.StringValue = stringValue)
            .BuildServiceProvider();

        var factory = provider.GetRequiredService<IOptionsFactory<TestOptions>>();

        // Act
        var options = factory.Create(optionsName);

        // Assert
        Assert.Equal(stringValue, options.StringValue);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.PostConfigure{TOptions}"/> method should
    /// only setup the post-configure action for the options with the specified name.
    /// </summary>
    [Fact]
    public void PostConfigure_SkipsNonEqualName()
    {
        // Arrange
        var optionsName1 = "OptionsName1";
        var optionsName2 = "OptionsName2";
        var stringValue1 = "Test1";
        var stringValue2 = "Test2";

        var provider = new ServiceCollection()
            .AddOptions<TestOptions>(optionsName1)
            .Services
            .AddOptions<TestOptions>(optionsName2)
            .Services
            .PostConfigure<TestOptions>(optionsName1, (options, _) => options.StringValue = stringValue1)
            .PostConfigure<TestOptions>(optionsName2, (options, _) => options.StringValue = stringValue2)
            .BuildServiceProvider();

        var factory = provider.GetRequiredService<IOptionsFactory<TestOptions>>();

        // Act
        var options1 = factory.Create(optionsName1);
        var options2 = factory.Create(optionsName2);

        // Assert
        Assert.Equal(stringValue1, options1.StringValue);
        Assert.Equal(stringValue2, options2.StringValue);
    }
    #endregion
}