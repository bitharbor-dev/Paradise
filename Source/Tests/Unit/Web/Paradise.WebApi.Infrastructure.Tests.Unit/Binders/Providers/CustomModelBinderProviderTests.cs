using Microsoft.AspNetCore.Mvc.ModelBinding;
using Paradise.Models;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.AspNetCore.Mvc.ModelBinding;
using Paradise.WebApi.Infrastructure.Binders;
using Paradise.WebApi.Infrastructure.Binders.Providers;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Binders.Providers;

/// <summary>
/// <see cref="CustomModelBinderProvider"/> test class.
/// </summary>
public sealed class CustomModelBinderProviderTests
{
    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    public CustomModelBinderProvider Provider { get; } = CustomModelBinderProvider.Instance;

    /// <summary>
    /// Provides member data for <see cref="GetBinder"/> method.
    /// </summary>
    public static TheoryData<Type, Type> GetBinder_MemberData { get; } = new()
    {
        { typeof(PagedListQueryModel),  typeof(PagedListQueryModelBinder)   }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="CustomModelBinderProvider.GetBinder"/> method
    /// should return the <see cref="IModelBinder"/> instance if the input
    /// model type is mapped to any binder.
    /// </summary>
    /// <param name="modelType">
    /// The input model type.
    /// </param>
    /// <param name="expectedBinderType">
    /// Expected binder type.
    /// </param>
    [Theory, MemberData(nameof(GetBinder_MemberData))]
    public void GetBinder(Type modelType, Type expectedBinderType)
    {
        // Arrange
        var context = new FakeModelBinderProviderContext(modelType);

        // Act
        var binder = Provider.GetBinder(context);

        // Assert
        Assert.IsType(expectedBinderType, binder);
    }

    /// <summary>
    /// The <see cref="CustomModelBinderProvider.GetBinder"/> method should
    /// return <see langword="null"/> if the input
    /// model type is not mapped to any binder.
    /// </summary>
    [Fact]
    public void GetBinder_ReturnsNullOnUnknownModelType()
    {
        // Arrange
        var modelType = typeof(object);

        var context = new FakeModelBinderProviderContext(modelType);

        // Act
        var binder = Provider.GetBinder(context);

        // Assert
        Assert.Null(binder);
    }

    /// <summary>
    /// The <see cref="CustomModelBinderProvider.GetBinder"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="ModelBinderProviderContext"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void GetBinder_ThrowsOnNull()
    {
        // Arrange
        var context = null as ModelBinderProviderContext;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => Provider.GetBinder(context!));
    }

    /// <summary>
    /// The <see cref="CustomModelBinderProvider.Instance"/> property should
    /// always return the same instance.
    /// </summary>
    [Fact]
    public void Instance_ReturnsSingleton()
    {
        // Arrange
        var staticProvider = CustomModelBinderProvider.Instance;

        // Act

        // Assert
        Assert.Same(staticProvider, Provider);

    }
    #endregion
}