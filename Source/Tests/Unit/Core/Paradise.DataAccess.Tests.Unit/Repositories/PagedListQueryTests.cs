using Microsoft.EntityFrameworkCore;
using Paradise.DataAccess.Repositories;
using Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Base;
using System.Linq.Expressions;

namespace Paradise.DataAccess.Tests.Unit.Repositories;

/// <summary>
/// <see cref="PagedListQuery{TEntity}"/> test class.
/// </summary>
public sealed partial class PagedListQueryTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="PageSkip"/> method.
    /// </summary>
    public static TheoryData<int, int, int> PageSkip_MemberData { get; } = new()
    {
        { 01,   10, 00  },
        { 02,   10, 10  },
        { 03,   10, 20  },
        { 01,   25, 00  }
    };

    /// <summary>
    /// Provides member data for <see cref="Apply_AddsOrderingExpression"/> method.
    /// </summary>
    public static TheoryData<bool> Apply_AppendsOrderingExpression_MemberData { get; } = new()
    {
        { true  },
        { false }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="PagedListQuery{TEntity}.PageSize"/> property should
    /// throw the <see cref="ArgumentOutOfRangeException"/> if the input
    /// value is less than or equal to 0.
    /// </summary>
    [Fact]
    public void PageSize_ThrowsOnZero()
    {
        // Arrange

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(()
            => Test.Target.PageSize = 0);
    }

    /// <summary>
    /// The <see cref="PagedListQuery{TEntity}.PageNumber"/> property should
    /// throw the <see cref="ArgumentOutOfRangeException"/> if the input
    /// value is less than or equal to 0.
    /// </summary>
    [Fact]
    public void PageNumber_ThrowsOnNegative()
    {
        // Arrange

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(()
            => Test.Target.PageNumber = -3);
    }

    /// <summary>
    /// The <see cref="PagedListQuery{TEntity}.PageSkip"/> property should
    /// return the correct number of items to skip based on the page number and size.
    /// </summary>
    /// <param name="pageNumber">
    /// The page number.
    /// </param>
    /// <param name="pageSize">
    /// The number of items per page.
    /// </param>
    /// <param name="expectedSkip">
    /// The expected page "skip" value.
    /// </param>
    [Theory, MemberData(nameof(PageSkip_MemberData))]
    public void PageSkip(int pageNumber, int pageSize, int expectedSkip)
    {
        // Arrange
        Test.Target.PageNumber = pageNumber;
        Test.Target.PageSize = pageSize;

        // Act
        var skip = Test.Target.PageSkip;

        // Assert
        Assert.Equal(expectedSkip, skip);
    }

    /// <summary>
    /// The <see cref="PagedListQuery{TEntity}.Apply"/> method should
    /// include the <see cref="PagedListQuery{TEntity}.NavigationProperties"/> property value
    /// as EF Core "Include" operations attached to the query.
    /// </summary>
    [Fact]
    public void Apply_IncludesNavigationProperties()
    {
        // Arrange
        var propertyName = nameof(TestNamedEntity.Name);

        Test.Target.NavigationProperties = new[] { propertyName };

        // Act
        Test.Target.Apply(ref Test.Queryable);

        // Assert
        var includeCall = Assert.Single(Test.GetMethodCalls(nameof(EntityFrameworkQueryableExtensions.Include)));

        Assert.Contains(includeCall.Arguments,
                        argument => argument is ConstantExpression constant && constant.Value?.Equals(propertyName) == true);
    }

    /// <summary>
    /// The <see cref="PagedListQuery{TEntity}.Apply"/> method should
    /// not include the empty <see cref="PagedListQuery{TEntity}.NavigationProperties"/> property value
    /// as EF Core "Include" operations attached to the query.
    /// </summary>
    [Fact]
    public void Apply_SkipsNavigationProperties()
    {
        // Arrange

        // Act
        Test.Target.Apply(ref Test.Queryable);

        // Assert
        Assert.Empty(Test.GetMethodCalls(nameof(EntityFrameworkQueryableExtensions.Include)));
    }

    /// <summary>
    /// The <see cref="PagedListQuery{TEntity}.Apply"/> method should
    /// include the <see cref="PagedListQuery{TEntity}.LookupProperties"/> and
    /// <see cref="PagedListQuery{TEntity}.LookupValue"/> property values
    /// as EF Core "Where" operation attached to the query.
    /// </summary>
    [Fact]
    public void Apply_AddsLookupFilteringExpression()
    {
        // Arrange
        Test.Target.LookupProperties = new[] { nameof(TestNamedEntity.Name) };
        Test.Target.LookupValue = "test";

        // Act
        Test.Target.Apply(ref Test.Queryable);

        // Assert
        Assert.Single(Test.GetMethodCalls(nameof(Queryable.Where)));
    }

    /// <summary>
    /// The <see cref="PagedListQuery{TEntity}.Apply"/> method should
    /// not include the empty <see cref="PagedListQuery{TEntity}.LookupValue"/> property value
    /// as EF Core "Where" operation attached to the query.
    /// </summary>
    [Fact]
    public void Apply_SkipsLookupFilteringWhenLookupValueIsNull()
    {
        // Arrange
        Test.Target.LookupProperties = new[] { nameof(TestNamedEntity.Name) };
        Test.Target.LookupValue = null;

        // Act
        Test.Target.Apply(ref Test.Queryable);

        // Assert
        Assert.Empty(Test.GetMethodCalls(nameof(Queryable.Where)));
    }

    /// <summary>
    /// The <see cref="PagedListQuery{TEntity}.Apply"/> method should
    /// include the <see cref="PagedListQuery{TEntity}.Filter"/> property value
    /// as EF Core "Where" operation attached to the query.
    /// </summary>
    [Fact]
    public void Apply_AddsFilteringExpression()
    {
        // Arrange
        Test.Target.Filter = entity => entity.Id == Guid.Empty;

        // Act
        Test.Target.Apply(ref Test.Queryable);

        // Assert
        Assert.Single(Test.GetMethodCalls(nameof(Queryable.Where)));
    }

    /// <summary>
    /// The <see cref="PagedListQuery{TEntity}.Apply"/> method should
    /// include the <see cref="PagedListQuery{TEntity}.OrderBy"/> and
    /// <see cref="PagedListQuery{TEntity}.OrderAscending"/> property values
    /// as EF Core "OrderBy" or "OrderByDescending" operation attached to the query.
    /// </summary>
    /// <param name="orderAscending">
    /// Indicates whether the results should be sorted
    /// in ascending (<see langword="true"/>)
    /// or descending (<see langword="false"/>) order.
    /// </param>
    [Theory, MemberData(nameof(Apply_AppendsOrderingExpression_MemberData))]
    public void Apply_AddsOrderingExpression(bool orderAscending)
    {
        // Arrange
        (var expectedMethodName, var expectedToBeAbsent) = orderAscending
            ? (nameof(Queryable.OrderBy), nameof(Queryable.OrderByDescending))
            : (nameof(Queryable.OrderByDescending), nameof(Queryable.OrderBy));

        Test.Target.OrderBy = nameof(TestNamedEntity.Created);
        Test.Target.OrderAscending = orderAscending;

        // Act
        Test.Target.Apply(ref Test.Queryable);

        // Assert
        Assert.Single(Test.GetMethodCalls(expectedMethodName));
        Assert.Empty(Test.GetMethodCalls(expectedToBeAbsent));
    }

    /// <summary>
    /// The <see cref="PagedListQuery{TEntity}.Apply"/> method should
    /// include the <see cref="PagedListQuery{TEntity}.Projection"/> property value
    /// as EF Core "Select" operation attached to the query.
    /// </summary>
    [Fact]
    public void Apply_AddsProjectionExpression()
    {
        // Arrange
        Test.Target.Projection = entity => new TestNamedEntity { Name = entity.Name };

        // Act
        Test.Target.Apply(ref Test.Queryable);

        // Assert
        Assert.Single(Test.GetMethodCalls(nameof(Queryable.Select)));
    }
    #endregion
}