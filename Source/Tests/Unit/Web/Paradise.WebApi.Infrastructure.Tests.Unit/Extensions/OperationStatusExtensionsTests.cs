using Paradise.Models;
using Paradise.WebApi.Infrastructure.Extensions;
using System.Net;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Extensions;

/// <summary>
/// <see cref="OperationStatusExtensions"/> test class.
/// </summary>
public sealed class OperationStatusExtensionsTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="GetStatusCode"/> method.
    /// </summary>
    public static TheoryData<OperationStatus, HttpStatusCode> GetStatusCode_MemberData => new()
    {
        { OperationStatus.Success, HttpStatusCode.OK },
        { OperationStatus.Failure, HttpStatusCode.InternalServerError },
        { OperationStatus.InvalidInput, HttpStatusCode.BadRequest },
        { OperationStatus.Created, HttpStatusCode.Created },
        { OperationStatus.Missing, HttpStatusCode.NotFound },
        { OperationStatus.Received, HttpStatusCode.Accepted },
        { OperationStatus.Blocked, HttpStatusCode.UnprocessableEntity },
        { OperationStatus.Prohibited, HttpStatusCode.Forbidden },
        { OperationStatus.Unauthorized, HttpStatusCode.Unauthorized }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="OperationStatusExtensions.GetStatusCode"/> method should
    /// return the correct <see cref="HttpStatusCode"/>
    /// for each supported <see cref="OperationStatus"/>.
    /// </summary>
    /// <param name="status">
    /// The <see cref="OperationStatus"/> to convert.
    /// </param>
    /// <param name="expectedStatusCode">
    /// Expected status code.
    /// </param>
    [Theory, MemberData(nameof(GetStatusCode_MemberData))]
    public void GetStatusCode(OperationStatus status, HttpStatusCode expectedStatusCode)
    {
        // Arrange

        // Act
        var result = status.GetStatusCode();

        // Assert
        Assert.Equal(expectedStatusCode, result);
    }

    /// <summary>
    /// The <see cref="OperationStatusExtensions.GetStatusCode"/> method should
    /// throw the <see cref="KeyNotFoundException"/> if the input
    /// operation status is invalid.
    /// </summary>
    [Fact]
    public void GetStatusCode_ThrowsOnInvalidStatus()
    {
        // Arrange
        var status = (OperationStatus)int.MaxValue;

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(()
            => status.GetStatusCode());
    }
    #endregion
}
