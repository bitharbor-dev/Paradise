using Microsoft.EntityFrameworkCore.Metadata;

namespace Paradise.Tests.Doubles.Fakes.Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

/// <summary>
/// Provides an abstraction to make entity configuration
/// provider-agnostic during tests execution.
/// </summary>
public interface IEntityNormalizer
{
    #region Methods
    /// <summary>
    /// Checks whether the current implementation can perform any
    /// normalization over the given <paramref name="entity"/>.
    /// </summary>
    /// <param name="entity">
    /// The <see cref="IConventionEntityType"/> to check.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="entity"/>
    /// can be normalized using the current implementation,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    bool CanNormalize(IConventionEntityType entity);

    /// <summary>
    /// Performs entity configuration normalization
    /// to make it provider-agnostic and execute tests.
    /// </summary>
    /// <param name="entity">
    /// The <see cref="IConventionEntityType"/> to normalize.
    /// </param>
    void Normalize(IConventionEntityType entity);

    /// <summary>
    /// Attempts to normalize the given <paramref name="entity"/>
    /// if the <see cref="CanNormalize"/> returns <see langword="true"/>.
    /// </summary>
    /// <param name="entity">
    /// The <see cref="IConventionEntityType"/> to normalize.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="entity"/>
    /// was normalized using the current implementation,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    bool TryNormalize(IConventionEntityType entity)
    {
        if (CanNormalize(entity))
        {
            Normalize(entity);
            return true;
        }

        return false;
    }
    #endregion
}