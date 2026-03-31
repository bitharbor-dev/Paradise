using Paradise.Models;
using Paradise.Models.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Paradise.Tests.Miscellaneous.TestData.Shared.Models;

/// <summary>
/// Test <see cref="ErrorCode"/> implementation.
/// </summary>
public enum TestErrorCode
{
    /// <summary>
    /// Default member which does not have any attributes assigned.
    /// </summary>
    DefaultMember,
    /// <summary>
    /// A member with it's display value set to a plain string.
    /// </summary>
    [Display(Name = "Test")]
    DisplayValueWithoutParametersMember,
    /// <summary>
    /// A member with it's display value set to a format string.
    /// </summary>
    [Display(Name = "Test {0}")]
    DisplayValueWithParametersMember,
    /// <summary>
    /// A member marked with <c><see cref="IsCriticalAttribute.Value"/> = <see langword="true"/></c>.
    /// </summary>
    [IsCritical(true)]
    IsCriticalTrue,
    /// <summary>
    /// A member marked with <c><see cref="IsCriticalAttribute.Value"/> = <see langword="false"/></c>.
    /// </summary>
    [IsCritical(false)]
    IsCriticalFalse
}