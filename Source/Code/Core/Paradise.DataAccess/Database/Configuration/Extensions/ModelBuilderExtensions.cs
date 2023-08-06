using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior;

namespace Paradise.DataAccess.Database.Configuration.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="ModelBuilder"/> class.
/// </summary>
internal static class ModelBuilderExtensions
{
    #region Public methods
    /// <summary>
    /// Marks column with the name <paramref name="columnName"/> as read-only
    /// across all models inside the <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">
    /// Target model builder.
    /// </param>
    /// <param name="columnName">
    /// Name of the column to be marked as read-only.
    /// </param>
    /// <param name="throwError">
    /// Indicates whether an error should be thrown in case of a column update attempt.
    /// Simply ignores the updates if set to <see langword="false"/>.
    /// </param>
    public static void MarkColumnAsReadOnly(this ModelBuilder builder, string columnName, bool throwError = false)
    {
        var entities = builder.Model.GetEntityTypes();

        foreach (var entity in entities)
            entity.FindProperty(columnName)?.SetAfterSaveBehavior(throwError ? Throw : Ignore);
    }
    #endregion
}