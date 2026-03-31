using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Paradise.Models;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.AspNetCore.Mvc.ModelBinding;
using Paradise.WebApi.Infrastructure.Binders;
using Paradise.WebApi.Infrastructure.Binders.Base;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Binders;

/// <summary>
/// <see cref="PagedListQueryModelBinder"/> test class.
/// </summary>
public sealed class PagedListQueryModelBinderTests
{
    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    internal PagedListQueryModelBinder Binder { get; } = new();
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="PagedListQueryModelBinder"/> "GetInstance" method should
    /// initialize the target model instance using the data
    /// provided in the input binding context.
    /// </summary>
    /// <remarks>
    /// The <see cref="ModelBinderBase{T}.GetInstance"/> method is
    /// indirectly called via the base <see cref="ModelBinderBase{T}.BindModelAsync"/> method.
    /// </remarks>
    [Fact]
    public async Task GetInstance()
    {
        // Arrange
        var context = CreateBindingContext(new()
        {
            [nameof(PagedListQueryModel.PageSize)] = new("25"),
            [nameof(PagedListQueryModel.PageNumber)] = new("3"),
            [nameof(PagedListQueryModel.OrderAscending)] = new("true"),
            [nameof(PagedListQueryModel.OrderBy)] = new("OrderByProperty"),
            [nameof(PagedListQueryModel.LookupProperties)] = new(["LookupProperty1", "LookupProperty2"]),
            [nameof(PagedListQueryModel.LookupValue)] = new("LookupValue")
        });

        // Act
        await Binder.BindModelAsync(context);

        // Assert
        Assert.True(context.Result.IsModelSet);
        Assert.True(context.ModelState.IsValid);

        var model = Assert.IsType<PagedListQueryModel>(context.Result.Model);

        Assert.Equal(25, model.PageSize);
        Assert.Equal(3, model.PageNumber);
        Assert.True(model.OrderAscending);
        Assert.Equal("OrderByProperty", model.OrderBy);
        Assert.Equal(["LookupProperty1", "LookupProperty2"], model.LookupProperties);
        Assert.Equal("LookupValue", model.LookupValue);
    }

    /// <summary>
    /// The <see cref="PagedListQueryModelBinder"/> "GetInstance" method should
    /// initialize the target model instance using the data
    /// provided in the input binding context and
    /// normalize page size and page number to 1 when zero values are provided.
    /// </summary>
    /// <remarks>
    /// The <see cref="ModelBinderBase{T}.GetInstance"/> method is
    /// indirectly called via the base <see cref="ModelBinderBase{T}.BindModelAsync"/> method.
    /// </remarks>
    [Fact]
    public async Task BindModelAsync_NormalizesZeroPagingValues()
    {
        // Arrange
        var context = CreateBindingContext(new()
        {
            [nameof(PagedListQueryModel.PageSize)] = new("0"),
            [nameof(PagedListQueryModel.PageNumber)] = new("0")
        });

        // Act
        await Binder.BindModelAsync(context);

        // Assert
        Assert.True(context.Result.IsModelSet);
        Assert.True(context.ModelState.IsValid);

        var model = Assert.IsType<PagedListQueryModel>(context.Result.Model);

        Assert.Equal(1, model.PageSize);
        Assert.Equal(1, model.PageNumber);
    }

    /// <summary>
    /// The <see cref="PagedListQueryModelBinder"/> "GetInstance" method should
    /// initialize the target model instance using the data
    /// provided in the input binding context and
    /// default paging values when parsing fails.
    /// </summary>
    /// <remarks>
    /// The <see cref="ModelBinderBase{T}.GetInstance"/> method is
    /// indirectly called via the base <see cref="ModelBinderBase{T}.BindModelAsync"/> method.
    /// </remarks>
    [Fact]
    public async Task BindModelAsync_DefaultsPagingOnInvalidParsing()
    {
        // Arrange
        var context = CreateBindingContext(new()
        {
            [nameof(PagedListQueryModel.PageSize)] = new(string.Empty),
            [nameof(PagedListQueryModel.PageNumber)] = new(string.Empty)
        });

        // Act
        await Binder.BindModelAsync(context);

        // Assert
        Assert.True(context.Result.IsModelSet);
        Assert.True(context.ModelState.IsValid);

        var model = Assert.IsType<PagedListQueryModel>(context.Result.Model);

        Assert.Equal(1, model.PageSize);
        Assert.Equal(1, model.PageNumber);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates a fully initialized <see cref="ModelBindingContext"/> instance.
    /// </summary>
    /// <param name="values">
    /// A dictionary containing the key-value representation if the binding target.
    /// </param>
    /// <returns>
    /// A fully initialized <see cref="ModelBindingContext"/> instance.
    /// </returns>
    private static DefaultModelBindingContext CreateBindingContext(Dictionary<string, StringValues> values) => new()
    {
        ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(PagedListQueryModel)),
        ModelName = nameof(PagedListQueryModel),
        ModelState = new ModelStateDictionary(),
        ValueProvider = new FakeValueProvider(values),
    };
    #endregion
}