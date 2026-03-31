using Paradise.DataAccess.Repositories;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.EntityFrameworkCore;
using Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Base;
using System.Linq.Expressions;

namespace Paradise.DataAccess.Tests.Unit.Repositories;

public sealed partial class PagedListQueryTests : IDisposable
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Dispose()
        => Test.Dispose();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="PagedListQueryTests"/> class.
    /// </summary>
    private sealed class TestEnvironment : IDisposable
    {
        #region Fields
        private readonly MethodCallCollector _collector;
        private readonly FakeDbContext _context;

        /// <summary>
        /// An <see cref="IQueryable{T}"/> of <see cref="TestNamedEntity"/>.
        /// </summary>
        /// <remarks>
        /// Implemented as a <see langword="field"/> to allow usage with <see langword="ref"/> methods.
        /// </remarks>
        public IQueryable<TestNamedEntity> Queryable;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _collector = new();
            _context = new();
            Queryable = _context.Set<TestNamedEntity>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public PagedListQuery<TestNamedEntity> Target { get; } = new();
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public void Dispose()
            => _context.Dispose();

        /// <summary>
        /// Gets the <see cref="MethodCallExpression"/> instances with the given
        /// <paramref name="methodName"/> collected during <see cref="Queryable"/> modifications.
        /// </summary>
        /// <param name="methodName">
        /// The name of the method calls to look up for.
        /// </param>
        /// <returns>
        /// The list of <see cref="MethodCallExpression"/> with the given <paramref name="methodName"/>.
        /// </returns>
        public IEnumerable<MethodCallExpression> GetMethodCalls(string methodName)
        {
            _collector.MethodCalls.Clear();

            _collector.Visit(Queryable.Expression);

            return _collector
                .MethodCalls
                .Where(call => call.Method.Name.Equals(methodName, StringComparison.Ordinal));
        }
        #endregion
    }

    /// <summary>
    /// An <see cref="ExpressionVisitor"/> implementation that collects
    /// all of the <see cref="MethodCallExpression"/> instances from the input <see cref="Expression"/>.
    /// </summary>
    private sealed class MethodCallCollector : ExpressionVisitor
    {
        #region Properties
        /// <summary>
        /// The list of collected method calls.
        /// </summary>
        public IList<MethodCallExpression> MethodCalls { get; } = [];
        #endregion

        #region Protected methods
        /// <inheritdoc/>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            MethodCalls.Add(node);

            return base.VisitMethodCall(node);
        }
        #endregion
    }
    #endregion
}