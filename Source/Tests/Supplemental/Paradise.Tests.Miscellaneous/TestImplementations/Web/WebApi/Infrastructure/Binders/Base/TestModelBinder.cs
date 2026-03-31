using Microsoft.AspNetCore.Mvc.ModelBinding;
using Paradise.Tests.Miscellaneous.TestData.Shared.Models;
using Paradise.WebApi.Infrastructure.Binders.Base;

namespace Paradise.Tests.Miscellaneous.TestImplementations.Web.WebApi.Infrastructure.Binders.Base;

/// <summary>
/// Test <see cref="ModelBinderBase{T}"/> implementation.
/// </summary>
internal sealed class TestModelBinder : ModelBinderBase<TestModel>
{
    #region Public methods
    /// <inheritdoc/>
    private protected override TestModel GetInstance(ModelBindingContext bindingContext) => new()
    {
        BooleanValue = GetBool(bindingContext, nameof(TestModel.BooleanValue)),
        IntegerValue = GetInt(bindingContext, nameof(TestModel.IntegerValue)),
        StringArrayValue = GetStrings(bindingContext, nameof(TestModel.StringArrayValue)),
        StringValue = GetString(bindingContext, nameof(TestModel.StringValue)),
        UnsignedIntegerValue = GetUint(bindingContext, nameof(TestModel.UnsignedIntegerValue))
    };
    #endregion
}