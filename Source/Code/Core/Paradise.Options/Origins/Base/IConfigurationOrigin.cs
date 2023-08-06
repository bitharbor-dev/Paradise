using Microsoft.Extensions.Configuration;

namespace Paradise.Options.Origins.Base;

/// <summary>
/// An <see cref="IConfiguration"/> origin abstraction.
/// </summary>
/// <typeparam name="TOptions">
/// Source options type.
/// </typeparam>
public interface IConfigurationOrigin<TOptions> : IConfigurationOrigin
{
    #region Properties
    /// <summary>
    /// Configuration source options.
    /// </summary>
    TOptions Options { get; }
    #endregion
}

/// <summary>
/// An <see cref="IConfiguration"/> origin abstraction.
/// </summary>
public interface IConfigurationOrigin
{
    #region Methods
    /// <summary>
    /// Gets the configuration.
    /// </summary>
    /// <returns>
    /// An <see cref="IConfiguration"/> instance containing
    /// application configuration.
    /// </returns>
    IConfiguration GetConfiguration();
    #endregion
}