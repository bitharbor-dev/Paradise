using Paradise.Models;

namespace Paradise.WebApi.Base;

/// <summary>
/// Defines a contract for providing metadata about problem details.
/// </summary>
/// <remarks>
/// This abstraction exists due to the fact that
/// common problem details class implementation
/// can only be referenced via adding a reference
/// to the whole AspNetCore framework,
/// which is not desirable for a base library.
/// By defining this interface, we can decouple the problem details metadata
/// from the specific implementation and allow for more flexibility
/// in how problem details are handled across different projects.
/// <para>
/// Remove it once the common problem details class is moved to a separate package
/// (<see href="https://github.com/dotnet/aspnetcore/issues/58551">#58551</see>).
/// </para>
/// </remarks>
public interface IProblemDetailsMetadata
{
    #region Properties
    /// <summary>
    /// The collection of application-specific errors associated with the problem details.
    /// </summary>
    IEnumerable<ApplicationError> Errors { get; }

    /// <summary>
    /// Gets the <see cref="IDictionary{TKey, TValue}"/> for extension members.
    /// <para>
    /// Problem type definitions MAY extend the problem details object with additional members. Extension members appear in the same namespace as
    /// other members of a problem type.
    /// </para>
    /// </summary>
    IDictionary<string, object?> Extensions { get; }
    #endregion
}