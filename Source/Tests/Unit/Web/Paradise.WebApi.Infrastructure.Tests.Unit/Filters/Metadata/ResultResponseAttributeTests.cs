using Microsoft.AspNetCore.Mvc;
using Paradise.Models;
using Paradise.WebApi.Infrastructure.Extensions;
using Paradise.WebApi.Infrastructure.Filters.Metadata;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Filters.Metadata;

/// <summary>
/// <see cref="ResultResponseAttribute"/> test class.
/// </summary>
public sealed class ResultResponseAttributeTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="Constructor"/> method.
    /// </summary>
    public static TheoryData<OperationStatus> Constructor_MemberData { get; } = [.. Enum.GetValues<OperationStatus>()];
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="ResultResponseAttribute(OperationStatus, string?)"/> constructor should
    /// properly initialize a new instance of the <see cref="ResultResponseAttribute"/> class,
    /// by setting the response type to <see cref="Result"/> and mapping
    /// <see cref="ProducesResponseTypeAttribute.StatusCode"/>
    /// to the supplied <see cref="OperationStatus"/> value.
    /// </summary>
    [Theory, MemberData(nameof(Constructor_MemberData))]
    public void Constructor(OperationStatus status)
    {
        // Arrange
        var resultType = typeof(Result);
        var expectedStatusCode = (int)status.GetStatusCode();

        // Act
        var attribute = new ResultResponseAttribute(status);

        // Assert
        Assert.Equal(status, attribute.Status);
        Assert.Equal(resultType, attribute.Type);
        Assert.Equal(expectedStatusCode, attribute.StatusCode);
    }
    #endregion
}