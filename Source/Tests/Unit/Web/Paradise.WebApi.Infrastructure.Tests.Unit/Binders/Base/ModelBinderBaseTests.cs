using Paradise.Tests.Miscellaneous.TestData.Shared.Models;
using Paradise.WebApi.Infrastructure.Binders.Base;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Binders.Base;

/// <summary>
/// <see cref="ModelBinderBase{T}"/> test class.
/// </summary>
public sealed partial class ModelBinderBaseTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="ModelBinderBase{T}.BindModelAsync"/> method should
    /// bind the sample data to the resulting model instance.
    /// </summary>
    [Fact]
    public async Task BindModelAsync()
    {
        // Arrange
        Test.Sample.BooleanValue = true;
        Test.Sample.IntegerValue = int.MaxValue;
        Test.Sample.StringArrayValue = ["StringArrayValue"];
        Test.Sample.StringValue = "StringValue";
        Test.Sample.UnsignedIntegerValue = uint.MaxValue;

        var context = Test.CreateContext();

        // Act
        await Test.Target.BindModelAsync(context);

        // Assert
        Assert.True(context.Result.IsModelSet);

        var model = Assert.IsType<TestModel>(context.Result.Model);
        Assert.Equivalent(Test.Sample, model);
    }

    /// <summary>
    /// The <see cref="ModelBinderBase{T}.BindModelAsync"/> method should
    /// bind the sample data to the resulting model instance
    /// and populate the validation errors dictionary
    /// when sample data does not pass the validation.
    /// </summary>
    [Fact]
    public async Task BindModelAsync_FailsOnValidationErrors()
    {
        // Arrange
        Test.Sample.StringValue = null;

        var context = Test.CreateContext();

        // Act
        await Test.Target.BindModelAsync(context);

        // Assert
        Assert.True(context.Result.IsModelSet);
        Assert.False(context.ModelState.IsValid);

        Assert.True(context.ModelState.ContainsKey(nameof(TestModel.StringValue)));
    }
    #endregion
}