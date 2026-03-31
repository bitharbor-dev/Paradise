using Paradise.Models.Attributes;

namespace Paradise.Models.Tests.Unit.Attributes;

public sealed partial class RequiredAtLeastOneAttributeTests
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="RequiredAtLeastOneAttributeTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Properties
        /// <summary>
        /// A model instance used to run tests against.
        /// </summary>
        public TestModel Model { get; } = new();

        /// <summary>
        /// Indicates whether the empty or whitespace strings would be treated as an invalid value.
        /// </summary>
        public bool RestrictEmptyOrWhitespaceStrings { get; set; }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates a <see cref="RequiredAtLeastOneAttribute"/> instance configured
        /// for the test model properties.
        /// </summary>
        /// <param name="propertyNames">
        /// The names of the properties that are required.
        /// </param>
        /// <returns>
        /// A new <see cref="RequiredAtLeastOneAttribute"/> instance.
        /// </returns>
        public RequiredAtLeastOneAttribute CreateAttribute(params string[] propertyNames)
        {
            var names = propertyNames.Length is 0
                ? [nameof(TestModel.IntegerValue), nameof(TestModel.StringValue)]
                : propertyNames;

            return new(RestrictEmptyOrWhitespaceStrings, names);
        }
        #endregion
    }

    /// <summary>
    /// A test-helper class that eases writing and isolates test methods.
    /// </summary>
    private sealed class TestModel
    {
        #region Properties
        /// <summary>
        /// Test string property.
        /// </summary>
        public string? StringValue { get; set; }

        /// <summary>
        /// Test integer property.
        /// </summary>
        public int? IntegerValue { get; set; }
        #endregion
    }
    #endregion
}