using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Paradise.WebApi.Infrastructure.Binders.Base;

/// <summary>
/// Provides a base implementation for custom model binders.
/// Handles instance creation, validation, and value extraction from the binding context.
/// </summary>
/// <typeparam name="T">
/// The type of the model to bind.
/// </typeparam>
internal abstract class ModelBinderBase<T> : IModelBinder
{
    #region Public methods
    /// <inheritdoc/>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var instance = GetInstance(bindingContext);

        ValidateInstance(bindingContext, instance);

        bindingContext.Result = ModelBindingResult.Success(instance);

        return Task.CompletedTask;
    }
    #endregion

    #region Private protected methods
    /// <summary>
    /// Creates an instance of <typeparamref name="T"/> from the binding context.
    /// </summary>
    /// <param name="bindingContext">
    /// The context for model binding.
    /// </param>
    /// <returns>
    /// An instance of <typeparamref name="T"/>.
    /// </returns>
    private protected abstract T GetInstance(ModelBindingContext bindingContext);

    /// <summary>
    /// Retrieves an integer value from the binding context for the specified key.
    /// Adds a model error if parsing fails.
    /// </summary>
    /// <param name="bindingContext">
    /// The context for model binding.
    /// </param>
    /// <param name="key">
    /// The key to retrieve the value for.
    /// </param>
    /// <returns>
    /// The parsed <see cref="int"/> value.
    /// </returns>
    private protected static int GetInt(ModelBindingContext bindingContext, string key)
    {
        int.TryParse(GetValue(bindingContext, key).FirstValue, CultureInfo.InvariantCulture, out var integer);

        return integer;
    }

    /// <summary>
    /// Retrieves an unsigned integer value from the binding context for the specified key.
    /// Adds a model error if parsing fails.
    /// </summary>
    /// <param name="bindingContext">
    /// The context for model binding.
    /// </param>
    /// <param name="key">
    /// The key to retrieve the value for.
    /// </param>
    /// <returns>
    /// The parsed <see cref="uint"/> value.
    /// </returns>
    private protected static uint GetUint(ModelBindingContext bindingContext, string key)
    {
        uint.TryParse(GetValue(bindingContext, key).FirstValue, CultureInfo.InvariantCulture, out var integer);

        return integer;
    }

    /// <summary>
    /// Retrieves a boolean value from the binding context for the specified key.
    /// Adds a model error if parsing fails.
    /// </summary>
    /// <param name="bindingContext">
    /// The context for model binding.
    /// </param>
    /// <param name="key">
    /// The key to retrieve the value for.
    /// </param>
    /// <returns>
    /// The parsed <see cref="bool"/> value.
    /// </returns>
    [SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Default value is applicable.")]
    private protected static bool GetBool(ModelBindingContext bindingContext, string key)
    {
        bool.TryParse(GetValue(bindingContext, key).FirstValue, out var boolean);

        return boolean;
    }

    /// <summary>
    /// Retrieves a string value from the binding context for the specified key.
    /// </summary>
    /// <param name="bindingContext">
    /// The context for model binding.
    /// </param>
    /// <param name="key">
    /// The key to retrieve the value for.
    /// </param>
    /// <returns>
    /// The <see cref="string"/> value, or <see langword="null"/> if not found.
    /// </returns>
    private protected static string? GetString(ModelBindingContext bindingContext, string key)
        => GetValue(bindingContext, key).FirstValue;

    /// <summary>
    /// Retrieves a collection of string values from the binding context for the specified key.
    /// </summary>
    /// <param name="bindingContext">
    /// The context for model binding.
    /// </param>
    /// <param name="key">
    /// The key to retrieve the values for.
    /// </param>
    /// <returns>
    /// An enumerable of <see cref="string"/> values.
    /// </returns>
    private protected static IEnumerable<string> GetStrings(ModelBindingContext bindingContext, string key)
        => GetValue(bindingContext, key);
    #endregion

    #region Private methods
    /// <summary>
    /// Retrieves the value provider result for the specified key from the binding context.
    /// </summary>
    /// <param name="bindingContext">
    /// The context for model binding.
    /// </param>
    /// <param name="key">
    /// The key to retrieve the value for.
    /// </param>
    /// <returns>
    /// The value provider result.
    /// </returns>
    private static ValueProviderResult GetValue(ModelBindingContext bindingContext, string key)
        => bindingContext.ValueProvider.GetValue(key);

    /// <summary>
    /// Validates the provided instance using data annotations and adds any validation errors to the model state.
    /// </summary>
    /// <param name="bindingContext">
    /// The context for model binding.
    /// </param>
    /// <param name="instance">
    /// The instance to validate.
    /// </param>
    private static void ValidateInstance(ModelBindingContext bindingContext, T instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        var validationContext = new ValidationContext(instance);
        var validationResults = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(instance, validationContext, validationResults, true);

        if (!isValid)
        {
            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                    bindingContext.ModelState.AddModelError(memberName, validationResult.ErrorMessage!);
            }
        }
    }
    #endregion
}