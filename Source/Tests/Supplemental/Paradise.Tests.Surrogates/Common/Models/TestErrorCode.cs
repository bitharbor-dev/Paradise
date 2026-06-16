using System.ComponentModel.DataAnnotations;

namespace Paradise.Tests.Surrogates.Common.Models;

/// <summary>
/// Test error code implementation.
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
    DisplayValueWithParametersMember
}