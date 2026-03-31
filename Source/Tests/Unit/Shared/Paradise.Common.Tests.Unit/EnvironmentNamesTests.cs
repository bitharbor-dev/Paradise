namespace Paradise.Common.Tests.Unit;

/// <summary>
/// <see cref="EnvironmentNames"/> test class.
/// </summary>
public sealed class EnvironmentNamesTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="ThrowIfNotAllowedEnvironment_DoesNotThrow"/> method.
    /// </summary>
    public static TheoryData<string> ThrowIfNotAllowedEnvironment_DoesNotThrow_MemberData { get; } = new()
    {
        { EnvironmentNames.Development          },
        { EnvironmentNames.DevelopmentDocker    },
        { EnvironmentNames.Staging              },
        { EnvironmentNames.StagingDocker        },
        { EnvironmentNames.Production           },
        { EnvironmentNames.ProductionDocker     }
    };

    /// <summary>
    /// Provides member data for <see cref="IsDevelopment_ReturnsTrue"/> method.
    /// </summary>
    public static TheoryData<string> IsDevelopment_ReturnsTrue_MemberData { get; } = new()
    {
        { EnvironmentNames.Development          },
        { EnvironmentNames.DevelopmentDocker    }
    };

    /// <summary>
    /// Provides member data for <see cref="IsStaging_ReturnsTrue"/> method.
    /// </summary>
    public static TheoryData<string> IsStaging_ReturnsTrue_MemberData { get; } = new()
    {
        { EnvironmentNames.Staging          },
        { EnvironmentNames.StagingDocker    }
    };

    /// <summary>
    /// Provides member data for <see cref="IsProduction_ReturnsTrue"/> method.
    /// </summary>
    public static TheoryData<string> IsProduction_ReturnsTrue_MemberData { get; } = new()
    {
        { EnvironmentNames.Production       },
        { EnvironmentNames.ProductionDocker }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="EnvironmentNames.AllowedEnvironments"/> property should
    /// return the list of allowed environment names, containing 3 different
    /// environments and their "Docker" sub-environments.
    /// </summary>
    [Fact]
    public void AllowedEnvironments()
    {
        // Arrange

        // Act

        // Assert
        Assert.Contains(EnvironmentNames.Development, EnvironmentNames.AllowedEnvironments);
        Assert.Contains(EnvironmentNames.DevelopmentDocker, EnvironmentNames.AllowedEnvironments);
        Assert.Contains(EnvironmentNames.Staging, EnvironmentNames.AllowedEnvironments);
        Assert.Contains(EnvironmentNames.StagingDocker, EnvironmentNames.AllowedEnvironments);
        Assert.Contains(EnvironmentNames.Production, EnvironmentNames.AllowedEnvironments);
        Assert.Contains(EnvironmentNames.ProductionDocker, EnvironmentNames.AllowedEnvironments);
    }

    /// <summary>
    /// The <see cref="EnvironmentNames.ThrowIfNotAllowedEnvironment"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// environment name is not in the list of allowed environment names.
    /// </summary>
    [Fact]
    public void ThrowIfNotAllowedEnvironment_Throws()
    {
        // Arrange
        var environmentName = string.Empty;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(()
            => EnvironmentNames.ThrowIfNotAllowedEnvironment(environmentName));
    }

    /// <summary>
    /// The <see cref="EnvironmentNames.ThrowIfNotAllowedEnvironment"/> method should
    /// not throw the <see cref="InvalidOperationException"/> if the input
    /// environment name is in the list of allowed environment names.
    /// </summary>
    /// <param name="environmentName">
    /// Environment name to check.
    /// </param>
    [Theory, MemberData(nameof(ThrowIfNotAllowedEnvironment_DoesNotThrow_MemberData))]
    public void ThrowIfNotAllowedEnvironment_DoesNotThrow(string environmentName)
    {
        // Arrange

        // Act
        var exception = Record.Exception(()
            => EnvironmentNames.ThrowIfNotAllowedEnvironment(environmentName));

        // Assert
        Assert.Null(exception);
    }

    /// <summary>
    /// The <see cref="EnvironmentNames.IsDevelopment"/> method should
    /// return <see langword="true"/> if the input
    /// environment name belongs to the "Development" environment names group.
    /// </summary>
    /// <param name="environmentName">
    /// The environment name to check.
    /// </param>
    [Theory, MemberData(nameof(IsDevelopment_ReturnsTrue_MemberData))]
    public void IsDevelopment_ReturnsTrue(string environmentName)
    {
        // Arrange

        // Act
        var result = EnvironmentNames.IsDevelopment(environmentName);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="EnvironmentNames.IsDevelopment"/> method should
    /// return <see langword="false"/> if the input
    /// environment name does not belong to the "Development" environment names group.
    /// </summary>
    [Fact]
    public void IsDevelopment_ReturnsFalse()
    {
        // Arrange
        var environmentName = "EnvironmentName";

        // Act
        var result = EnvironmentNames.IsDevelopment(environmentName);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="EnvironmentNames.IsStaging"/> method should
    /// return <see langword="true"/> if the input
    /// environment name belongs to the "Staging" environment names group.
    /// </summary>
    /// <param name="environmentName">
    /// The environment name to check.
    /// </param>
    [Theory, MemberData(nameof(IsStaging_ReturnsTrue_MemberData))]
    public void IsStaging_ReturnsTrue(string environmentName)
    {
        // Arrange

        // Act
        var result = EnvironmentNames.IsStaging(environmentName);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="EnvironmentNames.IsStaging"/> method should
    /// return <see langword="false"/> if the input
    /// environment name does not belong to the "Staging" environment names group.
    /// </summary>
    [Fact]
    public void IsStaging_ReturnsFalse()
    {
        // Arrange
        var environmentName = "EnvironmentName";

        // Act
        var result = EnvironmentNames.IsStaging(environmentName);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="EnvironmentNames.IsProduction"/> method should
    /// return <see langword="true"/> if the input
    /// environment name belongs to the "Production" environment names group.
    /// </summary>
    /// <param name="environmentName">
    /// The environment name to check.
    /// </param>
    [Theory, MemberData(nameof(IsProduction_ReturnsTrue_MemberData))]
    public void IsProduction_ReturnsTrue(string environmentName)
    {
        // Arrange

        // Act
        var result = EnvironmentNames.IsProduction(environmentName);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="EnvironmentNames.IsProduction"/> method should
    /// return <see langword="false"/> if the input
    /// environment name does not belong to the "Production" environment names group.
    /// </summary>
    [Fact]
    public void IsProduction_ReturnsFalse()
    {
        // Arrange
        var environmentName = "EnvironmentName";

        // Act
        var result = EnvironmentNames.IsProduction(environmentName);

        // Assert
        Assert.False(result);
    }
    #endregion
}