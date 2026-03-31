using Microsoft.AspNetCore.Mvc.ModelBinding;
using Paradise.Tests.Miscellaneous.TestData.Shared.Models;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.AspNetCore.Mvc.ModelBinding;
using Paradise.Tests.Miscellaneous.TestImplementations.Web.WebApi.Infrastructure.Binders.Base;
using Paradise.WebApi.Infrastructure.Binders.Base;
using System.Globalization;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Binders.Base;

public sealed partial class ModelBinderBaseTests
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="ModelBinderBaseTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public ModelBinderBase<TestModel> Target { get; set; } = new TestModelBinder();

        /// <summary>
        /// A sample <see cref="TestModel"/> instance
        /// used to build the <see cref="ModelBindingContext"/> used in test methods.
        /// </summary>
        public TestModel Sample { get; } = new();
        #endregion

        #region Public methods
        /// <summary>
        /// Creates a fully initialized <see cref="ModelBindingContext"/> instance.
        /// </summary>
        /// <returns>
        /// A fully initialized <see cref="ModelBindingContext"/> instance.
        /// </returns>
        public DefaultModelBindingContext CreateContext()
        {
            var booleanValue = Sample.BooleanValue.ToString();
            var integerValue = Sample.IntegerValue.ToString(CultureInfo.InvariantCulture);
            var stringArrayValue = Sample.StringArrayValue?.ToArray();
            var stringValue = Sample.StringValue;
            var unsignedIntergerValue = Sample.UnsignedIntegerValue.ToString(CultureInfo.InvariantCulture);

            var valueProvider = new FakeValueProvider(new()
            {
                [nameof(TestModel.BooleanValue)] = new(booleanValue),
                [nameof(TestModel.IntegerValue)] = new(integerValue),
                [nameof(TestModel.StringArrayValue)] = new(stringArrayValue),
                [nameof(TestModel.StringValue)] = new(stringValue),
                [nameof(TestModel.UnsignedIntegerValue)] = new(unsignedIntergerValue),
            });

            return new DefaultModelBindingContext
            {
                BinderModelName = nameof(TestModel),
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(TestModel)),
                ModelState = new(),
                ValueProvider = valueProvider
            };
        }
        #endregion
    }
    #endregion
}