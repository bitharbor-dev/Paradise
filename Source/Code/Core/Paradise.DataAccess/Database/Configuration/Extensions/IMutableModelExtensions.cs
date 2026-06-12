using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Paradise.DataAccess.Database.Configuration.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IMutableModel"/> <see langword="interface"/>.
/// </summary>
internal static class IMutableModelExtensions
{
    #region Public methods
    /// <summary>
    /// Marks property with the name <paramref name="propertyName"/> as read-only
    /// across all models inside the <paramref name="model"/>.
    /// </summary>
    /// <param name="model">
    /// Target model.
    /// </param>
    /// <param name="propertyName">
    /// Name of the property to be marked as read-only.
    /// </param>
    public static void MarkPropertyAsReadOnly(this IMutableModel model,
                                              string propertyName)
    {
        var entities = model.GetEntityTypes();

        foreach (var entity in entities)
        {
            var property = entity.FindProperty(propertyName);

            property?.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        }
    }

    /// <summary>
    /// Disables value generation for property with the name
    /// <paramref name="propertyName"/> across all entities inside the
    /// <paramref name="model"/>.
    /// </summary>
    /// <param name="model">
    /// Target model.
    /// </param>
    /// <param name="propertyName">
    /// Name of the property for which value generation should be disabled.
    /// </param>
    /// <param name="baseType">
    /// Optional base type used to filter target entities.
    /// Only entities assignable from the specified type are processed.
    /// </param>
    public static void DisableValueGenerationFor(this IMutableModel model,
                                                 string propertyName,
                                                 Type? baseType = null)
    {
        var entities = model.GetEntityTypes();

        if (baseType is not null)
            entities = entities.Where(entity => entity.ClrType.IsAssignableTo(baseType));

        foreach (var entity in entities)
        {
            var property = entity.FindProperty(propertyName);

            property?.ValueGenerated = ValueGenerated.Never;
        }
    }

    /// <summary>
    /// Sets database schema for entity of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the target entity.
    /// </typeparam>
    /// <param name="model">
    /// Target model.
    /// </param>
    /// <param name="schema">
    /// Name of the database schema to assign.
    /// </param>
    public static void SetSchemaFor<T>(this IMutableModel model, string schema)
    {
        var entity = model.FindEntityType(typeof(T));
        entity?.SetSchema(schema);
    }
    #endregion
}