using Microsoft.Extensions.Configuration;
using Paradise.Common.Extensions;

namespace Paradise.Common.Tests.Unit.Extensions;

/// <summary>
/// <see cref="IConfigurationExtensions"/> test class.
/// </summary>
public sealed partial class IConfigurationExtensionsTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="IConfigurationExtensions.BindSection"/> method should
    /// bind the configuration section named correspondingly to the input instance type
    /// and apply all property values to that instance.
    /// </summary>
    [Fact]
    public void BindSection()
    {
        // Arrange
        Test.ConfigurationInstance = new()
        {
            IntegerValue = 1,
            StringValue = nameof(BindingTarget),
            Child = new()
            {
                IntegerValue = 2,
                StringValue = nameof(BindingTarget.Child)
            }
        };

        var actualInstance = new BindingTarget();

        var configuration = Test.GetConfiguration();

        // Act
        configuration.BindSection(actualInstance);

        // Assert
        Assert.Equivalent(Test.ConfigurationInstance, actualInstance, true);
    }

    /// <summary>
    /// The <see cref="IConfigurationExtensions.BindSection"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="IConfiguration"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void BindSection_ThrowsOnNull()
    {
        // Arrange
        Test.ConfigurationInstance = new();

        var configuration = null as IConfiguration;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => configuration!.BindSection(Test.ConfigurationInstance));
    }

    /// <summary>
    /// The <see cref="IConfigurationExtensions.BindSection"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// binding target object does not have the corresponding configuration section.
    /// </summary>
    [Fact]
    public void BindSection_ThrowsOnUnknownConfiguration()
    {
        // Arrange
        Test.ConfigurationInstance = new()
        {
            IntegerValue = 1,
            StringValue = nameof(BindingTarget),
            Child = new()
            {
                IntegerValue = 2,
                StringValue = nameof(BindingTarget.Child)
            }
        };

        var actualInstance = new BindingTarget();

        var configuration = Test.GetConfiguration(true);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(()
            => configuration.BindSection(actualInstance));
    }

    /// <summary>
    /// The <see cref="IConfigurationExtensions.BindOptionalSection"/> method should
    /// bind the configuration section named correspondingly to the input instance type
    /// and apply all property values to that instance.
    /// </summary>
    [Fact]
    public void BindOptionalSection()
    {
        // Arrange
        Test.ConfigurationInstance = new()
        {
            IntegerValue = 1,
            StringValue = nameof(BindingTarget),
            Child = new()
            {
                IntegerValue = 2,
                StringValue = nameof(BindingTarget.Child)
            }
        };

        var actualInstance = new BindingTarget();

        var configuration = Test.GetConfiguration();

        // Act
        configuration.BindOptionalSection(actualInstance);

        // Assert
        Assert.Equivalent(Test.ConfigurationInstance, actualInstance, true);
    }

    /// <summary>
    /// The <see cref="IConfigurationExtensions.BindOptionalSection"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="IConfiguration"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void BindOptionalSection_ThrowsOnNull()
    {
        // Arrange
        Test.ConfigurationInstance = new();

        var configuration = null as IConfiguration;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => configuration!.BindOptionalSection(Test.ConfigurationInstance));
    }

    /// <summary>
    /// The <see cref="IConfigurationExtensions.BindOptionalSection"/> method should
    /// not throw an exception if the input
    /// binding target object does not have the corresponding configuration section.
    /// </summary>
    [Fact]
    public void BindOptionalSection_SkipsOnUnknownConfiguration()
    {
        // Arrange
        Test.ConfigurationInstance = new()
        {
            IntegerValue = 1,
            StringValue = nameof(BindingTarget),
            Child = new()
            {
                IntegerValue = 2,
                StringValue = nameof(BindingTarget.Child)
            }
        };

        var actualInstance = new BindingTarget();

        var configuration = Test.GetConfiguration(true);

        // Act
        var exception = Record.Exception(()
            => configuration.BindOptionalSection(actualInstance));

        // Assert
        Assert.Null(exception);
    }

    /// <summary>
    /// The <see cref="IConfigurationExtensions.GetRequiredInstance"/> method should
    /// create an instance of the given type by using the configuration section
    /// named correspondingly to the input type name.
    /// </summary>
    [Fact]
    public void GetRequiredInstance()
    {
        // Arrange
        Test.ConfigurationInstance = new()
        {
            IntegerValue = 1,
            StringValue = nameof(BindingTarget),
            Child = new()
            {
                IntegerValue = 2,
                StringValue = nameof(BindingTarget.Child)
            }
        };

        var configuration = Test.GetConfiguration();

        // Act
        var actualInstance = configuration.GetRequiredInstance<BindingTarget>();

        // Assert
        Assert.Equivalent(Test.ConfigurationInstance, actualInstance, true);
    }

    /// <summary>
    /// The <see cref="IConfigurationExtensions.GetRequiredInstance"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// type does not have the corresponding configuration section.
    /// </summary>
    [Fact]
    public void GetRequiredInstance_ThrowsOnUnknownConfiguration()
    {
        // Arrange
        Test.ConfigurationInstance = new()
        {
            IntegerValue = 1,
            StringValue = nameof(BindingTarget),
            Child = new()
            {
                IntegerValue = 2,
                StringValue = nameof(BindingTarget.Child)
            }
        };

        var configuration = Test.GetConfiguration(true);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(configuration.GetRequiredInstance<BindingTarget>);
    }

    /// <summary>
    /// The <see cref="IConfigurationExtensions.GetRequiredInstance"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// type's configuration section value is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void GetRequiredInstance_ThrowsOnNullConfigurationSectionValue()
    {
        // Arrange
        Test.ConfigurationInstance = null;

        var configuration = Test.GetConfiguration();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(configuration.GetRequiredInstance<BindingTarget>);
    }
    #endregion
}