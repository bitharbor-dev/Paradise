namespace Paradise.Common.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IEnumerable{T}"/> <see langword="interface"/>.
/// </summary>
public static class IEnumerableExtensions
{
    #region Public methods
    /// <summary>
    /// Performs the specified action on each element of the <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Element type.
    /// </typeparam>
    /// <param name="values">
    /// Target collection.
    /// </param>
    /// <param name="action">
    /// The <see cref="Action{T}"/> delegate to perform on each element of the <see cref="IEnumerable{T}"/>.
    /// </param>
    public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
    {
        foreach (var item in values)
            action(item);
    }
    #endregion
}