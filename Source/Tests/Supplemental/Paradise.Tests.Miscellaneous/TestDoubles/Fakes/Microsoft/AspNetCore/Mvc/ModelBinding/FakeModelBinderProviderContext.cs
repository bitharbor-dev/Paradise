using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.AspNetCore.Mvc.ModelBinding;

/// <summary>
/// Fake <see cref="ModelBinderProviderContext"/> implementation.
/// </summary>
public sealed class FakeModelBinderProviderContext : ModelBinderProviderContext
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeModelBinderProviderContext"/> class.
    /// </summary>
    /// <param name="modelType">
    /// Target model type.
    /// </param>
    public FakeModelBinderProviderContext(Type modelType)
    {
        MetadataProvider = new EmptyModelMetadataProvider();
        Metadata = MetadataProvider.GetMetadataForType(modelType);
        BindingInfo = new BindingInfo
        {
            BinderModelName = Metadata.BinderModelName,
            BinderType = Metadata.BinderType,
            BindingSource = Metadata.BindingSource,
            PropertyFilterProvider = Metadata.PropertyFilterProvider
        };
    }
    #endregion

    #region Properties
    /// <inheritdoc/>
    public override BindingInfo BindingInfo { get; }

    /// <inheritdoc/>
    public override ModelMetadata Metadata { get; }

    /// <inheritdoc/>
    public override IModelMetadataProvider MetadataProvider { get; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override IModelBinder CreateBinder(ModelMetadata metadata)
        => throw new NotImplementedException();
    #endregion
}