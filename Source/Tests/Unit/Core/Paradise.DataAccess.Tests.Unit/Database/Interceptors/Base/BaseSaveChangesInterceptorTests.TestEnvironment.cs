using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Paradise.DataAccess.Database.Interceptors.Base;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.EntityFrameworkCore;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.System;
using Paradise.Tests.Miscellaneous.TestImplementations.Core.DataAccess.Database.Interceptors.Base;
using Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Base;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.DataAccess.Tests.Unit.Database.Interceptors.Base;

public sealed partial class BaseSaveChangesInterceptorTests : IDisposable
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();

    /// <summary>
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </summary>
    public CancellationToken Token { get; } = TestContext.Current.CancellationToken;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Dispose()
        => Test.Dispose();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="BaseSaveChangesInterceptorTests"/> class.
    /// </summary>
    private sealed class TestEnvironment : IDisposable
    {
        #region Fields
        private readonly IList<EntityEntry> _interceptedEntries = [];
        #endregion

        #region Fields
        private readonly FakeTimeProvider _timeProvider;
        private readonly TestBaseSaveChangesInterceptor _target;
        private readonly FakeDbContext _context;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _timeProvider = new();
            _target = new(_timeProvider);
            _target.Intercepted += OnIntercepted;

            _context = new();

            Target = _target;
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public BaseSaveChangesInterceptor Target { get; }

        /// <summary>
        /// The <see cref="IEqualityComparer{T}"/> to be used to
        /// compare <see cref="EntityEntry"/> instances.
        /// </summary>
        public IEqualityComparer<EntityEntry> Comparer { get; } = new TestEqualityComparer();

        /// <summary>
        /// The list of entity entries which were intercepted.
        /// </summary>
        public IEnumerable<EntityEntry> InterceptedEntries
            => _interceptedEntries;
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public void Dispose()
        {
            _target.Intercepted -= OnIntercepted;
            _context.Dispose();
        }

        /// <summary>
        /// Intercepts the <see cref="BaseSaveChangesInterceptor.EntityFilter"/>
        /// property call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target property.
        /// </param>
        public void SetEntityFilter(Func<EntityEntry, bool> resultingDelegate)
            => _target.EntityFilterResult = resultingDelegate;

        /// <summary>
        /// Creates and attaches an entity to the underlying context,
        /// returning the resulting <see cref="EntityEntry"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="EntityEntry"/> representing the attached entity.
        /// </returns>
        [SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance",
            Justification = "Intentional encapsulation.")]
        public EntityEntry CreateLinkedEntry()
            => _context.Attach(new TestNamedEntity());

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextEventData"/> class.
        /// </summary>
        /// <param name="passNullDbContext">
        /// Indicates whether the <see langword="null"/> should be passed instead of
        /// an actual <see cref="DbContext"/> instance.
        /// </param>
        /// <returns>
        /// A new instance of the <see cref="DbContextEventData"/> class.
        /// </returns>
        public DbContextEventData GetEventData(bool passNullDbContext = false)
        {
            var context = passNullDbContext
                ? null
                : _context;

            return new(null!, null!, context);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// <see cref="TestBaseSaveChangesInterceptor.Intercepted"/> event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="EntryInterceptedEventArgs"/> instance containing the event data.
        /// </param>
        private void OnIntercepted(object? sender, EntryInterceptedEventArgs e)
            => _interceptedEntries.Add(e.Entry);
        #endregion
    }

    /// <summary>
    /// Test-only equality comparer implementation.
    /// </summary>
    private sealed class TestEqualityComparer : IEqualityComparer<EntityEntry>
    {
        #region Public methods
        /// <inheritdoc/>
        public bool Equals(EntityEntry? x, EntityEntry? y)
            => x?.Entity == y?.Entity;

        /// <inheritdoc/>
        public int GetHashCode([DisallowNull] EntityEntry obj)
            => obj.Entity.GetHashCode();
        #endregion
    }
    #endregion
}