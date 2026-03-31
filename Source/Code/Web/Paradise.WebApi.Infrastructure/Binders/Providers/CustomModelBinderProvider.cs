using Microsoft.AspNetCore.Mvc.ModelBinding;
using Paradise.Models;

namespace Paradise.WebApi.Infrastructure.Binders.Providers;

/// <summary>
/// The <see cref="IModelBinderProvider"/> implementation for custom model
/// binders registration.
/// </summary>
public sealed class CustomModelBinderProvider : IModelBinderProvider
{
    #region Fields
    private static readonly Dictionary<Type, IModelBinder> _binderMap = new()
    {
        [typeof(PagedListQueryModel)] = new PagedListQueryModelBinder()
    };
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomModelBinderProvider"/> class.
    /// </summary>
    private CustomModelBinderProvider() { }
    #endregion

    #region Properties
    /// <summary>
    /// Singleton <see cref="CustomModelBinderProvider"/> instance.
    /// </summary>
    public static CustomModelBinderProvider Instance { get; } = new();
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return _binderMap.TryGetValue(context.Metadata.ModelType, out var modelBinder)
            ? modelBinder
            : null;
    }
    #endregion
}