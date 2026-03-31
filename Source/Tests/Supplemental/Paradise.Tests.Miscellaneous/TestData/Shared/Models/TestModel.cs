using System.ComponentModel.DataAnnotations;

namespace Paradise.Tests.Miscellaneous.TestData.Shared.Models;

/// <summary>
/// A test model class that eases writing and isolates test methods.
/// </summary>
public sealed class TestModel
{
    #region Properties
    /// <summary>
    /// Test string property.
    /// </summary>
    [Required]
    public string? StringValue { get; set; }

    /// <summary>
    /// Test integer property.
    /// </summary>
    public int IntegerValue { get; set; }

    /// <summary>
    /// Test unsigned integer property.
    /// </summary>
    public uint UnsignedIntegerValue { get; set; }

    /// <summary>
    /// Test boolean value.
    /// </summary>
    public bool BooleanValue { get; set; }

    /// <summary>
    /// Test string array value.
    /// </summary>
    public IEnumerable<string>? StringArrayValue { get; set; }
    #endregion
}