using Microsoft.AspNetCore.Http;
using Paradise.Models;
using Paradise.WebApi.Base.Extensions;

namespace Paradise.WebApi.Base.Tests.Unit.Extensions;

/// <summary>
/// <see cref="OperationStatusExtensions"/> test class.
/// </summary>
public sealed class OperationStatusExtensionsTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="GetStatusCode"/> method.
    /// </summary>
    public static TheoryData<OperationStatus, int> GetStatusCode_MemberData => new()
    {
        { OperationStatus.Success, StatusCodes.Status200OK                  },
        { OperationStatus.Failure, StatusCodes.Status500InternalServerError },
        { OperationStatus.InvalidInput, StatusCodes.Status400BadRequest     },
        { OperationStatus.Created, StatusCodes.Status201Created             },
        { OperationStatus.Missing, StatusCodes.Status404NotFound            },
        { OperationStatus.Received, StatusCodes.Status202Accepted           },
        { OperationStatus.Blocked, StatusCodes.Status422UnprocessableEntity },
        { OperationStatus.Prohibited, StatusCodes.Status403Forbidden        },
        { OperationStatus.Unauthorized, StatusCodes.Status401Unauthorized   }
    };

    /// <summary>
    /// Provides member data for <see cref="GetOperationStatus"/> method.
    /// </summary>
    public static TheoryData<int, OperationStatus> GetOperationStatus_MemberData => new()
    {
        { StatusCodes.Status200OK, OperationStatus.Success                  },
        { StatusCodes.Status500InternalServerError, OperationStatus.Failure },
        { StatusCodes.Status400BadRequest, OperationStatus.InvalidInput     },
        { StatusCodes.Status201Created, OperationStatus.Created             },
        { StatusCodes.Status404NotFound, OperationStatus.Missing            },
        { StatusCodes.Status202Accepted, OperationStatus.Received           },
        { StatusCodes.Status422UnprocessableEntity, OperationStatus.Blocked },
        { StatusCodes.Status403Forbidden, OperationStatus.Prohibited        },
        { StatusCodes.Status401Unauthorized, OperationStatus.Unauthorized   }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="OperationStatusExtensions.GetStatusCode"/> method should
    /// return the correct status code
    /// for each supported <see cref="OperationStatus"/>.
    /// </summary>
    /// <param name="status">
    /// The <see cref="OperationStatus"/> to convert.
    /// </param>
    /// <param name="expectedStatusCode">
    /// Expected status code.
    /// </param>
    [Theory, MemberData(nameof(GetStatusCode_MemberData))]
    public void GetStatusCode(OperationStatus status, int expectedStatusCode)
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

    /// <summary>
    /// The <see cref="OperationStatusExtensions.GetOperationStatus"/> method should
    /// return the correct <see cref="OperationStatus"/>
    /// for each supported status code.
    /// </summary>
    /// <param name="statusCode">
    /// The status code to convert.
    /// </param>
    /// <param name="expectedStatus">
    /// Expected <see cref="OperationStatus"/>.
    /// </param>
    [Theory, MemberData(nameof(GetOperationStatus_MemberData))]
    public void GetOperationStatus(int statusCode, OperationStatus expectedStatus)
    {
        // Arrange

        // Act
        var result = statusCode.GetOperationStatus();

        // Assert
        Assert.Equal(result, expectedStatus);
    }

    /// <summary>
    /// The <see cref="OperationStatusExtensions.GetOperationStatus"/> method should
    /// throw the <see cref="KeyNotFoundException"/> if the input
    /// status code is invalid.
    /// </summary>
    [Fact]
    public void GetOperationStatus_ThrowsOnInvalidStatus()
    {
        // Arrange
        var statusCode = int.MaxValue;

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(()
            => statusCode.GetOperationStatus());
    }
    #endregion
}
