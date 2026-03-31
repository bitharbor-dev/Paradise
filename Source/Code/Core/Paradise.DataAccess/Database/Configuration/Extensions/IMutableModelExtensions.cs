using Microsoft.EntityFrameworkCore.Metadata;

namespace Paradise.DataAccess.Database.Configuration.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IMutableModel"/> <see langword="interface"/>.
/// </summary>
internal static class IMutableModelExtensions
{
    #region Public methods
    /// <summary>
    /// Marks column with the name <paramref name="columnName"/> as read-only
    /// across all models inside the <paramref name="model"/>.
    /// </summary>
    /// <param name="model">
    /// Target model.
    /// </param>
    /// <param name="columnName">
    /// Name of the column to be marked as read-only.
    /// </param>
    public static void MarkColumnAsReadOnly(this IMutableModel model, string columnName)
    {
        var entities = model.GetEntityTypes();

        foreach (var entity in entities)
        {
            var property = entity.FindProperty(columnName);

            property?.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        }
    }
    #endregion
}